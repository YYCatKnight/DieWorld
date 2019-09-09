//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.Networking;

namespace F2DEngine
{
     
    public class BundleManager : ManagerTemplate<BundleManager>
    {

        public static bool IsLoadComplete { get { return _isLoadComplete; }}
        internal static bool _isLoadComplete = false;
        public delegate void BundleAskCall(StreamConfig lastConfig, StreamConfig newConfig, AskBundle data);
        public const string SpriteType = ".sprite";
        public const string TextureType = ".png";
        public const string MaterialType = ".mat";
        public const string FontType = ".ttf";
        public const string PrefabType = ".prefab";
        public const string AssetType = ".asset";

        public static int ConfigTime = 5;
        public static string PathBudleData = "";
        public static int MaxThread = 25;//最大加载携程
        public static int MaxMomory = 1024*1024*25;//25M大小释放一次内存实际是25*2
        protected static long mNextRunIndex = 0;
        protected static long mLockIndex = 0;
        protected static FEngineApp mAppVersion;
        private bool mHaveBackDown = false;
        //上个版被更改的文件
        public class LegacyPackFile
        {
            public Dictionary<string, int> Files = new Dictionary<string, int>();//缓存区有变动文件路径
            public Dictionary<string, int> Bundles = new Dictionary<string, int>();//缓存区有变动的bundle
        }

        public class FEngineApp:Save_Object
        {
            public int appTimes;//app次数
            public string microMD5;//微端版本号
        }

        public class CustomAppConfig
        {
            public string ShowVersion;//显示版本
            public string Parames;//其他信息
        }

        public enum FResFileType
        {
            FT_Null = 0,//其他文件
            FT_Unity = 1,//Unity 资源

            FT_Zip = 2,//zip
            FT_F = 3,//自定义资源文件
        }
        public class StreamConfig
        {
            public bool IsOverWrite = true;
            public bool IsBackDownMicro = false;
            public class NormalFile
            {
                public string path;
                public string fileName;
                public int FileType;//FResFileType
                public string md5;
                public string resType;
            }
            
            public int versionId = 0;//版本号
            public string microMD5 = "";
            public bool IsCache = false;
            public bool DelectZip = true;
            public long zipSize = 0;
            public List<string> microsFiles = new List<string>();
            public List<NormalFile> fileDatas = new List<NormalFile>();//总文件
            private Dictionary<string, NormalFile> mFileDic = new Dictionary<string, NormalFile>();
            public void Init()
            {
                if (mFileDic.Count == 0)
                {
                    for (int i = 0; i < fileDatas.Count; i++)
                    {
                        mFileDic[fileDatas[i].fileName] = fileDatas[i];
                    }
                }
            }
            public string GetMd5(string fileName)
            {
                if(mFileDic.Count == 0)
                {
                    Init();
                }
                NormalFile norfile = null;
                if (mFileDic.TryGetValue(fileName,out norfile))
                {
                    return norfile.md5;
                }
                return "";
            }
            public CustomAppConfig CustomConfig;
        }

        public class BundleConfig
        {
            public class BundleFileData
            {
                public string bundleFile;
                public string resFileName;
                public string md5;
                public string resType;
                public bool mIsCache = false;//是否缓存
            }
            public class PathBundles
            {
                public string path;  //路径
                public bool mIsOnly = false;//是唯一Bundle
                public bool mIsAsync = false;//是否异步加载     
                public  Dictionary<string,BundleFileData> fileBundles = new Dictionary<string, BundleFileData>();
            }
            public int versionId = 0;
            public LegacyPackFile legacyPackFile;//变动文件
            public Dictionary<string, PathBundles> pathBundles = new Dictionary<string, PathBundles>();


            public WWW CreateBundleWWW(string name, Hash128 version)
            {
                WWW www = null;
                if (IsHaveCache(name))
                {
                    www = WWW.LoadFromCacheOrDownload(FEngineManager.ASSETBUNDLE + "/" + name, version);
                }
                else
                {
                    www = new WWW(FEngineManager.STREAMINGPATHASSETBUNDLE + "/" + name);
                }
                return www;
            }

            public bool IsHaveCache(string name)
            {
                if (legacyPackFile != null)
                {
                    return legacyPackFile.Bundles.ContainsKey(name);
                }
                return false;
            }

            public string GetMd5(string path, string resFile)
            {

                PathBundles pb = null;
                if(pathBundles.TryGetValue(path,out pb))
                {
                    BundleFileData bfd = null;
                    if(pb.fileBundles.TryGetValue(resFile,out bfd))
                    {
                        return bfd.md5;
                    }
                }
                return "";
            }
        }

        public class AssetBundleAndObject
        {
            public AssetBundle assetBundle;
            private UnityEngine.Object mObj;
            public BundleConfig.BundleFileData budlefileData;
            private bool mIsLoad = false;

            public void Clear()
            {
                mObj = null;
                mIsLoad = false;
            }

            public AssetBundleRequest LoadObjectAsync<T>()
            {
                return assetBundle.LoadAssetAsync<T>(budlefileData.resFileName);
            }

            public AssetBundleRequest LoadObjectAsync()
            {
                AssetBundleRequest obj = null;
                switch (budlefileData.resType)
                {
                    case PrefabType:
                        obj = assetBundle.LoadAssetAsync(budlefileData.resFileName);
                        break;
                    case SpriteType:
                        obj = assetBundle.LoadAssetAsync<Sprite>(budlefileData.resFileName);
                        break;
                    case TextureType:
                        obj = assetBundle.LoadAssetAsync<Texture2D>(budlefileData.resFileName);
                        break;
                    case MaterialType:
                        obj = assetBundle.LoadAssetAsync<Material>(budlefileData.resFileName);
                        break;
                    case AssetType:
                        obj = assetBundle.LoadAssetAsync<ScriptableObject>(budlefileData.resFileName);
                        break;
                    case FontType:
                        obj = assetBundle.LoadAssetAsync<Font>(budlefileData.resFileName);
                        break;
                    default:
                        obj = assetBundle.LoadAssetAsync(budlefileData.resFileName);
                        break;
                }
                return obj;
            }

