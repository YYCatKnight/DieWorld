using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;

public class UICheck
{
    //[MenuItem("Assets/检查贴图使用情况")]
    public static void CheckUI()
    {
        string[] tempPath = Selection.assetGUIDs;
        if(tempPath.Length != 0)
        {
            string[] prfabsPath = AssetDatabase.FindAssets("t:Prefab");
            string keyPath = AssetDatabase.GUIDToAssetPath(tempPath[0]);
            string tempName = "";
            for (int i = 0; i < prfabsPath.Length; i++)
            {
                string realPath = AssetDatabase.GUIDToAssetPath(prfabsPath[i]);
                string[] paths = AssetDatabase.GetDependencies(realPath);
                foreach (string path in paths)
                {
                    if (path.IndexOf(keyPath) != -1)
                    {
                        tempName += "被预制件["+realPath+"]包含了\n";
                    }
                }
            }
            UnityEngine.Debug.Log(keyPath + "使用情况\n" + tempName);
        }
    }
}
