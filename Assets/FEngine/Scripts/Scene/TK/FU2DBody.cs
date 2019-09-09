//----------------------------------------------
//  F2DEngine: time: 2017.2  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
namespace F2DEngine
{
    public class FU2DBody : FBodyBase
    {
        public SpriteRenderer nBody;
        public FAnimator nBodyAnimatror;
        public override void Init()
        {
            nBodyAnimatror.Init();
        }
        public override void SetColor(Color c)
        {
            nBody.color = c;
        }

        public override string GetAnimationName(int lay)
        {
            return nBodyAnimatror.GetCurAnimationName();
        }
        public override int GetAnimationKey(string key)
        {
            return nBodyAnimatror.GetKey(key);
        }
        public override void PlayAnimationKey(string key, int value)
        {
            nBodyAnimatror.SetKey(key, value);
        }

        public override void PlayAnimation(string keyName, bool isLoop, int lay)
        {
            nBodyAnimatror.Play(keyName);
        }
        public override void SetSpeed(float fps)
        {
            nBodyAnimatror.SetSpeed(fps);
        }


        public override void AddEventName(string anmation, float len, string key)
        {
            nBodyAnimatror.AddEventName(anmation, len, key);
        }

        public override void RegEvent(Action<FAnimatorData> callBack)
        {
            nBodyAnimatror.RegEvent(callBack);
        }

    }
}
