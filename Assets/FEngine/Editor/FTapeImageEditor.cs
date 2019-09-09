using UnityEngine;
using System.Collections;
using F2DEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(FTapeImage))]
public class FTapeImageEditor : Editor
{
    private FTapeImage TI;
    private SelectPrefabEditor mSelectPrefab;
    //private bool IsChange = false;
    private int mCurSelectIndex = 0;
    void OnEnable()
    {  
        TI = target as FTapeImage;  
    }
    void OnSceneGUI()
    {
        if (TI.mMesh != null)
            return;

        float width = 10;
        Vector3 verSize = HandleUtility.WorldToGUIPoint(Vector3.one) - HandleUtility.WorldToGUIPoint(Vector3.zero);
        width /= verSize.x;
        verSize.x = Mathf.Abs(verSize.x)* width;
        verSize.y = Mathf.Abs(verSize.y)* width;
       
        EventType mousetType = Event.current.type;
        List<FTapeImage.TapePoint> pointVector = TI.mBuffsPoint;
        for (int i = 0; i < pointVector.Count; i++)
        {
            Vector3 worldPos = TI.transform.TransformPoint(pointVector[i].pos);
            pointVector[i].pos = TI.transform.InverseTransformPoint(Handles.FreeMoveHandle(worldPos, Quaternion.identity, width, Vector3.one, Handles.SphereHandleCap));
            Vector3 point = HandleUtility.WorldToGUIPoint(worldPos);
            Rect rect = new Rect(point.x- verSize.x/ 2, point.y - verSize .y/ 2, verSize.x, verSize.y);

            ////GUISkin gs = new GUISkin();
            //Color temp = GUI.color;
            //GUI.color = Color.red;
            //GUI.skin.label.fontSize = (int)(verSize.x*0.7f);
            //GUI.skin.label.alignment = TextAnchor.MiddleCenter;
           
            ////// 调试绘制是否计算正确
            //Handles.BeginGUI();
            //GUI.Label(rect, i.ToString());
            //Handles.EndGUI();

            //GUI.color = temp;

            if (mousetType == EventType.MouseDown)
            {
                // 是否鼠标在控制柄上
                if (rect.Contains(Event.current.mousePosition))
                {
                    // 显示缩放图标的箭头
                    mCurSelectIndex = i;
                }
            }
        }



       
        //绘制界面
        //开始绘制GUI
        Handles.BeginGUI();

        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(Screen.width - 250, 50, 220, 250));
        GUILayout.Box("", GUILayout.Width(210),GUILayout.Height(250));
        GUILayout.EndArea();
        Color tempColor = GUI.color;
        GUI.color = Color.red;
        GUILayout.BeginArea(new Rect(Screen.width - 250, 50, 150, 250));
        GUILayout.Label("当前操作: " + mCurSelectIndex.ToString());
        GUI.color = tempColor;
        GUILayout.EndArea();
        if (TI.mBuffsPoint.Count > mCurSelectIndex&&mCurSelectIndex >= 0)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 250, 70, 150, 50));
            FTapeImage.TapePoint ftt = TI.mBuffsPoint[mCurSelectIndex];
            ftt.pos = EditorGUILayout.Vector3Field("位置:", ftt.pos);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(Screen.width - 250, 120, 100, 150));
            EditorGUILayout.LabelField("宽度:");
            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(Screen.width - 210, 120, 100, 150));
            ftt.width = EditorGUILayout.FloatField(ftt.width, GUILayout.Width(40));
            GUILayout.EndArea();
        }
        else
        {
            mCurSelectIndex = 0;
        }
        
       



        //规定GUI显示区域
        GUILayout.BeginArea(new Rect(Screen.width-100, Screen.height-100, 100, 100));

        if (GUILayout.Button("添加点",GUILayout.Width(100),GUILayout.Height(25)))
        {
            FTapeImage.TapePoint ftp = new FTapeImage.TapePoint();
            if (TI.mBuffsPoint.Count == 0)
            {
                ftp.pos = Vector3.zero;
            }
            else
            {
                ftp.pos = TI.mBuffsPoint[TI.mBuffsPoint.Count - 1].pos + new Vector3(width * 2, width * 2, 0);
            }
            TI.mBuffsPoint.Add(ftp);
            mCurSelectIndex = TI.mBuffsPoint.Count - 1;
        }
        GUI.color = Color.red;
        if (GUILayout.Button("删除点", GUILayout.Width(100), GUILayout.Height(25)))
        {
            if (TI.mBuffsPoint.Count > mCurSelectIndex)
            {
                TI.mBuffsPoint.RemoveAt(mCurSelectIndex);
                mCurSelectIndex--;
            }
        }
        GUI.color = tempColor;
        GUILayout.EndArea();

        Handles.EndGUI();

        UpdateResetData();
    }

    private void  UpdateResetData()
    {
        if (GUI.changed)
        {
            TI.SetAllDirty();
            EditorUtility.SetDirty(TI);
        }
    }

    public override void OnInspectorGUI()
    {

        TI.mTexture = (Texture)EditorGUILayout.ObjectField("Texture",TI.mTexture, typeof(Texture),false);

        TI.mSize = EditorGUILayout.IntField("节点数量", TI.mSize);

        if(TI.mMesh == null&& TI.mBuffsPoint.Count < 2)
        {
            TI.mMesh =  (Mesh)EditorGUILayout.ObjectField("Mesh", TI.mMesh, typeof(Mesh), false);
        }

        base.OnInspectorGUI();

        if (TI.mMesh == null)
        {
            if (GUILayout.Button("合并成Mesh", GUILayout.Width(100), GUILayout.Height(25)))
            {
                TI.ComputeMese();
            }
        }
        else
        {
            if (GUILayout.Button("拆分Mesh", GUILayout.Width(100), GUILayout.Height(25)))
            {
                TI.mMesh = null;
            }
        }

        UpdateResetData();
    }
}
