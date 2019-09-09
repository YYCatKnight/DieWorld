using UnityEngine;
using System.Collections;
using F2DEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor.SceneManagement;

public class FPackageEditor : EditorWindow
{
    public static List<string> mZipFilesName = new List<string>(){ ResConfig.FDESASSETBUNDLE };//需要压缩的文件夹
                                                                                                //不过滤的后缀，加载的时候,有时候要带后缀(名字中要加的后缀)
                                                                                                // public static string SUFFIX = ".asset;"
    /// /////////////////////////////////////////////////////
    public static string F_NOCOPYSTREAM = "NoneStream";//不拷贝缓存目录
    public static string F_EX = PackType.part.ToString();//"part"; //分开打包
    //public static string F_ASYNC = PackType.asy.ToString(); //"asy";//异步加载
    //public static string F_NOBUNDLE = PackType.nb.ToString();//"nb";//不打Bundle
    //public static string F_Only = PackType.only.ToString(); //"only";//被引用才会打包

    public enum PackType
    {
        none = 0,//默认
        part = 1 << 1,//分开打包
        asy = 1 << 2,//异步加载
        nb = 1 << 3,//不打包
        only = 1 << 4,//引用才打包
        micro = 1 << 5,//微端资源
        cache = 1 << 6,//缓存资源
    }

    [MenuItem("FEngine/打开临时目录", false, 1)]
    static void OpenLinShiPath()
    {
        System.Diagnostics.Process.Start(SceneManager.WRPath);
    }


    //[MenuItem("FEngine/删除Bundle")]
    static void OpenWindow()
    {
        string path = SceneManager.ASSETBUNDLE.Replace("file://", "");
        MyEdior.DeletFile(path);
    }

    [MenuItem("FEngine/开发/生成不打包配置", false, 90)]
    static void OpenBundleWindow()
    {
        CreatePackage(false);
    }

