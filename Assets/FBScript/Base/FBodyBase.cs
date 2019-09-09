//----------------------------------------------
//  F2DEngine: time: 2017.2  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
namespace F2DEngine
{
    public class FBodyBase : FTKObject
    {
        public virtual void Init()
        {
            
        }
        public virtual void SetColor(Color c)
        {

        }

        public virtual string GetAnimationName(int lay = 0)
        {
            return "";
        }
        public virtual int GetAnimationKey(string key)
        {
            return 0;
        }
        public virtual void PlayAnimationKey(string key, int value)
        {

        }

        public  void Play(string keyName, float fps = 1, bool isReset = false,bool isLoop =  true,int lay = 0)
        {
            if(fps >= 0)
            {
                SetSpeed(fps);
            }
            if (isReset || GetAnimationName() != keyName)
            {
                PlayAnimation(keyName,isLoop,lay);
            }
        }

        public virtual void PlayUpdate(float dp)
        {

        }

        public virtual void PlayAnimation(string keyName,bool isLoop,int lay)
        {

        }


        public virtual void SetSpeed(float fps)
        {

        }

        public virtual void AddEventName(string anmation, float len, string key="")
        {

        }

        public virtual void  RegEvent(Action<FAnimatorData> callBack)
        {

        }
    }
}
