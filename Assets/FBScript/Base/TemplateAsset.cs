//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class BaseAssetProperty
    {
        public string Only_id;
        public int ID32 { get { return int.Parse(Only_id); } }
    }

    public class TemplateAsset<T, F> : BaseAsset
        where T : TemplateAsset<T, F>
        where F : BaseAssetProperty
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FEngineManager.LoadAssetData<T>(typeof(T).FullName);
                    if(_instance == null)
                    {
                        Debug.LogError("没有找到AssetData:"+ typeof(T).FullName);
                    }
                }
                return _instance;
            }
        }

        public List<F> ProList = new List<F>();
        protected Dictionary<string, F> MapList = new Dictionary<string, F>();
        public  F GetProperty(int only_id)
        {
            return GetProperty(only_id.ToString());
        }

        public override void ResetTable()
        {
            _instance = null;
        }

        public  F GetProperty(string only_id)
        {
            F pro = null;
            if (MapList.TryGetValue(only_id, out pro))
            {
                return pro;
            }
            return ShowNoLog(only_id);
        }

        public F CopyProperty(int only_id)
        {
            return CopyProperty(only_id.ToString());
        }

        public F CopyProperty(string only_id)
        {
            var f = GetProperty(only_id);
            return f;
        }

        protected virtual F ShowNoLog(string only_id)
        {
            Debug.LogError(typeof(T).FullName + "数据表中没有找到: id[" + only_id + "]");
            if (ProList.Count > 0)
            {
                return ProList[0];
            }
            return null;
        }

        protected virtual void InitData()
        {

        }

        public Dictionary<string, F> GetMap()
        {
            return MapList;
        }

        public sealed override void init()
        {
            for (int i = 0; i < ProList.Count; i++)
            {
                F pro = ProList[i];
                MapList[pro.Only_id] = pro;
            }
            InitData();
        }

    }
}
