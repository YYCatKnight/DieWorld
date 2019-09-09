//----------------------------------------------
//  F2DEngine: time: 2018.9  by fucong QQ:353204643
//----------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace F2DEngine
{
    //核心类
    public static class FEngineManager
    {
        #region 基本路径设置
        public static System.Action<BasePlane> ShowWindosCall{ get; set; }

        private static string _website = "http://192.168.1.222:8080/server/";
        public static string WWWServe
        {
            get
            {
                return _website;
            }
            set
            {
                _website = value;
            }
        }

        private static string _wrpath;
        public static string WRPath
        {
            get
            {
                if (_wrpath == null)
                {
                    if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        _wrpath = Application.persistentDataPath + "//";
                    }
                    else if (Application.platform == RuntimePlatform.Android)
                    {
                        _wrpath = Application.persistentDataPath + "//";
                    }
                    else
                    {
                        _wrpath = Application.persistentDataPath + "/";
                    }
                }
                return _wrpath;
            }
        }

        private static string _assetbundle;
        public static string ASSETBUNDLE
        {
            get
            {
                if(_assetbundle == null)
                {
                    _assetbundle = "file:///" + WRPath + ResConfig.FDESASSETBUNDLE;
                }
                return _assetbundle;
            }
        }

        private static string _streamingpath;
        public static string STREAMINGPATH
        {
            get
            {
                if (_streamingpath == null)
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        _streamingpath =  "jar:file://" + Application.dataPath + "!/assets/";
                    }
                    else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    {
                        
                        _streamingpath = "file://"+ Application.streamingAssetsPath+"/";//"file:///" + Application.dataPath + "/Raw/";
                    }
                    else
                    {
                        _streamingpath = "file://" + Application.dataPath + "/StreamingAssets/";
                    }
                }
                return _streamingpath;
            }
        }

        private static string _streamassetbundle;
        public static string STREAMINGPATHASSETBUNDLE
        {
            get
            {
                if (_streamassetbundle == null)
                {
                    _streamassetbundle = STREAMINGPATH + ResConfig.FDESASSETBUNDLE;
                }
                return _streamassetbundle;
            }
        }

        private static FBEngine _fbengine;

        public static MonoBehaviour Engine
        {
            get { return _fbengine.GetMonoInstance();}
        }


        public static FNetMsgCore GetNetMsgCore()
        {
            if(_fbengine != null)
            {
                return _fbengine.GetNetMsgCore();
            }
            return null;
        }

        public static FBEngine GetFBEngine()
        {
            return _fbengine;
        }

        public static bool IsInitFEngine()
        {
            return _fbengine != null&& Engine != null;
        }


        public static void SetEngine(FBEngine engine)
        {
            _fbengine = engine;
            if(IsInitFEngine())
            {
                AddComponent<TimerController>(Engine.gameObject);
            }

            FEConfigure.SetConfig(_fbengine.GetFESetting());

            if(FEConfigure.mIsHideLog)
            {
                MyLog.SetLog(MyLog.MyLogType.LT_Hide);
            }
            else if(FEConfigure.mIsLog)
            {
                MyLog.SetLog(MyLog.MyLogType.LT_Normal);
            }
        }


        public static void CloseEngine()
        {
            Manager.Dispose();
        }

        #endregion

        #region 加载物体

        public static T AddComponent<T>(GameObject go) where T : Component
        {
            T t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
            }
            return t;
        }

        public static T InstantiateObject<T>(T ob) where T : Object
        {
            T clone = Object.Instantiate<T>(ob);
            clone.name = ob.name;
            return clone;
        }


        public static T CloneObject<T>(GameObject go, GameObject trans = null) where T : Component
        {
            GameObject clone = CloneObject(go, trans);
            return clone.GetComponent<T>();
        }

        public static T CreatePrefab<T>(string name, GameObject parent = null) where T : Component
        {
            GameObject obj = LoadPrefab<GameObject>(name);
            return CloneObject<T>(obj, parent);
        }

        public static T LoadAsset<T>(string name) where T : ScriptableObject
        {
            return LoadPrefab<T>(name + BundleManager.AssetType);
        }

        public static GameObject CloneObject(GameObject mainObject, GameObject pos = null)
        {
            GameObject clone = (GameObject)GameObject.Instantiate(mainObject);
            if (pos != null)
            {
                clone.transform.SetParent(pos.transform);
            }
            else
            {
                clone.transform.SetParent(mainObject.transform.parent);
            }
            clone.transform.localRotation = mainObject.transform.localRotation;
            clone.transform.localPosition = mainObject.transform.localPosition;
            clone.transform.localScale = mainObject.transform.localScale;

            RectTransform cloneRect = clone.transform as RectTransform;
            if(cloneRect != null)
            {
                RectTransform mainRect = mainObject.transform as RectTransform;
                if(mainRect!= null)
                {
                    cloneRect.offsetMax = mainRect.offsetMax;
                    cloneRect.offsetMin = mainRect.offsetMin;
                }
            }
            return clone;
        }

        public static Sprite CreateSprite(string name)
        {
            Sprite spr = LoadPrefab<Sprite>(name);
            return spr;
        }

        public static GameObject Create(string name, GameObject pos = null)
        {
            GameObject obj = LoadPrefab<GameObject>(name);
            if (obj == null)
            {
                Debug.LogError("没有控件" + name);
                return null;
            }
            obj = CloneObject(obj, pos);
            return obj;
        }

        public static UnitObject CreateObject(string name, GameObject pos)
        {
            GameObject obj = Create(name, pos);
            UnitObject uo = obj.GetComponent<UnitObject>();
            if (uo == null)
            {
                uo = AddComponent<UnitObject>(obj);
            }
            return uo;
        }

        public static void  Remove(GameObject go)
        {
            if (go != null)
            {
                GameObject.Destroy(go);
            }
        }


        public static T LoadPrefab<T>(string name) where T : Object
        {
            Object ob = null;

            //开发模式下免打包加载
            if (_fbengine != null && FEConfigure.mIsNoPack)
            {
                ob = _fbengine.GetNoPackObject<T>(name);
            }

            if (ob == null)
            {
                ob = BundleManager.instance.GetAssetObject<T>(name);
            }

            if (ob == null)
            {
                string tempName = name;
                int index = tempName.LastIndexOf(".");
                if (index != -1)
                {
                    tempName = tempName.Substring(0, index);
                }
                ob = Resources.Load<T>(tempName);
            }
            T t = ob as T;
            return t;
        }

        //异步加载资源接口
        public static void LoadPrefabAsync<T>(string name, System.Action<T> callBack, MonoBehaviour mo = null,Timer_Mix mix = null) where T : Object
        {
            BundleManager.instance.LoadAssetObjectAsync<T>(name, callBack, mo,mix);
        }


        public static void CreateObjectAsync(string name, System.Action<GameObject> callBack, MonoBehaviour mo = null)
        {
            LoadPrefabAsync<GameObject>(name, (s) =>
            {
                if (s == null)
                {
                    Debug.LogError("没有控件" + name);
                    callBack(null);
                }
                else
                {
                    callBack(CloneObject(s));
                }
            }, mo);
        }

        #endregion

        #region 回收池加载
        public class PoolControl
        {
            private static int mPoolNum = 0;
            private GameObject mParent;
            internal PoolControl()
            {
                mPoolNum++;
            }
            internal void CreateTag()
            {
                mParent = new GameObject("PoolControl_"+mPoolNum.ToString());
                mParent.transform.SetParent(Engine.gameObject.transform);
            }
            public Dictionary<string, List<UnitObject>> UnitObjPools = new Dictionary<string, List<UnitObject>>();
            public UnitObject Create(string name,GameObject pos)
            {
                List<UnitObject> tempObjectList = null;
                if (UnitObjPools.TryGetValue(name, out tempObjectList))
                {
                    if (tempObjectList.Count > 0)
                    {
                        UnitObject uo = tempObjectList[0];
                        tempObjectList.RemoveAt(0);
                        if (pos != null)
                        {
                            uo.gameObject.transform.SetParent(pos.transform);
                        }
                        uo.ResetData();
                        uo.gameObject.SetActive(true);
                        return uo;
                    }
                }
                UnitObject ob = CreateObject(name, pos);
                ob.PoolName = name;
                ob.IsPool = true;
                ob.gameObject.name = name;
                return ob;
            }
            public void Delete(UnitObject uo)
            {
                if (uo != null && uo.IsPool)
                {
                    List<UnitObject> tempObjectList = null;
                    if (!UnitObjPools.TryGetValue(uo.PoolName, out tempObjectList))
                    {
                        tempObjectList = new List<UnitObject>();
                        UnitObjPools[uo.PoolName] = tempObjectList;
                    }
                    if (tempObjectList.Count < uo.GetPoolCount())
                    {
                        uo.Clear();
                        uo.gameObject.SetActive(false);
                        uo.gameObject.transform.SetParent(mParent == null?Engine.gameObject.transform: mParent.transform);
                        tempObjectList.Add(uo);
                    }
                    else
                    {
                        Remove(uo.gameObject);
                    }
                }
                else if (uo != null)
                {
                    Remove(uo.gameObject);
                }
            }
            public void Release()
            {
                Remove(mParent);
                UnitObjPools = null;
            }
        }

        private static PoolControl mPoolControl = new PoolControl();//实体池mono obj

        public static PoolControl CreatePoolControl()
        {
            PoolControl pc = new PoolControl();
            pc.CreateTag();
            return pc;
        }

        public static T PoolObject<T>(string name, GameObject pos = null) where T : UnitObject
        {
            return (T)PoolObject(name, pos);
        }

        public static UnitObject PoolObject(string name, GameObject pos = null)
        {
           return mPoolControl.Create(name, pos);
        }

        public static void DeletObject(UnitObject uo)
        {
            mPoolControl.Delete(uo);
        }

        #endregion

        #region UI界面管理
        /////////////////////界面逻辑
        private static List<BasePlane> mActivePlane = new List<BasePlane>();
        private static List<WindosUIData> mBackWindos = new List<WindosUIData>(); //回退界面
        private static Dictionary<string, BasePlane> mPoolWindos = new Dictionary<string, BasePlane>(); //界面池
        private static int mOpenTimes = 0;
        private static void PushUIPool(BasePlane ui)
        {
            ui.BackPoolClear();
            ui.gameObject.SetActive(false);
            mPoolWindos[ui.PoolName] = ui;
        }

        private static BasePlane CreatePoolUI(string key)
        {
            BasePlane bp = null;
            if (mPoolWindos.TryGetValue(key, out bp))
            {
                if (bp != null)
                {
                    if (bp.PoolName == key)
                    {
                        bp.gameObject.SetActive(true);
                        mPoolWindos.Remove(key);
                        return bp;
                    }
                }
            }
            return null;
        }

        public static BasePlane ShowWindos(string name, params object[] arg)
        {
            return ShowWindos(name, WinShowType.WT_Normal, arg);
        }

        public static BasePlane ShowWindos(string name, WinShowType type = WinShowType.WT_Normal, params object[] arg)
        {
            if (string.IsNullOrEmpty(name) || name == "null")
                return null;
            mOpenTimes++;
            BasePlane up = GetActiveWindos(name);
            BasePlane.OpenType ot = BasePlane.OpenType.OT_Normal;

            if (up != null)
            {
                if (up.RefreshType == UIRefresh_Type.None)
                {
                    return up;
                }
                else if(up.RefreshType == UIRefresh_Type.Refresh)
                {
                    ot = BasePlane.OpenType.OT_Active;
                }
                else if(up.RefreshType == UIRefresh_Type.Reset)
                {
                    up.CloseMySelf();
                    return ShowWindos(name,type,arg);
                }
            }
            else
            {
                //先用池加载
                up = CreatePoolUI(name);

                if (up == null)
                {
                    GameObject go = PoolObject(name, null).gameObject;
                    up = go.GetComponent<BasePlane>();
                    if (up == null)
                    {
                        up = go.AddComponent<BasePlane>();
                        Debug.LogError(name + "界面没有继承BasePlane");
                    }

                    FCMainCanvas.instance.SetLayer(up.LayerType, go);
                    up.IsPool = up.UsePool;
                    ot = BasePlane.OpenType.OT_Normal;
                }
                else
                {
                    ot = BasePlane.OpenType.OT_BackPool;
                }

                up.SetHandle(name,mOpenTimes);

                if (GetIsCloseOther(up.nUiType) && !IsHaveSameWinType(type, WinShowType.WT_NoClose))
                {
                    _CloseWindos("", up.HandleID, type);
                }
                mActivePlane.Add(up);
            }

            if (up == null)
            {
                Debug.LogError("未找到窗口： " + name);
                return null;
            }
            up.mWinShowType = type;
           
            if(ShowWindosCall != null)
            {
                ShowWindosCall(up);
            }
            up.openInit(ot, arg);
            return up.mIsClose ? null : up;
        }

        public static void CloseWindos(BasePlane p)
        {
            if (p != null)
            {
                _CloseWindos(p.HandleID);
            }
        }

        private class WindosUIData
        {
            public string keyWindosName;
            public string windosName;
            public WinShowType showType;
            public object[] arg = null;
            public List<WindosUIData> nextUIData = new List<WindosUIData>();
        }

        public static void ClearBackWindos()
        {
            mBackWindos.Clear();
            foreach (var k in mPoolWindos)
            {
                k.Value.CloseClear();
                FEngineManager.DeletObject(k.Value);
            }
            mPoolWindos.Clear();
        }

        public static void ResetWindos(string keyName)
        {
            if (mBackWindos.Count != 0)
            {
                WindosUIData wud = mBackWindos[mBackWindos.Count - 1];
                if (wud.keyWindosName == keyName)
                {
                    for (int i = wud.nextUIData.Count - 1; i >= 0; i--)
                    {
                        ShowWindos(wud.nextUIData[i].windosName, wud.nextUIData[i].showType, wud.nextUIData[i].arg);
                    }
                    ShowWindos(wud.windosName, wud.showType, wud.arg);
                    mBackWindos.Remove(wud);
                }
                else
                {
                    BasePlane bp = GetActiveWindos(keyName);
                    if (bp != null && !GetIsCloseOther(bp.nUiType))
                    {
                        ClearBackWindos();
                    }
                }
            }
        }

        private static bool GetIsCloseOther(UIWIND_TYPE utype)
        {
            if (utype == UIWIND_TYPE.UI_TEMP || utype == UIWIND_TYPE.UI_SOLF)
            {
                return false;
            }
            return true;
        }

        private static bool GetAutoIsByClose(UIWIND_TYPE utype)
        {
            if (utype == UIWIND_TYPE.UI_SOLID || utype == UIWIND_TYPE.UI_SOLF)
            {
                return false;
            }
            return true;
        }

        public static bool IsHaveSameWinType(WinShowType main, WinShowType use)
        {
            return ((int)(main & use)) != 0;
        }

        internal static void _CloseWindos(string name = "", string curOpenName = "", WinShowType type = WinShowType.WT_Normal)
        {
            if (name == "" && curOpenName == "")
            {
                ClearBackWindos();
            }

            bool isFirst = true;
            for (int i = mActivePlane.Count - 1; i >= 0; i--)
            {
                if (name == "" || mActivePlane[i].HandleID == name)
                {
                    BasePlane up = mActivePlane[i];
                    if (GetAutoIsByClose(up.nUiType) || name != "")
                    {
                        //卸载代码
                        if (IsHaveSameWinType(type, WinShowType.WT_Back))
                        {
                            WindosUIData w = new WindosUIData();
                            w.keyWindosName = curOpenName;
                            w.windosName = up.PoolName;
                            w.showType = up.mWinShowType;
                            w.arg = up.mArgs;
                            if (isFirst)
                            {
                                mBackWindos.Add(w);
                                isFirst = false;
                            }
                            else
                            {
                                if (mBackWindos.Count > 0)
                                {
                                    WindosUIData widn = mBackWindos[mBackWindos.Count - 1];
                                    widn.nextUIData.Add(w);
                                }
                            }
                        }
                        mActivePlane.RemoveAt(i);

                        up.SetDelayEvent(() =>
                        {
                            if (IsHaveSameWinType(type, WinShowType.WT_Pool))
                            {
                                PushUIPool(up);
                            }
                            else
                            {
                                up.CloseClear();
                                FEngineManager.DeletObject(up);
                            }

                            if (IsHaveSameWinType(type, WinShowType.WT_Reset))
                            {
                                if (mActivePlane.Count > 0)
                                {
                                    BasePlane rplane = null;
                                    for (int p = mActivePlane.Count - 1; p >= 0; p--)
                                    {
                                        var plane = mActivePlane[p];
                                        if (plane.nUiType == UIWIND_TYPE.UI_NORMAL)
                                        {
                                            rplane = plane;
                                            break;
                                        }
                                    }

                                    if (rplane != null)
                                    {
                                        _CloseWindos(rplane.HandleID);
                                        var lastWinShow = rplane.mWinShowType;
                                        var newPlane = ShowWindos(rplane.PoolName, WinShowType.WT_NoClose, rplane.mArgs);
                                        if (newPlane != null)
                                        {
                                            newPlane.mWinShowType = lastWinShow;
                                        }
                                    }

                                }
                            }
                        }, name != "");
                    }
                }
            }
        }

        public static void CloseWindos(string name = "",bool isAuto = true)
        {
            _CloseWindos(name);
            if(!isAuto)
            {
                if (string.IsNullOrEmpty(name))
                {
                    for (int i = mActivePlane.Count - 1; i >= 0; i--)
                    {
                        CloseWindos(mActivePlane[i]);
                    }
                }
            }
        }

        public static bool IsActiveWindos(UIWIND_TYPE type)
        {
            for (int i = 0; i < mActivePlane.Count; i++)
            {
                if (mActivePlane[i].nUiType == type)
                {
                    return true;
                }
            }
            return false;
        }

        public static BasePlane GetActiveWindos(string name)
        {
            for (int i = 0; i < mActivePlane.Count; i++)
            {
                if (mActivePlane[i].HandleID == name)
                {
                    return mActivePlane[i];
                }
            }
            return null;
        }


        public static void LookForWindows(System.Func<BasePlane, bool> lookAction)
        {
            for (int i = mActivePlane.Count - 1; i >= 0; i--)
            {
                if (lookAction(mActivePlane[i]))
                {
                    return;
                }
            }
        }

        public static bool CloseFrontWindos()
        {
            for(int i = mActivePlane.Count -1;i>= 0;i--)
            {
                if(mActivePlane[i].nUiType == UIWIND_TYPE.UI_NORMAL|| mActivePlane[i].nUiType == UIWIND_TYPE.UI_TEMP)
                {
                    mActivePlane[i].CloseMySelf();
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Asset数据加载
        //assetData
        public static Dictionary<string, BaseAsset> mAssetDataDic = new Dictionary<string, BaseAsset>();

        public static T LoadAssetData<T>(string keyName) where T : BaseAsset
        {
            return (T)GetBaseAsset(keyName);
        }

        private static  BaseAsset GetBaseAsset(string keyName)
        {
            if (!mAssetDataDic.ContainsKey(keyName))
            {
                var assData = LoadAsset<BaseAsset>(ResConfig.ASSET_PAHT + "/" + keyName);
                if (assData != null)
                {
                    mAssetDataDic[keyName] = assData;
                    mAssetDataDic[keyName].init();
                }
                else
                {
                    Debug.LogError("配置表:[" + keyName + "]未找到");
                    return null;
                }
            }
            return mAssetDataDic[keyName];
        }
        #endregion

        #region 声音管理
        private static Dictionary<string, AudioClip> mAudioClipList = new Dictionary<string, AudioClip>();
        private static List<FCMusciObject> mMusicPool = new List<FCMusciObject>();
        private static Dictionary<string, FCMusciObject> mActiveMusic = new Dictionary<string, FCMusciObject>();
        private static Vector2 mMusciSize = Vector2.zero;

        public static void StopSound(FCMusciObject fmo)
        {
            StopSound(fmo.gameObject.name);
        }
        public static void StopSound(string musicName = "")
        {
            if (musicName == "")
            {
                List<string> deletKeys = new List<string>(mActiveMusic.Keys);
                for (int i = 0; i < deletKeys.Count; i++)
                {
                    StopSound(deletKeys[i]);
                }
            }
            else
            {
                if (mActiveMusic.ContainsKey(musicName))
                {
                    FCMusciObject fmo = mActiveMusic[musicName];
                    fmo.Clear();
                    fmo.gameObject.SetActive(false);
                    mActiveMusic.Remove(musicName);
                    mMusicPool.Add(fmo);
                }
            }
        }
        private static void _SetSound(float size, FCMusciObject.MusicType m)
        {
            foreach (var k in mActiveMusic)
            {
                FCMusciObject fmo = k.Value;
                if (fmo.mMusicType == m)
                {
                    fmo.nAudioSource.volume = fmo.GetCurValue() * size;
                }
            }
        }
        public static void SetSoundVolume(float size)
        {
            mMusciSize.y = size;
            _SetSound(size,FCMusciObject.MusicType.Sound);
        }
        public static void SetMusicVolume(float size)
        {
            mMusciSize.x = size;
            _SetSound(size, FCMusciObject.MusicType.Music);
        }

        public static FCMusciObject GetSound(string name)
        {
            if (mActiveMusic.ContainsKey(name))
            {
                return mActiveMusic[name];
            }
            return null;
        }

        public static FCMusciObject PlaySound(string name,float startTime = 0,float volume = 1)
        {
            return PlaySound(name,false,FCMusciObject.MusicType.Sound,null, 0,startTime, volume);
        }

        public static FCMusciObject PlayMusic(string name,string group = null, float volume = 1)
        {
            return PlaySound(name, true, FCMusciObject.MusicType.Music, group,0,0,volume);
        }

        public static FCMusciObject PlaySound(string name,bool loop,FCMusciObject.MusicType type = FCMusciObject.MusicType.Sound, string group = null,float len = 0,float startTime = 0,float volume = 1)
        {
            group =  string.IsNullOrEmpty(group)? name : group;
            if (mActiveMusic.ContainsKey(group))
            {
                StopSound(group);
            }

            if (!mAudioClipList.ContainsKey(name))
            {
                AudioClip ac = LoadPrefab<AudioClip>(ResConfig.MUSIC_PATH + name);
                if (ac == null)
                {
                    Debug.Log("<color=#660000>"+name + ":声音没有找到</color>");
                    return null;
                }
                mAudioClipList[name] = ac;
            }

            FCMusciObject mo = null;
            if (mMusicPool.Count > 0)
            {
                mo = mMusicPool[0];
                mMusicPool.RemoveAt(0);
            }
            else
            {
                mo = (FCMusciObject)CreateObject(ResConfig.CC_MUSICAUDION, Engine.gameObject);
            }

            if (mo != null)
            {
                mo.gameObject.SetActive(true);
                mActiveMusic[group] = mo;
                mo.gameObject.name = group;
                mo.SetCurValue(volume);
                mo.mMusicType = type;
                float v = 0;
                if (type == FCMusciObject.MusicType.Music)
                {
                    v = mMusciSize.x;
                }
                else
                {
                    v = mMusciSize.y;
                }
                mo.PlaySound(name, mAudioClipList[name],len, volume * v, startTime, loop);
            }
            return mo;
        }


        #endregion

        #region 其他

        //拷贝复制
        public static T CopyData<T>(T obj)
        {
            var pack = BytesSerialize.Serialize(obj);
            return (T)BytesSerialize.Deserialize<T>(pack);
        }

        public static string ConvertPath(string fileName, FFilePath pathType = FFilePath.FP_Cache)
        {
            string converPath = "";
            if (pathType == FFilePath.FP_Relative)
            {
                converPath = fileName.ToLower();
                if (FEConfigure.mIsNoPack)
                {
                    converPath = _fbengine.GetEditorPath(converPath);
                }
                else
                {
                    converPath = WRPath + ResConfig.FDESASSETBUNDLE + "/" + converPath;
                }
            }
            else if (pathType == FFilePath.FP_Cache)
            {
                converPath = WRPath + fileName;
            }
            else
            {
                converPath = fileName;
            }
            return converPath;
        }

        public static string GetMD5HashFromFile(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, System.IO.FileMode.Open);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static void DeletFile(string path)
        {
            if (Directory.Exists(path))
            {
                // 获得文件夹数组
                string[] strDirs = Directory.GetDirectories(path);
                // 获得文件数组  
                string[] strFiles = System.IO.Directory.GetFiles(path);

                // 遍历所有子文件夹 
                foreach (string strFile in strFiles)
                {
                    // 删除文件夹  
                    File.Delete(strFile);
                }
                // 遍历所有文件  
                foreach (string strdir in strDirs)
                {
                    Directory.Delete(strdir, true);
                }
                Directory.Delete(path, true);
            }
        }

        public static void NormalRectTransform(this RectTransform rt)
        {
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            rt.localRotation = Quaternion.identity;
        }


        public static List<T> RandomSortList<T>(List<T> ListT)
        {
            List<T> newList = new List<T>(ListT);
            int currentIndex;
            T tempValue;
            for (int i = 0; i < newList.Count; i++)
            {
                currentIndex = UnityEngine.Random.Range(0, newList.Count - i);
                tempValue = newList[currentIndex];
                newList[currentIndex] = newList[newList.Count - i - 1];
                newList[newList.Count - i - 1] = tempValue;
            }
            return newList;
        }

        public static void SetList<T>(List<T> list, System.Action<T, int> stateCall)
        {
            for (int i = 0; i < list.Count; i++)
            {
                stateCall(list[i], i);
            }
        }

        #endregion
    }

}
