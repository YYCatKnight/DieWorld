//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    //池对象
    public interface UnitPool
    {
        void PushPool();//删除入池
    }

    public interface UnitData
    {

    }


    public enum FMask
    {
        Mask0 = 0,
        Mask1 = 1<<1,
        Mask2 = 1 << 2,
        Mask3 = 1 << 3,
        Mask4 = 1 << 4,
        Mask5 = 1 << 5,
        Mask6 = 1 << 6,
        Mask7 = 1 << 7,
        Mask8 = 1 << 8,
        Mask9 = 1 << 9,
        Mask10 = 1 << 10,
        Mask11 = 1 << 11,
        Mask12 = 1 << 12,
        Mask13 = 1 << 13,
        Mask14 = 1 << 14,
        Mask15 = 1 << 15,
        Mask16 = 1 << 16,
        Mask17 = 1 << 17,
        Mask18 = 1 << 18,
    }

    public class CloneUnitData
    {
        public CloneUnitData GetCloneInistane()
        {
            return (CloneUnitData)this.MemberwiseClone();
        }
        object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Unit_Vector2
    {
        public float x;
        public float y;

        public void SetVector2(Vector2 v2)
        {
            x = v2.x;
            y = v2.y;
        }

        public Vector2 GetVector2()
        {
            return new Vector2(x, y);
        }
    }

    public class Unit_Vector3
    {
        public float x;
        public float y;
        public float z;

        public void SetVector3(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3 GetVector3()
        {
            return new Vector3(x,y,z);
        }
    }

    public class Unit_Rect
    {
        public Unit_Vector2 position;
        public Unit_Vector2 size;

       
        public void SetRect(Rect rt)
        {
            position = new Unit_Vector2();
            position.SetVector2(rt.position);
            size = new Unit_Vector2();
            size.SetVector2(rt.size);
        }

        public Rect GetRect()
        {
            return new Rect(position.GetVector2(), size.GetVector2());
        }
    }

    public class BaseUnit : UnitData
    {
        public Vector2 u_vector2;
        public Vector3 u_vector3;
        public int u_int;
        public bool u_bool;
        public float u_float;
        public UnitObject u_unitObject;
    }

    public class ZipThreadData
    {
        public string ZipFile;
        public string TargetDirectory;
        public string Password;
        public LoadPercent loadPercent;
        public bool OverWrite;
        public bool IsZipIng;
        public string error;
    }

    //消息模型
    public class MsgMesh
    {
        private EventStorage mEventStorage = new EventStorage();
        private TimerStorage mTimerStorage = new TimerStorage();
        private NetworkStorage mNetworkStorage = new NetworkStorage();

        #region 客服端事件
        public void RegEvent(string key, EventManager.AnyCallEvent callEvent)
        {
            mEventStorage.RegAnyEvent(key, callEvent);
        }

        public void RegAction<T>(string key,Action<T> call)
        {
            mEventStorage.RegAction(key, call);
        }

        public void RegGetEvent<T,S>(string key,Func<T,S> call)
        {
            mEventStorage.RegGetEvent<T,S>(key, call);
        }


        public void ClearAllEvent()
        {
            mEventStorage.Clear();
        }

        public void ClearEvent(string name)
        {
            mEventStorage.Remove(name);
        }
        #endregion

        public void RegProtocol<T>(Action<T> callBack) where T : FNetHead, new()
        {
            mNetworkStorage.RegNetEvent(callBack);
        }

        public void ClearAllProtocol()
        {
            mNetworkStorage.Clear();
        }

        public void ClearProtocol<T>() where T : FNetHead, new()
        {
            mNetworkStorage.Remove<T>();
        }

        #region 定时器

        public Timer_Logic RegTimer(Timer_Logic.TimerCall callBack, float waitTime = 1.0f,MonoBehaviour mo = null)
        {
            Timer_Logic logic = new Timer_Logic(callBack, waitTime,mo);
            mTimerStorage.RegTimer<Timer_Logic>(logic);
            return logic;
        }

        public Timer_Frequency RegFrequency(Timer_Frequency.TimerCall callBack, int fre = 3, MonoBehaviour mo = null)
        {
            Timer_Frequency frequency = new Timer_Frequency(callBack, fre,mo);
            mTimerStorage.RegTimer<Timer_Frequency>(frequency);
            return frequency;
        }

        public Timer_Coroutine RegCoroutine(IEnumerator tor, Timer_Coroutine.TimerCall FinishedEvent,MonoBehaviour mo = null)
        {
            Timer_Coroutine coroutine = new Timer_Coroutine(tor,FinishedEvent,mo);
            mTimerStorage.RegYield(coroutine);
            return coroutine;
        }

        public Timer_CYield RegCYield(IEnumerator tor, Timer_CYield.TimerCall FinishedEvent, MonoBehaviour mo = null)
        {
            Timer_CYield coroutine = new Timer_CYield(tor, FinishedEvent, mo);
            mTimerStorage.RegTimer(coroutine);
            return coroutine;
        }
        #endregion
        public void Clear()
        {
            mEventStorage.Clear();
            mNetworkStorage.Clear();
            mTimerStorage.Clear();
        }
    }


}
