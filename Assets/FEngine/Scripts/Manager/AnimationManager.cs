//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public enum AniType
    {
        AT_NONE = 0,
        AT_STAND,
        AT_WALK,
        AT_UP,
        AT_DOWN,
        AT_PLAY

    }

    public class AnimationData
    {
        public AniType nAniType;
        public string nKeyName;
        public bool nIsLoop;

        public AnimationData(AniType aniType, string keyName, bool isLoop)
        {
            nAniType = aniType;
            nKeyName = keyName;
            nIsLoop = isLoop;
        }

    }

    public class AnimationManager : ManagerTemplate<AnimationManager>
    {
       
        private Dictionary<AniType, AnimationData> mAniDataList = new Dictionary<AniType, AnimationData>();

        private void AddAniData(AnimationData ad)
        {
            mAniDataList[ad.nAniType] = ad;
        }

        public AnimationData getAniName(AniType at)
        {
            if (mAniDataList.Count == 0)
                Init();
            return mAniDataList[at];
        }

        public class AnimationLibraryData
        {
            public AnimationLibraryData(int npc, string path)
            {
                nNpcId = npc;
                nPath = path;
            }

            public int nNpcId;
            public string nPath;
        }



        protected override void OnInit()
        {
            AddAniData(new AnimationData(AniType.AT_NONE, "at_none", true));
            AddAniData(new AnimationData(AniType.AT_STAND, "at_stand", true));
            AddAniData(new AnimationData(AniType.AT_WALK, "at_walk", true));
            AddAniData(new AnimationData(AniType.AT_UP, "at_up", true));
            AddAniData(new AnimationData(AniType.AT_DOWN, "at_down", true));
            AddAniData(new AnimationData(AniType.AT_PLAY, "at_play", true));

        }
    }
}