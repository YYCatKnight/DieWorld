using UnityEngine;
using System.Collections;
using F2DEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;

public class FSettingEdior : EditorWindow
{

    #region 基本配置
    [MenuItem("FEngine/其他/配置窗口")]
    static void OpenWindow()
    {
        //创建窗口
        FSettingEdior window = GetWindow<FSettingEdior>("模块配置");
        window.Show();
        window.Init();
    }

    public class FunSettingData
    {
        public bool isShow = false;
        public string title = "";
        public Action CallBack;
    }

    public List<FunSettingData> mSetting = new List<FunSettingData>();

    private void _UpdateUI(FunSettingData data)
    {
        EditorGUI.indentLevel++;
        data.CallBack();
        EditorGUI.indentLevel--;
    }

    private bool mIsChange = false;
     //绘制窗口时调用
    void OnGUI()
    {
        EditorGUILayout.LabelField("部分配置需要重新加载后生效");
        EditorGUILayout.LabelField("**********************************************");
        for (int i = 0; i < mSetting.Count;i++)
        {
            var sett = mSetting[i];
            sett.isShow = EditorGUILayout.Foldout(sett.isShow, sett.title);
            if(sett.isShow)
            {
                _UpdateUI(sett);
            }
        }
        if (GUI.changed)
        {
            mIsChange = true;
        }
    }

    private void NullCall()
    {
        EditorGUILayout.LabelField("暂未定义配置");
    }

    private void Reg(string title,Action callBack)
    {
        FunSettingData fsd = new FunSettingData();
        fsd.title = title;
        fsd.CallBack = callBack==null?NullCall:callBack;
        mSetting.Add(fsd);
    }

    public class SettingData:Save_Object
    {
        public Dictionary<string, int> mInts = new Dictionary<string, int>();
        public Dictionary<string, string> mStrings = new Dictionary<string, string>();
    }

    private static SettingData mDataSetting;
    private static SettingData GetSettingData()
    {
        if (mDataSetting == null)
        {
            mDataSetting = new SettingData();
            mDataSetting.ReadFile();
        }
        return mDataSetting;
    }

    private static void SetingCofig(string key,int value)
    {
        GetSettingData().mInts[key] = value;
    }

    private static void SetingCofig(string key, string value)
    {
        GetSettingData().mStrings[key] = value;
    }

    public static string GetSettingString(string key)
    {
        string value = "";
        GetSettingData().mStrings.TryGetValue(key, out value);
        return value;
    }

    public static int GetSettingInt(string key)
    {
        int value = 0;
        GetSettingData().mInts.TryGetValue(key, out value);
        return value;
    }

    private static void SaveConfig()
    {
        GetSettingData().SaveFile();
    }

    private void OnDestroy() 
    {   
        if (mIsChange)
        {
            SaveConfig();
        }
        mDataSetting = null;
    }

    #endregion
    private void Init()
    {
        mSetting.Clear();

        PackFileEditor.PackFileSettingType editorPackType = (PackFileEditor.PackFileSettingType)GetSettingInt(PackFileEditor.PACKFILEEDITOR);
        Reg("打包配置", ()=> 
        {
            editorPackType = (PackFileEditor.PackFileSettingType)EditorGUILayout.EnumPopup((System.Enum)editorPackType);
            if(GUI.changed)
            {
                SetingCofig(PackFileEditor.PACKFILEEDITOR,(int)editorPackType);
            }
        });
    }

  

}
