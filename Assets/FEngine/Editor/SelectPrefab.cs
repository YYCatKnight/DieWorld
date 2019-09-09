using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
public class SelectPrefabEditor
{
    public string selectKey = "";

    public string[] tempAllStrs;
    public string[] lastStrs;
    public string Realkey = "";
    public string LastKey = "xxxx";
    private int selectId = -1;

    private string mKeyName = ".prefab";
    private string mFileName = "/Assetbundle/";
    public bool mIsAllPath = false;
	public string mProName = "路径";
	private Func<string,bool> mSelectCallBack;
	public SelectPrefabEditor(string keyName, string fileName, string _selectKey,bool isALLPath = false,Func<string,bool> selectCallBack = null)
    {
        mKeyName = keyName;
        mFileName = fileName;
        mIsAllPath = isALLPath;
		mSelectCallBack = selectCallBack;
        Init(_selectKey);
    }
    public SelectPrefabEditor(string _selectKey)
    {
        Init(_selectKey);
    }

    private void Init(string _selectKey)
    {
        InitEnable();
        if (_selectKey != "")
        {
            selectKey = _selectKey + mKeyName;
        }
    }

    public string getSelectKey()
    {
        return selectKey.Replace(mKeyName, "");
    }
    private void InitEnable()
    {
        if (tempAllStrs == null)
        {
            tempAllStrs = AssetDatabase.GetAllAssetPaths();
            List<string> mShowPath = new List<string>();
            for (int i = 0; i < tempAllStrs.Length; i++)
            {
                string path = tempAllStrs[i];
                int index = path.IndexOf(mFileName);
                if (index != -1)
                {
                    if (path.IndexOf(mKeyName) != -1)
                    {
						if (mSelectCallBack != null) 
						{
							if (!mSelectCallBack (path)) 
							{
								continue;
							}
						}
                        string xxPath = path.Substring(index + mFileName.Length);
                        if (mIsAllPath)
                        {
                            mShowPath.Add(path);
                        }
                        else
                        {
                            mShowPath.Add(xxPath);
                        }
                    }
                }
            }
            tempAllStrs = mShowPath.ToArray();
        }
    }

    private void getKeyStr()
    {
        if (Realkey == LastKey)
            return;
        Realkey = Realkey.ToLower();
        LastKey = Realkey;
        List<string> tempKey = new List<string>();
        for (int i = 0; i < tempAllStrs.Length; i++)
        {
            string temp = tempAllStrs[i];
            temp = temp.ToLower();
            if (temp.IndexOf(Realkey) != -1 || tempAllStrs[i] == selectKey)
            {
                tempKey.Add(tempAllStrs[i]);
            }
        }

        if (tempKey.Count <= 1)
        {
            if (tempKey.Count == 0)
            {
                tempKey.Add("none");
            }
            if (tempAllStrs.Length != 0)
            {
                if (Realkey.Length > 0)
                {
                    Realkey = Realkey.Remove(Realkey.Length - 1);
                    getKeyStr();
                    return;
                }
            }
        }

        lastStrs = tempKey.ToArray();
    }


    private int getSelectID()
    {
        for (int i = 0; i < lastStrs.Length; i++)
        {
            if (lastStrs[i] == selectKey)
            {
                return i;
            }
        }
        return 0;
    }


    public void InitGUI()
    {
        GUILayout.Label("关键字搜索:");
        Realkey = GUILayout.TextField(Realkey);

        getKeyStr();

        int curSelectId = getSelectID();

		selectId = EditorGUILayout.Popup(mProName, curSelectId, lastStrs);
   
        if (selectId != curSelectId || selectKey == "")
        {
            selectKey = lastStrs[selectId];
        }
    }

    public List<string> GetSubGather(string Suffix)
    {
        List<string> subs = new List<string>();
        if (tempAllStrs != null)
        {
            for (int i = 0; i < tempAllStrs.Length; i++)
            {
                var path = tempAllStrs[i];
                if (path.IndexOf(Suffix) != -1)
                {
                    subs.Add(path);
                }
            }
        }
        return subs;
    }

}
