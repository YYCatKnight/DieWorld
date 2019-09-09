//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace F2DEngine
{
    public class SceneManager : ManagerTemplate<SceneManager>
    {

        public static string MianMusicGroup = "MianGroup";
        #region 基本路径设置
        public static string WWWServe
        {
            get
            {
                return FEngineManager.WWWServe;
            }
        }
        public static string ASSETBUNDLE
        {
            get
            {
                return FEngineManager.ASSETBUNDLE;
            }
        }

        public static string STREAMINGPATHASSETBUNDLE
        {
            get
            {
                return FEngineManager.STREAMINGPATHASSETBUNDLE;
            }
        }
        public static string STREAMINGPATH
        {
            get
            {
                return FEngineManager.STREAMINGPATH;
            }
        }

        public static string WRPath
        {
            get
            {
                return FEngineManager.WRPath;
            }
        }


        #endregion

        #region 声音管理
        public void StopSound(FCMusciObject fmo)
        {
            FEngineManager.StopSound(fmo);
        }

        public void StopSound(string musicName = "")
        {
            FEngineManager.StopSound(musicName);
        }
        public void SetSoundVolume(float size)
        {
            FEngineManager.SetSoundVolume(size);
        }
        public void SetMusicVolume(float size)
        {
            FEngineManager.SetMusicVolume(size);
        }
        public FCMusciObject PlayMusic(string name, string group = null, float volume = 1)
        {
            return FEngineManager.PlayMusic(name, group, volume);
        }
        public FCMusciObject GetSound(string name)
        {
            return FEngineManager.GetSound(name);
        }
        public FCMusciObject PlaySound(string name, float startTime = 0, float volume = 1)
        {
            return FEngineManager.PlaySound(name, startTime, volume);
        }
        public FCMusciObject PlaySound(string name, bool loop, FCMusciObject.MusicType type = FCMusciObject.MusicType.Sound, string group = null, float len = 0, float startTime = 0, float volume = 1)
        {
            return FEngineManager.PlaySound(name, loop, type, group, len, startTime, volume);
        }

        public FCMusciObject PlaySoundByID(string id, string group = null)
        {
            if (soundsAsset.instance == null)
                return null;
            soundsProperty mp = soundsAsset.instance.GetProperty(id);
            if (mp != null)
            {
                return PlaySound(mp.name, mp.isloop, mp.isloop ? FCMusciObject.MusicType.Music : FCMusciObject.MusicType.Sound, group, mp.time, mp.starttime, mp.volume);
            }
            return null;
        }
        #endregion

        #region 加载物体
        public T AddComponent<T>(GameObject go) where T : Component
        {
            return FEngineManager.AddComponent<T>(go);
        }


        public T InstantiateObject<T>(T ob) where T : Object
        {
            return FEngineManager.InstantiateObject<T>(ob);
        }


        public static T CloneObject<T>(GameObject go, GameObject trans = null) where T : Component
        {
            return FEngineManager.CloneObject<T>(go, trans);
        }

        public static T CreatePrefab<T>(string name, GameObject parent = null) where T : Component
        {
            return FEngineManager.CreatePrefab<T>(name, parent);
        }

        public static T LoadAsset<T>(string name) where T : ScriptableObject
        {
            return FEngineManager.LoadAsset<T>(name);
        }


        public List<string> GetPathFiles(string path, string exEnd)
        {

#if UNITY_EDITOR
            if (FEngine.GetIsNoPack())
            {
                return EditorScripts.GetExistFilesPath(path, exEnd);
            }
#endif
            List<string> tempString = new List<string>();
            tempString = FCommonFunction.GetFiles(SceneManager.WRPath + ResConfig.FDESASSETBUNDLE + "/" + path.ToLower(), tempString, exEnd.ToLower());
            return tempString;
        }

        public static T LoadPrefab<T>(string name) where T : Object
        {
            return FEngineManager.LoadPrefab<T>(name);
        }

        public Sprite CreateSprite(string name)
        {
            return FEngineManager.CreateSprite(name);
        }

        public static GameObject CloneObject(GameObject mainObject, GameObject pos = null)
        {
            return FEngineManager.CloneObject(mainObject, pos);
        }






        public FCMeshBody CreateMeshBody(string boneName, GameObject pos = null)
        {
            GameObject go = new GameObject("Mesh_Clone");
            if (pos != null)
            {
                go.transform.SetParent(pos.transform);
            }
            go.transform.localPosition = Vector3.zero;
            FCMeshBody meshBody = go.AddComponent<FCMeshBody>();
            meshBody.CreateBones(boneName);
            return meshBody;
        }

        public FSpineBody CreateSpineBody(string name, GameObject pos = null)
        {
            return null;
        }

        public GameObject Create(string name, GameObject pos = null)
        {
            return FEngineManager.Create(name, pos);
        }

        public void Remove(GameObject go)
        {
            FEngineManager.Remove(go);
        }


        //异步加载资源接口
        public void LoadPrefabAsync<T>(string name, System.Action<T> callBack, MonoBehaviour mo = null) where T : Object
        {
            FEngineManager.LoadPrefabAsync<T>(name, callBack, mo);
        }

        public void CreateObjectAsync(string name, System.Action<GameObject> callBack, MonoBehaviour mo = null)
        {
            FEngineManager.CreateObjectAsync(name, callBack, mo);
        }


        #endregion

        #region 回收池加载

        public UnitObject CreateObject(string name, GameObject pos)
        {
            return FEngineManager.CreateObject(name, pos);
        }

        public T PoolObject<T>(string name, GameObject pos = null) where T : UnitObject
        {
            return FEngineManager.PoolObject<T>(name, pos);
        }

        public UnitObject PoolObject(string name, GameObject pos = null)
        {
            return FEngineManager.PoolObject(name, pos);
        }



        public void DeletObject(UnitObject uo)
        {
            FEngineManager.DeletObject(uo);
        }

        #endregion


        #region Asset数据加载
        public static T LoadAssetData<T>(string keyName) where T : BaseAsset
        {
            return FEngineManager.LoadAssetData<T>(keyName);
        }
        #endregion

        #region 预加载

        public static Timer_Mix PreLoadObject(string configId, bool isLoad = true)
        {
            if (isLoad)
            {
                var pro = PreBundleAsset.instance.GetProperty(configId);
                if (pro != null)
                {
                    return BundleManager.instance.PreLoadBackObject(configId, pro.PreFiles, pro.PrePahts, 1);
                }
            }
            else
            {
                //BundleManager.instance.DeletePreLoad(configId);
            }
            return null;
        }
        #endregion

        public void ComputeLangTable()
        {
            BaseAsset langAsset = null;
            if (FEngineManager.mAssetDataDic.TryGetValue("local_langAsset", out langAsset))
            {
                langAsset.ResetTable();
                FEngineManager.mAssetDataDic.Remove("local_langAsset");
            }
            var map = local_langAsset.instance.GetMap();
            var langMap = Lange_ListAsset.instance.GetMap();
            foreach(var k in map)
            {
                langMap[k.Key] = k.Value;
            }
        }
    }
}