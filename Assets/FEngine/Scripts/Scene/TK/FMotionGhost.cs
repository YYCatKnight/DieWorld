using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    //Spine残影效果
    public class FMotionGhost : UnitObject
    {
        public Color GhostColor = Color.blue;
        public float GhostLifeTime = 0.5f;
        public float GhostFrequency = 0.1f;
        private FSpineBody mFSpineBody;
        private List<SpineData> mBodys = new List<SpineData>();
        private List<FSpineBody> mPools = new List<FSpineBody>();
        private Timer_Logic mLogicEvent;
        public class SpineData
        {
            public FSpineBody body;
            public float lifeTime = 1.0f;
        }

        private FBasePeople mHostPeople;
        public enum GhostType
        {
            GT_Spine = 0,
        }

        void Start()
        {
            mHostPeople = this.GetComponent<FBasePeople>();
            if(mHostPeople.transform.parent == null)
            {
                Debug.LogError("FMotionGhost必须在一个父节点下");
            }
            mLogicEvent = Timer_Logic.SetTimer(UpdateGhost,1,this);
        }

        private void OnDestroy()
        {
            if (mLogicEvent != null)
            {
                mLogicEvent.StopTimer();
                mLogicEvent = null;
            }
        }
        private float  UpdateGhost(Timer_Logic le)
        {
            CreateGhost();
            return GhostFrequency;
        }

        void Update()
        {
            for(int i = mBodys.Count -1; i >= 0;i--)
            {
                SpineData sd = mBodys[i];
                if((sd.lifeTime -= Time.deltaTime/GhostLifeTime) > 0)
                {                   
                    sd.body.SetColor(new Color(GhostColor.r,GhostColor.g,GhostColor.b,sd.lifeTime));
                }
                else
                {
                    sd.body.gameObject.SetActive(false);
                    mPools.Add(sd.body);
                    mBodys.Remove(sd);
                }
            }
        }

        private void CreateGhost()
        {

        }

            
    }
}
