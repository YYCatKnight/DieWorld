//----------------------------------------------
//  F2DEngine: time: 2016.6  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace F2DEngine
{
    public class FArpgMachine
    {

        public class FArgpBaseData
        {
            [HideInInspector]
            public string conditionState = ""; //状态名字
            [HideInInspector]
            public float conditionTimes = 0; //状态改变时间
            [HideInInspector]
            public string nodeName = "";    //节点名字

            [HideInInspector]
            [System.NonSerialized]
            public Func<FArpgNode, FArpgNode, bool>[] ConditionCallBack = new Func<FArpgNode, FArpgNode, bool>[0]; //条件判断
            [HideInInspector]
            [System.NonSerialized]
            public Action<FArpgNode,float,int>[]PlayStateCallBacks = new Action<FArpgNode,float,int>[0]; //状态判断,第三个参数0,表示开始,-1,为结束

            protected FArpgNode mNode;



            public T GetData<T>(string key)
            {
               return mNode.GetData<T>(key);
            }

            public FArpgMachine GetArpgMachine()
            {
                return mNode.GetArpgMachine();
            } 

            public bool SetNode(FArpgNode node)
            {
                mNode = node;
                return true;
            }


            public void TestPlay(FArpgNode fad, float timedp, int type)
            {

            }

            public bool BaseTotalLess(FArpgNode fad, FArpgNode next)
            {
                if (fad.totalTimes <= next.mFArgpBaseData.conditionTimes)
                {
                    return true;
                }
                return false;
            }

            public  bool TotalGreater(FArpgNode fad, FArpgNode next)
            {
                if (fad.totalTimes >= next.mFArgpBaseData.conditionTimes)
                {
                    return true;
                }
                return false;
            }

            public  bool BaseLess(FArpgNode fad, FArpgNode next)
            {
                if (fad.curTimes <= next.mFArgpBaseData.conditionTimes)
                {
                    return true;
                }
                return false;
            }

            public bool Greater(FArpgNode fad, FArpgNode next)
            {
                if (fad.curTimes >= next.mFArgpBaseData.conditionTimes)
                {
                    return true;
                }
                return false;
            }


            public  FArgpBaseData GetCloneInistane()
            {
                return (FArgpBaseData)this.MemberwiseClone(); 
            }

            object Clone()
            {
                return this.MemberwiseClone();
            }

        }

        public class FArpgNode
        {
            public FArgpBaseData mFArgpBaseData;
            public float curTimes = 0;
            public float totalTimes = 0;
            public List<FArpgNode> mNextData = new List<FArpgNode>();
            public FArpgNode mSkipNode;
            public Func<FArpgNode, FArpgNode, bool> mConditionCall;
            public Rect mRect = new Rect(); //特殊编辑器使用
            public FArpgNode mMian;
            public FArpgMachine mArpgMachine;
            public Action<FArpgNode> mMainCallBack;
            public Dictionary<string, FArpgNode> mMainBuffs = null;

            public FArpgNode GetRealArpg()
            {
                if (mSkipNode != null)
                    return mSkipNode;

                if(mMian.mMainBuffs.ContainsKey(mFArgpBaseData.nodeName))
                {
                    return mMian.mMainBuffs[mFArgpBaseData.nodeName];
                }
                return this;
            } 

        
             

            public FArpgNode(Action<FArpgNode> CallBack, FArpgMachine mach)
            {
                mMian = this;
                mMian.mMainCallBack = CallBack;
                mConditionCall = Greater;
                mArpgMachine = mach;
                mMainBuffs = new Dictionary<string, FArpgNode>();
            }

            public FArpgNode(FArpgNode fad, FArgpBaseData abd, bool isCreate = true, Func<FArpgNode, FArpgNode, bool> ConditionCall = null)
            {
                mMian = fad.mMian;
                mFArgpBaseData = abd;
                if (ConditionCall == null)
                {
                    if (mFArgpBaseData.ConditionCallBack == null || mFArgpBaseData.ConditionCallBack.Length == 0)
                    {
                        mConditionCall = TotalGreater;
                    }
                }
                else
                {
                    mConditionCall = ConditionCall;
                }
                
                if (isCreate)
                {
                    mMian.mMainBuffs[mFArgpBaseData.nodeName] = this;
                }
            }


            public T GetData<T>(string key)
            {
                return mMian.mArpgMachine.GetData<T>(key);
            }

            public FArpgMachine GetArpgMachine()
            {
                return mMian.mArpgMachine;
            }

            public FArpgNode Regs(FArpgNode node,bool sameRemove = false)
            {
                if (node == null)
                    return null;

                node.mFArgpBaseData.SetNode(node);

                int index = 0;
                for (int i = 0; i < mNextData.Count; i++)
                {
                    if (mNextData[i].mFArgpBaseData.conditionTimes > node.mFArgpBaseData.conditionTimes)
                    {
                        index = i;
                        break;
                    }
                }
                if (!mNextData.Contains(node))
                {
                    mNextData.Insert(index, node);
                    return node;
                }
                else if(sameRemove)
                {
                    mNextData.Remove(node);
                }
                return null;
            }

            //节点名字，状态控制,几秒转换，是否创建,
            public FArpgNode Regs(FArgpBaseData abd, bool isCreate = true, Func<FArpgNode, FArpgNode, bool> ConditionCall = null)
            {
                FArpgNode fad = new FArpgNode(this,abd,isCreate,ConditionCall);
                return Regs(fad);
            }


            public void Reset(bool isALL = true)
            {
                curTimes = 0;
                if (isALL)
                {
                    totalTimes = 0;
                }
            }

            private bool ConditonCall(FArpgNode node)
            {
                if (node.mConditionCall != null)
                {
                    if (!node.mConditionCall(this, node))
                    {
                        return false;
                    }
                }

                if (node.mFArgpBaseData.ConditionCallBack != null)
                {
                    for (int i = 0; i < node.mFArgpBaseData.ConditionCallBack.Length; i++)
                    {
                        var callBack = node.mFArgpBaseData.ConditionCallBack[i];
                        if (!callBack(this, node))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public void PlayStateCallBack(float timeDp,int type)
            {
                if (mFArgpBaseData != null)
                {
                    if (mFArgpBaseData.PlayStateCallBacks != null)
                    {
                        for (int i = 0; i < mFArgpBaseData.PlayStateCallBacks.Length; i++)
                        {
                            mFArgpBaseData.PlayStateCallBacks[i](this, timeDp, type);
                        }
                    }
                }
            }

            public FArpgNode Play(float time, List<string> state,out bool isChange)
            {
                isChange = false;
                this.curTimes += time;
                this.totalTimes += time;
                PlayStateCallBack(time, 1);
                for (int i = 0; i < mNextData.Count; i++)
                {
                    FArpgNode fad = mNextData[i];
                    if (fad.mFArgpBaseData.conditionState == ""|| state.Contains(fad.mFArgpBaseData.conditionState))
                    {
                        if (ConditonCall(fad))
                        {
                            Reset();
                            fad.Reset();
                            fad.PlayStateCallBack(0, -1);
                            fad = fad.GetRealArpg();
                            fad.Reset();
                            fad.PlayStateCallBack(0,0);
                            
                            if (mMian.mMainCallBack != null)
                            {
                                mMian.mMainCallBack(fad);
                            }

                            isChange = true;
                            return fad;
                        }
                    }
                }
                return this;
            }
        }



        public static bool BaseTotalLess(FArpgNode fad, FArpgNode next)
        {
            if (fad.totalTimes <= next.mFArgpBaseData.conditionTimes)
            {
                return true;
            }
            return false;
        }

        public static bool TotalGreater(FArpgNode fad, FArpgNode next)
        {
            if (fad.totalTimes >= next.mFArgpBaseData.conditionTimes)
            {
                return true;
            }
            return false;
        }

        public static bool BaseLess(FArpgNode fad, FArpgNode next)
        {
            if (fad.curTimes <= next.mFArgpBaseData.conditionTimes)
            {
                return true;
            }
            return false;
        }

        public static bool Greater(FArpgNode fad, FArpgNode next)
        {
            if (fad.curTimes >= next.mFArgpBaseData.conditionTimes)
            {
                return true;
            }
            return false;
        }

        public Action<FArpgNode> StateCallBack;
        public Action<StateTimeEvent.STimeData,StateTimeEvent.STimeType> TimeStateCallBack;
        private Dictionary<string, object> mDataBuff = new Dictionary<string, object>();
        public StateTimeEvent mStateTimeEvent;
        private FArpgNode mCurFArpgData;
        public string mStartNodeName = "";
        public bool mIsPause = false;
        private bool mIsLock = false;
        public FArpgMachine()
        {
            NewRoot();
        }


        private void NewRoot()
        {
            mStateTimeEvent = new StateTimeEvent(UpdateTimeState);
            mCurFArpgData = new FArpgNode(CallBackStateChange,this);
        }

        public void SetStartNode(string node)
        {
            if (node != "")
            {
                var root = GetRoots();
                if (root.mMainBuffs.ContainsKey(node))
                {
                    mCurFArpgData = root.mMainBuffs[node];
                }
            }
        }

        private void UpdateTimeState(StateTimeEvent.STimeData st,StateTimeEvent.STimeType type)
        {
            if(TimeStateCallBack != null)
            {
                TimeStateCallBack(st,type);
            }
        }

        private void CallBackStateChange(FArpgNode fad)
        {
            if(StateCallBack != null)
            {
                StateCallBack(fad);
            }
        }


        public FArpgNode GetCurNode()
        {
            return mCurFArpgData;
        }

        public bool PlayUpdate(float dp)
        {
            if (mIsPause)
                return false;
            List<StateTimeEvent.STimeData> stds = mStateTimeEvent.GetStimeDataList("");
            List<string> tempStates = new List<string>();
            for(int i = 0; i < stds.Count;i++)
            {
                tempStates.Add(stds[i].Group);
            }

            mStateTimeEvent.PlayLogic(dp);
           
            bool isChange = false;
            if (mCurFArpgData != null&& !mIsLock)
            {
                mIsLock = true;
                mCurFArpgData = mCurFArpgData.Play(dp,tempStates,out isChange);
                mIsLock = false;
            }
            return isChange;
        }

         
        public StateTimeEvent.STimeData GetFirstSTimeData()
        {
            return mStateTimeEvent.GetFirstSTimeData(); 
        }

        public bool SetArgpState(string key, float time = 0.3f,string lay = "none")
        {
            if (mCurFArpgData.mFArgpBaseData != null)
            {
                if (mCurFArpgData.mFArgpBaseData.conditionState == key)
                {
                    mCurFArpgData.Reset(false);
                }
                bool isChange = false;
                isChange = PlayUpdate(0);
                mStateTimeEvent.AddState(lay, time, key);
                return isChange;
            }
            return false;
        }

        public FArpgNode GetRoots()
        {
            return mCurFArpgData.mMian;
        }


        public void SetData(string key,object obj)
        {
            mDataBuff[key] = obj;
        }

        public T GetData<T>(string key)
        {
            if (mDataBuff.ContainsKey(key))
            {
                return (T)mDataBuff[key];
            }
            return default(T);
        }

        public FArpgNode RemoveNode(FArpgNode node)
        {
            FArpgNode root = GetRoots();
            if(root.mMainBuffs.ContainsKey(node.mFArgpBaseData.nodeName))
            {
                root.mMainBuffs.Remove(node.mFArgpBaseData.nodeName);
            }

            if(root.mNextData.Contains(node))
            {
                root.mNextData.Remove(node);
            }

            foreach(var k in root.mMainBuffs)
            {
                if(k.Value.mSkipNode == node)
                {
                    k.Value.mSkipNode = null;
                }
                for(int i = 0; i < k.Value.mNextData.Count;i++)
                {
                    FArpgNode temp = k.Value.mNextData[i];
                    if(temp == node)
                    {
                        k.Value.mNextData.Remove(temp);
                        break;
                    }
                }
            }
            return node;
        }

        public class ArpgFileData
        {
            public class Data
            {
                public Unit_Rect rect;
                public string[] nextData;
                public string skipNode = "";
                public string[] conditionMothods;
                public string[] playMothods;
            }
            public string headHodeName;//其实节点名字
            public string typeName;//类型数据
            public string param;//类型参数
            public Data[] data;//基本数据
        }

        public string LoadFile(string name,FFilePath pathType = FFilePath.FP_Relative)
        {
            string tempPath = "." + ResConfig.ARPGEX;
            if (!name.EndsWith(tempPath))
            {
                name += tempPath;
            }

            NewRoot();
            //加载配置
            FSaveHandle sd = FSaveHandle.Create(name, pathType);
            ArpgFileData afd = new ArpgFileData();
            sd.FromObject(afd);
            mStartNodeName = afd.headHodeName;
            IList baseData = (IList)StringSerialize.Deserialize(afd.param,typeof(List<>).MakeGenericType(Assembly.Load("Assembly-CSharp").GetType(afd.typeName)));

            if(afd != null)
            {
                FArpgNode roots = GetRoots();
                for (int i = 0; i < afd.data.Length; i++)
                {
                    var d = afd.data[i];
                    FArgpBaseData farpgData = (FArgpBaseData)baseData[i];
                    if (d.conditionMothods != null)
                    {
                        farpgData.ConditionCallBack = new Func<FArpgNode, FArpgNode, bool>[d.conditionMothods.Length];
                        for (int c = 0; c < d.conditionMothods.Length; c++)
                        {
                            farpgData.ConditionCallBack[c] = (System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>)System.Delegate.CreateDelegate(typeof(System.Func<FArpgMachine.FArpgNode, FArpgMachine.FArpgNode, bool>), farpgData, d.conditionMothods[c]);
                        }
                    }

                    if (d.playMothods != null)
                    {
                        farpgData.PlayStateCallBacks = new Action<FArpgNode, float, int>[d.playMothods.Length];
                        for (int c = 0; c < d.playMothods.Length; c++)
                        {
                            farpgData.PlayStateCallBacks[c] = (System.Action<FArpgNode, float, int>)System.Delegate.CreateDelegate(typeof(System.Action<FArpgNode, float, int>), farpgData, d.playMothods[c]);
                        }
                    }


                    FArpgNode node = roots.Regs(farpgData, true,null);
                    node.mRect = d.rect.GetRect();
                }

                for(int i = 0; i < afd.data.Length; i++)
                {
                    var d = afd.data[i];
                    FArgpBaseData farpgData = (FArgpBaseData)baseData[i];
                    FArpgNode node = roots.mMainBuffs[farpgData.nodeName];
                    if(d.skipNode != "")
                    {
                        node.mSkipNode = roots.mMainBuffs[d.skipNode];
                    }
                    if(d.nextData != null)
                    {
                        for(int j = 0; j < d.nextData.Length;j++)
                        {
                            var nextNode = roots.mMainBuffs[d.nextData[j]];
                            node.Regs(nextNode);
                        }
                    }
                }
            }

            SetStartNode(mStartNodeName);
            return afd.typeName;
        }


        public void SaveFile(string fileName,Type type)
        {
            //保存配置
            FArpgNode roots = GetRoots();
            Dictionary<string,FArpgNode> nodes = roots.mMainBuffs;

            FSaveHandle sd = FSaveHandle.Create(fileName, FFilePath.FP_Abs, FOpenType.OT_Write);
            ArpgFileData arpgFileData = new ArpgFileData();
            arpgFileData.typeName = type.FullName;
            arpgFileData.headHodeName = mStartNodeName;
            ArpgFileData.Data[] datas = new ArpgFileData.Data[nodes.Count];
            arpgFileData.data = datas;
            IList paramList = Array.CreateInstance(type, nodes.Count);

            int index = 0;
            foreach (var k in nodes)
            {

                var afd = new ArpgFileData.Data();
                datas[index] = afd;
                afd.rect = new Unit_Rect();
                afd.rect.SetRect(k.Value.mRect);

                //条件判断事件
                var conditions = k.Value.mFArgpBaseData.ConditionCallBack;
                if (conditions != null && conditions.Length != 0)
                {
                    afd.conditionMothods = new string[conditions.Length];
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        afd.conditionMothods[i] = conditions[i].Method.Name;
                    }
                }

                //状态执行事件
                var states = k.Value.mFArgpBaseData.PlayStateCallBacks;
                if (states != null && states.Length != 0)
                {
                    afd.playMothods = new string[states.Length];
                    for (int i = 0; i < states.Length; i++)
                    {
                        afd.playMothods[i] = states[i].Method.Name;
                    }
                }


                if (k.Value.mSkipNode != null)
                {
                    afd.skipNode = k.Value.mSkipNode.mFArgpBaseData.nodeName;
                }

                afd.nextData = new string[k.Value.mNextData.Count];
                for (int i = 0; i < k.Value.mNextData.Count; i++)
                {
                    afd.nextData[i] = k.Value.mNextData[i].mFArgpBaseData.nodeName;
                }

                paramList[index++] = k.Value.mFArgpBaseData;

            }
            arpgFileData.param = StringSerialize.Serialize(paramList);
            sd.PushObject(arpgFileData);
            sd.Save();
        }
    }   
}
