//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class TipsManger : ManagerTemplate<TipsManger>
    {
        #region 通用接口
        public interface TipsParams
        {
             string GetTipsPlane();
             int GetLay();
        }
        private  int mMaskCode = 0;
        private  Dictionary<int, Queue<TipsParams>> mDelayData = new Dictionary<int, Queue<TipsParams>>();
        public void SetMaskCode(int lay,bool mask = true)
        {
            int code = 1 << lay;
            mMaskCode = FUniversalFunction.SetMaskCode(mMaskCode, code, mask);
            if(!mask)
            {
                if (code != 0)
                {
                    Queue<TipsParams> qTips = null;
                    if (mDelayData.TryGetValue(code, out qTips))
                    {
                        _CreateQueueTips(qTips);
                    }
                }
                else
                {
                    foreach(var k in mDelayData)
                    {
                        _CreateQueueTips(k.Value);
                    }
                }
            }
        }
        private  void _CreateQueueTips(Queue<TipsParams> qTips)
        {
            while (qTips.Count > 0)
            {
                var param = qTips.Dequeue();
                FEngineManager.ShowWindos(param.GetTipsPlane(), WinShowType.WT_Normal, param);
            }
        }
        private BasePlane CreateTips(TipsParams param)
        {
            var code = 1<< param.GetLay();
            if(FUniversalFunction.IsContainSameType(mMaskCode,code))
            {
                Queue<TipsParams> qTips = null;
                if(!mDelayData.TryGetValue(code,out qTips))
                {
                    qTips = new Queue<TipsParams>();
                    mDelayData[code] = qTips;
                }
                qTips.Enqueue(param);
            }
            else
            {
               return FEngineManager.ShowWindos(param.GetTipsPlane(),WinShowType.WT_Normal,param);
            }
            return null;
        }      
        public static BasePlane ShowTips(string key,bool isNormal = true)
        {
            if (isNormal)
            {
               return instance.CreateTips(Normal_TipParams.Create(key));
            }
            else
            {
                return instance.CreateTips(Flow_TipParams.Create(key));
            }
        }
        public static BasePlane ShowTips(TipsParams param)
        {
           return  instance.CreateTips(param);
        }
        private static int mMaskWait = 0;
        private static BasePlane mWaitPlane;
        public static BasePlane ShowWait(int lay, bool mask = true)
        {
            mMaskWait = FUniversalFunction.SetMaskCode(mMaskWait,1<< lay, mask);
            if(mask)
            {
                if(mMaskWait != 0&&mWaitPlane == null)
                {
                    mWaitPlane = FEngineManager.ShowWindos(ResConfig.CC_WAITPLANE);
                }
            }
            else
            {
                if(mMaskWait == 0&& mWaitPlane!= null)
                {
                    FEngineManager.CloseWindos(mWaitPlane);
                }
            }
            return mWaitPlane;
        }
        #endregion
        #region 扩展接口
        public class Normal_TipParams : TipsParams
        {
            public string Context { get; protected set; }
            public static Normal_TipParams Create(string txt)
            {
                Normal_TipParams tips = new Normal_TipParams();
                tips.Context = txt;
                return tips;
            }
            public int GetLay()
            {
                return 1;
            }
            public string GetTipsPlane()
            {
                return ResConfig.CC_NORMALTIPS;
            }
        }

        public class Flow_TipParams : TipsParams
        {
            public string Context { get; protected set; }
            public static Flow_TipParams Create(string txt)
            {
                Flow_TipParams tips = new Flow_TipParams();
                tips.Context = txt;
                return tips;
            }
            public int GetLay()
            {
                return 2;
            }
            public string GetTipsPlane()
            {
                return ResConfig.CC_FLOWTIPS;
            }
        }

        #endregion
    }
}
