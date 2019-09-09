//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FCAnimator : UnitObject
    {
        public const string OVERKEY = "OVERKEY";
        public RuntimeAnimatorController nAIController;
        public bool mIsCallOverEvent = false;
        private Action<FAnimatorData> mCallEvent;
        private  Dictionary<string, AnimationClip> mCloneClips = new Dictionary<string, AnimationClip>(); 
        private  Dictionary<string, FAnimatorData> mAnimatorEvent = new Dictionary<string, FAnimatorData>();
        private FAniEvent mEvent;
        private bool mPlayCross = false;
        private Animator mAni;    
        private void MyAnimatorEvent(string other)
        {
            FAnimatorData dat = null;
            if (mAnimatorEvent.TryGetValue(other, out dat))
            {
                if (mCallEvent != null)
                {
                    mCallEvent(dat);
                }
                dat._SendMsg();
            }
        }
        public void CopyAnimatorController(RuntimeAnimatorController control)
       {
            if (control != null)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController();
                overrideController.runtimeAnimatorController = control;
                if (mAni != null)
                {
                    AnimationClip[] acs = mAni.runtimeAnimatorController.animationClips;
                    for (int i = 0; i < acs.Length; i++)
                    {
                        AnimationClip clip = acs[i];
                        overrideController[clip.name] = clip;
                    }
                }
                else
                {
                    mAni = this.gameObject.AddComponent<Animator>();
                }
                mAni.runtimeAnimatorController = overrideController;
            }
        }

        public Animator GetAnimator()
        {
            return mAni;
        }

        //新的注册事件方法
        public void RegEvent(string anmation,float len,System.Action<FAnimatorData> callBack)
        {
            _AddAnimationEvent(anmation,len,len.ToString(),callBack);
        }

        public void AddEventName(string anmation,float pre,string key="")
        {
            _AddAnimationEvent(anmation, pre, key);
        }
        //5.5新方法
        public RuntimeAnimatorController GetEffectiveController(Animator animator)
        {
            RuntimeAnimatorController controller = animator.runtimeAnimatorController;

            AnimatorOverrideController overrideController = controller as AnimatorOverrideController;
            while (overrideController != null)
            {
                controller = overrideController.runtimeAnimatorController;
                overrideController = controller as AnimatorOverrideController;
            }

            return controller;
        }

        //同一帧里添加动画事件，然后播放此动画,会有播放不了动画的bug,解决方案,延迟一帧播放
        private void _AddAnimationEvent(string anmation,float pre,string key,System.Action<FAnimatorData> callBack = null)
        {
            string anmationKey = anmation + "_@_" + key;
            if (!mAnimatorEvent.ContainsKey(anmationKey))
            {
                AnimationClip[] acs = mAni.runtimeAnimatorController.animationClips;
                if (acs != null)
                {
                    for (int i = 0; i < acs.Length; i++)
                    {
                        AnimationClip clip = acs[i];
                        string aName = clip.name;

                        anmationKey = anmation == "" ? aName + "_@_" + key : anmationKey;
                        if (anmation == aName || (anmation == ""&&!mAnimatorEvent.ContainsKey(anmationKey)))
                        {
                            //if (!mCloneClips.ContainsKey(aName))
                            {
                                AnimationClip cloneClip = UnityEngine.Object.Instantiate<AnimationClip>(clip);
                                AnimationEvent ae = new AnimationEvent();
                                ae.time = pre * cloneClip.length;
                                ae.functionName = FAniEvent.EVENTNAME;
                                ae.stringParameter = anmationKey;
                                cloneClip.AddEvent(ae);
                                cloneClip.name = aName;
                                AnimatorOverrideController overrideController = new AnimatorOverrideController();
                                overrideController.runtimeAnimatorController = GetEffectiveController(mAni);
                                overrideController[aName] = cloneClip;
                                mAni.runtimeAnimatorController = overrideController;
                                //mCloneClips[aName] = cloneClip;
                                clip = cloneClip;
                            }

                            FAnimatorData fd = new FAnimatorData(key,aName,pre,callBack);
                            mAnimatorEvent[anmationKey] = fd;
                            if (anmation != "")
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        void Awake()
        {
            Init();
        }
        public void Init()
        {
            if (mAni == null)
            {
                mAni = this.GetComponent<Animator>();
                mAni = mAni == null ? this.GetComponentInChildren<Animator>() : mAni;
                if (mAni == null&& nAIController == null)
                {
                    Debug.LogError("所在的物体和子物体下没有找到Animator动画");
                    return;
                }
                mEvent = mAni.gameObject.AddComponent<FAniEvent>();
                mEvent.CallEvent = MyAnimatorEvent;
                CopyAnimatorController(nAIController);
                if (mIsCallOverEvent)
                {
                    _AddAnimationEvent("",1,OVERKEY);
                }
            }
        }
        public void RegEvent(Action<FAnimatorData> call)
        {
            mCallEvent = call;
        }
        public void SetSpeed(float speed)
        {
            mAni.speed = speed;
        }
        public AnimationClip GetAnimation(string name)
        {
            AnimationClip[] acs = mAni.runtimeAnimatorController.animationClips;
            for (int i = 0; i < acs.Length; i++)
            {
                if (acs[i].name == name)
                {
                    return acs[i];
                }
            }
            return null;
        }
        private AnimatorClipInfo _GetCrossAnimation()
        {
            AnimatorClipInfo[] aci = mAni.GetNextAnimatorClipInfo(0);
            if (aci != null && aci.Length >= 1)
            {
                return aci[0];
            }
            else
            {
                aci = mAni.GetCurrentAnimatorClipInfo(0);
                if (aci != null && aci.Length >= 1)
                {
                    return aci[0];
                }
            }
            return new AnimatorClipInfo();
        }
        private AnimatorClipInfo _GetCurAnimation()
        {
            AnimatorClipInfo[] aci = mAni.GetCurrentAnimatorClipInfo(0);
            if(aci != null&&aci.Length >= 1)
            {
                return aci[0];
            }
            else
            {
                aci = mAni.GetNextAnimatorClipInfo(0);
                if (aci != null && aci.Length >= 1)
                {
                    return aci[0];
                }
            }
            return new AnimatorClipInfo();
        }

        public AnimatorStateInfo GetCurStateInfo()
        {
            AnimatorStateInfo info = mAni.GetCurrentAnimatorStateInfo(0);
            return info;
        }
        public AnimatorClipInfo GetAnimatoionClip()
        {
            AnimatorClipInfo ac = mPlayCross ? _GetCrossAnimation() : _GetCurAnimation();
            return ac;
        }
        public void SetKey(string keyName,int value)
        {
            mPlayCross = false;
            mAni.SetInteger(keyName, value);
        }
        public string GetCurAnimationName()
        {
            AnimatorClipInfo ac = GetAnimatoionClip();
            return ac.clip != null? ac.clip.name :"";
        }
        public void Play(string name,float pre = 0)
        {
            mPlayCross = false;
            mAni.Play(name,-1, pre);
        }
        public void PlayCrossFade(string name,float crosstime = 0.2f)
        {
            mPlayCross = true;
            mAni.CrossFadeInFixedTime(name, crosstime, -1);
        }
        public int GetKey(string key)
        {
            return mAni.GetInteger(key);
        }

    }
    public class FAnimatorData
    {
        public string Key { get; protected set; }
        public string Name { get; protected set; }
        public float Percent { get; protected set; }
        private Action<FAnimatorData> mCallBack;
        public FAnimatorData(string key,string animation,float pre, Action<FAnimatorData> fun)
        {
            Key = key;
            Name = animation;
            Percent = pre;
            mCallBack = fun;
        }
        public void _SendMsg()
        {
            if(mCallBack != null)
            {
                mCallBack(this);
            }
        }
    }

}
