//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    #region 通用结构
    public interface FGroupTool
    {
        void Init(string name,Component temp);
        void HandTool(int index,Component obj);
    }
    public class FGroup<T> : UnitData, UnitPool,IEnumerable<T> where T : Component
    {
        public T Last{ get { return mBuffs[mMaxNum - 1]; } }
        public int Count { get { return mMaxNum; } }
        public List<T> Values { get { return mBuffs.GetRange(0, Count);} }
        private class GroupConfig
        {
            public FGroupTool toolData;
            public T tempT;
            public int frameNum;
            public string resName;
            public Transform parent;
        }
        private Timer_Coroutine mTimer;
        private List<T> mBuffs = new List<T>();
        private Action<int, T> mCallEvent;
        private int mMaxNum = 0;
        private Dictionary<string, int> mAlias = new Dictionary<string, int>();
        private GroupConfig mGroupConfig = new GroupConfig();   
        public FGroup()
        {
            
        }
        private void _Init(string resName,T obj,Transform parent,System.Action<int,T> call,FGroupTool tool,int num)
        {
            mGroupConfig.tempT = obj;
            mGroupConfig.toolData = tool;
            mCallEvent = call;
            mGroupConfig.resName = resName;
            mGroupConfig.parent = parent;
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
            if (tool != null)
            {
                tool.Init(resName, obj);
            }
            if(num != 0)
            {
                Refurbish(num);
            }
        }

        public void Init(T obj,Action<int,T> call = null,int num = 0)
        {
            _Init("", obj, null, call, null,num);
        }

        public void Init(T obj,FGroupTool tool,Action<int,T> call = null,int num = 0)
        {
            _Init("", obj, null, call, tool, num);
        }

        public void Init(string resName,GameObject parent,Action<int,T> call = null,int num = 0)
        {
            _Init(resName,null, parent.transform, call, null, num);
        }

        public void Init(string resName,GameObject parent,FGroupTool tool, Action<int, T> call = null,int num =0)
        {
            _Init(resName, null, parent.transform, call,tool, num);
        }

        public void Init(GameObject parent, Action<int, T> call,bool isOnlyChild = true)
        {
            if(isOnlyChild)
            {
                int count = parent.transform.childCount;
                for(int i = 0; i < count;i++)
                {
                    var t = parent.transform.GetChild(i).GetComponent<T>();
                    if(t != null)
                    {
                        mBuffs.Add(t);
                    }
                }
            }
            else
            {
                T[] temps = parent.GetComponentsInChildren<T>(true);
                if(temps.Length > 0)
                {
                    if(temps[0].gameObject == parent)
                    {
                        for(int i = 1;i <temps.Length;i++)
                        {
                            mBuffs.Add(temps[i]);
                        }
                    }
                    else
                    {
                        mBuffs.AddRange(temps);
                    }
                }            
            }
            mMaxNum = mBuffs.Count;
            mCallEvent = call;
            if(mCallEvent != null)
            {
                for(int i = 0; i < mMaxNum;i++)
                {
                    mCallEvent(i, mBuffs[i]);
                }
            }
        }
        
        public void SetAsy(int num)
        {
            mGroupConfig.frameNum = num;
        }

        public void PushPool()
        {
                 
        }

        public void Clear(bool immediately = false)
        {
            Refurbish(0);
        }

        public void Refurbish(int count, System.Action<int, T> CallBack = null)
        {
            mMaxNum = count;
            if(CallBack != null)
            {
                mCallEvent = CallBack;
            }
            CloneObjectList(mBuffs,mMaxNum, mCallEvent,mGroupConfig);
        }

        public void Add(int count = 1, int startIndex = -1)
        {
            int lastNum = mMaxNum;
            mMaxNum += count;
            if (startIndex == -1)
            {
                startIndex = lastNum;
                for (int i = startIndex; i < mMaxNum; i++)
                {
                    _CloneOneObject(i, mBuffs, null,mGroupConfig);
                }
            }
            else
            {
                var tool = mGroupConfig.toolData;
                mGroupConfig.toolData = null;
                CloneObjectList(mBuffs, mMaxNum,null,mGroupConfig);
                mGroupConfig.toolData = tool;
                for (int i = 0; i < count; i++)
                {
                    var tempObject = mBuffs[mMaxNum - 1];
                    mBuffs.Remove(tempObject);
                    mBuffs.Insert(startIndex + i, tempObject);
                    tempObject.transform.SetSiblingIndex(startIndex + i - 1);
                }
                CloneObjectList(mBuffs,mMaxNum, null,mGroupConfig);
            }
            Replace(startIndex, count);
        }

        public void Remove(int count = 1, int startIndex = -1,bool immediately = false)
        {
            if (startIndex == -1)
            {
                if (immediately)
                {
                    for (int i = mMaxNum -1; i >= mMaxNum - count; i--)
                    {
                        var obj = mBuffs[i];
                        mBuffs.Remove(obj);
                        FEngineManager.Remove(obj.gameObject);
                    }
                }
                else
                {
                    for (int i = mMaxNum - count; i < mMaxNum; i++)
                    {
                        mBuffs[i].gameObject.SetActive(false);
                    }
                }
                mMaxNum -= count;
            }
            else
            {
                if (immediately)
                {
                    for (int i = startIndex + count - 1; i >= startIndex; i--)
                    {
                        var tempObject = mBuffs[i];
                        mBuffs.Remove(tempObject);
                        FEngineManager.Remove(tempObject.gameObject);
                    }
                }
                else
                {
                    for (int i = startIndex + count - 1; i >= startIndex; i--)
                    {
                        var tempObject = mBuffs[i];
                        mBuffs.Remove(tempObject);
                        mBuffs.Add(tempObject);
                        tempObject.transform.SetAsLastSibling();
                    }
                }
                mMaxNum -= count;
                CloneObjectList(mBuffs, mMaxNum,null, mGroupConfig);
            }
        }
     
        public T this[string key]
        {
            get
            {
                return GetValueByAlias(key);
            }
        }

        public T this[int index]
        {
            get { return GetValue(index); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return mBuffs[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public T GetValue(int id)
        {
            if (mMaxNum > id)
            {
                return mBuffs[id];
            }
            return null;
        }

        public void Replace(int index)
        {
            if (index < Count && mCallEvent != null)
            {
                mCallEvent(index, mBuffs[index]);
            }
        }
        
        public void Replace(int index,int num)
        {
            for(int i = 0; i < num;i++)
            {
                Replace(index + i);
            }
        }

        public void ReplaceAll()
        {
            
        }

        public void SetAlias(int index,string name)
        {
            mAlias[name] = index;
        }

        public int GetIndexByAlias(string name)
        {
            int index = 0;
            if (mAlias.TryGetValue(name, out index))
            {
                return index;
            }
            return -1;
        }

        public T GetValueByAlias(string name)
        {
            int index = GetIndexByAlias(name);
            if(index != -1)
            {
                return GetValue(index);
            }
            return null;
        }

        public void ReplaceByAlias(string name)
        {
            int index = GetIndexByAlias(name);
            if (index != -1)
            {
                Replace(index);
            }
        }

        public void SetAction(Action<int, T> call)
        {
            mCallEvent = call;
        }

        private void CloneObjectList(List<T> list,int count,System.Action<int, T> CallBack,GroupConfig config)
        {
            _CloneObjectList(list, count, CallBack, config);
        }

        private T _CloneOneObject(int i,List<T> list,Action<int,T> CallBack,GroupConfig config)
        {
            T t = null;
            if (i < list.Count)
            {
                t = list[i];
            }
            else
            {
                if (config.tempT == null)
                {
                    t = (T)((Component)FEngineManager.PoolObject(config.resName, config.parent.gameObject));
                }
                else
                {
                    t = FEngineManager.CloneObject<T>(config.tempT.gameObject, null);
                }
                list.Add(t);
            }

            t.gameObject.SetActive(true);

            if (config.toolData != null)
            {
                config.toolData.HandTool(i,t);
            }

            if (CallBack != null)
            {
                CallBack(i, t);
            }
            return t;
        }

        private  void _CloneObjectList(List<T> list,int count,System.Action<int, T> CallBack,GroupConfig config)
        {
            for (int i = 0; i < count; i++)
            {
                _CloneOneObject(i, list,CallBack, config);
            }

            for (int i = count; i < list.Count; i++)
            {
                list[i].gameObject.SetActive(false);
            }
        }

        #region 旧接口
        [Obsolete("这个不久将被移除,请用Init替换")]
        public FGroup(Vector2 nextPos, T obj, int count = 0, int xNum = -1, Action<int, T> callBack = null, int dpNum = 0)
        {
            GLayPosition pos = null;
            if (nextPos.x != 0 || nextPos.y != 0)
            {
                pos = GLayPosition.Create(nextPos.x, nextPos.y, xNum);
            }
            Init(obj, pos, callBack);
            if (count != 0)
            {
                Refurbish(count);
            }
        }

        #endregion
    }
    #endregion

    #region 扩展接口
    public class GLayPosition : FGroupTool
    {
        private Vector3 mStartPos;
        private float mX;
        private float mY;
        private int mNum = -1;
        public static GLayPosition Create(float x,float y = 0,int xNum = -1)
        {
            GLayPosition lp = new GLayPosition();
            lp.SetX(x).SetY(y).SetWLine(xNum);
            return lp;
        }
        public GLayPosition SetX(float x)
        {
            mX = x;
            return this;
        }
        public GLayPosition SetY(float y)
        {
            mY = y;
            return this;
        }
        public GLayPosition SetWLine(int num)
        {
            mNum = num;
            return this;
        }
        public GLayPosition SetStart(Vector3 pos)
        {
            mStartPos = pos;
            return this;
        }

        public void Init(string name,Component obj)
        {
            if (obj != null)
            {
                mStartPos = obj.transform.localPosition;
            }
            else
            {
                mStartPos = Vector3.zero;
            }
        }
        public void HandTool(int index, Component obj)
        {
            if (mNum <= 0)
            {
                Vector3 pos = mStartPos;
                pos.x += index * mX;
                obj.transform.localPosition = pos;
            }
            else
            {
                int x = index % mNum;
                int y = index / mNum;
                Vector3 pos = mStartPos;
                pos.x += x * mX;
                pos.y += y * mY;
                obj.transform.localPosition = pos;
            }
        }
    }
    #endregion
}