    public static bool ClearAssetBundleName()
    {
        {
            string[] assetbundles = AssetDatabase.GetAllAssetBundleNames();
            int length = assetbundles.Length;
            string[] oldAssetBundleNames = new string[assetbundles.Length];
            for (int i = 0; i < length; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("加载数据资源", "正在加载 " + assetbundles[i], (float)i / length))
                    return false;
                oldAssetBundleNames[i] = assetbundles[i];
            }

            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("清除旧资源", "正在清除 " + oldAssetBundleNames[j], (float)j / length))
                {
                    return false;
                }

                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
        }
        return true;
    }

    public class ProPathHandle
    {
        public string path;
        public string realPath = "";
        public int packType;
    }

    public class PackFileData
    {
        public ProPathHandle ProHandle;
        public string realFileName;
        public string budleName;
    }
    public class EditorDay:Save_Object
    {
        public int day = -1;
        public bool IsSpeedPack = false;
    }

    public static void CreatePackage(bool isCreateBundle = true)
    {

        List<string> NoBundlesFiles = new List<string>();
        string path = SceneManager.STREAMINGPATH.Replace("file://", "") + ResConfig.FDESASSETBUNDLE;//正式路径
        string tempPath = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", "")).Replace("Assets/StreamingAssets", "FPartPack/") + ResConfig.FDESASSETBUNDLE;//打包缓存路径
        //删除上次数据
        if (FEPath.Exists(path))
        {
            MyEdior.DeletFile(path);
        }

        if (File.Exists(path + ResConfig.FZIPNAMEEX))
        {
            File.Delete(path + ResConfig.FZIPNAMEEX);
        }

        AssetDatabase.Refresh();

        string onlyKey = "[" + PackType.only + "]";

        SelectPrefabEditor spe = new SelectPrefabEditor(".", "/" + ResConfig.ASSETBUNDLE + "/", "", true);

        EditorDay day = new EditorDay();
        day.SaveFile();
        bool IsSpeedPack = day.IsSpeedPack; //连续打包,不用清空，快速打包模式
        day.IsSpeedPack = true;
        day.SaveFile();

        if (!IsSpeedPack)
        {
            if (isCreateBundle)
            {
                IsSpeedPack = !EditorUtility.DisplayDialog("打包提示", "是否强制清空缓存打包?\n(提示:差异包不建议清空,可能产生Md5码的变化)", "清空", "不清空");
            }
        }

        if (isCreateBundle && !IsSpeedPack)
        {
            if (!ClearAssetBundleName())
            {
                Debug.LogError("强制终止打包");
                EditorUtility.ClearProgressBar();
                return;
            }
        }

        if (!IsSpeedPack)
        {
            if (FEPath.Exists(tempPath))
            {
                MyEdior.DeletFile(tempPath);
            }
        }


        EditorUtility.ClearProgressBar();
        FEPath.CreateDirectory(path);
        FEPath.CreateDirectory(tempPath);

        BundleManager.BundleConfig bundleConfig = new BundleManager.BundleConfig();
        Dictionary<string, Dictionary<string, BundleManager.BundleConfig.BundleFileData>> mBundleFileDatas = new Dictionary<string, Dictionary<string, BundleManager.BundleConfig.BundleFileData>>();
        Dictionary<string, int> mBundleEncrypts = new Dictionary<string, int>();
        List<EditorScripts.NoPackAsset.PackData> packData = new List<EditorScripts.NoPackAsset.PackData>();
        List<AssetImporter> Importers = new List<AssetImporter>();


        var pmode = Enum.GetValues(typeof(FPackageEditor.PackType));
        Dictionary<string, int> PackTypeDic = new Dictionary<string, int>();
        foreach (var fp in pmode)
        {
            FPackageEditor.PackType tempType = (FPackageEditor.PackType)fp;
            PackTypeDic[tempType.ToString()] = (int)tempType;
        }

        ScriptsTime.Begin();
        //先处理文件夹一下得到打包需要的数据
        Dictionary<string, int> HeadPath = new Dictionary<string, int>();
        Dictionary<string, ProPathHandle> datas = new Dictionary<string, ProPathHandle>();
        Dictionary<string, int> allMushResouress = new Dictionary<string, int>();//所有引用的资源
        List<PackFileData> packFiles = new List<PackFileData>();
        Dictionary<string, string> WarningFile = new Dictionary<string, string>();
        for (int i = 0; i < spe.tempAllStrs.Length; i++)
        {
            PackFileData pfd = new PackFileData();
            packFiles.Add(pfd);
            string realName = spe.tempAllStrs[i];
            pfd.realFileName = realName;
            string pathFile = FEPath.GetDirectoryName(realName);
            ProPathHandle ProHandle = null;
            if (!datas.TryGetValue(pathFile, out ProHandle))
            {
                ProHandle = new ProPathHandle();
                datas[pathFile] = ProHandle;
                ProHandle.path = pathFile;
                int headPos = pathFile.IndexOf("/" + ResConfig.ASSETBUNDLE + "/");
                string otherFilePath = "";
                if (headPos == -1)
                {
                    //纯路经
                    otherFilePath = pathFile + "/";
                }
                else
                {
                    otherFilePath = pathFile.Substring(0, headPos + 1);
                }

                string[] tempHeadPaths = otherFilePath.Split('/');
                string tString = "";
                for (int t = 0; t < tempHeadPaths.Length - 1; t++)
                {
                    HeadPath[tString + tempHeadPaths[t]] = 1;
                    tString += (tempHeadPaths[t] + "/");
                }
                HeadPath[tString + ResConfig.ASSETBUNDLE] = 0;

                //文件夹打包类型获取
                int packType = 0;
                if (headPos != -1)
                {
                    string validPath = pathFile.Substring(headPos + ResConfig.ASSETBUNDLE.Length + 2);
                    var keyEncrypt = FUniversalFunction.GetChunk(validPath, "[", "]");
                    ProHandle.realPath = keyEncrypt[0].ToLower();
                    if (ProHandle.realPath != "")
                    {
                        if (WarningFile.ContainsKey(ProHandle.realPath))
                        {
                            Debug.Log("<color=#660000>存在相同的相对路径打包文件</color>:" + WarningFile[ProHandle.realPath] + "<color=#660000>****</color>" + pathFile);
                        }
                        WarningFile[ProHandle.realPath] = pathFile;
                    }

                    string[] proPaths = validPath.Split('/');
                    string startFile = pathFile.Substring(0, headPos + ResConfig.ASSETBUNDLE.Length + 1);
                    for (int p = 0; p < proPaths.Length; p++)
                    {
                        startFile = startFile + "/" + proPaths[p];
                        var parms = PackFileEditor.PackFileConfig.GetPackType(startFile);
                        if (parms != null)
                        {
                            packType |= parms.type;
                        }
                    }

                    for (int p = 1; p < keyEncrypt.Count; p++)
                    {
                        packType |= PackTypeDic[keyEncrypt[p]];
                    }
                }
                ProHandle.packType = packType;
            }

            int lastIndex = realName.LastIndexOf("/");
            string fileName = realName.Substring(lastIndex + 1).ToLower();
            pfd.budleName = string.IsNullOrEmpty(ProHandle.realPath) ? fileName : (ProHandle.realPath + "/" + fileName);
            pfd.ProHandle = ProHandle;
            if (!PackFileEditor.IsHaveSameWinType(ProHandle.packType, (int)PackType.only))
            {
                string[] paths = AssetDatabase.GetDependencies(realName);
                for (int p = 0; p < paths.Length; p++)
                {
                    allMushResouress[paths[p]] = 1;
                }
            }
        }


        int AssetsNum = "Assets".Length + 1;

        for (int i = 0; i < packFiles.Count; i++)
        {
            PackFileData pf = packFiles[i];
            ProPathHandle pHandle = pf.ProHandle;
            string spePath = pf.realFileName;
            string pathName = pHandle.path;

            if (EditorUtility.DisplayCancelableProgressBar("打包数据资源", "转换进度： " + spePath, (float)i / spe.tempAllStrs.Length))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            EditorScripts.NoPackAsset.PackData enp = new EditorScripts.NoPackAsset.PackData();
            enp.allName = spePath;

            AssetImporter ai = AssetImporter.GetAtPath(spePath);
            int packType = pf.ProHandle.packType;//PackFileEditor.IsHaveSameWinType(ProHandle.packType, (int)PackType.only)

            if (ai != null)
            {
                Importers.Add(ai);
                string houPro = "";

                string budleName = pf.budleName;
                string encryptFile = budleName;

                //需要智能排除
                if (PackFileEditor.IsHaveSameWinType(packType, (int)PackType.only))
                {
                    if (!allMushResouress.ContainsKey(spePath))
                    {
                        ai.assetBundleName = "";
                        continue;
                    }
                }
                else
                {
                    packData.Add(enp);
                }

                bool isNoBundle = PackFileEditor.IsHaveSameWinType(packType, (int)PackType.nb);

                int headPos = budleName.LastIndexOf(".");
                string typeName = BundleManager.PrefabType;
                if (headPos != -1)
                {
                    string hou = budleName.Substring(headPos);
                    if (hou != "" && BundleManager.AssetType.IndexOf(hou) != -1)
                    {
                        houPro = hou;
                    }
                    budleName = budleName.Substring(0, headPos);
                    typeName = hou.ToLower();
                }

                string pathFile = "";
                headPos = budleName.LastIndexOf("/");
                if (headPos != -1)
                {
                    pathFile += (budleName.Substring(0, headPos));
                }

                Dictionary<string, BundleManager.BundleConfig.BundleFileData> bmbfds = new Dictionary<string, BundleManager.BundleConfig.BundleFileData>();
                mBundleEncrypts[pathFile] = packType;
                if (mBundleFileDatas.ContainsKey(pathFile))
                {
                    bmbfds = mBundleFileDatas[pathFile];
                }
                else
                {
                    if (!isNoBundle)
                    {
                        mBundleFileDatas[pathFile] = bmbfds;
                    }
                }
                enp.key = (budleName + houPro).ToLower();
                string assetbundleName = budleName + ResConfig.ASSETBUNDLESUFFIXES;
                //路径打包规则
                if (pathFile != "")
                {
                    string txtName = pathFile.ToLower();
                    int filePos = pathFile.LastIndexOf("/");
                    if (filePos != -1)
                    {
                        txtName = txtName.Substring(filePos + 1);
                    }
                    if (!PackFileEditor.IsHaveSameWinType(packType, (int)PackType.part))
                    {
                        assetbundleName = pathFile + "/" + txtName + ResConfig.ASSETBUNDLESUFFIXES;
                    }
                }

                enp.type = packType;
                if (isNoBundle)
                {
                    string CopyBunld = encryptFile;
                    string copyPath = tempPath + "/" + encryptFile;//修改
                    string dir = FEPath.GetDirectoryName(copyPath);
                    if (isCreateBundle)
                    {
                        if (!FEPath.Exists(dir))
                        {
                            FEPath.CreateDirectory(dir);
                        }

                        if (File.Exists(copyPath))
                        {
                            File.Copy(spePath, copyPath, true);
                        }
                        else
                        {
                            File.Copy(spePath, copyPath);
                        }
                        NoBundlesFiles.Add(copyPath);
                    }
                    enp.key = CopyBunld.ToLower();
                    enp.bunldName = enp.key;
                }
                else
                {
                    ai.assetBundleName = assetbundleName.ToLower();
                    enp.bunldName = ai.assetBundleName;
                    //特殊处理
                    if (ai is TextureImporter)
                    {
                        TextureImporter ti = ai as TextureImporter;
                        if (ti.textureType == TextureImporterType.Sprite)
                        {
                            typeName = BundleManager.SpriteType;
                        }
                        else
                        {
                            typeName = BundleManager.TextureType;
                        }

                        string tagName = ai.assetBundleName.Replace(ResConfig.ASSETBUNDLESUFFIXES, ".PackTag");

                        if (PackFileEditor.IsHaveSameWinType(packType, (int)PackType.part))
                        {
                            tagName = "";
                        }

                        if (tagName != ti.spritePackingTag)
                        {
                            ti.spritePackingTag = tagName;
                            ti.SaveAndReimport();
                        }
                    }

                    BundleManager.BundleConfig.BundleFileData fd = new BundleManager.BundleConfig.BundleFileData();
                    fd.bundleFile = assetbundleName.ToLower();
                    fd.resFileName = (budleName.Replace(pathFile + "/", "") + houPro).ToLower();
                    fd.resType = typeName;
                    fd.mIsCache = PackFileEditor.IsHaveSameWinType(packType, (int)PackType.cache);
                    bmbfds[fd.resFileName] = fd;
                }
            }
        }

        ScriptsTime.Debug("打包资源完成");

        //创建免打包配置
        EditorScripts.CreateNoPackAsset(packData);
        //创建文件夹外观表现
        PackFileEditor.PackFileConfig.SetHeadFile(HeadPath);

        ScriptsTime.ShowTime("计算打包配置文件");
        if (isCreateBundle)
        {
            foreach (var ke in mBundleFileDatas)
            {
                var encrypts = mBundleEncrypts[ke.Key];
                BundleManager.BundleConfig.PathBundles bbf = new BundleManager.BundleConfig.PathBundles();
                bbf.path = ke.Key;
                bbf.fileBundles = ke.Value;
                bbf.mIsAsync = PackFileEditor.IsHaveSameWinType(encrypts, (int)PackType.asy);
                bbf.mIsOnly = !PackFileEditor.IsHaveSameWinType(encrypts, (int)PackType.part);
                bundleConfig.pathBundles[ke.Key] = bbf;
            }

            //按时间设置更新版本
            bundleConfig.versionId = MyEdior.GetTimeVersion();
            PlayerPrefs.SetInt("versionId", bundleConfig.versionId);//记录版本号
            FSaveHandle sd = FSaveHandle.Create(tempPath + "/" + ResConfig.ASSETBUNDLECONFIFILE, FFilePath.FP_Abs, FOpenType.OT_Write);
            sd.PushObject(bundleConfig);
            sd.Save();

            if (IsSpeedPack)
            {
                BuildPipeline.BuildAssetBundles(tempPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            }
            else
            {
                BuildPipeline.BuildAssetBundles(tempPath, BuildAssetBundleOptions.ForceRebuildAssetBundle| BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            }
        }
        //拷贝
        MyEdior.CopyDirectory(tempPath, path);

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
        CreateStreamingConfig();

        //清空配置文件
        if(NoBundlesFiles.Count != 0)
        {
            for(int i = 0; i < NoBundlesFiles.Count;i++)
            {
                File.Delete(NoBundlesFiles[i]);
                Debug.Log("清空了其他未知文件:" + NoBundlesFiles[i]);
            }
            NoBundlesFiles.Clear();
        }
    }


    [MenuItem("FEngine/打包/更新所有数据", false)]
    public static void ApplyAllData()
    {
        ScriptsTime.BeginTag("pack");
        if (AssetEdior.CreateAllAssetData())
        {
            CreatePackage(true);
        }
        ScriptsTime.ShowTimeTag("pack", "完整打包");
    }

    [MenuItem("FEngine/打包/设置/生成主包信息", false, 98)]
    public static void ApplyCreateMainPart()
    {
        if (EditorUtility.DisplayDialog("主包生成", "你确定要生成包文件吗,以前的主包文件将会被替换掉?", "确定", "取消"))
        {
            CretatePartPack(true);
        }
    }

    [MenuItem("FEngine/打包/生成差异包", false, 97)]
    public static void ApplyCreatePart()
    {
        CretatePartPack(false);
    }

   

    public class MicroFile
    {
        public class Data
        {
            public string bundleName;
            public string md5;
        }
        public List<Data> datas = new List<Data>();
    }

    [MenuItem("FEngine/打包/删减成微端包", false, 96)]
    public static void ApplyMicroMain()
    {
        ApplyMicroPart(true);
    }

    [MenuItem("FEngine/打包/设置/生成微端资源", false, 95)]
    public static void ApplyMicroResoure()
    {
        if (EditorUtility.DisplayDialog("微端资源生成", "你确定要生成微端资源文件吗,以前的微端资源文件将会被替换掉?", "确定", "取消"))
        {
            ApplyMicroPart(false);
        }
    }

    public static void ApplyMicroPart(bool isMain)
    {
        string tempPath = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", "")) +"/"+ResConfig.FDESASSETBUNDLE;//打包缓存路径
        string microTempPath = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", "")).Replace("Assets/StreamingAssets", "FPartPack/Micro/") + ResConfig.FDESASSETBUNDLE;
        string microPath = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", "")).Replace("Assets/StreamingAssets", "FPartPack/Micro/");
        MicroFile micros = new MicroFile();
        if (isMain)
        {
            string zip = microPath + ResConfig.MicroBundleName + ".zip";
            if (File.Exists(zip))
            {
                string streamConfig = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", ""));
                FSaveHandle firstData = FSaveHandle.Create(streamConfig + "/" + ResConfig.STREAMINGASSETSCONFIG, FFilePath.FP_Abs);
                if (!firstData.IsLoad)
                {
                    Debug.LogError("没找到新配置文件:" + streamConfig + "/" + ResConfig.STREAMINGASSETSCONFIG + "操作失败");
                    return;
                }
                BundleManager.StreamConfig firstStreamConfi = new BundleManager.StreamConfig();
                firstData.FromObject(firstStreamConfi);
                if(!string.IsNullOrEmpty(firstStreamConfi.microMD5))
                {
                    Debug.LogError("已经是微端包,无需重复生成");
                    return;
                }

                EditorUtility.DisplayCancelableProgressBar("微端生成中", "正在处理... ", 0);
                List<string> changeFiles = new List<string>();
                FZipTool.UnZip(zip, FEPath.GetDirectoryName(zip), ResConfig.FZIPPASS);
                AssetDatabase.Refresh();
                FSaveHandle msave = FSaveHandle.Create(microTempPath + "/" + ResConfig.MicroBundleName, FFilePath.FP_Abs, FOpenType.OT_Txt);
                if (msave.IsLoad)
                {
                    msave.FromObject(micros);
                    for (int i = 0; i < micros.datas.Count; i++)
                    {                       
                        var data = micros.datas[i];
                        EditorUtility.DisplayCancelableProgressBar("微端生成中", "正在处理：" + data.bundleName, i / (float)micros.datas.Count);
                        var realyPath = tempPath + "/" + data.bundleName;
                        if (File.Exists(realyPath))
                        {
                            var md5 = FEngineManager.GetMD5HashFromFile(realyPath);
                            if (data.md5 == md5)
                            {
                                File.Delete(realyPath);
                                changeFiles.Add(data.bundleName);
                            }
                        }
                        else
                        {
                            Debug.LogError("文件拷贝失败:" + realyPath);
                        }
                    }

                    //更改配置文件
                    firstStreamConfi.microMD5 = FEngineManager.GetMD5HashFromFile(zip);
                    firstStreamConfi.microsFiles = changeFiles;
                    firstStreamConfi.zipSize = new System.IO.FileInfo(zip).Length / 1024;
                    firstData.PushObject(firstStreamConfi);
                    firstData.Save();
                    Debug.Log("微端生成成功");
                    EditorUtility.ClearProgressBar();
                    
                }
                else
                {
                    Debug.LogError(ResConfig.MicroBundleName+"微端资源列表没有找到");
                }
            }
            else
            {
                Debug.LogError("微端资源文件没有找到");
            }
        }
        else
        {
            //读取配置文件
            FSaveHandle sd = FSaveHandle.Create(Application.dataPath + "/" + ResConfig.NOPACKPATH, FFilePath.FP_Abs, FOpenType.OT_Binary);
            if (sd.IsLoad)
            {
                List<string> haveMicros = new List<string>();
                EditorScripts.NoPackAsset nopack = new EditorScripts.NoPackAsset();
                sd.FromObject(nopack);
                EditorUtility.DisplayCancelableProgressBar("微端资源处理", "正在处理... ", 0);
                for (int i = 0; i < nopack.mlist.Length; i++)
                {
                    var data = nopack.mlist[i];
                    if (PackFileEditor.IsHaveSameWinType(data.type, (int)PackType.micro))
                    {
                        EditorUtility.DisplayCancelableProgressBar("微端资源处理", "正在处理：" + data.bunldName, i / (float)nopack.mlist.Length);
                        //拷贝文件到微端
                        var realyPath = tempPath + "/" + data.bunldName;
                        if (File.Exists(realyPath))
                        {
                            if (!haveMicros.Contains(data.bunldName))
                            {
                                haveMicros.Add(data.bunldName);
                                MicroFile.Data md = new MicroFile.Data();
                                md.bundleName = data.bunldName;
                                md.md5 = FEngineManager.GetMD5HashFromFile(realyPath);
                                micros.datas.Add(md);
                                var copyPath = microTempPath + "/" + data.bunldName;
                                string dir = FEPath.GetDirectoryName(copyPath);
                                if (!FEPath.Exists(dir))
                                {
                                    FEPath.CreateDirectory(dir);
                                }
                                if (File.Exists(copyPath))
                                {
                                    File.Copy(realyPath, copyPath, true);
                                }
                                else
                                {
                                    File.Copy(realyPath, copyPath);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError("文件拷贝失败:" + realyPath);
                        }
                    }
                }

                FSaveHandle msave = FSaveHandle.Create(microTempPath + "/" + ResConfig.MicroBundleName, FFilePath.FP_Abs, FOpenType.OT_Txt | FOpenType.OT_Write);
                msave.PushObject(micros);
                msave.Save();

                EditorUtility.DisplayCancelableProgressBar("微端资源处理", "压缩文件中", 0.3f);

                //打成微包
                FZipTool.ZipDirectory(microTempPath, microPath, ".meta.delete", ResConfig.MicroBundleName);
                //删除压缩前的文件夹
                EditorUtility.ClearProgressBar();
                string beiFen = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", ""));
                string copyPack = beiFen.Replace("Assets/StreamingAssets", "FTempCopyPack/Micro_" + DateTime.Now.ToString("MM_dd--HH_mm") + ".zip");
                FEPath.CreateDirectory(FEPath.GetDirectoryName(copyPack));
                File.Copy(microPath + "/" + ResConfig.MicroBundleName + ".zip", copyPack);              
                Debug.Log("微端文件生成成功");
                AssetDatabase.Refresh();
                System.Diagnostics.Process.Start(microPath + "/");
            }
            else
            {
                Debug.LogError(Application.dataPath + "/" + ResConfig.NOPACKPATH + "没有找到配置文件");
            }
        }
        MyEdior.DeletFile(microTempPath);
    }

    private static void WaitTime(float time)
    {
        var now = System.DateTime.Now;
        while ((System.DateTime.Now - now).TotalSeconds < time)
        {
        }
    }

    public static void CretatePartPack(bool isMain)
    {
        string copyPath = FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", ""));
        string newPath = copyPath.Replace("StreamingAssets", "_TempStreamingAssets");

        FSaveHandle firstData = FSaveHandle.Create(copyPath + "/" + ResConfig.STREAMINGASSETSCONFIG, FFilePath.FP_Abs);
        if (!firstData.IsLoad)
        {
            Debug.LogError("没找到新配置文件:" + copyPath + "/" + ResConfig.STREAMINGASSETSCONFIG + "操作失败");
            return;
        }

        BundleManager.StreamConfig firstStreamConfi = new BundleManager.StreamConfig();
        firstData.FromObject(firstStreamConfi);

        MyEdior.DeletFile(newPath);
        MyEdior.CopyDirectory(copyPath, newPath);
        AssetDatabase.Refresh();

        CreateConfig(newPath,firstStreamConfi,0);

        string playFile = "";
        string mainPack = copyPath.Replace("Assets/StreamingAssets", "FPartPack/MainPack");
        
        if (isMain)
        {
            string copyPack = copyPath.Replace("Assets/StreamingAssets", "FTempCopyPack/Pack_" + DateTime.Now.ToString("MM_dd--HH_mm")); 
            MyEdior.DeletFile(mainPack);

            FEPath.CreateDirectory(mainPack);

            EditorUtility.DisplayCancelableProgressBar("备份资源", "正在处理 ",0.5f);
            File.Copy(newPath + "/" + ResConfig.STREAMINGASSETSCONFIG, mainPack + "/"+ ResConfig.STREAMINGASSETSCONFIG, true);
            MyEdior.CopyDirectory(copyPath, copyPack+"/StreamingAssets");
            File.Copy(newPath + "/" + ResConfig.STREAMINGASSETSCONFIG, copyPack + "/" + ResConfig.STREAMINGASSETSCONFIG, true);
            FZipTool.ZipDirectory(copyPack, FEPath.GetDirectoryName(copyPack)+"/");
            MyEdior.DeletFile(copyPack);
            EditorUtility.ClearProgressBar();
            playFile = mainPack;
        }
        else
        {
          
            //加载新配置
            string newstreamingPath = newPath + "/" + ResConfig.STREAMINGASSETSCONFIG;
            FSaveHandle sd = FSaveHandle.Create(newstreamingPath, FFilePath.FP_Abs);
            if (sd == null)
            {
                Debug.LogError("没找到新配置文件:" + ResConfig.STREAMINGASSETSCONFIG + "对比文件失败");
            }
            else
            {
                BundleManager.StreamConfig NewStreamConif = new BundleManager.StreamConfig();
                sd.FromObject(NewStreamConif);
                //加载旧配置
                string lastStremingPath = copyPath.Replace("Assets/StreamingAssets", "FPartPack/MainPack") +"/" + ResConfig.STREAMINGASSETSCONFIG;
                sd = FSaveHandle.Create(lastStremingPath,FFilePath.FP_Abs);

                if (sd == null)
                {
                    Debug.LogError(lastStremingPath + "文件夹下,没有找到旧的配置文件" + ResConfig.STREAMINGASSETSCONFIG + "对比文件失败");
                }
                else
                {
                    //曾经变化文件
                    string changeFile = mainPack + "/"+ ResConfig.FCACHEFILE;
                    BundleManager.LegacyPackFile cfiles = new BundleManager.LegacyPackFile();
                    FSaveHandle cSave = FSaveHandle.Create(changeFile,FFilePath.FP_Abs);
                    if (cSave != null)
                    {
                        cSave.FromObject(cfiles);
                    }


                    BundleManager.StreamConfig LastStreamConif = new BundleManager.StreamConfig();
                    sd.FromObject(LastStreamConif);

                    if (NewStreamConif.versionId <= LastStreamConif.versionId)
                    {
                        Debug.LogError(ResConfig.STREAMINGASSETSCONFIG + ":新的配置文件versionId比旧配置文件要低,对比文件失败");
                    }
                    else
                    {
                        //精简文件
                        for (int i = 0; i < NewStreamConif.fileDatas.Count; i++)
                        {
                            var newData = NewStreamConif.fileDatas[i];
                            string tempBundleName = newData.fileName;
                            string newMd5 = NewStreamConif.GetMd5(tempBundleName);
                            string lastMd5 = LastStreamConif.GetMd5(tempBundleName);
                            if(cfiles.Files.ContainsKey(newData.fileName) )//&& newData.path.Contains("fxlua"))
                            {
                                ScriptsTime.Debug("以前版本有变动的文件:" + newData.path + "==" + tempBundleName);
                            }
                            else if ((newMd5 != lastMd5 || lastMd5 == "") )//&& newData.path.Contains("fxlua"))
                            {
                                //文件有更新
                                ScriptsTime.Debug("产生新的文件:" + newData.path + "==" + tempBundleName);
                                cfiles.Files[newData.fileName] = 0;
                                string exten = FEPath.GetExtension(newData.fileName);
                                if (exten == ResConfig.ASSETBUNDLESUFFIXES)
                                {
                                    cfiles.Bundles[newData.fileName.Replace(ResConfig.FDESASSETBUNDLE + "/", "")] = 1;
                                }
                            }
                            else
                            {
                                string spePath = newPath + "/"+ newData.fileName;
                                File.Delete(spePath);
                            }
                        }

                        
                        string cachefile = newPath + "/" + ResConfig.FDESASSETBUNDLE + "/"+  ResConfig.FCACHEFILE;
                        cSave = FSaveHandle.Create(cachefile,FFilePath.FP_Abs,FOpenType.OT_Write);
                        cSave.PushObject(cfiles);
                        cSave.Save();
                        File.Copy(cachefile, changeFile,true);


                        //重新生成压缩文件
                        int versioin = firstStreamConfi.versionId;
                        CreateConfig(newPath,firstStreamConfi,1);
                        string partversionFile = "";
                        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                        {
                            partversionFile = copyPath.Replace("Assets/StreamingAssets", "FPartPack/" + versioin+ "/ios1");
                        }
                        else
                        {
                            partversionFile = copyPath.Replace("Assets/StreamingAssets", "FPartPack/" + versioin+"/az1");
                        }
                        MyEdior.DeletFile(partversionFile);
                        MyEdior.CopyDirectory(newPath, partversionFile);
                        var t = FSaveHandle.Create(partversionFile+ "/version", FFilePath.FP_Abs, FOpenType.OT_Write|FOpenType.OT_Txt);
                        string md5 = FCommonFunction.GetMD5HashFromFile(partversionFile+ "/FAssetbundle.zip");
                        int type = EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS ? 0:1;
                        t.SetContext("{\"version\":"+ versioin.ToString()+ ",\"md5\":\""+ md5+",\"type\":\"" + type.ToString() + "\"}");
                        t.Save();
                        File.Delete(partversionFile + "/FAssetbundle.zip.meta");
                        File.Delete(partversionFile + "/FStreamingAssets");
                        File.Delete(partversionFile + "/FStreamingAssets.meta");
                        playFile = partversionFile;
                    }
                }
            }
        }
        MyEdior.DeletFile(newPath);
        AssetDatabase.Refresh();
        if (playFile != "")
        {
            System.Diagnostics.Process.Start(playFile);
            Debug.Log(isMain ? "主包生成完成" : "分包生成完成");
        }
    }
    
    private static int GetResType(BundleManager.StreamConfig.NormalFile file)
    {
        if(file.resType == ResConfig.FZIPNAMEEX)
        {
            return (int)BundleManager.FResFileType.FT_Zip;
        }
        if(file.resType == ".unity3d"|| file.resType == ".manifest"|| file.resType == ".meta")
        {
            return (int)BundleManager.FResFileType.FT_Unity;
        }
        else if(string.IsNullOrEmpty(file.path))
        {
            return (int)BundleManager.FResFileType.FT_Null;
        }
        return (int)BundleManager.FResFileType.FT_F;
    }

    //OperationType 0 解压,1压缩,2不处理
    public static void CreateConfig(string stream,BundleManager.StreamConfig copyStream,int OperationType)
    {
        long fileSize = 0;
        string streamTempPath = stream+"/";
        string streamingPath = streamTempPath + ResConfig.STREAMINGASSETSCONFIG;

        EditorUtility.DisplayCancelableProgressBar("打包资源", "正在处理 " + stream, 0.3f);
        string unKey = ".meta";
        if(OperationType == 2)
        {
            if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                OperationType = 1;
                unKey = ".meta.delete.unity3d.manifest";
            }
        }
        if (OperationType != 2)
        {
            //插入压缩,解压流程
            for (int i = 0; i < mZipFilesName.Count; i++)
            {
                string pathFile = streamTempPath + mZipFilesName[i];
                string zipDir = FEPath.GetDirectoryName(pathFile) + "/";
                if (OperationType == 1)
                {
                    if (FEPath.Exists(pathFile))
                    {
                        //string unKey = ".meta";//: ".meta.delete.unity3d.manifest";
                        string outPath = FZipTool.ZipDirectory(pathFile, zipDir, unKey);
                        //删除压缩前的文件夹
                        if (unKey == ".meta")
                        {
                            MyEdior.DeletFile(pathFile);
                        }
                        fileSize += new System.IO.FileInfo(outPath).Length / 1024;
                    }
                }
                else
                {
                    pathFile += ResConfig.FZIPNAMEEX;
                    if (File.Exists(pathFile))
                    {
                        FZipTool.UnZip(pathFile, zipDir, ResConfig.FZIPPASS);
                        //删除解压前的文件夹
                        File.Delete(pathFile);
                    }
                }
            }
        }

        AssetDatabase.Refresh();

        BundleManager.StreamConfig streamConfig = new BundleManager.StreamConfig();
        List<string> fileList = new List<string>();
        string keyStream = "/" + FEPath.GetFileNameWithoutExtension(FEPath.GetDirectoryName(streamTempPath))+"/";
        SelectPrefabEditor spe = new SelectPrefabEditor("", keyStream, "", true);
        for (int i = 0; i < spe.tempAllStrs.Length; i++)
        {
            string path = spe.tempAllStrs[i];
            if (File.Exists(path) && path.IndexOf(F_NOCOPYSTREAM) == -1 && (path.IndexOf("Assembly-CSharp") == -1))
            {
                int pos = path.IndexOf(keyStream);
                fileList.Add(path.Substring(pos + keyStream.Length));
            }
        }

        if (fileList.Count != 0)
        {
            for (int index = 0; index < fileList.Count; index++)
            {
                string fileName = fileList[index];
                var normaFile = new BundleManager.StreamConfig.NormalFile();
                int pathPos = fileName.LastIndexOf("/");
                string pathFile = "";
                if (pathPos != -1)
                {
                    pathFile = fileName.Substring(0, pathPos);
                }
                normaFile.path = pathFile;
                normaFile.fileName = fileName;
                normaFile.md5 = FEngineManager.GetMD5HashFromFile(streamTempPath + fileName);
                normaFile.resType = FEPath.GetExtension(fileName);
                normaFile.FileType = GetResType(normaFile);// (normaFile.resType == ResConfig.FZIPNAMEEX);
                streamConfig.fileDatas.Add(normaFile);
            }
        }

        ScriptsTime.Debug("创建StreamingConfig完成");

        streamConfig.IsBackDownMicro = copyStream.IsBackDownMicro;
        streamConfig.versionId = copyStream.versionId;
        streamConfig.CustomConfig = copyStream.CustomConfig;
        streamConfig.zipSize = fileSize;

        FSaveHandle sd = FSaveHandle.Create(streamingPath,FFilePath.FP_Abs,FOpenType.OT_Write);
        sd.PushObject(streamConfig);
        sd.Save();
        EditorUtility.ClearProgressBar();
    }

    public static void CreateStreamingConfig()
    {
        BundleManager.StreamConfig newConfig = new BundleManager.StreamConfig();
        newConfig.IsBackDownMicro = true;
        newConfig.versionId = MyEdior.GetTimeVersion();
        newConfig.CustomConfig = new BundleManager.CustomAppConfig();
        newConfig.CustomConfig.ShowVersion = PlayerSettings.bundleVersion;
        newConfig.CustomConfig.Parames = FEngine.CheckVersion.ToString();
        CreateConfig(FEPath.GetDirectoryName(SceneManager.STREAMINGPATH.Replace("file://", "")), newConfig,2);
    }
}

//图片改变设置
/*
public class ConfigToSprite : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D spr)
    {
        Debug.LogError("==============");
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);
        }
        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
    }

    static void ConfigObject(string pathFile)
    {

    }

}
*/


