//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public class FCUniversalPanel : UnitObject
    {
        [HideInInspector]
        [SerializeField]
        public List<GameObject> mValue = new List<GameObject>();

        [HideInInspector]
        [SerializeField]
        public string mKey = "F_";

        public void ApplyData(string key)
        {
            mKey = key;
            ApplyData();
        }

        public void ApplyData()
        {
            mValue.Clear();
            int len = mKey.Length;
            List<string> keepName = new List<string>();
            Transform[] ts = gameObject.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < ts.Length; i++)
            {
                var go = ts[i].gameObject;
                string goName = go.name;
                if (goName.Contains(mKey))
                {
                    mValue.Add(go);
                    keepName.Add(goName);
                }
            }
        }

        public GameObject GetFObject(string keyName)
        {
            for(int i = 0; i < mValue.Count;i++)
            {
                if(mValue[i].name == keyName)
                {
                    return mValue[i];
                }
            }

            Debug.LogError(string.Format("没有找到物体{0}", keyName));
            //容错处理
            GameObject go = new GameObject();
            go.transform.SetParent(this.transform);
            go.transform.localPosition = new Vector3(99999, 99999, 99999);
            go.name = keyName;
            return null;
        }

        public T GetItem<T>(string key,bool isRemoveShell = false) where T : UnitObject
        {
            string realyKey = key + "_unitObject";
            var universalItem = GetFObject<FCUniversalItem>(key);
            var ins = universalItem.GetInstance<T>();
            if (isRemoveShell)
            {
                mValue.Remove(universalItem.gameObject);
                universalItem.RemoveShell();
            }
            return ins;
        }

        public T GetFObject<T>(string key) where T : Component
        {
            GameObject go = GetFObject(key);
            if (go == null)
            {
                Debug.LogError("没有找到物体：" + key);
                return default(T);
            }
            T t = go.GetComponent<T>();
            if (t != null)
            {
                return t;
            }
            else
            {
                Debug.LogError("没有在" + key + "物体上找到相应脚本:" + typeof(T).FullName);
                //容错处理
                t = go.AddComponent<T>();
            }
            return null;
        }

    }
}
