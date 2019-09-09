//----------------------------------------------
//  F2DEngine: time: 2015.11  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace F2DEngine
{
    public class FCharacterData : UnitData
    {
        protected List<FCharacterData> mDataList = new List<FCharacterData>();
        public float mTimeDp = 0;


        FCharacterData GetCloneInistane()
        {
            return (FCharacterData)this.MemberwiseClone();
        }
        object Clone()
        {
            return this.MemberwiseClone();
        }
        public virtual string GetKey()
        {
            return "";
        }
        public void ComputeData(float timeDp)
        {
            for (int j = mDataList.Count - 1; j >= 0; j--)
            {
                FCharacterData fd = mDataList[j];
                LogicCharacter(timeDp,fd);
                if((fd.mTimeDp -= timeDp) < 0)
                {
                    mDataList.Remove(fd);
                    EndCharacter(fd);
                }
            }
        }
        public virtual void EndCharacter(FCharacterData fcd)
        { 
        }
        public void AddFCharacter(FCharacterData fcd)
        {
            mDataList.Add(fcd.GetCloneInistane());
            BeginCharacter(fcd);
            
        }
        protected virtual void BeginCharacter(FCharacterData fcd)
        {

        }
        protected virtual void LogicCharacter(float timeDp,FCharacterData fcd)
        {

        }
    }

    public class SpeedData : FCharacterData
    {
        public float mSpeed = 1;
        public float mRatio = 1;

        public Action<float> mSpeedEvent;
        public SpeedData(float sp,float Ra)
        {
            mSpeed = sp;
            mRatio = Ra;
        }
        public float GetSpeed()
        {
            float curSpeed = mSpeed;
            for(int i = 0;i < mDataList.Count;i++)
            {
                SpeedData sd = (SpeedData)mDataList[i];
                curSpeed *= sd.mRatio;
            }
            return curSpeed;
        }

        public override void EndCharacter(FCharacterData fcd)
        {
            SentEvent();
        }

        private void SentEvent()
        {
            if (mSpeedEvent != null)
            {
                mSpeedEvent(GetSpeed());
            }
        }
        protected override void BeginCharacter(FCharacterData fcd)
        {
            SentEvent();
        }


        public override string GetKey()
        {
            return "speed";
        }
    }

    public class BloodData : FCharacterData
    {
        public int blood;
        public int defend;
        public int attack;
        private int curBlood = 0;
        public Action<int> mBloodEvent;
        public BloodData(int bl,int de,int att)
        {
            blood = bl;
            defend = de;
            attack = att;
            curBlood = blood;
        }
        public int GetBlood()
        {
            return blood;
        }
        protected override void BeginCharacter(FCharacterData fcd)
        {
            BloodData sd = (BloodData)fcd;
            int bl = sd.attack - defend;
            if (blood <= 0)
            {
                //Debug.LogError("多次计算碰撞错误");
                return;
            }
            blood -= (bl > 0 ? bl : 0);
            if (mBloodEvent != null && curBlood != blood)
            {
                curBlood = blood;
                mBloodEvent(blood);
            }
        }

        public override string GetKey()
        {
            return "BloodData";
        }

    }

    public class FStateMachine:UnitData
    {
        public List<FCharacterData> mFCharacterDataList = new List<FCharacterData>();

        public void ClearData()
        {
            mFCharacterDataList.Clear();
        }
        public void RegisterData(FCharacterData fd)
        {
            mFCharacterDataList.Add(fd);
        }

        public FCharacterData GetCharacterData(string key)
        {
            for(int  i = 0; i < mFCharacterDataList.Count;i++)
            {
                if (key == mFCharacterDataList[i].GetKey())
                    return mFCharacterDataList[i];
            }
            return null;
        }
        public void InfluenceMachine(FStateMachine sm)
        {
            for(int  i = 0; i < sm.mFCharacterDataList.Count;i++)
            {
                FCharacterData other = sm.mFCharacterDataList[i];
                FCharacterData fd = GetCharacterData(other.GetKey());
                if(fd != null)
                {
                    fd.AddFCharacter(other);
                }
            }
        }
        
        public void PlayUpdate(float timeDp)
        {
            for(int i = 0; i < mFCharacterDataList.Count;i++)
            {
                mFCharacterDataList[i].ComputeData(timeDp);
            }
        }
    }

}
