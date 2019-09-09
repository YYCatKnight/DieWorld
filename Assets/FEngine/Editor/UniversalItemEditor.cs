using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using F2DEngine;
using System.Reflection;

[CustomEditor(typeof(FCUniversalItem),true)]
public class UniversalItemEditor : Editor
{
    private FCUniversalItem np;
    private SelectPrefabEditor mSelectPrefab;
    private string mCurSelectName;
    // 添加UniversalPanel组件的GameObject被选中时触发该函数
    void OnEnable()
    {
        np = target as FCUniversalItem;
        mSelectPrefab = new SelectPrefabEditor(np.nTypeKey);
        mCurSelectName = np.nTypeKey;
    }

    // 重写Inspector检视面板
    public override void OnInspectorGUI()
    {
        np.StartInit = EditorGUILayout.Toggle("运行显示", np.StartInit);
        np.StartDataConfig = EditorGUILayout.Toggle("启用数据", np.StartDataConfig);
        if (!string.IsNullOrEmpty(np.mDataConfig))
        {
            if (GUILayout.Button("删除数据文件"))
            {
                np.mDataConfig = null;
            }
        }
        //数据序列化
        if (np.StartDataConfig)
        {
            if(!string.IsNullOrEmpty(np.mDataConfig))
            {
                GUILayout.Label("---------------------------------------------------");
                GUILayout.Label("数据解析:"+np.mDataConfig);
                GUILayout.Label("---------------------------------------------------");
            }
            if (np.mUnitObject != null)
            {
                MyEdior.DrawConverter.DrawObject(np.mUnitObject, null);
                if (GUILayout.Button("保存数据"))
                {
                    np.mDataConfig = StringSerialize.Serialize(np.mUnitObject);
                }              
            }
        }

        string tempName = np.StartConfig ? "保存配置" : "Delete";

        mSelectPrefab.InitGUI();
        np.nTypeKey = mSelectPrefab.getSelectKey();

        if(mCurSelectName != np.nTypeKey)
        {
            mCurSelectName = np.nTypeKey;
        }
        if (np.mUnitObject == null)
        {
            GUILayout.Space(10);
            if (GUILayout.Button("Create"))
            {
                np.SetActive(true);
            }
        }
        else
        {
            GUILayout.Space(10);
            if (GUILayout.Button(tempName))
            {
                GameObject.DestroyImmediate(np.mUnitObject.gameObject);
            }
        }

        MyEdior.KeepScene();
    }
}
