//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace F2DEngine
{
    #region 协议管理器
    public class NetworkManager : ManagerTemplate<NetworkManager>
    {
        private const string NETWORK_MSG = "Network_Msg_";
        private NetInterface mNetMsg = new NetInterface();
        protected override void OnInit()
        {
            mNetMsg.Init(NETWORK_MSG, FEngineManager.GetNetMsgCore());
        }

        public void ConnectNet(string ip,int port,Action<NetMsgResult> callBack)
        {
            mNetMsg.ConnectNet(ip, port, callBack);
        }


        public void SendNetEvent(int id,FNetHead data)
        {
            mNetMsg.SendNetEvent(id, data);
        }

        public void CloseSocket()
        {
            mNetMsg.CloseSocket();
        }

        internal EventManager.ToolEvent RegNetEvent<T>(Action<T> call) where T:FNetHead,new ()
        {
            return mNetMsg.RegNetEvent<T>(call);
        }

        internal void RemvoeNetEvent(EventManager.ToolEvent tool)
        {
            mNetMsg.RemvoeNetEvent(tool);
        }

        public T GetMsgCore<T>()
        {
            return mNetMsg.GetMsgCore<T>();
        }

        protected override void OnDispose()
        {
            CloseSocket();
        }

    }
    public class NetInterface
    {
        private string NETWORK_MSG = "Network_Msg_";
        private Timer_Logic mTimerUpdate;
        private FNetMsgCore mMsgCore;
        private Action<NetMsgResult> mCallBack;
        public  void Init(string msgcode,FNetMsgCore core)
        {
            NETWORK_MSG = msgcode;
            mMsgCore = core;
            mMsgCore.Init();
        }

        public void ConnectNet(string ip, int port, Action<NetMsgResult> callBack)
        {
            mCallBack = callBack;
            Action<NetMsgResult> newCall = (f) =>
            {
                _ConnectResult(f);
                if (mCallBack != null)
                {
                    mCallBack(f);
                    //保证错误只发送一次事件
                    if (f.result != NetMsgResult.MsgResult.Msg_Suc)
                    {
                        mCallBack = null;
                    }
                }
            };
            if (mMsgCore.Connect(ip, port, newCall, HandlePackStream))
            {
                BeginTimer();
            }
        }

        public void BeginTimer()
        {
            if (mTimerUpdate == null)
            {
                mTimerUpdate = Timer_Logic.SetTimer(TimerUpdate,0,null);
            }
        }

        private float TimerUpdate(Timer_Logic le)
        {
            mMsgCore.FixedUpdate();
            return 0;
        }

        private void HandlePackStream(FPackStream ps)
        {
            if (ps != null)
            {
                EventManager.instance.SendEvent<FPackStream>(NETWORK_MSG + ps.GetPackName(), ps);
            }
        }

        public void SendNetEvent(int id, FNetHead data)
        {
            if (mMsgCore != null)
            {
                mMsgCore.Send(id, data);
            }
        }

        private void _ConnectResult(NetMsgResult result)
        {
            if (result.result != NetMsgResult.MsgResult.Msg_Suc)
            {
                Debug.Log("NetworkManager:服务连接失败:" + result.result);
                CloseSocket();
            }
        }

        public void CloseSocket()
        {
            if (mTimerUpdate != null)
            {
                mTimerUpdate.StopTimer();
                mTimerUpdate = null;
            }
            if (mMsgCore != null)
            {
                mMsgCore.Close();
            }
        }


        public EventManager.ToolEvent RegNetEvent<T>(Action<T> call) where T : FNetHead, new()
        {
            string netmsg = NETWORK_MSG + typeof(T).FullName;
            Action<FPackStream> NetCall = (f) =>
            {
                T obj = new T();
                mMsgCore.Deserialize(f, obj);
                call(obj);
            };
            return EventManager.instance.AddEvent<FPackStream>(netmsg, NetCall);
        }

        public void RemvoeNetEvent(EventManager.ToolEvent tool)
        {
            EventManager.instance.RemoveEvent(tool);
        }

        public T GetMsgCore<T>()
        {
            return (T)mMsgCore;
        }

    }
    #endregion
    #region 协议存储器
    public class NetworkStorage
    {
        private Dictionary<Type, EventManager.ToolEvent> mNetEvents = new Dictionary<Type, EventManager.ToolEvent>();
        public void RegNetEvent<T>(Action<T> callBack) where T : FNetHead, new()
        {
            EventManager.ToolEvent pr = NetworkManager.instance.RegNetEvent<T>(callBack);
            if (pr != null)
            {
                mNetEvents.Add(typeof(T), pr);
            }
        }
        public void Remove<T>() where T : FNetHead, new()
        {
            Type type = typeof(T);
            if (mNetEvents.ContainsKey(type))
            {
                NetworkManager.instance.RemvoeNetEvent(mNetEvents[type]);
                mNetEvents.Remove(type);
            }
        }
        public void Clear()
        {
            foreach (var k in mNetEvents)
            {
                NetworkManager.instance.RemvoeNetEvent(k.Value);
            }
            mNetEvents.Clear();
        }
    }
    #endregion
    #region 协议扩展结构

    //网络连接结果
    public class NetMsgResult
    {
        public enum MsgResult
        {
            Msg_Suc,//连接成功
            Msg_Fail,//连接失败
            Msg_Error,//网络发生错误
            Msg_Close,//网络关闭
            Msg_Unknown,//未知
        }
        public MsgResult result;
        public int type;
        public string error;
        public object data;       
    }
    //协议头
    public interface FNetHead
    {

    }
    //包流
    public interface FPackStream
    {
        string GetPackName();
    }
    //消息core
    public interface FNetMsgCore
    {
        void Init();
        bool Connect(string ip, int port, Action<NetMsgResult> resultCall, Action<FPackStream> handleCall);
        void FixedUpdate();
        void Send(int id, FNetHead data);
        void Close();
        void Deserialize(FPackStream stream, object obj);
    }

    #endregion
}
