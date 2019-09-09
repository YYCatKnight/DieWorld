//----------------------------------------------
//  F2DEngine: time: 2018.9  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    #region 新的定时器
    public class TimerController : FBaseController<TimerController>
    {
        private  static TimerMachine mWorldTimerMachine = new TimerMachine();       
        internal static Timer_Coroutine SetYieldTimer(Timer_Coroutine timer)
        {
            instance.StartCoroutine(instance.TimerCoroutine(timer));
            return timer;
        }
        private IEnumerator TimerCoroutine(Timer_Coroutine timer)
        {
            timer.Start();
            IEnumerator tor = timer.GetIEnumerator();
            while (timer.state != TEvent.EventState.ES_Stop)
            {
                if (timer.IsValid())
                {
                    if (timer.state == TEvent.EventState.ES_Pause)
                    {
                        yield return 0;                     
                    }
                    else
                    {
                        if (tor != null && tor.MoveNext())
                        {
                            yield return tor.Current;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    timer.LogShow();
                    break;
                }
            }
            timer.End();
        }
        internal class TimerMachine
        {
            private static int DEGREE = 10;//精度DEGREE毫秒
            public TimerMachine()
            {
                mTimerRunTime = ScriptsTime.GetTimes();
                mStartTime = mTimerRunTime;
            }
            //private FPriorityList<TimerEvent> mStaticTimerEvents = new FPriorityList<TimerEvent>(new TimerWeightComparer());//长时间定时器
            private List<TimerEvent> mDynamicTimerEvents = new List<TimerEvent>();//短时间循环定时器<5s
            private List<TimerEvent> mPauseEvents = new List<TimerEvent>();//暂停定时器
            private List<TimerEvent> mTurnEvents = new List<TimerEvent>();//待转换定时器
            private int mLastTick = 0;
            private float mStartTime;
            private float mTimerRunTime = 0.0f;//单位毫秒
            private float mDeltaTime;
            private float mTempDeltaTime;
            private int _PauseSpy = 0;//暂停监控
            public T SetTimer<T>(T timer) where T :TimerEvent
            {
                timer.mMachine = this;
                PustTimer(timer);
                return timer;
            }
            private enum TimerState
            {
                T_None,//未执行
                T_Carry,//执行
                T_Turn,//变换
            }
            private TimerState _PlayTimer(TimerEvent timer)
            {
                if (timer.IsValid())
                {
                    if (timer.state == TEvent.EventState.ES_Normal)
                    {
                        if (timer.nextTime <= mTimerRunTime)
                        {
                            float t = timer.PlayCallBack(mDeltaTime);
                            if (t < 0)
                            {
                                //定时器移除
                                timer.StopTimer();
                                return TimerState.T_Turn;
                            }
                            else
                            {
                                //定时器继续
                                timer.SetNextTime(t);
                                return TimerState.T_Carry;
                            }
                        }
                        return TimerState.T_None;
                    }
                }
                else
                {
                    timer.StopTimer();
                }
                return TimerState.T_Turn;
            }         
            public void PushTurnEvent(TimerEvent.TimerTurnType type,TimerEvent timer)
            {
                var turnType = timer.mTurnType;
                //内置调用
                switch (type)
                {
                    case TimerEvent.TimerTurnType.TT_DynamicTurn:
                        if (turnType == TimerEvent.TimerTurnType.TT_None)
                        {
                            if (timer.IsStatic)
                            {
                                mTurnEvents.Add(timer);
                            }
                            else
                            {
                                mTurnEvents.Insert(0, timer);
                            }
                            timer.mTurnType = type;
                        }
                        else
                        {
                            Debug.LogError(string.Format("定时器失败无法:{0}->{1}", turnType, TimerEvent.TimerTurnType.TT_DynamicTurn.ToString()));
                        }
                        break;
                    case TimerEvent.TimerTurnType.TT_Pause:
                        if (turnType == TimerEvent.TimerTurnType.TT_None || turnType == TimerEvent.TimerTurnType.TT_UnPause)
                        {
                            _PushTurn(type, timer);
                        }
                        break;
                    case TimerEvent.TimerTurnType.TT_UnPause:
                        if (turnType == TimerEvent.TimerTurnType.TT_Pause)
                        {
                            _PushTurn(type, timer);
                        }
                        break;
                    case TimerEvent.TimerTurnType.TT_Reset:
                        if (turnType == TimerEvent.TimerTurnType.TT_DynamicTurn)
                        {
                            if (mTurnEvents.Contains(timer))
                            {
                                mTurnEvents.Remove(timer);
                            }
                        }
                        _PushTurn(type, timer);
                        break;
                    case TimerEvent.TimerTurnType.TT_Delete:
                        if (turnType == TimerEvent.TimerTurnType.TT_DynamicTurn)
                        {
                            if (mTurnEvents.Contains(timer))
                            {
                                mTurnEvents.Remove(timer);
                            }
                        }
                        _PushTurn(type, timer);
                        break;
                    default:
                        break;
                }
            }
            private void _PushTurn(TimerEvent.TimerTurnType type, TimerEvent timer)
            {
                if (!mTurnEvents.Contains(timer))
                {
                    mTurnEvents.Insert(0, timer);
                }
                timer.mTurnType = type;
            }
            private void ForceFree(TimerEvent timer,bool tagDelete = true)
            {             
                if(timer.mRunQueue != TimerEvent.TimerRunQueue.Q_Free)
                {
                    if (timer.mRunQueue == TimerEvent.TimerRunQueue.Q_Static)
                    {

                    }
                    else if (timer.mRunQueue == TimerEvent.TimerRunQueue.Q_Dynamic)
                    {
                        mDynamicTimerEvents.Remove(timer);
                    }
                    else if (timer.mRunQueue == TimerEvent.TimerRunQueue.Q_Pause)
                    {
                        mPauseEvents.Remove(timer);
                    }
                    timer.mRunQueue = TimerEvent.TimerRunQueue.Q_Free;
                }
                if (tagDelete)
                {
                    timer.mTurnType = TimerEvent.TimerTurnType.TT_Delete;
                }
            }                     
            private void PustTimer(TimerEvent timer)
            {
                ForceFree(timer,false);
                if (timer.state == TEvent.EventState.ES_Pause)
                {
                    if (!mPauseEvents.Contains(timer))
                    {
                        mPauseEvents.Add(timer);
                    }
                    timer.mRunQueue = TimerEvent.TimerRunQueue.Q_Pause;
                    timer.mTurnType = TimerEvent.TimerTurnType.TT_Pause;
                }
                else
                {
                    if (timer.IsStatic)
                    {
                        timer._Tag = (int)((timer.nextTime - mStartTime) / DEGREE); 
                        timer.mRunQueue = TimerEvent.TimerRunQueue.Q_Static;
                    }
                    else
                    {
                        mDynamicTimerEvents.Add(timer);
                        timer.mRunQueue = TimerEvent.TimerRunQueue.Q_Dynamic;
                    }
                    timer.mTurnType = TimerEvent.TimerTurnType.TT_None;
                }              
            }
            public void Update()
            {
                mTempDeltaTime = ScriptsTime.GetTimes();
                mDeltaTime = (mTempDeltaTime - mTimerRunTime)/1000.0f;
                mTimerRunTime = mTempDeltaTime;
                //定时器转换
                if (mTurnEvents.Count != 0)
                {
                    for(int i = 0; i < mTurnEvents.Count;i++)
                    {
                        var timer = mTurnEvents[i];
                        if(timer.mTurnType == TimerEvent.TimerTurnType.TT_DynamicTurn)
                        {                           
                            PustTimer(timer);
                        }
                        else
                        {
                            switch (timer.mTurnType)
                            {
                                case TimerEvent.TimerTurnType.TT_Pause:
                                    PustTimer(timer);
                                    break;
                                case TimerEvent.TimerTurnType.TT_UnPause:
                                    PustTimer(timer);
                                    break;
                                case TimerEvent.TimerTurnType.TT_Reset:
                                    timer.ResetData();
                                    PustTimer(timer);
                                    break;
                                case TimerEvent.TimerTurnType.TT_Delete:
                                    ForceFree(timer,true);
                                    break;
                            }
                        }
                    }
                    mTurnEvents.Clear();
                }

                //动态定时器
                if (mDynamicTimerEvents.Count != 0)
                {
                    for (int i = mDynamicTimerEvents.Count - 1; i >= 0; i--)
                    {
                        var timer = mDynamicTimerEvents[i];
                        TimerState result = _PlayTimer(timer);
                        if(result == TimerState.T_Carry)
                        {
                            if (timer.IsStatic)
                            {
                                PushTurnEvent(TimerEvent.TimerTurnType.TT_DynamicTurn, timer);
                            }
                        }
                        else if(result == TimerState.T_Turn)
                        {
                            ForceFree(timer,false);
                        }
                    }
                }

                int tick = (int)((mTempDeltaTime - mStartTime) / DEGREE);//DEGREE毫秒的精度
                mLastTick = tick;
            }
            private void HandleWhell(List<TimerEvent> timers)
            {
                for(int i = 0; i < timers.Count;i++)
                {
                    var timer = timers[i];
                    TimerState result = _PlayTimer(timer);
                    if (result == TimerState.T_Carry)
                    {
                        PushTurnEvent(TimerEvent.TimerTurnType.TT_DynamicTurn, timer);
                    }
                    //ForceFree(timer,false);
                }
            }
        }
        private class TimerWeightComparer : IComparer<TimerEvent>
        {
            public int Compare(TimerEvent t1, TimerEvent t2)
            {
                if (t1.nextTime < t2.nextTime)
                    return 1;
                return -1;
            }
        }
        internal static T SetTimer<T>(T timer) where T : TimerEvent
        {
            return mWorldTimerMachine.SetTimer(timer);
        }
        internal static void StopTimer(TimerEvent timer)
        {
            mWorldTimerMachine.PushTurnEvent(TimerEvent.TimerTurnType.TT_Delete,timer);
        }
        public override void End()
        {
            mWorldTimerMachine = new TimerMachine();
        }
        //执行函数
        void Update()
        {
            mWorldTimerMachine.Update();
        }
        public static void EditorUpdate()
        {
            if (instance == null)
            {
                mWorldTimerMachine.Update();
            }
        }
        public static void EditorClear()
        {
            mWorldTimerMachine = new TimerMachine();
        }
    }
    public abstract class TEvent:UnitData
    {
        public enum EventState
        {
            ES_Normal,
            ES_Stop,
            ES_Pause,//暂停
            ES_Reset,
        }
        public float totalTime { get { return (ScriptsTime.GetTimes()- _startTime)/1000.0f;}}//总时间
        public long handle { get; protected set; }//句柄
        public EventState state { get; protected set; }//状态
        public Component stBody { get; protected set; }//实体物体
        protected static long MaxHandle = 0;
        protected string _BodyName = "";
        protected float _startTime = 0;
        private bool _IsEditor = false;
        public TEvent()
        {
            handle = ++MaxHandle;
            stBody = FEngineManager.Engine;
            _startTime = ScriptsTime.GetTimes();
            state = EventState.ES_Normal;
        }
        public virtual void StopTimer()
        {
            state = EventState.ES_Stop;
        }
        public virtual void PauseTimer(bool ispause = true)
        {
            if(ispause)
            {
                if (state == EventState.ES_Normal)
                {
                    state = EventState.ES_Pause;
                }
            }
            else if(state == EventState.ES_Pause)
            {
                state = EventState.ES_Normal;
            }
        }
        public bool IsValid()
        {
            if(state == EventState.ES_Stop || (stBody == null && !_IsEditor))
            {
                return false;
            }
            return true;
        }
        public virtual void SetBody(Component com,string keyName = null)
        {
            stBody = com == null ? FEngineManager.Engine : com;
            _IsEditor = stBody == null;
            if (_IsEditor)
            {
                _BodyName = "Editor_";
            }
            else
            {
                _BodyName = keyName == null ? stBody.name : keyName;
            }
        }
        public virtual bool ResetTimer()
        {
            if(stBody == null)
            {
                Debug.LogError(_BodyName + ":定时器实体已删除,无法重置");
                return false;
            }
            state = EventState.ES_Reset;
            return true;
        }
        public virtual void  LogShow()
        {
            if (stBody == null)
            {
                Debug.Log("<color=#660000>" + _BodyName + ":实体已删除,但是定时器没有清空....</color>");
            }
        }
        public virtual void ResetData()
        {
            _startTime = ScriptsTime.GetTimes();
            state = EventState.ES_Normal;
        }
    }
    public abstract class TimerEvent: TEvent
    {
        internal TimerController.TimerMachine mMachine;
        internal enum TimerTurnType
        {
            TT_None,
            TT_DynamicTurn,//动态,静态转换
            TT_Pause,
            TT_UnPause,
            TT_Reset,
            TT_Delete,//移除
        }
        internal int _Tag;//时间轮标记
        public int frequency { get; protected set; }//总运行次数       
        public float nextTime { get; protected set; }//真实执行时间;
        public bool IsStatic { get; protected set; }//静态定时器;
        internal bool _IsPause = false;//内部定时器
        internal enum TimerRunQueue
        {
            Q_Free,
            Q_Static,
            Q_Dynamic,
            Q_Pause,
        }
        internal TimerTurnType mTurnType = TimerTurnType.TT_None;//定时器状态时间; 
        internal TimerRunQueue mRunQueue = TimerRunQueue.Q_Free;
        private float _pauseTime = 0;//暂停时间
        internal TimerEvent SetNextTime(float waitTime)
        {
            nextTime = ScriptsTime.GetTimes() + waitTime * 1000;
            IsStatic = false;
            return this;
        }
        private void TurnTimer(TimerTurnType type)
        {
            mMachine.PushTurnEvent(type,this);
        }
        public override void StopTimer()
        {
            base.StopTimer();
            TurnTimer(TimerTurnType.TT_Delete);
        }
        public override void PauseTimer(bool ispause = true)
        {
            _IsPause = ispause;
            if (ispause)
            {
                if(state == EventState.ES_Normal)
                {
                    base.PauseTimer(ispause);
                    _pauseTime = ScriptsTime.GetTimes();
                    TurnTimer(TimerTurnType.TT_Pause);                    
                }
            }
            else
            {
                if(state == EventState.ES_Pause)
                {
                    float curTime = ScriptsTime.GetTimes();
                    float pt = curTime - _pauseTime;
                    _startTime += pt;
                    nextTime += pt;
                    IsStatic = (nextTime - curTime) >= 5000;
                    base.PauseTimer(ispause);
                    TurnTimer(TimerTurnType.TT_UnPause);
                }
            }       
        }
        internal float PlayCallBack(float dp)
        {
            frequency++;
            return PlayLogic(dp);
        }
        public virtual float PlayLogic(float timeDp)
        {
            return -1;
        }
        public  override bool ResetTimer()
        {
            if (base.ResetTimer())
            {
                _IsPause = false;
                TurnTimer(TimerTurnType.TT_Reset);
            }
            return false;
        }
        public override void ResetData()
        {
            frequency = 0;
            _pauseTime = 0;
            base.ResetData();
            if(_IsPause&&state == EventState.ES_Normal)
            {
                _pauseTime = ScriptsTime.GetTimes();
                state = EventState.ES_Pause;
            }
        }
    }
    public abstract class CYieldObject
    {
      public virtual void Begin()
      {
          
      }

      public virtual float DoIt()
      {
            return -1;
      }

      public virtual void Stop()
      {

      }
    }
    public class WaitCYield: CYieldObject
    {
        private float mWaitTime;
        private float mCurTime;
        public override void Begin()
        {
            mCurTime = mWaitTime;
        }
        public WaitCYield(float time)
        {
            mWaitTime = time;
        }
        public override float DoIt()
        {
            float nextTime = mCurTime;
            mCurTime = -1;
            return nextTime;
        }
    }
    #endregion

    #region 扩展定时器
    //事件存储器
    internal class TimerStorage
    {
        private Dictionary<long, TimerEvent> mTimerEvent = new Dictionary<long, TimerEvent>();
        private Dictionary<long, Timer_Coroutine> mYieldEvent = new Dictionary<long, Timer_Coroutine>();
        public TimerStorage()
        {

        }

        public T RegTimer<T>(T timer) where T : TimerEvent
        {
            var logic = TimerController.SetTimer(timer);
            mTimerEvent[logic.handle] = logic;
            return logic;
        }

        public Timer_Coroutine RegYield(Timer_Coroutine timer)
        {
            TimerController.SetYieldTimer(timer);
            mYieldEvent[timer.handle] = timer;
            return timer;
        }

        public void Clear()
        {
            foreach (var k in mTimerEvent)
            {
                TimerController.StopTimer(k.Value);
            }
            mTimerEvent.Clear();

            foreach (var k in mYieldEvent)
            {
                k.Value.StopTimer();
            }

            mYieldEvent.Clear();
        }
    }
    //自定义携程事件,非unity
    public class Timer_CYield : TimerEvent
    {
        public delegate void TimerCall(Timer_CYield timer);
        private List<IEnumerator> mIEList = new List<IEnumerator>();
        private IEnumerator mInitTor;
        private IEnumerator mTor;
        private TimerCall mFinishEvent;
        private CYieldObject mYieldObject;
        public object result { get; set; }//结果
        public Timer_CYield(IEnumerator tor, TimerCall FinishedEvent, Component mo = null)
        {
            mInitTor = tor;
            mTor = tor;
            mFinishEvent = FinishedEvent;
            SetNextTime(0);
            SetBody(mo);
        }
        private float Over()
        {
            if(mFinishEvent != null)
            {
                mFinishEvent(this);
            }
            return -1;
        }
        public override float PlayLogic(float timeDp)
        {
            return -1;
        }
        public static Timer_CYield SetTimer(IEnumerator tor, TimerCall FinishedEvent = null, Component mo = null)
        {
            return TimerController.SetTimer(new Timer_CYield(tor, FinishedEvent, mo));
        }
        public override void ResetData()
        {
                
        }
    }
    //携程事件
    public class Timer_Coroutine: TEvent
    {
        public delegate void TimerCall(Timer_Coroutine timer);
        protected IEnumerator mTor;
        protected TimerCall mFinishEvent;
        protected TimerCall mEndCallBack;
        public object result{ get; set; }//结果

        public void Start()
        {

        }

        public Timer_Coroutine(Component mo = null)
        {
            SetBody(mo);
        }

        public void SetIEnumerator(IEnumerator tor)
        {
            mTor = tor;
        }

        //深度关闭
        public  void StopDepthTimer()
        {
            StopTimer();
            if (mTor != null && TimerController.instance != null)
            {
                TimerController.instance.StopCoroutine(mTor);
            }
        }

        public void SetFinishedEvent(TimerCall FinishedEvent)
        {
            mFinishEvent = FinishedEvent;
        }

        public Timer_Coroutine(IEnumerator tor, TimerCall FinishedEvent, Component mo = null)
        {
            mTor = tor;
            mFinishEvent = FinishedEvent;
            SetBody(mo);
        }

        public  IEnumerator GetIEnumerator()
        {
            return mTor;
        }

        public void _SetEndCallBack(TimerCall end)
        {
            mEndCallBack += end;
        }

        public  void End()
        {
            if (IsValid())
            {
                if (mFinishEvent != null)
                {
                    mFinishEvent(this);
                }
            }

            if (mEndCallBack != null)
            {
                mEndCallBack(this);
            }
        }

        public static Timer_Coroutine SetTimer(IEnumerator tor, TimerCall FinishedEvent = null, Component mo = null)
        {
           return TimerController.SetYieldTimer(new Timer_Coroutine(tor, FinishedEvent,mo));
        }
    }
    //逻辑事件
    public class Timer_Logic:TimerEvent
    {
        public delegate float TimerCall(Timer_Logic timer);
        protected TimerCall mCallBack;
        public float delayTime { get; protected set; }//等待时间
        private float mWaitTime = 0;
        public Timer_Logic(TimerCall callBack, float waitTime, Component mo)
        {
            mWaitTime = waitTime;
            mCallBack = callBack;
            SetNextTime(waitTime);
            SetBody(mo);
        }

        public override float PlayLogic(float timeDp)
        {
            delayTime = timeDp;
            return mCallBack(this);
        }

        public static Timer_Logic SetTimer(TimerCall fun,float waitTime, Component mo)
        {
            return TimerController.SetTimer(new Timer_Logic(fun, waitTime,mo));
        }

        public override void ResetData()
        {
            SetNextTime(mWaitTime);
            base.ResetData();
        }
    }
    //逻辑帧事件
    public class Timer_Frequency : TimerEvent
    {
        public delegate void  TimerCall(Timer_Frequency timer);
        public delegate int TimerFun(Timer_Frequency timer);
        private int mDelayValue = 0;
        private int mDelayFrequen = 0;
        private TimerCall mCallBack;
        private TimerFun mFunBack;
        private bool isFun = false;
        public Timer_Frequency(TimerCall callBack, int Fre, Component mo,string tagName = null)
        {
            mDelayValue = Fre;
            mDelayFrequen = Fre;
            mCallBack = callBack;
            SetNextTime(0);
            SetBody(mo, tagName);
            isFun = false;
        }

        public Timer_Frequency(int Fre,TimerFun callBack,Component mo, string tagName = null)
        {
            mDelayValue = Fre;
            mDelayFrequen = Fre;
            mFunBack = callBack;
            SetNextTime(0);
            SetBody(mo, tagName);
            isFun = true;
        }

        public override void ResetData()
        {
            mDelayFrequen = mDelayValue;
            SetNextTime(0);
            base.ResetData();
        }

        public override float PlayLogic(float timeDp)
        {
            if ((mDelayFrequen--) <= 0)
            {
                if (isFun)
                {
                    mDelayFrequen = mFunBack(this);
                    if(mDelayFrequen < 0)
                    {
                        return -1;
                    }
                }
                else
                {
                    mCallBack(this);
                    return -1;
                }
            }
            return 0;
        }

        public static Timer_Frequency SetTimer(TimerCall fun, int fre, Component mo = null)
        {
            return TimerController.SetTimer(new Timer_Frequency(fun, fre, mo));
        }

        public static Timer_Frequency SetTimerEx(TimerFun fun, int fre, Component mo = null)
        {
            return TimerController.SetTimer(new Timer_Frequency(fre,fun,mo));
        }
    }
    //携程加载器
    public class Timer_Mix
    {
        private int mMaxUse = 1;
        private List<Timer_Coroutine> mTimers = new List<Timer_Coroutine>();
        private List<Timer_Coroutine> mProgressTimers = new List<Timer_Coroutine>();
        private Timer_Logic mTimerLogic;
        private System.Action<Timer_Mix> mUpdateAction;
        public bool IsOver { get; protected set; }//是否结束
        public int MaxNum { get; protected set; }
        public int CurNum { get; protected set; }
        public Timer_Mix(int max)
        {
            //暂时有问题,待修复
            //mMaxUse = max;
        }
        public void Play()
        {
            if(IsOver|| mTimerLogic == null)
            {
                IsOver = false;
                mTimerLogic = Timer_Logic.SetTimer(PlayFun,0,null);
            }
        }
        public void Reset()
        {
            Clear();
            IsOver = false;
            MaxNum = 0;
            CurNum = 0;
            mTimerLogic = null;
        }
        public void Clear()
        {
            if (mTimerLogic != null)
            {
                mTimerLogic.StopTimer();
            }
            IsOver = true;
            mTimers.Clear();
            for(int i = 0; i < mProgressTimers.Count;i++)
            {
                mProgressTimers[i].StopTimer();
            }
            mProgressTimers.Clear();
        }
        public void AddTimer(Timer_Coroutine cor)
        {
            mTimers.Add(cor);
            MaxNum++;
        }
        private void FinishEvent(Timer_Coroutine logic)
        {
            mProgressTimers.Remove(logic);
            CurNum++;
            SendUpdateAction();
            PlayFun(null);
        }
        public void OnUpdate(System.Action<Timer_Mix> callBack)
        {
            mUpdateAction = callBack;
        }
        private void SendUpdateAction()
        {
            if (mUpdateAction != null)
            {
                mUpdateAction(this);
            }
        }
        private float PlayFun(Timer_Logic logic)
        {
            if (mProgressTimers.Count < mMaxUse)
            {
                if (mTimers.Count > 0)
                {
                    var timer = mTimers[0];
                    mTimers.Remove(timer);
                    mProgressTimers.Add(timer);
                    timer._SetEndCallBack(FinishEvent);
                    TimerController.SetYieldTimer(timer);
                }
                else if (mProgressTimers.Count == 0)
                {
                    IsOver = true;
                    return -1;
                }
            }
            return 0;
        }
        public IEnumerator WaitPlay()
        {
            Play();
            yield return WaitOver();
        }
        public IEnumerator WaitOver()
        {
            yield return 0;
            while (!IsOver)
            {
                yield return 0;
            }
        }
    }
    //线程事件
    public class Timer_Thread : TEvent
    {
        public delegate float ThreadCall(Timer_Thread timer, ThreadResult result);
        protected ThreadCall mThreadCallBack;
        protected System.Threading.Thread mThread;
        protected ThreadResult mResult = new ThreadResult();
        public class ThreadResult
        {
            public System.Action<Timer_Thread,object> callBack;
            public object value;
            internal void Reset()
            {
                callBack = null;
                value = null;
            }
        }
        public static Timer_Thread SetTimer(ThreadCall threadCall,float waitTime,Component mo)
        {
            Timer_Thread timer = new Timer_Thread();
            timer.mThreadCallBack = threadCall;
            timer.SetBody(mo);
            timer.BeginThread(waitTime);
            return timer;
        }
        private void BeginThread(float waitTime)
        {
            System.Threading.Thread thread = new System.Threading.Thread(ThreadRec);
            mThread = thread;
            thread.Start(waitTime);
        }
        private void ThreadRec(object time)
        {
            while (IsValid())
            {
                float nextTime = mThreadCallBack(this, mResult);
                if (mResult.callBack != null)
                {
                    var call = mResult.callBack;
                    var value = mResult.value;                 
                    if (call != null)
                    {
                        TimerController.SetTimer<Timer_Frequency>(new Timer_Frequency((f) =>
                        {
                            call(this, value);
                        }, 0, stBody, "Thread"));
                    }
                }
                mResult.Reset();
                if(nextTime < 0)
                {
                    StopTimer();
                }
            }
        }
    }
    #endregion
}
