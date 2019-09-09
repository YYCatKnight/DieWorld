//----------------------------------------------
//  F2DEngine: time: 2018.4  by fucong QQ:353204643
//----------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public enum BehaviorStatus
    {
        Completed,
        Failure,
        Running,//一般不直接归还,使用WaitForSeconds
    }

    public class FTreeData: FBehaviorData
    {
        private Func<FTreeData, TickType, BehaviorStatus> mTickCallBackFun;
        private Func<FTreeData, bool> mCheckCallBackFun;
        public void SetEvent(Func<FTreeData, TickType, BehaviorStatus> tick, Func<FTreeData, bool> check)
        {
            mTickCallBackFun = tick;
            mCheckCallBackFun = check;
        }
        public sealed override bool DefalutCheck()
        {
            return mCheckCallBackFun(this);
        }

        public sealed override BehaviorStatus DefalutTick(TickType tt)
        {
            return mTickCallBackFun(this, tt);
        }

    }

    //[FTagAttr("Check","Tick")]//有Check事件,Tick事件显示
    public class FBehaviorData
    {
        [HideInInspector]
        public string nodeName = "";    //节点名字
        [HideInInspector]
        [System.NonSerialized]
        public Func<bool> mCheck;//扩展判断接口
        [HideInInspector]
        [System.NonSerialized]
        public Func<TickType, BehaviorStatus> mTick;
        protected FBehaviorNode mNode;

        public enum TickType
        {
            TT_First,//第一次进入
            TT_Run,//Run循环进入
            TT_UnFirst,//非第一次状态
        }

        public void Init(FBehaviorNode node)
        {
            mNode = node;
        }


        //行动节点执行
        [HideInInspector]
        public  BehaviorStatus Tick(TickType type)//0
        {
            if(mTick != null)
            {
               return mTick(type);
            }
            return DefalutTick(type);
        }

        [HideInInspector]
        public bool Check()
        {
            if (mCheck != null)
            {
               return  mCheck();
            }
            return DefalutCheck();
        }

        //默认判断节点
        [HideInInspector]
        public virtual bool DefalutCheck()
        {
            return true;
        }

        //默认判断节点
        [HideInInspector]
        public virtual BehaviorStatus DefalutTick(TickType tt)
        {
            return BehaviorStatus.Completed;
        }

        public FBehaviorData GetCloneInistane()
        {
            return (FBehaviorData)this.MemberwiseClone();
        }

        object Clone()
        {
            return this.MemberwiseClone();
        }

        protected BehaviorStatus WaitForSeconds(float time)
        {
            return mNode.WaitForSeconds(time);
        }
    }

    public class  FBehaviorNode
    {
        public enum NodeType
        {
            NT_Selector = 0,//单选
            NT_RandSelector=1,//随机排列,然后单选
            NT_Multiple=2,//多选
            NT_Action = 100,//执行节点
        }

        protected FBehaviorTree mMainTree;
        public NodeType mNodeType = NodeType.NT_Selector;
        public FBehaviorNode mParentNode;
        public List<FBehaviorNode> mChildNodes = new List<FBehaviorNode>();
        public Rect mRect;
        public FBehaviorData mData;
        private int mStateIndex = -99;//状态记数器
        /// /////////////////////////////////////////////////////////////
        private Func<BehaviorStatus> mNodeTypeCallBack;
        private BehaviorStatus mLastStatus = BehaviorStatus.Failure;


        public BehaviorStatus WaitForSeconds(float time)
        {
            return mMainTree.WaitForSeconds(time);
        }


        public void ResetNode(bool isRun = false)
        {
            if (isRun)
            {
                if (mLastStatus == BehaviorStatus.Running)
                    mLastStatus = BehaviorStatus.Completed;
            }
            else
            {
                mLastStatus = BehaviorStatus.Failure;
            }
        }

        public void SetMainTree(FBehaviorTree tree)
        {
            mMainTree = tree;
        }

        public bool HaveState(string state)
        {
            return mMainTree.HaveState(state);
        }

        public T GetStateValue<T>(string key)
        {
            return mMainTree.GetStateValue<T>(key);
        }

        

        public BehaviorStatus _SelfTryTick(BehaviorStatus bs)
        {
            if (bs == BehaviorStatus.Running)
            {
                if (mLastStatus == BehaviorStatus.Running)
                {
                    if (mNodeType == NodeType.NT_Action)
                    {
                        return Run(FBehaviorData.TickType.TT_Run);
                    }
                    else
                    {
                        return TryTick();
                    }
                }
                return mLastStatus;
            }
            return TryTick();
        }

        //单选
        private BehaviorStatus SelectorFun()
        {
            return BehaviorStatus.Failure;
        }

        //多选
        private BehaviorStatus MultipleFun()
        {
            return BehaviorStatus.Completed;
        }


        //随机单选
        private BehaviorStatus RandSelectorFun()
        {
            return BehaviorStatus.Failure;
        }


        //随机单选
        private BehaviorStatus ActionFun()
        {
            return Run(FBehaviorData.TickType.TT_First);
        }

        public BehaviorStatus Run(FBehaviorData.TickType type)
        {
            int index = mMainTree.GetStateIndex();
            if (type == FBehaviorData.TickType.TT_First)
            {
                type = (index== mStateIndex +1)? FBehaviorData.TickType.TT_UnFirst: FBehaviorData.TickType.TT_First;
            }

            var bs = mData.Tick(type);
            return bs;
        }

        public void Init()
        {
            if (mNodeType == NodeType.NT_Selector)
            {
                mNodeTypeCallBack = SelectorFun;
            }
            else if(mNodeType == NodeType.NT_RandSelector)
            {
                mNodeTypeCallBack = RandSelectorFun;
            }
            else if(mNodeType == NodeType.NT_Multiple)
            {
                mNodeTypeCallBack = MultipleFun;
            }
            else if(mNodeType == NodeType.NT_Action)
            {
                mNodeTypeCallBack = ActionFun;
            }
        }
    
        public BehaviorStatus PlayCallBack()
        {
            return mNodeTypeCallBack();
        }

        protected  BehaviorStatus TryTick()
        {
            bool go = true;
            if(mLastStatus != BehaviorStatus.Running)
            {
                go = Check();
            }

            if (go)
            {
                mLastStatus = PlayCallBack();
                return mLastStatus;
            }
            return BehaviorStatus.Failure;
        }


        public virtual bool Check()
        {
            return mData.Check();
        }
    }

    public class FBehaviorTree
    {
        public bool mIsPause = false;
        public Action<StateTimeEvent.STimeData, StateTimeEvent.STimeType> TimeStateCallBack;
        public StateTimeEvent mStateTimeEvent;
        public Dictionary<string, FBehaviorNode> mBuffNodes = new Dictionary<string, FBehaviorNode>();
        private Dictionary<string, object> mDataBuff = new Dictionary<string, object>();
        private BehaviorStatus mLastStatue = BehaviorStatus.Failure;
        private int mStateIndex = 0;//状态运行次数计数器
        private float mTimeStatus = 0;//状态持续时间计时器
        public int GetStateIndex()
        {
            return mStateIndex;
        }

        public float ComputerStatusTime()
        {
            mTimeStatus -= Time.deltaTime;
            if (mTimeStatus <= 0)
                ResetNode(true);
            return mTimeStatus;
        }

        public void ResetNode(bool isRun = false)
        {
            mLastStatue = BehaviorStatus.Failure;
            foreach (var k in mBuffNodes)
            {
                k.Value.ResetNode(isRun);
            }
        }

        public void SetState(string key,float time)
        {
            mStateTimeEvent.AddState(key, time);
        }

        public void DeleteState(string key)
        {
            mStateTimeEvent.Reset(key);
        }

        public bool HaveState(string key)
        {
            return mStateTimeEvent.GetStimeData(key) != null;
        }

        public void SetStateValue<T>(string key,T value,float time)
        {
            mStateTimeEvent.AddStateVlaue<T>(key, value, time);
        }


        public StateTimeEvent.STimeData GetStateValue(string key)
        {
            return mStateTimeEvent.GetStimeData(key);
        }

        public T GetStateValue<T>(string key)
        {
            return mStateTimeEvent.GetStimeValue<T>(key);
        }

        public void InitData<T>(Action<string,T> CallBack)where T: FBehaviorData
        {
            foreach(var k in mBuffNodes)
            {
                CallBack(k.Key,(T)k.Value.mData);
            }
        }

        public void SetData(string key,object value)
        {
            mDataBuff[key] = value;
        }

        public object GetData(string key)
        {
            if (mDataBuff.ContainsKey(key))
                return mDataBuff[key];
            return null;
        }

        //注册一个节点
        public FBehaviorNode Reg(FBehaviorData fn)
        {
            FBehaviorNode node = new FBehaviorNode();
            node.SetMainTree(this);
            node.mData = fn;
            fn.Init(node);
            if(mRootNode == null)
            {
                mRootNode = node;
            }
            mBuffNodes[fn.nodeName] = node;
            return node;
        }

        public void RemoveNode(FBehaviorNode node)
        {
            if (node.mParentNode != null)
            {
                node.mParentNode.mChildNodes.Remove(node);
                node.mParentNode = null;
            }

            for(int i = 0; i < node.mChildNodes.Count;i++)
            {
                node.mChildNodes[i].mParentNode = null;
            }

            mBuffNodes.Remove(node.mData.nodeName);
        }


        public void RegChild(FBehaviorNode node,FBehaviorNode parent)
        {
            if(node.mParentNode != null)
            {
                node.mParentNode.mChildNodes.Remove(node);
                node.mParentNode = null;
            }

            if (parent != null)
            {
                node.mParentNode = parent;
                node.mParentNode.mChildNodes.Add(node);
            }
        }


        public FBehaviorNode mRootNode;

        public void SetHead(string headHead)
        {
            mRootNode = GetNode(headHead);
            ResetNode();
        }

        //状态持续时长
        public BehaviorStatus WaitForSeconds(float time)
        {
            mTimeStatus = time;
            return BehaviorStatus.Running;
        }

        public BehaviorStatus UpdateTick()
        {
            if (mIsPause)
                return BehaviorStatus.Failure;

            return mLastStatue;
        }

        public FBehaviorNode GetNode(string nodeName)
        {
            if (mBuffNodes.ContainsKey(nodeName))
                return mBuffNodes[nodeName];
            return null;
        }

        public class SaveTreeFile
        {
            public class Data
            {
                public Unit_Rect rect;
                public string[] nextData;
                public string conditionMothods;
                public string playMothods;
                public FBehaviorNode.NodeType nodeType;
            }
            public string headHodeName;//其实节点名字
            public string typeName;//类型数据
            public string param;//类型参数
            public Data[] data;//基本数据
        }


        public void LoadFile<T>(string name, Func<T, FBehaviorData.TickType, BehaviorStatus> TickBack, Func<T, bool> CheckBack) where T : FTreeData
        {
            LoadFile(name,FFilePath.FP_Relative);
            InitData<T>((s, t) =>
            {
                t.SetEvent((data, ticktype) =>
                {
                    return TickBack((T)data, ticktype);
                },
                (data) =>
                {
                    return CheckBack((T)data);
                });
            });
        }

        private void UpdateTimeState(StateTimeEvent.STimeData st, StateTimeEvent.STimeType type)
        {
            if (TimeStateCallBack != null)
            {
                TimeStateCallBack(st, type);
            }
        }

        public string LoadFile(string name,FFilePath pathType)
        {
            mStateTimeEvent = new StateTimeEvent(UpdateTimeState);
            string tempPath = "." + ResConfig.BEHAVIORTREE;
            if (!name.EndsWith(tempPath))
            {
                name += tempPath;
            }

            //加载配置
            FSaveHandle sd = FSaveHandle.Create(name, pathType);
            SaveTreeFile stFree = new SaveTreeFile();
            return stFree.typeName;
        }

        public void SaveFile(string fileName, Type type)
        {
            //保存配置
            FSaveHandle sd = FSaveHandle.Create(fileName, FFilePath.FP_Abs,FOpenType.OT_Write);
            SaveTreeFile stf = new SaveTreeFile();
            stf.typeName = type.FullName;
            stf.headHodeName = mRootNode.mData.nodeName;
            stf.data = new SaveTreeFile.Data[mBuffNodes.Count];
            IList paramList = Array.CreateInstance(type,mBuffNodes.Count);

            int index = 0;
            stf.param = StringSerialize.Serialize(paramList);
            sd.PushObject(stf);
            sd.Save();
        }
    }
}