            public T GetObject<T>() where T : UnityEngine.Object
            {
                if (mIsLoad)
                {
                    return mObj as T;
                }
                else
                {
                    if (assetBundle != null)
                    {
                        T obj = assetBundle.LoadAsset<T>(budlefileData.resFileName);
                        if (budlefileData.mIsCache)
                        {
                            mIsLoad = true;
                            mObj = obj;
                        }
                        return obj;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        //AssetBundle文件
        private Dictionary<string, Dictionary<string, AssetBundleAndObject>> mBundleObjectMap = new Dictionary<string, Dictionary<string, AssetBundleAndObject>>();//总文件
        private AssetBundleManifest mAssetBundleManifest = null;
        private BundleConfig mBundleConfig;
        private StreamConfig mNewStreamConfig;
        private Dictionary<string, AssetBundle> mAssetBundles = new Dictionary<string, AssetBundle>();
        private List<string> mLcokLoadingFiles = new List<string>();//正在加载中的文件...

        //异步加载资源
        public class AsynData
        {
            public List<string> mTemporaryBundles = new List<string>();//临时异步加载池 
            public List<string> mTemporaryPath = new List<string>();//临时异步路径
        }

        private AsynData mTempAsynData = new AsynData();//游戏临时加载
        private Timer_Mix mLoadMix = new Timer_Mix(4);//异步资源加载器

        private bool mIsLock = false;
        private bool IsLoad(string path)
        {
            if(mBundleObjectMap.ContainsKey(path))
            {
                return true;
            }
            return false;
        }

        private bool ClearBundle(string name)
        {
            if (name == "")
                return false;
            if (mAssetBundles.ContainsKey(name))
            {
                mAssetBundles[name].Unload(false);
                mAssetBundles.Remove(name);
                return true;
            }
            return false;
        }


        private IEnumerator _CopyFileResoures(BundleManager.StreamConfig.NormalFile file, BundleManager.StreamConfig bundleData, BundleManager.StreamConfig locatData, string curAassetBuldlePath, LoadPercent loadPercent,bool isServer)
        {

#if !UNITY_EDITOR
#if UNITY_ANDROID || UNITY_IPHONE
            Debug.LogError("请打成DLL使用");
            throw Exception("请打成DLL使用");
            yield return;
#endif
#endif
            string directoryPath = FEngineManager.WRPath + file.path;
            string path = file.path;
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string tempBundleName = file.fileName;
            string md5 = bundleData.GetMd5(tempBundleName);
            string locatMd5 = "";
            if (locatData != null)
            {
                locatMd5 = locatData.GetMd5(tempBundleName);
            }

            if (md5 != locatMd5 || locatMd5 == "")
            {
                bool IsZip = (file.FileType == (int)FResFileType.FT_Zip);
                string casePath = FEngineManager.WRPath + tempBundleName;
                bool isHavecase = (IsZip && File.Exists(casePath) && FEngineManager.GetMD5HashFromFile(casePath) == md5);

                /*
                //旧版本
                if (!isHavecase)
                {
                    bool isZipAndDown = IsZip && isServer;
                    string httpurl = isZipAndDown ? (curAassetBuldlePath + tempBundleName + _GetHttpHeadVersion()):(curAassetBuldlePath + tempBundleName);
                    using (UnityWebRequest www = UnityWebRequest.Get(httpurl))
                    {
                        WaitForSeconds wait = new WaitForSeconds(0.3f);
                        if (IsZip && isServer)
                        {
                            string fileSizeStr = (bundleData.zipSize / 1024.0f + 0.1).ToString("0.0") + "M";
                            //www.timeout = 20;
                            double msize = 1024 * 1024;
                            www.SendWebRequest();
                            loadPercent.SetKD("DownloadPack");
                            loadPercent.Reset("");
                            while (!www.isDone && !IsErrorWebRequest(www))
                            {
                                loadPercent.GoOn(www.downloadProgress,"("+(www.downloadedBytes / msize).ToString("0.0") + "/" + fileSizeStr + ")" + ((int)(www.downloadProgress * 100)).ToString() + "%");
                                yield return wait;
                            }
                        }
                        else
                        {
                            loadPercent.Reset("");
                            yield return www.SendWebRequest();
                        }

                        if (!string.IsNullOrEmpty(www.error))
                        {
                            Debug.LogError(tempBundleName + "拷贝错误(Bundle)\n" + www.error);
                            loadPercent.SetError(tempBundleName + "拷贝错误(Bundle)\n" + www.error);
                        }
                        else
                        {
                            using (var fileTemp = new FileStream(casePath, FileMode.Create, FileAccess.Write))
                            {
                                byte[] bs = www.downloadHandler.data;
                                fileTemp.Seek(0, SeekOrigin.Begin);
                                fileTemp.Write(bs, 0, bs.Length);
                                fileTemp.Close();
                                //有待验证
                                var NewMd5 = FEngineManager.GetMD5HashFromFile(bs);
                                if (NewMd5 != md5)
                                {
                                    Debug.LogError(tempBundleName + ":MD5码错误");
                                    loadPercent.SetError(tempBundleName + "MD5码错误");
                                }
                            }

                        }
                    }
                }
                */

                //新版本加载
                if (!isHavecase)
                {
                    bool isZipAndDown = IsZip && isServer;
                    if (isZipAndDown)
                    {
                        //下载文件包
                        int times = 5;
                        FDownloader loader = FDownloader.CreateUnityDown(curAassetBuldlePath + tempBundleName + _GetHttpHeadVersion(),FDownloader.GET);
                        if (!bundleData.DelectZip)
                        {
                            loader.Md5Tag = md5;
                        }
                        loader.FilePath = casePath;
                        loadPercent.SetKD("DownloadPack");
                        loadPercent.Reset("");
                        WaitForSeconds wait = new WaitForSeconds(0.3f);
                        bool isSuc = false;
                        long downSize = 0;
                        while (times-- > 0)
                        {
                            string fileSizeStr = (bundleData.zipSize / 1024.0f + 0.1).ToString("0.0") + "M";
                            //www.timeout = 20;
                            double msize = 1024 * 1024;
                            loader.Send((f) => { });

                            while (loader.State == FDownloader.DownState.Wait)
                            {
                                if (loader.TotalLength > 0)
                                {
                                    loadPercent.GoOn(loader.Progress, "(" + (loader.DowningLength / msize).ToString("0.0") + "/" + fileSizeStr + ")" + ((int)(loader.Progress * 100)).ToString() + "%");
                                }
                                yield return wait;
                            }

                            if (loader.State == FDownloader.DownState.Success)
                            {
                                isSuc = true;
                                break;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(loader.Error))
                                {
                                    Debug.LogError("FDownloder下载失败:(" + loader.DowningLength.ToString() + ")" + loader.Error);
                                }
                                if (loader.DowningLength > 1024)
                                {
                                    if(downSize != loader.DowningLength)
                                    {
                                        times = 20;
                                        downSize = loader.DowningLength;
                                    }
                                    yield return new WaitForSeconds(3.0f);
                                }
                                else
                                {
                                    times = times / 2;
                                }
                            }
                        }
                        if (!isSuc)
                        {
                            loadPercent.SetError("FDownloder下载失败");
                        }
                        loader.Clear();
                    }
                    else
                    {
                        //拷贝文件
                        using (UnityWebRequest www = UnityWebRequest.Get(curAassetBuldlePath + tempBundleName))
                        {
                            loadPercent.Reset("");
                            yield return www.SendWebRequest();
                            if (!string.IsNullOrEmpty(www.error))
                            {
                                Debug.LogError(tempBundleName + "拷贝错误(Bundle)\n" + www.error);
                                loadPercent.SetError(tempBundleName + "拷贝错误(Bundle)\n" + www.error);
                            }
                            else
                            {
                                using (var fileTemp = new FileStream(casePath, FileMode.Create, FileAccess.Write))
                                {
                                    byte[] bs = www.downloadHandler.data;
                                    fileTemp.Seek(0, SeekOrigin.Begin);
                                    fileTemp.Write(bs, 0, bs.Length);
                                    fileTemp.Close();
                                }
                            }
                        }
                    }
                    //有待验证

                    if (loadPercent.ErrorNum == 0)
                    {
                        var NewMd5 = FEngineManager.GetMD5HashFromFile(casePath);
                        if (NewMd5 != md5)
                        {
                            Debug.LogError(tempBundleName + ":MD5码错误");
                            loadPercent.SetError(tempBundleName + "MD5码错误");
                        }
                    }
                }

                //压缩文件格式,需要解压
                if (IsZip && loadPercent.ErrorNum == 0)
                {
                    GC.Collect();
                    loadPercent.Reset();
                    loadPercent.SetKD("Decompression");
                    var newPercent = new LoadPercent();
                    string zipFile = casePath;
                    string zipDir = FEPath.GetDirectoryName(zipFile) + "/";
                    ZipThreadData ztd = FEngineManager.GetFBEngine().ThreadUnZip(zipFile, zipDir, ResConfig.FZIPPASS, newPercent,bundleData.IsOverWrite);
                    ScriptsTime.Begin();
                    WaitForSeconds wait = new WaitForSeconds(0.3f);
                    while (true)
                    {
                        lock (newPercent)
                        {
                            var  pd = newPercent.GetPercent();
                            if (pd != null)
                            {
                                loadPercent.GoOn(pd.pre,pd.dec);
                            }
                            if (newPercent.IsOver)
                            {
                                break;
                            }
                        }
                        yield return wait;
                    }

                    if (!string.IsNullOrEmpty(ztd.error))
                    {
                        Debug.LogError(ztd.error);
                        loadPercent.SetError(ztd.error);
                        while (true)
                        {
                            yield return 0;
                        }
                    }

                    //压缩包解压文件
                    if (bundleData.DelectZip)
                    {
                        File.Delete(zipFile);
                    }
                    GC.Collect();
                    ScriptsTime._Debug("解压:" + tempBundleName);
                }
                ScriptsTime._Debug("拷贝成功" + tempBundleName);
            }
            yield break;
        }

        private IEnumerator _UpdateBundle(BundleManager.StreamConfig bundleData, BundleManager.StreamConfig locatData,string curAassetBuldlePath, LoadPercent loadPercent,byte[]bytes,bool isServer)
        {
            ScriptsTime._Debug("开始拷贝StreamingAssets文件");
            loadPercent.SetTimece(bundleData.fileDatas.Count);
            //新的多携程加载方法
            Timer_Mix Mix = new Timer_Mix(MaxThread);
            int updateRes = (int)FResFileType.FT_Unity;
            for (int i = 0; i < bundleData.fileDatas.Count; i++)
            {
                var data = bundleData.fileDatas[i];
                if (data.FileType > updateRes)
                {
                    Timer_Coroutine timer = new Timer_Coroutine();
                    timer.SetIEnumerator(_CopyFileResoures(data, bundleData, locatData, curAassetBuldlePath, loadPercent,isServer));
                    Mix.AddTimer(timer);
                }
            }
            yield return Mix.WaitPlay();

            if (loadPercent.ErrorNum == 0)
            {
                ScriptsTime._Debug("全部拷贝成功");
                loadPercent.GoOn(1);
                if (bytes != null)
                {
                    //写入最新版本数据
                    FileStream fileS = new FileStream(FEngineManager.WRPath + ResConfig.STREAMINGASSETSCONFIG + "_Loca", FileMode.Create, FileAccess.Write);
                    fileS.Seek(0, SeekOrigin.Begin);
                    fileS.Write(bytes, 0, bytes.Length);
                    fileS.Close();
                }
            }
            else
            {
                loadPercent.SetError("下载,拷贝文件出错了");
                Debug.LogError("下载,拷贝文件出错了");
            }
            loadPercent.Over();
            yield return 0;
        }


        //修复资源
        private IEnumerator _RepairBundle(LoadPercent loadPercent)
        {
            yield return 0;
            Timer_Mix Mix = new Timer_Mix(2);
            foreach(var k in mBundleObjectMap)
            {
                var abList = k.Value;
                foreach(var d in abList)
                {
                    AssetBundleAndObject aba = d.Value;
                    if(aba.assetBundle == null)
                    {
                        //进行修补
                        AssetBundle ab = null;
                        if (mAssetBundles.TryGetValue(aba.budlefileData.bundleFile, out ab))
                        {
                            if (ab != null)
                            {
                                aba.assetBundle = ab;
                                continue;
                            }
                            else
                            {
                                mAssetBundles.Remove(aba.budlefileData.bundleFile);
                            }
                        }

                        Timer_Coroutine timer = new Timer_Coroutine();
                        timer.SetIEnumerator(LoadAssetBundle(aba.budlefileData, abList, false));
                        Mix.AddTimer(timer);
                    }
                }
            }

            loadPercent.SetTimece(Mix.MaxNum);
            Mix.OnUpdate((m) =>
            {
                loadPercent.GoOn(Mix.CurNum + "/" + Mix.MaxNum);
            });
            yield return Mix.WaitPlay();
            loadPercent.Over();
            yield return 0;
        }

        //询问对象
        public enum AskType
        {
            None,
            Micro,//微端
            Configure,//配置表
            ResFrame,//帧资源
        }

        public enum AskState
        {
            Start,
            Error,
            Success,
            Back,
        }

        public enum AskResult
        {
            Wait,//等待
            Continue,//继续
            Jump,//跳过
            Quit,//退出
        }
        
        public class AskBundle
        {
            public AskType a_type { get; protected set; }
            public AskState a_state { get; protected set; }
            public object data { get; internal set; }
            internal AskResult mResult;//1为继续,0为退出,-1为退出
            public void SetTag(AskResult result)
            {
                mResult = result;
            }

            internal IEnumerator WaitAsk()
            {
                while (mResult == AskResult.Wait)
                {
                    yield return 0;
                }
                if (mResult == AskResult.Quit)
                {
                    Application.Quit();
                    while (true)
                    {
                        yield return 0;
                    }
                }
            }

            internal AskBundle(AskType type, AskState state)
            {
                a_type = type;
                a_state = state;
                mResult = AskResult.Wait;
            }
        }

        public StreamConfig GetAppConfig()
        {
            return mNewStreamConfig;
        }

        private bool IsErrorWebRequest(UnityWebRequest request)
        {
            if (request.isNetworkError||!string.IsNullOrEmpty(request.error))
            {
                Debug.LogError(request.url +":网络连接失败-"+ request.error);
                return true;
            }
            return false;
        }

        //日志打印
        private void DefalutBundleAskCall(StreamConfig lastConfig, StreamConfig newConfig, AskBundle data)
        {
            Debug.Log(data.a_state+"-"+data.a_type+"_"+data.mResult);
            data.SetTag(AskResult.Continue);
            if(data.a_state  == AskState.Error&&data.a_type == AskType.Configure)
            {
                data.SetTag(AskResult.Jump);
            }
        }


        public class BackStreamDown
        {
            public bool IsOver { get; internal set; }
            public StreamConfig downConfig { get; internal set; }//配置
            public LoadPercent loadPercent { get; internal set; }//进度
            internal Timer_Coroutine timer;
            internal Action mCallBack;
            public void BeginDown()
            {
                timer.PauseTimer(false);
            }
            public void SetOnComplete(Action callBack)
            {
                mCallBack = callBack;
            }
            internal void SentComplete()
            {
                IsOver = true;
                if (mCallBack != null)
                {
                    mCallBack();
                }
            }
        }

        internal IEnumerator CopyStreamingAsset(LoadPercent loadPercent = null, BundleAskCall askResult = null)
        {
            if(askResult == null)
            {
                askResult = DefalutBundleAskCall;
            }

            mAppVersion = new FEngineApp();
            mAppVersion.ReadFile();
            mAppVersion.appTimes++;
            if (loadPercent == null)
            {
                loadPercent = LoadPercent.GetNonePrecent();
            }
            loadPercent.SetKD("Initialize_Resources");
            loadPercent.Reset();
            if (mBundleConfig == null)
            {
                string streamingPath = FEngineManager.STREAMINGPATH + ResConfig.STREAMINGASSETSCONFIG;
                WWW www = new WWW(streamingPath);
                yield return www;
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.Log("<color=#660000>Streamingpath目录缺少配置文件,请重新生成配置文件,或者只使用不打包模式运行</color>");
                    yield break;
                }
                
                BundleManager.StreamConfig streamData = new BundleManager.StreamConfig();
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogError(ResConfig.STREAMINGASSETSCONFIG + "加载错误(Bundle)\n" + www.error);
                }
                else
                {
                    //保存配置文件
                    var streamConfigBytes = www.bytes;
                    //拷贝文件
                    FSaveHandle sd = FSaveHandle.Create("", FFilePath.FP_Cache, FOpenType.OT_Write);
                    sd.SetBytes(streamConfigBytes);
                    sd.FromObject(streamData);

                    //本地数据
                    FSaveHandle locat = FSaveHandle.Create(ResConfig.STREAMINGASSETSCONFIG + "_Loca", FFilePath.FP_Cache);
                    BundleManager.StreamConfig locatData = null;
                    if (locat.IsLoad)
                    {
                        locatData = new BundleManager.StreamConfig();
                        locat.FromObject(locatData);
                        if (locatData.versionId < streamData.versionId)
                        {
                            //证明是个新包
                            Debug.Log("替换安装,为新包,清空缓存");
                            FEngineManager.DeletFile(FEngineManager.WRPath + ResConfig.FDESASSETBUNDLE);
                            locatData = null;
                        }
                    }

                    //开始加载资源回调
                    {
                        AskBundle asd = new AskBundle(AskType.None, AskState.Start);
                        askResult(streamData, locatData != null ? locatData : streamData, asd);
                        yield return asd.WaitAsk();
                    }


                    if (!string.IsNullOrEmpty(streamData.microMD5) && (mAppVersion.microMD5 != streamData.microMD5 || locatData == null))
                    {
                        bool microOver = false;
                        //if (!string.IsNullOrEmpty(streamData.microMD5))
                        {
                            //服务器有连接
                            if (!string.IsNullOrEmpty(FEngineManager.WWWServe))
                            {
                                if (mAppVersion.microMD5 != streamData.microMD5)
                                {
                                    //提示微端下载
                                    {
                                        AskBundle asd = new AskBundle(AskType.Micro, AskState.Start);
                                        askResult(streamData, streamData, asd);
                                        yield return asd.WaitAsk();
                                    }
                                }

                                if (!string.IsNullOrEmpty(FEngineManager.WWWServe))
                                {
                                    var subPercent = loadPercent.CreateBranch(1);
                                    while (true)
                                    {
                                        BundleManager.StreamConfig microConfig = new BundleManager.StreamConfig();
                                        microConfig.IsOverWrite = false;
                                        microConfig.IsCache = mAppVersion.microMD5 == streamData.microMD5;
                                        microConfig.zipSize = streamData.zipSize;
                                        microConfig.DelectZip = false;
                                        var normalFile = CreateMicroFile(streamData.microMD5);
                                        microConfig.fileDatas.Add(normalFile);
                                        bool isBackDown = false;
                                        if (streamData.IsBackDownMicro)
                                        {
                                            string casePath = FEngineManager.WRPath + normalFile.fileName;
                                            isBackDown = !(File.Exists(casePath) && FEngineManager.GetMD5HashFromFile(casePath) == streamData.microMD5);
                                        }
                                        if (isBackDown)
                                        {
                                            mHaveBackDown = true;
                                            //暂时挂起,放入后台下载
                                            loadPercent.Reset();
                                            microOver = true;
                                            LoadPercent tempPercent = new LoadPercent();
                                            BackStreamDown backStream = new BackStreamDown();
                                            var timer = new Timer_Coroutine(_UpdateBundle(microConfig, null, FEngineManager.WWWServe, tempPercent, null, true), (f) =>
                                            {
                                                if (tempPercent.ErrorNum == 0)
                                                {
                                                    mAppVersion.microMD5 = streamData.microMD5;
                                                    mAppVersion.SaveFile();
                                                    tempPercent.SetKD("RepairBundle");
                                                    tempPercent.Reset();

                                                    //开始修复
                                                    Timer_Coroutine.SetTimer(_RepairBundle(tempPercent), (_) =>
                                                    {
                                                        backStream.SentComplete();
                                                    }, null);

                                                }
                                                else
                                                {
                                                    Debug.LogError("微端异步下载失败：" + tempPercent.ErrorDec);
                                                    backStream.SentComplete();
                                                }

                                            }, null);
                                            timer.PauseTimer();
                                            TimerController.SetYieldTimer(timer);
                                            AskBundle asd = new AskBundle(AskType.Micro, AskState.Back);
                                            backStream.timer = timer;
                                            backStream.loadPercent = tempPercent;
                                            backStream.downConfig = microConfig;
                                            asd.data = backStream;
                                            askResult(streamData, streamData, asd);
                                            break;
                                        }
                                        else
                                        {
                                            //立即下载
                                            yield return _UpdateBundle(microConfig, null, FEngineManager.WWWServe, subPercent, null, true);
                                            if (loadPercent.ErrorNum == 0)
                                            {
                                                mAppVersion.microMD5 = streamData.microMD5;
                                                mAppVersion.SaveFile();
                                                microOver = true;
                                                loadPercent.Reset();
                                                AskBundle asd = new AskBundle(AskType.Micro, AskState.Success);
                                                askResult(streamData, streamData, asd);
                                                yield return asd.WaitAsk();
                                                break;
                                            }
                                            else
                                            {
                                                //有错误
                                                AskBundle asd = new AskBundle(AskType.Micro, AskState.Error);
                                                askResult(streamData, streamData, asd);
                                                yield return asd.WaitAsk();
                                                if (asd.mResult == AskResult.Jump)
                                                {
                                                    break;
                                                }
                                                //重新连接
                                                subPercent.Clear();
                                                loadPercent.Clear();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (!microOver)
                        {
                            Debug.LogError("微端存在，但是下载失败,无法进入");
                            while (true)
                            {
                                yield return new WaitForSeconds(10.0f);
                            }
                        }
                    }


                    //解压本地资源一次
                    if (locatData == null)
                    {
                        Caching.ClearCache();
                        streamData.Init();
                        //解压资源
                        yield return _UpdateBundle(streamData, null, FEngineManager.STREAMINGPATH, loadPercent.CreateBranch(1), streamConfigBytes, false);
                        if (loadPercent.ErrorNum == 0)
                        {
                            locatData = streamData;
                        }
                        else
                        {
                            //解压有错误
                        }
                        mNewStreamConfig = streamData;
                    }

                    //服务器数据
                    if (ConfigTime >= 0)
                    {
                        byte[] serverbytes = null;
                        BundleManager.StreamConfig mServerStream = null;
                        {
                            AskBundle asd = new AskBundle(AskType.Configure, AskState.Start);
                            askResult(locatData, mServerStream, asd);
                            yield return asd.WaitAsk();

                            if (asd.mResult != AskResult.Jump)
                            {
                                //网上加载资源
                                if (!string.IsNullOrEmpty(FEngineManager.WWWServe) && (!FEConfigure.mIsNoPack))
                                {
                                    while (true)
                                    {
                                        string serverResourse = streamingPath.Replace(FEngineManager.STREAMINGPATH, FEngineManager.WWWServe);
                                        using (UnityWebRequest serverWWW = UnityWebRequest.Get(serverResourse + _GetHttpHeadVersion()))
                                        {
                                            if (ConfigTime != 0)
                                            {
                                                serverWWW.timeout = ConfigTime;
                                            }
                                            yield return serverWWW.SendWebRequest();
                                            if (!IsErrorWebRequest(serverWWW))
                                            {
                                                mServerStream = new BundleManager.StreamConfig();
                                                FSaveHandle serverhandle = FSaveHandle.Create("", FFilePath.FP_Cache, FOpenType.OT_Write);
                                                serverbytes = serverWWW.downloadHandler.data;
                                                serverhandle.SetBytes(serverbytes);
                                                serverhandle.FromObject(mServerStream);
                                                AskBundle askb = new AskBundle(AskType.Configure, AskState.Success);
                                                askResult(locatData, mServerStream, askb);
                                                yield return asd.WaitAsk();
                                                break;
                                            }
                                            else
                                            {
                                                AskBundle askb = new AskBundle(AskType.Configure, AskState.Error);
                                                askResult(locatData, mServerStream, askb);
                                                yield return asd.WaitAsk();
                                                if (askb.mResult == AskResult.Jump)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //服务器新版本
                        if (locatData != null && mServerStream != null)
                        {
                            if (locatData.versionId < mServerStream.versionId)
                            {
                                bool isUpdate = true;
                                {
                                    //提示有小版本更新
                                    AskBundle asd = new AskBundle(AskType.ResFrame, AskState.Start);
                                    askResult(locatData, mServerStream, asd);
                                    yield return asd.WaitAsk();
                                    if (asd.mResult == AskResult.Jump)
                                    {
                                        isUpdate = false;
                                    }
                                }

                                if (isUpdate)
                                {
                                    locatData.Init();
                                    mServerStream.Init();
                                    while (true)
                                    {
                                        loadPercent.Reset("");
                                        yield return _UpdateBundle(mServerStream, locatData, FEngineManager.WWWServe, loadPercent.CreateBranch(1), serverbytes, true);
                                        if (loadPercent.ErrorNum == 0)
                                        {
                                            mNewStreamConfig = mServerStream;
                                            break;
                                        }
                                        else
                                        {
                                            //下载补丁包有错误
                                            AskBundle asd = new AskBundle(AskType.ResFrame, AskState.Error);
                                            askResult(locatData, mServerStream, asd);
                                            yield return asd.WaitAsk();

                                            if (asd.mResult == AskResult.Jump)
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                loadPercent.Clear();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                FSaveHandle sdz = FSaveHandle.Create(ResConfig.FDESASSETBUNDLE + "/" + ResConfig.ASSETBUNDLECONFIFILE, FFilePath.FP_Cache);
                if (sdz.IsLoad)
                {
                    mBundleConfig = new BundleConfig();
                    sdz.FromObject(mBundleConfig);
                    mBundleConfig.legacyPackFile = null;
                    if (mBundleConfig.versionId > streamData.versionId)
                    {
                        FSaveHandle cSave = FSaveHandle.Create(ResConfig.FDESASSETBUNDLE + "/" + ResConfig.FCACHEFILE, FFilePath.FP_Cache);
                        if (cSave.IsLoad)
                        {
                            mBundleConfig.legacyPackFile = new LegacyPackFile();
                            cSave.FromObject(mBundleConfig.legacyPackFile);
                        }
                    }

                    if(streamData.microsFiles.Count > 0)
                    {
                        if(mBundleConfig.legacyPackFile == null)
                        {
                            mBundleConfig.legacyPackFile = new LegacyPackFile();
                        }

                        for(int i = 0; i < streamData.microsFiles.Count;i++)
                        {
                            mBundleConfig.legacyPackFile.Bundles[streamData.microsFiles[i]] = 0;
                        }
                    }
                } 
                else
                {
                    Debug.Log(ResConfig.ASSETBUNDLECONFIFILE + "<color=#660000>文件加载失败,只能运行不打包模式</color>");
                }
            }
            mAppVersion.SaveFile();
            _isLoadComplete = true;

            {
                AskBundle asd = new AskBundle(AskType.None, AskState.Success);
                askResult(mNewStreamConfig, mNewStreamConfig, asd);
                yield return asd.WaitAsk();
            }
            yield return 0;
        }

        private BundleManager.StreamConfig.NormalFile CreateMicroFile(string md5)
        {
            BundleManager.StreamConfig.NormalFile normalFile = new StreamConfig.NormalFile();
            normalFile.FileType = (int)FResFileType.FT_Zip;
            normalFile.resType = ".zip";
            normalFile.path = "";
            normalFile.md5 = md5;
            normalFile.fileName = ResConfig.MicroBundleName + ".zip";
            return normalFile;
        }

        private string _GetHttpHeadVersion()
        {
            return "?vs" + UnityEngine.Random.Range(100000, 999999).ToString();
        }

        //相对路径
        public T GetAssetObject<T>(string pathName) where T : UnityEngine.Object
        {
            var data = GetAssetBundleData(pathName);
            if(data != null)
            {
               return data.GetObject<T>();
            }
            return null;
        }

        public AssetBundleAndObject GetAssetBundleData(string pathName)
        {
            string fileName = pathName.ToLower();
            int tempIndex = fileName.LastIndexOf('/');
            string path = "";
            if (tempIndex != -1)
            {
                path = fileName.Substring(0, tempIndex);
                fileName = fileName.Substring(tempIndex + 1);
            }
            Dictionary<string, AssetBundleAndObject> oList = GetAssetBundle(path);
            AssetBundleAndObject uo = null;
            oList.TryGetValue(fileName, out uo);
            return uo;
        }

        public Dictionary<string, AssetBundleAndObject> GetAssetBundle(string path,bool isAddPath = false)
        {
            Dictionary<string, AssetBundleAndObject> abList = null;
            if (!mBundleObjectMap.TryGetValue(path, out abList))
            {
                abList = new Dictionary<string, AssetBundleAndObject>();
                if (isAddPath)
                {
                    mBundleObjectMap[path] = abList;
                }
            }
            return abList;
        }

        public IEnumerator LoadAssetBundles(string path,LoadPercent loadPercent = null)
        {
            path = path.ToLower();
            if(loadPercent == null)
            {
                loadPercent = LoadPercent.GetNonePrecent();
            }

            if (mBundleConfig != null)
            {
                if (mAssetBundleManifest == null)
                {
                    WWW www = WWW.LoadFromCacheOrDownload(FEngineManager.ASSETBUNDLE + "/" + ResConfig.FDESASSETBUNDLE, mBundleConfig.versionId);// mBundleDatas.CreateBundleWWW(ResConfig.FDESASSETBUNDLE, mBundleDatas.versionId);
                    yield return www;
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError(ResConfig.FDESASSETBUNDLE + "加载错误(Bundle)\n" + www.error);
                    }
                    else
                    {
                        AssetBundle ab = www.assetBundle;
                        www.Dispose();
                        mAssetBundleManifest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
                        ab.Unload(false);
                    }
                }

                if (!IsLoad(path))
                {
                    loadPercent.SetKD("LoadAssetBundles");
                    loadPercent.SetTimece(mBundleConfig.pathBundles.Count);
                    //新的多携程加载方法
                    Timer_Mix Mix = new Timer_Mix(2);
                    foreach(var k in mBundleConfig.pathBundles)
                    {
                        var data = k.Value;
                        if (!data.mIsAsync)
                        {
                            string temp = data.path;
                            if (temp.IndexOf(path) != -1)
                            {
                                Timer_Coroutine timer = new Timer_Coroutine();
                                timer.SetIEnumerator(_LoadAssetBundle(temp,null));
                                Mix.AddTimer(timer);
                            }
                        }
                        else
                        {
                            loadPercent.GoOn(1);
                            ScriptsTime._Debug("使用异步加载:" + data.path);
                        }
                    }
                    Mix.OnUpdate((m) =>
                    {
                        loadPercent.GoOn(Mix.CurNum + "/" + Mix.MaxNum);
                    });

                    yield return Mix.WaitPlay();

                }
            }
            loadPercent.Over();
            yield return 0;
        }

        private IEnumerator LoadAssetBundle(BundleConfig.BundleFileData bfd, Dictionary<string, AssetBundleAndObject> abList = null, bool useLock = true, AsynData sysData = null)
        {
            if (bfd == null)
                yield break;

            mIsLock = true;

            bool isTempBundle = false;
            string fineNameNoPath = bfd.resFileName; //bf.resFileName[i];
            if (abList == null)
            {
                string path = FEPath.GetDirectoryName(bfd.bundleFile);
                abList = GetAssetBundle(path, true);
                if (abList.ContainsKey(fineNameNoPath))
                {
                    mIsLock = false;
                    yield break;
                }
                isTempBundle = true;
                if (abList.Count == 0)
                {
                    sysData.mTemporaryPath.Add(path);
                }
            }

            string abundleName = bfd.bundleFile; //bf.bundleFile[i];

            if (!mAssetBundles.ContainsKey(abundleName))
            {
                string[] dps = mAssetBundleManifest.GetAllDependencies(abundleName);
                //加载依赖
                for (int a = 0; a < dps.Length; a++)
                {
                    string hashName = dps[a];
                    while (mLcokLoadingFiles.Contains(hashName))
                    {
                        yield return 0;
                    }
                    if (!mAssetBundles.ContainsKey(hashName))
                    {
                        mLcokLoadingFiles.Add(hashName);
                        WWW www = mBundleConfig.CreateBundleWWW(hashName, mAssetBundleManifest.GetAssetBundleHash(hashName));
                        yield return www;
                        mAssetBundles[hashName] = www.assetBundle;
                        mLcokLoadingFiles.Remove(hashName);
                        www.Dispose();
                        if (isTempBundle)
                        {
                            sysData.mTemporaryBundles.Add(hashName);
                        }
                    }
                }
            }

            {
                //加载资源
                AssetBundle resBundle = null;
                while (mLcokLoadingFiles.Contains(abundleName))
                {
                    yield return 0;
                }
                if (!mAssetBundles.ContainsKey(abundleName))
                {
                    mLcokLoadingFiles.Add(abundleName);
                    WWW resoureWWW = mBundleConfig.CreateBundleWWW(abundleName, mAssetBundleManifest.GetAssetBundleHash(abundleName));
                    yield return resoureWWW;

                    if (!string.IsNullOrEmpty(resoureWWW.error))
                    {
                        Debug.Log("<color=#660000>" + abundleName + "加载错误(NO Bundle)\n" + resoureWWW.error + "</color>");
                    }
                    else
                    {
                        resBundle = resoureWWW.assetBundle;
                        resoureWWW.Dispose();
                    }
                    mAssetBundles[abundleName] = resBundle;
                    mLcokLoadingFiles.Remove(abundleName);
                    if (isTempBundle)
                    {
                        sysData.mTemporaryBundles.Add(abundleName);
                    }
                }
                else
                {
                    resBundle = mAssetBundles[abundleName];
                }

                if (fineNameNoPath != "")
                {
                    AddBundleFileToBuff(bfd, abList);
                }
            }
            mIsLock = false;
        }

        private IEnumerator _LoadAssetBundle(string path = "", LoadPercent loadPercent = null)
        {
            //加载main
            if (loadPercent == null)
            {
                loadPercent = LoadPercent.GetNonePrecent();
            }
            if (mBundleConfig != null)
            {
                path = path.ToLower();
                
                if (!IsLoad(path))
                {
                    BundleConfig.PathBundles bf = GetBundleFiles(path);
                    if (bf != null)
                    {
                        Dictionary<string, AssetBundleAndObject> abList = GetAssetBundle(path, true);
                        Timer_Mix Mix = null;
                        loadPercent.SetKD("_LoadAssetBundle");
                        if (!bf.mIsOnly)
                        {
                            Mix = new Timer_Mix(4);
                            loadPercent.SetTimece(bf.fileBundles.Count);
                        }
                        else
                        {
                            loadPercent.SetTimece(1);
                        }

                        foreach (var k in bf.fileBundles)
                        {
                            var bfd = k.Value;
                            while (mLcokLoadingFiles.Contains(bfd.bundleFile))
                            {
                                yield return 0;
                            }
                            if (mAssetBundles.ContainsKey(bfd.bundleFile))
                            {
                                AddBundleFileToBuff(bfd, abList);
                            }
                            else
                            {
                                if (Mix == null)
                                {
                                    yield return LoadAssetBundle(bfd, abList, false);
                                }
                                else
                                {
                                    Timer_Coroutine timer = new Timer_Coroutine();
                                    timer.SetIEnumerator(LoadAssetBundle(bfd, abList, false));
                                    Mix.AddTimer(timer);
                                }
                            }
                        }
                        if(Mix != null)
                        {
                            Mix.OnUpdate((m) =>
                            {
                                loadPercent.GoOn(Mix.CurNum + "/" + Mix.MaxNum);
                            });
                            yield return Mix.WaitPlay();
                        }
                        else
                        {
                            loadPercent.GoOn("1/1");
                        }

                    }
                }
            }
            loadPercent.Over();
        }

        private void AddBundleFileToBuff(BundleConfig.BundleFileData bfd, Dictionary<string, AssetBundleAndObject> abList)
        {
            AssetBundleAndObject asbo = new AssetBundleAndObject();
            asbo.assetBundle = mAssetBundles[bfd.bundleFile];
            asbo.budlefileData = bfd;
            abList[bfd.resFileName] = asbo;
        }

       

        public void LoadAssetObjectAsync<T>(string pathName, Action<T> callBack, MonoBehaviour mo = null, Timer_Mix mix = null) where T : UnityEngine.Object
        {

        } 

        public BundleConfig.PathBundles GetBundleFiles(string path)
        {
            if (mBundleConfig == null)
                return null;
            BundleConfig.PathBundles pd = null;
            if(mBundleConfig.pathBundles.TryGetValue(path,out pd))
            {
                return pd;
            }
            return null;
        }

        public BundleConfig.BundleFileData GetBundleFileData(string fileName)
        {
            string path = FEPath.GetDirectoryName(fileName);
            string noPathName = FEPath.GetFileName(fileName);
            BundleConfig.PathBundles bf = GetBundleFiles(path);
            if (bf == null)
            {
                Debug.LogError("没有找到Bundle文件的加载路径:" + fileName);
                return null;
            }
            else
            {
                BundleConfig.BundleFileData bfd = null;
                if(bf.fileBundles.TryGetValue(noPathName,out bfd))
                {
                    return bfd;
                }
            }
            Debug.LogError("没有找到Bundle文件:" + fileName);
            return null;
        }

        //预加载
        public class MixProData
        {
            public Timer_Mix mix;
            public AsynData sysData;
        }
        private static Dictionary<string, List<UnityEngine.Object>> mPreObjects = new Dictionary<string, List<UnityEngine.Object>>();
        private static Dictionary<string, MixProData> mMixObjects = new Dictionary<string, MixProData>();
        public void DeletePreLoad(string configId)
        {
            if (mPreObjects.ContainsKey(configId))
            {
                mPreObjects.Remove(configId);
            }

            if(mMixObjects.ContainsKey(configId))
            {
                var mixPro = mMixObjects[configId];
                Timer_Mix mix = mixPro.mix;
                mix.Clear();
                AsynData sysData = mixPro.sysData;
                _UnLoadBundles(sysData);
                mMixObjects.Remove(configId);
            }
        }

        //预加载入口
        private IEnumerator ProLoadAutoBunldeSingle(string fileName, Timer_Coroutine timer,AsynData asyData)
        {
            BundleConfig.BundleFileData bfd = GetBundleFileData(fileName.ToLower());
            if (bfd != null)
            {
                yield return LoadAssetBundle(bfd, null, true, asyData);
                var abd = GetAssetBundleData(fileName);
                if (abd != null)
                {
                    AssetBundleRequest rq = abd.LoadObjectAsync();
                    if (rq != null)
                    {
                        yield return rq;
                        if (rq.isDone)
                        {
                            timer.result = rq.asset;
                        }
                    }
                }
                else
                {
                    Debug.LogError("未找到预加载AssetBundleAndObject文件:" + fileName);
                }
            }
            else
            {
                Debug.LogError("未找到预加载BundleFile文件:" + fileName);
            }
        }

        public Timer_Mix PreLoadBackObject(string configId,List<string> preFiles,List<string> pathFile,int mixNum = 2)
        {
            return null;
        }

        internal void UnLoadSysBundles()
        {
            _UnLoadBundles(mTempAsynData);
        }

        //释放异步加载
        private void _UnLoadBundles(AsynData sysData)
        {
            for (int i = 0; i < sysData.mTemporaryBundles.Count;i++)
            {
                string bunldName = sysData.mTemporaryBundles[i];
                var bunld = mAssetBundles[bunldName];
                mAssetBundles.Remove(bunldName);
                bunld.Unload(false);
            }
            sysData.mTemporaryBundles.Clear();
        }

    }
}
