//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F2DEngine
{
    public enum UIWIND_TYPE
    {
        UI_NORMAL,
        UI_TEMP,
        UI_SOLID, //固定窗口
        UI_SOLF, //软窗口
    }

    //界面刷新机制
    public enum UIRefresh_Type
    {
        None,//无
        Refresh,//刷新
        Overlying,//叠加
        Reset,//重置
    }

    public enum WinShowType
    {
        WT_Normal = 0,//正常打开界面
        WT_NoClose = 1 << 1,//不关闭任何窗口
        WT_Pool = 1 << 2,//使用缓存池
        WT_Back = 1 << 3,//关闭界面回退
        WT_Reset = 1 << 4,//返回时重刷界面
    }

    public enum LayerType
    {
        LT_Low = -1,
        LT_Down_Down = 0,
        LT_Down_Scene = 1,
        LT_Down_0 = 2,
        LT_Down_1 = 3,
        LT_Down_2 = 4,
        LT_Normal = 5,
        LT_Up_0 = 6,
        LT_Up_1 = 7,
        LT_Up_2 = 8,
        LT_Up_Load = 9,
        LT_Up_Up = 10,
        LT_Hig,
    }

    public class BasePlane : UnitObject
    {
        public UIWIND_TYPE nUiType;
        public UIRefresh_Type RefreshType = UIRefresh_Type.None; //界面以开，是否重置界面
        public LayerType LayerType = LayerType.LT_Normal;
        public Action<BasePlane> nClosePlaneEvent;
        public bool UsePool = false;
        //动画接口
        public string StarAnimation = "";
        public string CloseAnimation = "";
        internal bool mIsClose = false;
        internal WinShowType mWinShowType = WinShowType.WT_Normal;
        public object[] mArgs { get; protected set; }
        protected FCUniversalPanel mMainPlane;
        private Action mCloseDelayEvent;
        protected UniversalModel mUniMode = new UniversalModel();
        protected MsgMesh mMsgMesh = new MsgMesh();
        public enum OpenType
        {
            OT_Normal,
            OT_Active,
            OT_Pool,
            OT_BackPool,//回退池
            OT_None,
        }
        private bool mInitOver = false;
        private Action mInitDelayAction;
        private bool mIsRealPlane = false;
        internal void SetHandle(string poolName,int index = -1)
        {
            PoolName = poolName;
            HandleID = poolName;
        }
        internal string HandleID { get; set; }
        internal void openInit(OpenType type, params object[] o)
        {
            mIsRealPlane = true;
            mInitOver = false;
            mInitDelayAction = null;
            mMainPlane = this.gameObject.GetComponent<FCUniversalPanel>();
            mIsClose = false;
            mArgs = o;
            if (type != OpenType.OT_Normal)
            {
                openPool(type, o);
            }
            else
            {
                
                ScriptsTime.BeginTag("_plane");
                Init(o);
                ScriptsTime._ShowTimeTag("_plane",this.GetType().Name+"打开消耗");
            }

            mInitOver = true;
            if(mInitDelayAction!= null)
            {
                mInitDelayAction();
            }
        }

        public bool IsRealyPlane()
        {
            return mIsRealPlane;
        }

        public virtual void openPool(OpenType type, params object[] o)
        {
        }

        public bool GetIsClose()
        {
            return mIsClose;
        }

        public virtual void Init(params object[] o)
        {
          
        }
        public virtual void Click(string btName)
        {

        }
        public GameObject GetFObject(string keyName)
        {
            return mMainPlane.GetFObject(keyName);
        }
        public T GetItem<T>(string key, bool isRemoveShell = false) where T : UnitObject
        {
            return mMainPlane.GetItem<T>(key, isRemoveShell);
        }
        public T GetFObject<T>(string key) where T : Component
        {
            return mMainPlane.GetFObject<T>(key);
        }
        //临时回调
        internal virtual void BackPoolClear()
        {

        }
        internal void CloseClear()
        {
            mUniMode.Clear();
            mMsgMesh.Clear();
            mIsClose = true;
            if (nClosePlaneEvent != null)
            {
                Action<BasePlane> tempEvent = nClosePlaneEvent;
                nClosePlaneEvent = null;
                tempEvent(this);
            }
            Clear();
        }
        public override void Clear()
        {

        }

        internal void SetDelayEvent(Action call,bool isDelayClose = true)
        {
            if(!string.IsNullOrEmpty(CloseAnimation)&& isDelayClose)
            {
                mCloseDelayEvent = call;
            }
            else
            {
                call();
            }
        }
        internal void PlayDelayCloseEvent()
        {
            if(mCloseDelayEvent != null)
            {
                mCloseDelayEvent();
            }
        }
        public void ResetMySelf()
        {
            if (mInitOver)
            {
                int index = this.transform.GetSiblingIndex();
                CloseMySelf();
                var newPlane = FEngineManager.ShowWindos(PoolName, mWinShowType, mArgs);
                if (newPlane != null)
                {
                    newPlane.transform.SetSiblingIndex(index);
                }
            }
            else
            {
                mInitDelayAction = () =>
                {
                    ResetMySelf();
                };
            }
        }

        public virtual void CloseMySelfExtend(bool clearBack = false)
        {
            CloseMySelf(clearBack);
        }


        public void CloseMySelf(bool clearBack = false)
        {          
            if (mInitOver)
            {
                if (clearBack)
                {
                    FEngineManager.ClearBackWindos();
                }
                FEngineManager._CloseWindos(HandleID,"", FEngineManager.IsHaveSameWinType(mWinShowType, WinShowType.WT_Reset) ? WinShowType.WT_Reset : WinShowType.WT_Normal);
                FEngineManager.ResetWindos(HandleID);
            }
            else
            {
                mInitDelayAction = () =>
                {
                    CloseMySelf(clearBack);
                };
            }
        }

        private float mDragTime;
        public void OnDrag()
        {
            mDragTime = Time.time;
        }

        public bool CanSwipeClose(float timeDp)
        {
            if (Time.time - mDragTime > timeDp)
            {
                return true;
            }
            return false;
        }

        public void OnSwipeClose(float timeDp = 0.2f)
        {
            if (CanSwipeClose(timeDp))
            {
                CloseMySelfExtend();
            }
        }

        public virtual UIWIND_TYPE GetIsAutoType()
        {
            return nUiType;
        }
    }
}
