using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class FClassEditor<T> : Editor where T :Component
{
    public T myTarget;
    void OnEnable()
    {
        myTarget = target as T;
        Init();
    }

    protected virtual void Init()
    {

    }


    public Rect GetEditorRect(Rect rect)
    {
        Transform transform = myTarget.transform;
        //转到世界坐标
        Vector3 ScenePos = transform.TransformPoint(rect.center);
        ScenePos.z = 0;

        Vector3 SceneSize = transform.TransformPoint(rect.size);
        SceneSize -= transform.transform.TransformPoint(Vector3.zero);

        SceneSize.z = 100;
        Rect worldRect = new Rect();
        worldRect.size = SceneSize;
        worldRect.center = ScenePos;

        Vector3[] tempSize = new Vector3[4];//上下左右
        tempSize[0] = new Vector3((worldRect.xMin + worldRect.xMax) / 2.0f, worldRect.yMax, 0);
        tempSize[1] = new Vector3((worldRect.xMin + worldRect.xMax) / 2.0f, worldRect.yMin, 0);
        tempSize[2] = new Vector3(worldRect.xMin, (worldRect.yMin + worldRect.yMax) / 2.0f, 0);
        tempSize[3] = new Vector3(worldRect.xMax, (worldRect.yMin + worldRect.yMax) / 2.0f, 0);


        float width = 10;
        Vector3 verSize = HandleUtility.WorldToGUIPoint(Vector3.one) - HandleUtility.WorldToGUIPoint(Vector3.zero);
        width /= verSize.x;

        if (Tool.Rect == Tools.current)
        {
            Vector3 tempPos = Handles.FreeMoveHandle(tempSize[0], Quaternion.identity, width, Vector3.zero, Handles.CubeHandleCap);

            worldRect.yMax = Mathf.Clamp(tempPos.y, worldRect.yMin, tempPos.y);

            tempPos = Handles.FreeMoveHandle(tempSize[1], Quaternion.identity, width, Vector3.zero, Handles.CubeHandleCap);

            worldRect.yMin = Mathf.Clamp(tempPos.y, tempPos.y, worldRect.yMax);

            tempPos = Handles.FreeMoveHandle(tempSize[2], Quaternion.identity, width, Vector3.zero, Handles.CubeHandleCap);

            worldRect.xMin = Mathf.Clamp(tempPos.x, tempPos.x, worldRect.xMax);

            tempPos = Handles.FreeMoveHandle(tempSize[3], Quaternion.identity, width, Vector3.zero, Handles.CubeHandleCap);

            worldRect.xMax = Mathf.Clamp(tempPos.x, worldRect.xMin, tempPos.x);
            
        }

        Handles.DrawWireCube(worldRect.center, worldRect.size);

        //逆转坐标
        //转到局部坐标

        Vector3 localPos = transform.InverseTransformPoint(worldRect.center);
        ScenePos.z = 0;
        Vector3 localSize = transform.InverseTransformPoint(worldRect.size);
        localSize -= transform.transform.InverseTransformPoint(Vector3.zero);
        localSize.z = 100;

        Rect localRect = new Rect();
        localRect.size = localSize;
        localRect.center = localPos;
        return localRect;
    }
}
