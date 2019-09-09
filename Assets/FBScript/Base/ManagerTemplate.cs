//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public interface ManagerDispose
    {
        // 序号越大，最迟释放
        int GetDisposeOreder();
        string GetName();
        void SetDisposeOrder(int order);
        void Dispose(bool isReset =  true);
        void Init();
    }

    public class ManagerTemplate<T> : ManagerDispose where T : ManagerTemplate<T>, new()
    {
        private static T _instance;
        private int order;

        private static bool isDispose = false;
        protected bool mIsInit = false;
        protected Dictionary<Type, IMoudle> m_Moudles = new Dictionary<Type, IMoudle>();
        protected MsgMesh mMsgMesh = new MsgMesh();

        public static T instance
        {
            get
            {
                if (_instance == null || isDispose)
                {
                    isDispose = false;
                    _instance = null;
                    _instance = new T();
                    Manager.AddInstance(_instance);
                }
                return _instance;
            }
        }

        public static void SetNewInstance(T ins)
        {
            _instance.Dispose(false);
            Manager.AddInstance(ins);
            _instance = ins;
        }

        public void Init()
        {
            if (!mIsInit)
            {
                mIsInit = true;
                OnInit();
            }
        }

        public void Dispose(bool isReset = true)
        {
            mMsgMesh.Clear();
            DisposeMoudle();
            OnDispose();
            mIsInit = false;
            if (isReset)
            {
                isDispose = true;
            }
        }

        protected void DisposeMoudle()
        {
            foreach (var k in m_Moudles)
            {
                k.Value.Dispose();
            }
            m_Moudles.Clear();
        }

        protected virtual void OnDispose()
        {
            
        }

        protected virtual void OnInit() { }

        int ManagerDispose.GetDisposeOreder()
        {
            return this.order;
        }

        public void SetDisposeOrder(int order)
        {
            this.order = order;
        }

        string ManagerDispose.GetName()
        {
            return typeof(T).Name;
        }

        protected void RegisterMoudle(IMoudle moudle)
        {
            Type type = moudle.GetType();
            if (m_Moudles.ContainsKey(type))
            {
                Debug.LogError(string.Format("{0}已经注册过,无法重复注册", type.FullName));
            }
            m_Moudles[type] = moudle;
        }
        protected void RegisterMoudle<M>(params object[] o) where M : IMoudle, new()
        {
            M m = new M();
            m_Moudles[typeof(M)] = m;
            m.Init(mMsgMesh, o);
        }
        public static M GetMoudle<M>() where M : IMoudle, new()
        {
            return instance._GetMoudle<M>();
        }
        protected M _GetMoudle<M>() where M : IMoudle, new()
        {
            Type type = typeof(M);
            IMoudle moudle = null;
            if (!m_Moudles.TryGetValue(type, out moudle))
            {
                Debug.LogError(string.Format("模块{0}没有找到", type.FullName));
                //容错处理,自动注册
                RegisterMoudle<M>();
            }
            return (M)moudle;
        }

    }

    public class Manager
    {
        private static Dictionary<string, ManagerDispose> dict = new Dictionary<string, ManagerDispose>();

        static public void AddInstance(ManagerDispose obj)
        {
            int count = dict.Count;
            obj.SetDisposeOrder(count);
            if (dict.ContainsKey(obj.GetName()))
            {
                return;
            }
            dict.Add(obj.GetName(), obj);
        }

        static public void Dispose()
        {
            Dispose(new List<ManagerDispose>());
        }

        static public void Dispose(List<ManagerDispose> filterManager)
        {
            List<ManagerDispose> result = new List<ManagerDispose>();
            List<ManagerDispose> list = new List<ManagerDispose>(dict.Values);
            list.Sort((e, f) =>
            {
                int order1 = e.GetDisposeOreder();
                int order2 = f.GetDisposeOreder();
                if (order1 < order2)
                    return -1;
                if (order1 == order2)
                    return 0;
                return 1;
            });

            list.ForEach(e =>
            {
                if (filterManager.Find(f => f.GetName() == e.GetName()) != null)
                    result.Add(e);
                else
                    e.Dispose();
            });
            dict.Clear();
            result.ForEach(e =>
            {
                dict.Add(e.GetName(), e);
            });
        }

    }


    public interface IMoudle
    {
        void Init(MsgMesh mesh, params object[] o);
        void Dispose();
    }

    public abstract class IMoudle<T> : IMoudle where T : IMoudle<T>, new()
    {
        public static bool IsValid = false;
        public static T instance { get { return _instance; } }
        private static T _instance;
        protected MsgMesh mMsgMesh;
        public void Init(MsgMesh mesh, params object[] o)
        {
            if (IsValid)
            {
                Debug.LogError(string.Format("{0}模块无法重复注册", typeof(T).FullName));
            }
            else
            {
                _instance = (T)this;
                IsValid = true;
                mMsgMesh = mesh;
                OnInit(o);
            }
        }
        public void Dispose()
        {
            IsValid = false;
            mMsgMesh = null;
            OnDispose();
        }
        protected virtual void OnInit(params object[] o)
        {

        }
        public virtual void OnDispose()
        {

        }
    }
}