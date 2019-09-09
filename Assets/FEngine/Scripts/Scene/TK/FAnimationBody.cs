//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
namespace F2DEngine
{
    public class FAnimationBody : FTKObject
    {

        public enum FAnimationType
        {
            FAT_NONE = 0,
            FAT_U2D,
            FAT_SPINE,
        }

        public FBodyBase nRealBody;
        public Action<FAnimationBody> nFinishEvent;
        private float mCurRot = 0;

        public void Init()
        {
            nRealBody.Init();
        }
        public void SetColor(Color c)
        {
            nRealBody.SetColor(c);
        }


        public string GetAnimationName()
        {
            return nRealBody.GetAnimationName();
        }
        public int GetAnimationKey(string key)
        {
            return nRealBody.GetAnimationKey(key);
        }
        public void PlayAnimationKey(string key,int value)
        {
            nRealBody.PlayAnimationKey(key, value);
        }

        public void Play(AniType at, bool isReset = false)
        {
            AnimationData ad = AnimationManager.instance.getAniName(at);
            Play(ad.nKeyName);
        }


        public void Play(string keyName, float fps = 1, bool isReset = false,bool isLoop = true,int lay = 0)
        {
            nRealBody.Play(keyName, fps, isReset,isLoop,lay);
            
        }

        public void AddEventName(string anmation, float len, string key)
        {
            nRealBody.AddEventName(anmation, len, key);
        }

        public void RegEvent(Action<FAnimatorData> callBack)
        {
            nRealBody.RegEvent(callBack);
        }

        public void PlayUpdate(float dp)
        {
            nRealBody.PlayUpdate(dp);
        }

        public void Play(Vector2 v2, bool isUpAni = true)
        {
            if (isUpAni)
            {
                if (v2.y > 0.0005f)
                {
                    Play(AniType.AT_UP);
                }
                else if (v2.y < -0.0005f)
                {
                    Play(AniType.AT_DOWN);
                }
                else if (v2.x > 0.0005f || v2.x < -0.0005f)
                {
                    Play(AniType.AT_WALK);
                }
                else
                {
                    Play(AniType.AT_STAND);
                }
            }

            if (v2.x > 0.0005f)
            {
                mCurRot = 0;
            }
            else if (v2.x < -0.0005f)
            {
                mCurRot = 180;
            }
            if (this.transform.localEulerAngles.y != mCurRot)
            {
                this.transform.localEulerAngles = new Vector3(0, mCurRot, 0);
            }
        }
    }
}
