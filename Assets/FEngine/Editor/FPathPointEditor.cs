using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using F2DEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FPathPoint))]
public class FPathPointEditor : Editor
{
    private FPathPoint PP;
    private int mCurSelectIndex = 0;
    void OnEnable()
    {
        PP = target as FPathPoint;
    }

    public override void OnInspectorGUI()
    {

    }
    void OnSceneGUI()
    {

        Event curEvent = Event.current;
        EventType mousetType = curEvent.type;
        if (mousetType == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GetInstanceID());
        }

        float width = 0.2f;
        Vector3 verSize = HandleUtility.WorldToGUIPoint(Vector3.one) - HandleUtility.WorldToGUIPoint(Vector3.zero);
        verSize.x = Mathf.Abs(verSize.x) * width * 1.2f;

        bool IsRight = false;
        var paths = PP.pathData;
        for (int i = 0; i < paths.Count; i++)
        {
            var p = paths[i];

            Handles.color = Color.blue;
            for (int j = 0; j < p.mIds.Count; j++)
            {
                Handles.DrawAAPolyLine(3, PP.transform.TransformPoint(p.pos), PP.transform.TransformPoint(PP.GetPathById(p.mIds[j]) .pos));
            }
            Handles.color = Color.white;

            Vector3 worldPos = PP.transform.TransformPoint(p.pos);
            if (mCurSelectIndex != i)
            {
                var pos = Handles.FreeMoveHandle(worldPos, Quaternion.identity, width, Vector3.one, Handles.SphereHandleCap);
                if (!Event.current.shift)
                {
                    p.pos = PP.transform.InverseTransformPoint(pos);
                }
            }
            else
            {
                var pos = Handles.FreeMoveHandle(worldPos, Quaternion.identity, width, Vector3.one, Handles.CubeHandleCap);
                if (!Event.current.shift)
                {
                    p.pos = PP.transform.InverseTransformPoint(pos);
                }
            }

            if (mousetType == EventType.MouseUp)
            {
                Vector3 point = HandleUtility.WorldToGUIPoint(worldPos);
                Rect rect = new Rect(point.x - verSize.x / 2, point.y - verSize.x / 2, verSize.x, verSize.x);
                // 是否鼠标在控制柄上
                if (rect.Contains(Event.current.mousePosition))
                {

                    if (Event.current.shift)
                    {
                        if (mCurSelectIndex != i)
                        {
                            var selectPathData = paths[mCurSelectIndex];
                            if (selectPathData.mIds.Contains(i))
                            {
                                selectPathData.mIds.Remove(i);
                                p.mIds.Remove(mCurSelectIndex);
                            }
                            else
                            {
                                selectPathData.mIds.Add(i);
                                p.mIds.Add(mCurSelectIndex);
                            }
                        }
                    }

                    // 显示缩放图标的箭头
                    mCurSelectIndex = i;
                    if (Event.current.button == 1)
                    {
                        IsRight = true;
                    }
                }
            }
        }


        if (Event.current.shift)
        {
            if (paths.Count > mCurSelectIndex)
            {
                Color lastColor = Handles.color;
                Handles.color = Color.green;
                Handles.DrawAAPolyLine(2, PP.transform.TransformPoint(paths[mCurSelectIndex].pos), GetMousePostion());
                Handles.color = lastColor;
                SceneView.currentDrawingSceneView.Repaint();
                HandleUtility.Repaint();
            }
        }


        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(Screen.width - 250, 60, 220, 250));
        GUILayout.Box("", GUILayout.Width(210), GUILayout.Height(250));
        GUILayout.EndArea();
        Color tempColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.BeginArea(new Rect(Screen.width - 250, 70, 150, 250));
        GUILayout.Label("按shift进行连线");
        GUILayout.Label("当前操作: " + mCurSelectIndex.ToString());
        GUI.color = tempColor;
        GUILayout.EndArea();
        if (PP.pathData.Count > mCurSelectIndex && mCurSelectIndex >= 0)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 250, 110, 150, 50));
            var pd = PP.pathData[mCurSelectIndex];
            pd.pos = EditorGUILayout.Vector3Field("位置:", pd.pos);
            GUILayout.EndArea();
        }
        else
        {
            mCurSelectIndex = 0;
        }

        GUI.color = tempColor;

        if (!IsRight&&(mousetType == EventType.MouseDown&& Event.current.button == 1))
        {
            //绘制路径
            Vector3 viewworldPos = GetMousePostion();
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("添加节点"), false, CreateNode, viewworldPos);
            menu.ShowAsContext();
            SceneView.currentDrawingSceneView.Repaint();
            HandleUtility.Repaint();
        }

        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(Screen.width - 100, Screen.height - 100, 100, 100));

        GUI.color = Color.red;
        if (GUILayout.Button("删除点", GUILayout.Width(100), GUILayout.Height(25)))
        {
            if(PP.pathData.Count > mCurSelectIndex)
            {
                for(int i = 0; i < PP.pathData.Count;i++)
                {
                    var point = PP.pathData[i];
                    for(int j = point.mIds.Count -1; j >= 0;j--)
                    {
                        if(point.mIds[j] == mCurSelectIndex)
                        {
                            point.mIds.Remove(mCurSelectIndex);
                        }
                        else if(point.mIds[j] > mCurSelectIndex)
                        {
                            point.mIds[j] = point.mIds[j] - 1;
                        }
                    }
                }
                PP.pathData.RemoveAt(mCurSelectIndex);
                mCurSelectIndex = 0;
                SceneView.currentDrawingSceneView.Repaint();
                HandleUtility.Repaint();
            }
        }


        GUILayout.EndArea();
        MyEdior.KeepScene();    
    }
    private Vector3 GetMousePostion()
    {
        Vector3 biao = Event.current.mousePosition;
        Vector3 mousePosition = biao;
        float mult = EditorGUIUtility.pixelsPerPoint;
        mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y * mult;
        mousePosition.x *= mult;
        Vector3 fakePoint = mousePosition;
        fakePoint.z = 0;
        Vector3 viewworldPos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(fakePoint);
        return viewworldPos;
    }

    private void CreateNode(object obj)
    {
        Vector3 objPos = (Vector3)obj;
        objPos = PP.transform.InverseTransformPoint(objPos);
        objPos.z = 0;
        if(PP.pathData.Count > 0)
        {
            objPos.z = PP.pathData[PP.pathData.Count - 1].pos.z;
        }
        PP.RegPoint(objPos);
    }
}
