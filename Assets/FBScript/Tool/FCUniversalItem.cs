//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FCUniversalItem : UnitObject
    {
        #region  基本用法
        private const string UNIVERSALNAME = "FUniversalItem";
        public string nTypeKey = "";
        public UnitObject mUnitObject;
        public bool StartInit = false;

        public bool StartConfig = false;
        public UniversalItemConfig mConfig = null;

        //unit数据基本配置
        public bool StartDataConfig = false;
        public string mDataConfig;

        public UnitObject instance
        {
            get
            {
                SetActive(true);
                return mUnitObject;
            }
        }

        public T GetInstance<T>() where T : UnitObject
        {
            SetActive<T>(true);
            return (T)mUnitObject;
        }


        public void RemoveShell()
        {
            if(mUnitObject != null)
            {
                mUnitObject.transform.SetParent(this.transform.parent);
                FEngineManager.Remove(this.gameObject);
            }
        }

        public static FCUniversalItem CreateUniversalItem(string typeKey,GameObject go)
        {
            FCUniversalItem item = go.AddComponent<FCUniversalItem>();
            item.nTypeKey = typeKey;
            return item;
        }

        public override void Clear()
        {
            if (mUnitObject != null)
            {
                FEngineManager.Remove(mUnitObject.gameObject);
                mUnitObject = null;
            }
        }

        public T ChangeItem<T>(string typeKey, bool isCreate = false) where T : UnitObject
        {
            Clear();
            nTypeKey = typeKey;
            mConfig = null;
            if (isCreate)
            {
                return GetInstance<T>();
            }
            else
            {
                return default(T);
            }
        }
        void Start()
        {
            if (StartInit)
            {
                SetActive<UnitObject>(true);
            }
        }

        public void SetActive(bool isTrue)
        {
            SetActive<UnitObject>(isTrue);
        }

        public void SetActive<T>(bool isTrue)where T:UnitObject
        {
            if (mUnitObject != null)
            {
                mUnitObject.gameObject.SetActive(isTrue);
            }
            else
            {
                if (isTrue)
                {
                    _createInstance<T>();
                    mUnitObject.gameObject.SetActive(isTrue);
                }
            }
        }

        private UnitObject _createInstance<T>()where T:UnitObject
        {
            if(mUnitObject != null)
            {
                return mUnitObject;
            }

            var obj = FEngineManager.Create(nTypeKey, this.gameObject);
            if(obj != null)
            {
                mUnitObject = obj.GetComponent<T>();
                if(mUnitObject == null)
                {
                    mUnitObject = obj.AddComponent<T>();
                }
            }
            var tran = mUnitObject.transform as RectTransform;
            if (tran != null)
            {
                tran.anchoredPosition3D = Vector3.zero;
            }
            else
            {
                mUnitObject.transform.localPosition = Vector3.zero;
            }
            return mUnitObject;
        }
        #endregion

        public class UniversalItemConfig
        {

        }
    }
}
