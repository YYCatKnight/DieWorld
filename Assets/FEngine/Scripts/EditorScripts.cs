using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using F2DEngine;
using System.IO;

public class EditorScripts
{
    //免打包模式
    public static NoPackAsset NoPackAssetinstance;
    public class NoPackAsset
    {
        public class PackData
        {
            public string key;
            public string allName;
            public string bunldName;
            public int type;
        }

        
        public class EncryPath
        {
            public string key;
            public string value;
        }

        public PackData[] mlist;
        public string[] mPahtList;
        public string[] mPathEx;
        public EncryPath[] mEncryPath;
        public NoPackAsset()
        {

        } 

        public  List<string> GetExistPathFiles(string path,string endex)
        {
            string tempRealPath = path;
            string tempLow = path.ToLower();
            for (int i = 0; i < mEncryPath.Length; i++)
            {
                var enc = mEncryPath[i];
                if (enc.key == tempLow)
                {
                    tempRealPath = enc.value;
                    break;
                }
            }
            List<string> tempPaths = new List<string>();
            for (int i = 0; i <mPahtList.Length; i++)
            {
                string pblkPath = NoPackAssetinstance.mPahtList[i] + "/" + tempRealPath;
                if (FEPath.Exists(pblkPath))
                {
                    tempPaths = FCommonFunction.GetFiles(pblkPath, tempPaths, endex);
                }
            }
            return tempPaths;
        }

       
        public string GetRealPath(string key)
        {
            string lowKey = key.ToLower();
            int pos = lowKey.LastIndexOf("/");
            if (pos != -1)
            {
                string name = lowKey.Substring(pos+1);
                string path = lowKey.Substring(0, pos);
                for (int i = 0; i < mEncryPath.Length; i++)
                {
                    var enc = mEncryPath[i];
                    if(enc.key == path)
                    {
                        return enc.value + "/" + name;
                    }
                }
            }
            return key;
        }

        public NoPackAsset(List<PackData> list)
        {
            if (list != null)
            {
                mlist = list.ToArray();
                List<string> tempPahtData = new List<string>();
                List<string> tempPathExData = new List<string>();
                List<EncryPath> tempEncryData = new List<EncryPath>();
                List<string> encryKey = new List<string>(); 
                tempPathExData.Add(".prefab");
                tempPathExData.Add("");

                for (int i = 0;i < list.Count;i++)
                {
                    PackData pd = list[i]; 
                    string path = pd.allName;
                    int dPos = path.LastIndexOf(".");
                    int pathPos = path.IndexOf(ResConfig.ASSETBUNDLE);
                    if(dPos != -1&& pathPos != -1)
                    {
                        string DEx = path.Substring(dPos);
                        string noDPath = path.Substring(0, pathPos+ ResConfig.ASSETBUNDLE.Length);

                        string otherPath = path.Substring(pathPos + ResConfig.ASSETBUNDLE.Length+1);
                        otherPath = FEPath.GetDirectoryName(otherPath);
                        string tempOtherPath = FUniversalFunction.GetChunk(otherPath, "[", "]")[0];
                        if(otherPath != tempOtherPath)
                        {
                            if (!encryKey.Contains(tempOtherPath))
                            {
                                EncryPath ep = new EncryPath();
                                ep.key = tempOtherPath.ToLower();
                                ep.value = otherPath;
                                tempEncryData.Add(ep);
                                encryKey.Add(tempOtherPath);
                            }
                        }

                        if (!tempPahtData.Contains(noDPath))
                        {
                            tempPahtData.Add(noDPath);
                        }

                        if (!tempPathExData.Contains(DEx))
                        {
                            tempPathExData.Add(DEx);
                        }
                    }

                }
                mPahtList = tempPahtData.ToArray();
                mPathEx = tempPathExData.ToArray();
                mEncryPath = tempEncryData.ToArray();
            }
        }
        public PackData GetPackData(string name) 
        {
            if (mlist != null)
            {
                for (int i = 0; i < mlist.Length; i++)
                {
                    if (mlist[i].key == name)
                    {
                        return mlist[i];
                    }
                }
            }
            return null;
        }
    }

    public static void CreatePack()
    {
        if (NoPackAssetinstance == null)
        {
            FSaveHandle sd = FSaveHandle.Create(Application.dataPath + "/" + ResConfig.NOPACKPATH, FFilePath.FP_Abs, FOpenType.OT_Binary);
            if(sd.IsLoad)
            {
                NoPackAssetinstance = new NoPackAsset();
                sd.FromObject(NoPackAssetinstance);
            }
        }
    }

    public static List<string> GetExistFilesPath(string path,string endex)
    {
#if UNITY_EDITOR
        CreatePack();
        if (NoPackAssetinstance != null)
        {
           return NoPackAssetinstance.GetExistPathFiles(path, endex);
        }
#endif
        return new List<string>();
    } 

    public static string GetExistPath(string key)
    {
#if UNITY_EDITOR
        CreatePack();
        if (NoPackAssetinstance != null)
        {
            NoPackAsset.PackData packData = NoPackAssetinstance.GetPackData(key.ToLower());
            if (packData != null)
            {
                if (File.Exists(packData.allName))
                {
                    return packData.allName;
                }
            } 

            string realKey = NoPackAssetinstance.GetRealPath(key);
            for (int i = 0; i < NoPackAssetinstance.mPahtList.Length; i++)
            {
                for (int j = 0; j < NoPackAssetinstance.mPathEx.Length; j++)
                {
                    string pblkPath = NoPackAssetinstance.mPahtList[i] + "/"  + realKey + NoPackAssetinstance.mPathEx[j];
                    if (File.Exists(pblkPath))
                    {
                        return pblkPath;
                    }
                }
            }
        }
#endif
        return "";
    }

    public static Object GetNoPackObject<T>(string key) where T : Object
    {

#if UNITY_EDITOR 
        Object ob = null;
        var  ePath = GetExistPath(key);
        if(!string.IsNullOrEmpty(ePath))
        {
           return ob = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(ePath);
        }
#endif
        return null;
    }

    public static void  CreateNoPackAsset(List<NoPackAsset.PackData> pack)
    {
#if UNITY_EDITOR
        NoPackAsset packAsset = new NoPackAsset(pack);
        string path = Application.dataPath + "/" + ResConfig.NOPACKPATH;
        string pathDir = FEPath.GetDirectoryName(path);
        //如果解压到的目录不存在，则报错
        FEPath.CreateDirectory(pathDir);
        FSaveHandle sd = FSaveHandle.Create(path, FFilePath.FP_Abs,FOpenType.OT_Write|FOpenType.OT_Binary);
        sd.PushObject(packAsset);
        sd.Save();
#endif
    }




}

