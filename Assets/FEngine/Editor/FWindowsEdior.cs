using UnityEngine;
using System.Collections;
using F2DEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class FWindowsEdior : EditorWindow
{
    //[MenuItem("FEngine/其他/编辑器/算法编辑")]
    //static void OpenWindow()
    //{
    //    //创建窗口
    //    FWindowsEdior window = GetWindow<FWindowsEdior>("F_Window");
    //    window.Show();
    //}


    private float mPaiX = 0;
    private float mPaiY = 0;
    private bool[] boolBuf = new bool[]{false,false,false,false,false,false,false,false,false,false};
    private string[] titleStr = new string[] { "内部排序","A*地图","","","","","","","","","",""};


    //A*地图
    private GameObject mAstartMap;
    private GameObject mGeGeObject;
    private GameObject mWeiQiangObject;
    private List<GameObject> mGeGeList = new List<GameObject>();
    private void CreatePaiXu()
    {
        
        mPaiX = EditorGUILayout.FloatField("X-", mPaiX);
        mPaiY = EditorGUILayout.FloatField("Y-", mPaiY);
        if (GUILayout.Button("排序", GUILayout.Width(100)))
        {
            var selecGameObject = Selection.gameObjects;
            if (selecGameObject.Length == 1)
            {
                Transform curTrans = selecGameObject[0].transform;
                int childNum = curTrans.childCount;
                Vector3 posOne = Vector3.zero;
                for (int i = 0; i < childNum; i++)
                {
                    if (i == 0)
                    {
                        posOne = curTrans.GetChild(i).localPosition;
                        continue;
                    }
                    curTrans.GetChild(i).localPosition = posOne + new Vector3(mPaiX * i, +mPaiY * i, 0);
                }
            }
            else if (selecGameObject.Length > 1)
            {
                Vector3 posOne = Vector3.zero;
                for (int i = 0; i < selecGameObject.Length; i++)
                {
                    if (i == 0)
                    {
                        posOne = selecGameObject[i].transform.localPosition;
                        continue;
                    }
                    selecGameObject[i].transform.localPosition = posOne + new Vector3(mPaiX * i, +mPaiY * i, 0);
                }
            }
        }
        
    }

    private void _UpdateUI(int id)
    {
        EditorGUI.indentLevel++;

        if(id == 0)
        {
            CreatePaiXu();
        }
        else if(id == 1)
        {
            CreateAStarMap();
        }


        EditorGUI.indentLevel--;
    }

    private void CreateAStarMap()
    {

        
    }

    private void ClearGeGeMap()
    {
        for (int i = 0; i < mGeGeList.Count; i++)
        {
            if (mGeGeList[i] != null)
            {
                GameObject.DestroyImmediate(mGeGeList[i]);
            }
        }
        mGeGeList.Clear();
    }

     //绘制窗口时调用
    void OnGUI()
    {
        EditorGUILayout.LabelField("功能模块");
        for (int i = 0; i < boolBuf.Length;i++)
        {
            if (titleStr.Length > i && titleStr[i] != "")
            {
                boolBuf[i] = EditorGUILayout.Foldout(boolBuf[i], titleStr[i]);
                if(boolBuf[i])
                {
                    _UpdateUI(i);
                }
            }
            else
            {
                break;
            }
        }
    }
}
