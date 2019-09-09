//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.IO;

public class WebReqDataInfo
{
    public int point;
    public int channel;
    public string sign;
    public string deviceId;
}

namespace F2DEngine
{
    public class FEngine : MonoBehaviour, FBEngine
    {

        public enum GameState
        {
            GS_NONE,
            GS_HTTP,
            GS_LOGIN,
            GS_ENTER,
        }

        public static int CheckVersion = 1001;//当前审核版本
        public static string ShowVersion = "0.0.0";
        public static FEngine instance;


        public enum FDebugType
        {
            D_NONE = 0,
            D_Log = 1,
            D_HideLog = 2,
            D_Leaker = 3,
        }
        [NonSerialized]
        public bool UDPOPEN = false;
        //public bool UDPOPEN = true;
        public FDebugType DebugType = FDebugType.D_NONE;
        public bool IsNoPack = false;

        public static bool IsConnecting = false;
        public static bool IsInit = false;

        public float ConnectTime = 0;
        public int ConnectType = 0;
        public static IPData SelectIPData;
        public static int ResouresVersion = 0;
        public bool IsBackDown
        {
            get { return mBackDown != null; }
        }

        public class TakenFile : Save_Object
        {
            public string token = "";
            public int tokenType = 0;
        }

        public class ResouresFile : Save_Object
        {
            public int resoure;
            public int times = 0;
            public int downResoure;
        }

        public class PlatData
        {
            public string name;//渠道
        }
        public class IPData
        {
            public string ip;
            public int port;
            public Action<bool> mCallBack;
            public List<string> exips = new List<string>();
        }
        public bool ChackPack = false;

        public static GameState Game_State = GameState.GS_NONE;

        public static string AppVision = "1.1.1";

        public static BundleManager.BackStreamDown mBackDown;

        void Start()
        {
        }
        public class Log : Save_Object
        {
            public bool IsLog = false;
        }

        void Awake()
        {
            Application.runInBackground = true;
            //Application.targetFrameRate = 60;

#if UNITY_EDITOR
            if (DebugType == FDebugType.D_HideLog)
            {
                DebugType = FDebugType.D_NONE;
            }
#else
            if (serverPlat == ServerPlat.SP_Online)
            {
                DebugType = FDebugType.D_NONE;
            }
#endif
            instance = this;

            FEngineManager.SetEngine(this);
            //      LoadSceneManager.instance.SetScene(GameProgress.GP_Map1, "PveMap");

            DontDestroyOnLoad(this.gameObject);
            ConfigManager.instance.Init();
            InitManager();
            InitScene();
#if UNITY_EDITOR
            CheckProject();
#endif
        }

        public void InitManager()
        {
            //EventManager.instance.Init();
            SaveDataManager.instance.Init();
            if (GetComponent<KeyCodeManager>() == null)
                gameObject.AddComponent<KeyCodeManager>();
        }

        public void InitScene()
        {
            LoadSceneManager.instance.SetScene(GameProgress.GP_City, "Base");
            LoadSceneManager.instance.SetScene(GameProgress.GP_Battle, "Battle");
        }

        public void ResetManager()
        {
        }


        /// <summary>
        /// 基础类设置
        /// </summary>
        /// <returns></returns>
        public MonoBehaviour GetMonoInstance()
        {
            return this;
        }

        public bool GetIsNoPackMode()
        {
            return GetIsNoPack();
        }

        public T GetNoPackObject<T>(string fileName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (GetIsNoPackMode())
            {
                if (BundleManager.IsLoadComplete)
                {
                    return (T)EditorScripts.GetNoPackObject<T>(fileName);
                }
                else
                {
                    return null;
                }
            }
#endif
            return null;
        }

        public class GMBuildJson
        {
            public string uid;
        }

        public void SettingGM(GM.GMFunAction action)
        {
        }

        public IEnumerator WaitNextTimeRes(BundleManager.AskBundle data)
        {
            data.SetTag(BundleManager.AskResult.Wait);
            yield return new WaitForSeconds(3.0f);
            data.SetTag(BundleManager.AskResult.Continue);
        }

        public FESetting GetFESetting()
        {
            FESetting seting = FESetting.FE_NONE;
            if (DebugType == FDebugType.D_Log)
            {
                seting |= FESetting.FE_Log;
            }
            else if (DebugType == FDebugType.D_HideLog)
            {
                seting |= FESetting.FE_HideLog;
            }
            else if (DebugType == FDebugType.D_Leaker)
            {
                seting |= FESetting.FE_Log;
                seting |= FESetting.FE_Leaker;
            }

            if (GetIsNoPack())
            {
                seting |= FESetting.FE_NoPack;
            }
            return seting;
        }

        public string GetEditorPath(string fileName)
        {
#if UNITY_EDITOR
            if (GetIsNoPackMode())
            {
                string tempPath = EditorScripts.GetExistPath(fileName);
                if (tempPath != "")
                {
                    return tempPath;
                }
            }
#endif
            return fileName;
        }

        public ZipThreadData ThreadUnZip(string ZipFile, string TargetDirectory, string Password, LoadPercent loadPercent = null, bool OverWrite = true)//解压Zip
        {
            return FZipTool.ThreadUnZip(ZipFile, TargetDirectory, Password, loadPercent, OverWrite);
        }

        public IEnumerator LoadManager()
        {
            yield return ConfigManager.instance.LoadManager();
        }

        public Timer_Mix PreLoadObject(string id, bool isLoad)
        {
            return SceneManager.PreLoadObject(id, isLoad);
        }

        public void NetCallBack(bool result)
        {

        }

        //检查hase里面的协议是否有相同的key
        private void CheckProject()
        {

        }

        void OnDestroy()
        {
            Manager.Dispose();
        }
        public void LoadLevel(string name)
        {
            if (name != "")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(name);
            }
        }

        public static bool GetIsNoPack()
        {
#if UNITY_EDITOR
            if (FEngine.instance == null || FEngine.instance.IsNoPack)
            {
                return true;
            }
#endif
            return false;
        }

        public static bool CreateMySelf()
        {
            if (instance == null)
            {
                SceneManager.instance.CreateObject(ResConfig.CC_FENGINE, null);
                //#if UNITY_EDITOR
                string curName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (curName != LoadSceneManager.instance.GetSceneName(GameProgress.GP_LOG))
                {
                    LoadSceneManager.instance.SetScene(GameProgress.GP_Menu, UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    LoadSceneManager.instance.LoadDirectScene(GameProgress.GP_NONE);
                    LoadSceneManager.instance.LoadScene(GameProgress.GP_Menu);
                    return false;
                }
                //#endif
            }
            return true;
        }

        void OnApplicationQuit()
        {
            ConfigManager.instance.SaveConfig();
            FEngineManager.CloseEngine();
        }

        public void Update()
        {
        }

        public FNetMsgCore GetNetMsgCore()
        {
            return null;
        }

        public void LoadFrontScene(GameProgress gp)
        {
        }

        public BundleManager.BundleAskCall GetBundleAskCall()
        {
            throw new NotImplementedException();
        }
    }

}