//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
namespace F2DEngine
{
    public class FCAnimationBody : FTKObject
    {

        public enum FAnimationType
        {
            FAT_NONE = 0,
            FAT_U2D,
            FAT_SPINE,
            FAT_3DMesh,
        }

        public FBodyBase nRealBody;
        public Action<FCAnimationBody> nFinishEvent;
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
    }
}
