//----------------------------------------------
//  F2DEngine: time: 2016.3  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public class StateTimeEvent: TimerEvent
    {
        public enum STimeType
        {
            ST_Normal,
            ST_Reset,
            ST_Replace
        }

        private const string DEFAULT = "Default";
        private Action<STimeData,STimeType> mCallEvent;
        public class STimeData
        {
            public string key;
            public float time;
            public string Group;
            public object ex;
            public float allTime;
        }

        public StateTimeEvent(Action<STimeData,STimeType> callEvent)
        {
            mCallEvent = callEvent;
        }


        protected List<STimeData> mSTimeData = new List<STimeData>();
        protected Dictionary<string, STimeData> mMapSTimeData = new Dictionary<string, STimeData>();

        public STimeData GetFirstSTimeData()
        {
            if (mSTimeData.Count > 0)
                return mSTimeData[0];
            return null;
        }

        public void Reset(string key)
        {
            if(mMapSTimeData.ContainsKey(key))
            {
                var std = mMapSTimeData[key];
                mSTimeData.Remove(std);
                mMapSTimeData.Remove(std.key);
            }
        }

        public void ResetGroup(string group = DEFAULT)
        {
            for (int i = mSTimeData.Count - 1; i >= 0; i--)
            {
                STimeData std = mSTimeData[i];
                if (std.Group == group || group == "")
                {
                    mSTimeData.Remove(std);
                    mMapSTimeData.Remove(std.key);
                    if (mCallEvent != null)
                    {
                        mCallEvent(std, STimeType.ST_Reset);
                    }
                }
            }
        }

        public List<STimeData>GetStimeDataList(string group = DEFAULT)
        {
            if (group == "")
                return mSTimeData;

            List<STimeData> stdList = new List<STimeData>();
            for (int i = 0; i < mSTimeData.Count; i++)
            {
                STimeData std = mSTimeData[i];
                if (std.Group  == group)
                {
                    stdList.Add(std);
                }
            }
            return stdList;
        } 

        public STimeData GetStimeData(string key)
        {
            if (mMapSTimeData.ContainsKey(key))
                return mMapSTimeData[key];
            return null;
        }


        //得到状态值
        public T GetStimeValue<T>(string key)
        {
            var t = GetStimeData(key);
            if (t != null)
            {
                if (t.ex != null)
                    return (T)t.ex;
            }
            return default(T);
        }

        public STimeData AddStateVlaue<T>(string key,T value,float time,string group = DEFAULT)
        {
            STimeData sd = AddState(key, time, group);
            sd.ex = value;
            return sd;
        }

        public STimeData AddState(string key,float time,string group = DEFAULT)
        {
            STimeData std = GetStimeData(key);
            if(std == null)
            {
                std = new STimeData();
                mSTimeData.Add(std);
            }
            else
            {
                if (mCallEvent != null)
                {
                    mCallEvent(std, STimeType.ST_Reset);
                }
            }
            std.key = key;
            std.time = time;
            std.Group = group;
            mMapSTimeData[std.key] = std;
            return std;
        }


        public override float PlayLogic(float timeDp)
        {
            for (int i = mSTimeData.Count-1; i >= 0; i--)
            {
                STimeData std = mSTimeData[i];
                std.allTime += timeDp;
                if((std.time -= timeDp) <= 0)
                {
                    if(mCallEvent != null)
                    {
                        mCallEvent(std,STimeType.ST_Normal);  
                    }
                    if (std.time < 0)
                    {
                        mSTimeData.Remove(std);
                        mMapSTimeData.Remove(std.key);
                    }
                }
            }
            return 0;
        }
    }
}
