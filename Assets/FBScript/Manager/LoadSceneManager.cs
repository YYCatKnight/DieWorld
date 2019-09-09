//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public enum GameProgress
    {
        GP_NONE = 0,
        GP_LOG,
        GP_Menu,
        GP_MainScene,
        GP_Temp,
        GP_Home,
        GP_Battle,
        GP_City,
        GP_World,
        GP_Map1,
        GP_Map2,
        GP_Map3,
    }
    public class LoadSceneManager : ManagerTemplate<LoadSceneManager>
    {
        public const string LoadEvent = "LOADEVENT";
        public GameProgress GameActiveScene
        {
            get { return mCurGameProgress; }
        }

        public GameProgress GameBackScene
        {
            get { return mLastGameProgree; }
        }

        private GameProgress mCurGameProgress = GameProgress.GP_NONE;
        private GameProgress mLastGameProgree = GameProgress.GP_NONE;
        private Dictionary<GameProgress, string> mSceneProgress = new Dictionary<GameProgress, string>();
        public LoadMode CurMode { get; protected set; }
        public LoadMode LastMode { get; protected set; }
        protected override void OnInit()
        {
            mSceneProgress[GameProgress.GP_NONE] = "None";
            mSceneProgress[GameProgress.GP_LOG] = "Logo";
            mSceneProgress[GameProgress.GP_Menu] = "Menu";
            mSceneProgress[GameProgress.GP_MainScene] = "F_Examples";
            mSceneProgress[GameProgress.GP_Temp] = "";
            mSceneProgress[GameProgress.GP_City] = "Base";
            mSceneProgress[GameProgress.GP_World] = "World";
        }

        public string GetSceneName(GameProgress key)
        {
            return mSceneProgress[key];
        }
        public void SetScene(GameProgress key, string name)
        {
            mSceneProgress[key] = name;
        }   
        public enum LoadType
        {
            LT_Begin,//开始进入
            LT_BeginMask,//进入遮罩
            Lt_EnterMask,
            Lt_Enter,//进入完成
            Lt_Back,//离开
        }

        public interface LoadTool
        {
        }

        public void LoadDirectScene(GameProgress pro)
        {
            if (mSceneProgress.ContainsKey(pro))
            {
                if (pro != GameProgress.GP_NONE)
                {
                    mLastGameProgree = mCurGameProgress;
                    mCurGameProgress = pro;
                }
                string name = mSceneProgress[pro];
                if (name != "")
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(name);
                }
            }
        }

        public void  LoadScene(GameProgress pro,params LoadTool[] tools)
        {
            LoadMode mode = new LoadMode(tools);
            mode.SetProgress(mCurGameProgress, pro);
            LastMode = CurMode;
            CurMode = mode;
            FCLoadingPlane.StartLoad(pro, mode);
        }

        public void _SetCurGameProgress(GameProgress pro)
        {
            mCurGameProgress = pro;
        }
        internal IEnumerator LoadSceneAsy(GameProgress pro = GameProgress.GP_NONE)
        {
            mLastGameProgree = mCurGameProgress;
            mCurGameProgress = pro;
            var scene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(mSceneProgress[pro]);
            yield return scene;
        }
    }
    public class PlaneMode : LoadSceneManager.LoadTool
    {
        public string PlaneName { get; set; }//加载界面名字
    }
    public class ActionMode:LoadSceneManager.LoadTool
    {
        private Dictionary<LoadSceneManager.LoadType,System.Action> mCallBack = new Dictionary<LoadSceneManager.LoadType, System.Action>();
        public void SetAction(LoadSceneManager.LoadType type,System.Action callBack)
        {
            mCallBack[type] = callBack;
        }
        public void PlayAction(LoadSceneManager.LoadType type)
        {
            if(mCallBack.ContainsKey(type))
            {
                mCallBack[type]();
            }
        }
    }
    public class ParamsMode:LoadSceneManager.LoadTool
    {
        public object[] mObjs { get; set;}
    }
    public class LoadMode
    {
        
        public GameProgress FromSceneName { get; protected set; }
        public GameProgress ToSceneName { get; protected set; }
        internal void SetProgress(GameProgress from, GameProgress to)
        {
            FromSceneName = from;
            ToSceneName = to;
        }
        private Dictionary<string, LoadSceneManager.LoadTool> mTools = new Dictionary<string, LoadSceneManager.LoadTool>();
        public string PlaneName
        {
            get
            {
                PlaneMode mode = GetTool<PlaneMode>();
                if(mode != null)
                {
                    return mode.PlaneName;
                }
                return null;
            }
        }
        internal void PlayLoad(LoadSceneManager.LoadType type)
        {
            var mode = GetTool<ActionMode>();
            if(mode != null)
            {
                mode.PlayAction(type);
            }

        }
        public object[]GetParams()
        {
            var par = GetTool<ParamsMode>();
            if (par != null)
                return par.mObjs;
            return null;
        }
        private T GetTool<T>() where T: LoadSceneManager.LoadTool
        {
            LoadSceneManager.LoadTool lt = null;
            if(mTools.TryGetValue(typeof(T).FullName, out lt))
            {
                return (T)lt;
            }
            return default(T);
        }
        public LoadMode(LoadSceneManager.LoadTool[] tools)
        {
            if (tools != null)
            {
                for (int i = 0; i < tools.Length; i++)
                {
                    var t = tools[i];
                    if (t != null)
                    {
                        mTools[t.GetType().FullName] = t;
                    }
                }
            }
        }
        public static LoadSceneManager.LoadTool SetPName(string name)
        {
            PlaneMode mode = new PlaneMode();
            mode.PlaneName = name;
            return mode;
        }
        public static LoadSceneManager.LoadTool SetAction(System.Action call, LoadSceneManager.LoadType type = LoadSceneManager.LoadType.Lt_Back)
        {
            ActionMode mode = new ActionMode();
            mode.SetAction(type,call);
            return mode;
        }
        public static LoadSceneManager.LoadTool SetParamas(params object[] arg)
        {
            ParamsMode mode = new ParamsMode();
            mode.mObjs = arg;
            return mode;
        }
    }
}
