//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace F2DEngine
{
    public class EventManager : ManagerTemplate<EventManager>
    {    
        public delegate void AnyCallEvent(params object[] args);
        public class EventGather
        {
            public AnyCallEvent mAnyEvents;
            public ToolEvent mTActions;
            public ToolGetEvent mGetEvent;
        }
        public class HandleEvent
        {
            private EventGather mGather = new EventGather();

            public void AddAnyEvent(AnyCallEvent eve)
            {
                mGather.mAnyEvents += eve;
            }

            public void RemoveAnyEvent(AnyCallEvent eve)
            {
                mGather.mAnyEvents -= eve;
            }

            public void AddActionEvent(ToolEvent eve)
            {
                if(mGather.mTActions == null)
                {
                    mGather.mTActions = eve;
                }
                else
                {
                    mGather.mTActions.Add(eve);
                }
            }

            public void RemoveActionEvent(ToolEvent eve)
            {
                if (mGather.mTActions != null)
                {
                    mGather.mTActions.Remove(eve);
                }
            }

            public void AnyCallBack(params object[]objs)
            {
                if(mGather.mAnyEvents != null)
                {
                    mGather.mAnyEvents(objs);
                }
                if(mGather.mTActions != null)
                {
                    mGather.mTActions.CallBack(objs);
                }
            }


            public bool AddGetEvent(string key,ToolGetEvent eve)
            {
                if(mGather.mGetEvent == null)
                {
                    mGather.mGetEvent = eve;
                }
                else
                {
                    Debug.LogError(key + "唯一ToolGetEvent已经被注册了,无法重新注册");
                    return false;
                }
                return true;
            }

            public void RemoveGetEvent()
            {
                if (mGather.mGetEvent != null)
                {
                    mGather.mGetEvent = null;
                }
            }


            public S GetTEvent<T,S>(T t)
            {
                if(mGather.mGetEvent != null)
                {
                    return ((TGetEvent<T, S>)mGather.mGetEvent).GetEvent(t);
                }
                return default(S);
            }

            public void TCallBack<T>(T t1)
            {
                if (mGather.mAnyEvents != null)
                {
                    mGather.mAnyEvents(t1);
                }
                if (mGather.mTActions != null)
                {
                    ((TAction<T>)mGather.mTActions).TCallBack(t1);
                }
            }

        }
        public interface ToolGetEvent
        {

        }
        public class TGetEvent<T,S>:ToolGetEvent
        {
            private Func<T, S> mEvent;
            public TGetEvent(Func<T,S> call)
            {
                mEvent = call;
            }

            public S GetEvent(T t)
            {
                return mEvent(t);
            }

        }
        public interface ToolEvent
        {
             string GetName();
             void Add(ToolEvent eve);
             void Remove(ToolEvent eve);
             void CallBack(object[] objs);
        }
        public class TAction<T>: ToolEvent
        {
            private Action<T> mEvent;
            private string mName;
            public TAction(string key,Action<T> call)
            {
                mName = key;
                mEvent = call;
            }
            public string GetName()
            {
                return mName;
            }
            public void Add(ToolEvent eve)
            {
                mEvent += (((TAction<T>)eve).mEvent);
            }

            public void TCallBack(T t)
            {
                if (mEvent != null)
                {
                    mEvent(t);
                }
            }

            public void CallBack(object[] objs)
            {
                if (mEvent != null)
                {
                    mEvent((T)objs[0]);
                }
            }

            public void Remove(ToolEvent eve)
            {
                mEvent -= (((TAction<T>)eve).mEvent);
            }
        }
        public interface ToolEventParams
        {
            void CallBack(HandleEvent call);
            string GetName();
        }
        public class AnyTool:ToolEventParams
        {
            private object[] mArgs;
            private string mName;
            public AnyTool(string name,object[] objs)
            {
                mName = name;
                mArgs = objs;
            }

            public string GetName()
            {
                return mName;
            }

            public void CallBack(HandleEvent call)
            {
                call.AnyCallBack(mArgs);
            }


        }
        public class TTool<T> : ToolEventParams
        {
            private T mT;
            private string mName;
            public TTool(string name,T t)
            {
                mName = name;
                mT = t;
            }
            public string GetName()
            {
                return mName;
            }

            public void CallBack(HandleEvent call)
            {
                call.TCallBack<T>(mT);
            }
        }
        protected Dictionary<string, HandleEvent> mImEvents = new Dictionary<string, HandleEvent>();
        protected List<ToolEventParams> mDelayAcions = new List<ToolEventParams>();
        private void _AddAnyEvent(string key, AnyCallEvent eve)
        {
            HandleEvent he = null;
            if (!mImEvents.TryGetValue(key, out he))
            {
                he = new HandleEvent();
                mImEvents.Add(key, he);
            }
            he.AddAnyEvent(eve);
        }
        private void _AddTEvent(ToolEvent eve)
        {
            string key = eve.GetName();
            HandleEvent he = null;
            if (!mImEvents.TryGetValue(key, out he))
            {
                he = new HandleEvent();
                mImEvents.Add(key, he);
            }
            he.AddActionEvent(eve);
        }
        private bool _AddGetEvent(string key, ToolGetEvent eve)
        {
            HandleEvent he = null;
            if (!mImEvents.TryGetValue(key, out he))
            {
                he = new HandleEvent();
                mImEvents.Add(key, he);
            }
            return he.AddGetEvent(key,eve);
        }
        private HandleEvent _GetHandle(string key)
        {
            HandleEvent de = null;
            mImEvents.TryGetValue(key, out de);
            return de;
        }
        protected override void OnInit()
        {
            Timer_Logic.SetTimer(DelayUpdate, 0,null);
        }
        private  float DelayUpdate(Timer_Logic le)
        {
            lock (mDelayAcions)
            {
              
                if (mDelayAcions.Count != 0)
                {
                    for (int i = 0; i < mDelayAcions.Count; i++)
                    {
                        ToolEventParams mult = mDelayAcions[i];
                        HandleEvent eve = _GetHandle(mult.GetName());
                        if(eve != null)
                        {
                            mult.CallBack(eve);
                        }
                    }
                    mDelayAcions.Clear();
                }
               
            }
            return 0;
        }
        internal void RemoveEvent(string key,AnyCallEvent call)
        {
            HandleEvent he = null;
            if (mImEvents.TryGetValue(key, out he))
            {
                he.RemoveAnyEvent(call);
            }
        }
        internal void RemoveEvent(ToolEvent call)
        {
            if (call != null)
            {
                HandleEvent he = null;
                string key = call.GetName();
                if (mImEvents.TryGetValue(key, out he))
                {
                    he.RemoveActionEvent(call);
                }
            }
        }
        internal void RemoveGetEvent(string key)
        {
            HandleEvent he = null;
            if (mImEvents.TryGetValue(key, out he))
            {
                he.RemoveGetEvent();
            }
        }
        internal void RemoveEvent(string key, EventGather gather)
        {
            if(gather.mAnyEvents != null)
            {
                RemoveEvent(key, gather.mAnyEvents);
            }
            if(gather.mTActions != null)
            {
                RemoveEvent(gather.mTActions);
            }
            if(gather.mGetEvent != null)
            {
                RemoveGetEvent(key);
                gather.mGetEvent = null;
            }
        }
        internal void Add(string key, AnyCallEvent call)
        {
            _AddAnyEvent(key, call);
        }
        public void Send(string key,params object[] objs)
        {
            HandleEvent he = _GetHandle(key);
            if (he != null)
            {
                he.AnyCallBack(objs);
            }
        }
        public void SendDelay(string key, params object[] objs)
        {
            lock (mDelayAcions)
            {
                mDelayAcions.Add(new AnyTool(key, objs));
            }
        }
        internal ToolEvent AddEvent<T>(string key,Action<T> call)
        {
            TAction<T> eve = new TAction<T>(key,call);
            _AddTEvent(eve);
            return eve;
        }
        public void SendEvent<T>(string key,T t)
        {
            HandleEvent he = _GetHandle(key);
            if (he != null)
            {
                he.TCallBack<T>(t);
            }
        }

        internal ToolGetEvent AddGetEvent<T,S>(string key,Func<T,S>call)
        {
            TGetEvent<T,S> eve = new TGetEvent<T, S>(call);
            if (_AddGetEvent(key, eve))
            {
                return eve;
            }
            return null;
        }

    }
    public class EventStorage
    {
        private Dictionary<string, EventManager.EventGather> mEvents = new Dictionary<string, EventManager.EventGather>();

        public void  RegGetEvent<T,S>(string key,Func<T,S> callBack)
        {
            EventManager.EventGather gather = new EventManager.EventGather();
            mEvents.Add(key, gather);
            gather.mGetEvent = EventManager.instance.AddGetEvent<T,S>(key,callBack);
        }

        public void RegAction<T>(string key, Action<T> callEvent)
        {
            EventManager.EventGather gather = new EventManager.EventGather();
            mEvents.Add(key, gather);
            gather.mTActions = EventManager.instance.AddEvent<T>(key, callEvent);
        }

        public void RegAnyEvent(string key, EventManager.AnyCallEvent callEvent)
        {
            EventManager.EventGather gather = new EventManager.EventGather();
            gather.mAnyEvents = callEvent;
            mEvents.Add(key, gather);
            EventManager.instance.Add(key, callEvent);
        }

        public void Remove(string key)
        {
            if (mEvents.ContainsKey(key))
            {
                EventManager.instance.RemoveEvent(key, mEvents[key]);
                mEvents.Remove(key);
            }
        }

        public void Clear()
        {
            foreach (var k in mEvents)
            {
                EventManager.instance.RemoveEvent(k.Key, k.Value);
            }
            mEvents.Clear();
        }
    }
}
