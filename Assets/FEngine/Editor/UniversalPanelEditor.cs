using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using F2DEngine;


[CustomEditor(typeof(FUniversalPanel))]
public class UniversalPanelEditor : Editor 
{
    private string KeyStr = "F_";
    private FUniversalPanel np;
    private bool isUpdateShow = false;

    ////////////////////////////////
    private bool nOpenCreate = false;
    private bool nChangeKey = false;
    private SelectPrefabEditor mSelectPrefab;
   
    // 添加UniversalPanel组件的GameObject被选中时触发该函数
    void OnEnable()
    {
        np = target as FUniversalPanel;
        KeyStr = np.mKey;
        isUpdateShow = !CheckData(np,KeyStr);
        if(isUpdateShow)
        {
            if (!Application.isPlaying)
            {
                applyData();
            }
        }
        //applyData();
        mSelectPrefab = new SelectPrefabEditor("");
    }


     // 重写Inspector检视面板
    public override void OnInspectorGUI()
    {
            KeyStr = np.mKey;
            EditorGUILayout.LabelField("带有 "+KeyStr+" 的物体将会自动保存");
            nChangeKey = EditorGUILayout.Toggle("打开Key修改按钮", nChangeKey);

            if(nChangeKey)
            {
                GUILayout.Label("输入要保存的Key:");
                np.mKey = GUILayout.TextField(np.mKey);
                if(np.mKey == null||np.mKey == "")
                {
                    np.mKey = "F_";
                }
            }

            nOpenCreate = EditorGUILayout.Toggle("打开创建按钮", nOpenCreate);
            if (nOpenCreate)
            {

                mSelectPrefab.InitGUI();
                GUILayout.Space(10);
                if (GUILayout.Button("Create"))
                {
                    SceneManager.instance.Create(mSelectPrefab.getSelectKey(), np.gameObject);
                }
            }
            else
            {
                GUILayout.Space(10);
                if (isUpdateShow)
                {
                    EditorGUILayout.LabelField("有更新,请点击Apply进行更新");
                }
                if (GUILayout.Button("Apply"))
                {
                    applyData();
                }
            }
        MyEdior.KeepScene();
    }

     static public bool CheckData(FUniversalPanel fp,string keyName)
    {
        Transform[] ts = fp.gameObject.transform.GetComponentsInChildren<Transform>(true);
        List<Transform> tempTf = new List<Transform>();
        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i].gameObject.name.Length > keyName.Length)
            {
                if (ts[i].gameObject.name.Substring(0, keyName.Length) == keyName)
                {
                    tempTf.Add(ts[i]);
                }
            }
          }


        if (fp.mValue.Count == tempTf.Count)
        {
            for (int i = 0; i < tempTf.Count; ++i)
            {
                if (!fp.mValue.Contains(tempTf[i].gameObject))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    void applyData()
    {
        isUpdateShow = false;
        np.ApplyData();
        for (int i = 0; i < np.mValue.Count; i++)
        {
            Debug.Log(np.mValue[i] + "custon");
        }

        FUniversalPanel[] ts = np.transform.GetComponentsInChildren<FUniversalPanel>(true);
        for (int i = 0; i < ts.Length; i++)
        {
            if (ts[i] != np)
            {
                ts[i].ApplyData();
            }
        }
    }
}
