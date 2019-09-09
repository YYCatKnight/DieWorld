//----------------------------------------------
//  F2DEngine: time: 2018.12  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class LoadPercent : UnitData
    {
        private static LoadPercent Main = new LoadPercent();
        public class PerData
        {
            public string key="";
            public string dec="";
            public float pre;
        }
        public System.Action<LoadPercent> OnOver { get; set; }
        public bool IsOver { get;protected set; }
        public int  ErrorNum { get; protected set; }
        public string ErrorDec { get; protected set; }
        private float mRelativePer = 1.0f;//相对值
        private double mNextPer = 0.01f;
        private LoadPercent mParent;
        private PerData mPerDatas = new PerData();
        public LoadPercent()
        {
            Clear();
        }
        public void Clear()
        {
            IsOver = false;
            ErrorNum = 0;
            ErrorDec = "";
            GoOn(0, "");
        }
        public void SetKD(string key,string dec = null)
        {
            if(!string.IsNullOrEmpty(key))
            {
                mPerDatas.key = key;
            }

            if (!string.IsNullOrEmpty(dec))
            {
                mPerDatas.dec = dec;
            }
            else
            {
                mPerDatas.dec = "";
            }
            
            if(mParent != null)
            {
                mParent.SetKD(key, dec);
            }

        }
        public void GoOn(float per,string dec="")
        {
            if (mParent != null)
            {
                var lp = GetPercent();
                mParent.AddOn((per - lp.pre)* mRelativePer, dec);
            }
            mPerDatas.pre = per;
            if (!string.IsNullOrEmpty(dec))
            {
                mPerDatas.dec = dec;
            }
        }
        public void GoOn(string dec="",long times = 1)
        {
            var per = GetPercent();
            GoOn((float)(mNextPer * times)+ per.pre,dec);
        }
        private void AddOn(float per,string dec)
        {
            var pd = GetPercent();
            GoOn(pd.pre + per, dec);
        }
        public void Over(string dec = "",int errorNum = 0,string errorDec = null)
        {
            GoOn(1,dec);
            IsOver = true;
            ErrorNum += errorNum;
            if (!string.IsNullOrEmpty(errorDec))
            {
                ErrorDec += "\n" + errorDec;
            }
            if(mParent != null)
            {
                mParent.ErrorNum += ErrorNum;
                if (!string.IsNullOrEmpty(ErrorDec))
                {
                    mParent.ErrorDec += "\n" + ErrorDec;
                }
            }
            if(OnOver!= null)
            {
                OnOver(this);
            }
        }
        public void SetTimece(long num)
        {
            if(num != 0)
            {
                mNextPer = 1.0f / num;
            }
        }
        public void SetError(string error)
        {
            ErrorNum += 1;
            if (!string.IsNullOrEmpty(error))
            {
                ErrorDec += "\n" + error;
            }
            if(mParent!= null)
            {
                mParent.SetError(error);
            }
        }
        public PerData GetPercent()
        {
            return mPerDatas;
        }
        public List<LoadPercent> CreateBranchs(int num,float pre, bool abs = true)
        {
            List<LoadPercent> pers = new List<LoadPercent>();
            float absPre = pre;
            if (!abs)
            {
                var pro = GetPercent();
                absPre = (1 - pro.pre) * pre;
            }
            absPre /= num;
            for(int i = 0; i < num;i++)
            {
                pers.Add(CreateBranch(absPre, true));
            }
            return pers;
        }
        public LoadPercent CreateBranch(float pre,bool abs = true)
        {
            LoadPercent lp = new LoadPercent();
            if (abs)
            {
                lp.mRelativePer = pre;
            }
            else
            {
                var pro = GetPercent();
                lp.mRelativePer = (1 - pro.pre) * pre;
            }
            lp.mParent = this;
            return lp;
        }
        public void Reset(string dec = "")
        {
            GoOn(0,dec);
        }
        internal static LoadPercent GetNonePrecent()
        {
            Main.mPerDatas = new PerData();
            return Main;
        }
    }
}
