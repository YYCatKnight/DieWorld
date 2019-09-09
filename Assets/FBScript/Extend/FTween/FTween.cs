//----------------------------------------------
//  F2DEngine: time: 2018.8  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F2DEngine
{
    #region 通用接口
    public static class FTween
    {
        private static FTweenTool CreateFTween(FTweenEvent data, Component mo)
        {
            var d = TimerController.SetTimer(data);
            d.SetBody(mo);
            return d.mControl;
        }

        public static FTweenTool FOCustom(this GameObject go,Action<float,CustomFTween> callBack)
        {
            return  CreateFTween(new CustomFTween(go,callBack),go.transform);
        }

        public static FTweenTool FOMove(this Transform obj, Vector3 ep,bool IsRelative = false)
        {
            var tranTween = new FTransformTween(ep, obj, FTransformTween.FTranTweenType.TT_Move, IsRelative, FTransformTween.FTranXYZType.F_XYZ);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOMoveX(this Transform obj,float x, bool IsRelative = false)
        {
            var vec = obj.transform.localPosition;
            vec.x = x;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Move, IsRelative, FTransformTween.FTranXYZType.F_X);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOMoveY(this Transform obj, float y, bool IsRelative = false)
        {
            var vec = obj.transform.localPosition;
            vec.y = y;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Move, IsRelative, FTransformTween.FTranXYZType.F_Y);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOMoveZ(this Transform obj, float z, bool IsRelative = false)
        {
            var vec = obj.transform.localPosition;
            vec.z= z;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Move, IsRelative, FTransformTween.FTranXYZType.F_Z);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FORotate(this Transform obj, Vector3 ep, bool IsRelative = false)
        {
            var tranTween = new FTransformTween(ep, obj, FTransformTween.FTranTweenType.TT_Rot, IsRelative, FTransformTween.FTranXYZType.F_XYZ);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FORotateX(this Transform obj, float x, bool IsRelative = false)
        {
            var vec = obj.transform.localEulerAngles;
            vec.x = x;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Rot, IsRelative, FTransformTween.FTranXYZType.F_X);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FORotateY(this Transform obj, float y, bool IsRelative = false)
        {
            var vec = obj.transform.localEulerAngles;
            vec.y = y;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Rot, IsRelative, FTransformTween.FTranXYZType.F_Y);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FORotateZ(this Transform obj, float z, bool IsRelative = false)
        {
            var vec = obj.transform.localEulerAngles;
            vec.z = z;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Rot, IsRelative, FTransformTween.FTranXYZType.F_Z);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOScale(this Transform obj, Vector3 ep, bool IsRelative = false)
        {
            var tranTween = new FTransformTween(ep, obj, FTransformTween.FTranTweenType.TT_Scale, IsRelative, FTransformTween.FTranXYZType.F_XYZ);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOScaleX(this Transform obj, float x, bool IsRelative = false)
        {
            Vector3 vec = obj.transform.localScale;
            vec.x = x;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Scale, IsRelative, FTransformTween.FTranXYZType.F_X);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOScaleY(this Transform obj, float y, bool IsRelative = false)
        {
            Vector3 vec = obj.transform.localScale;
            vec.y = y;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Scale, IsRelative, FTransformTween.FTranXYZType.F_Y);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOScaleZ(this Transform obj, float z, bool IsRelative = false)
        {
            Vector3 vec = obj.transform.localScale;
            vec.z = z;
            var tranTween = new FTransformTween(vec, obj, FTransformTween.FTranTweenType.TT_Scale, IsRelative, FTransformTween.FTranXYZType.F_Z);
            var tween = CreateFTween(tranTween, obj);
            return tween;
        }

        public static FTweenTool FOColor(this SpriteRenderer obj, Color a,Color b)
        {
            var tween = CreateFTween(new FSpriteFadeTween(obj, a,b), obj);
            return tween;
        }

        public static FTweenTool FOColor(this MaskableGraphic obj, Color a, Color b)
        {
            var tween = CreateFTween(new FGraphicFadeTween(obj, a, b), obj);
            return tween;
        }

        public static FTweenTool FOFade(this SpriteRenderer obj, float a, float b)
        {
            var tween = CreateFTween(new FSpriteFadeTween(obj, a, b), obj);
            return tween;
        }

        public static FTweenTool FOFade(this MaskableGraphic obj, float a, float b)
        {
            var tween = CreateFTween(new FGraphicFadeTween(obj, a, b), obj);
            return tween;
        }
 

    }
    #endregion

    #region 基础结构
    public enum FTweenMode
    {
        TM_Onece = 0,//一次
        TM_Loop,//循环
        TM_YoYo,//往返
    }
    public enum FEaseMode
    {
        PM_Line = 0,//线性
        PM_EaseInQuad,
        PM_EaseOutQuad,
        PM_EaseInOutQuad,
        PM_EaseInCubic,
        PM_EaseInOutCubic,
        PM_EaseInQuart,
        PM_EaseOutQuart,
        PM_EaseInOutQuart,
        PM_EaseInQuint,
        PM_EaseOutQuint,
        PM_EaseInOutQuint,
        PM_EaseInSine,
        PM_EaseOutSine,
        PM_EaseInOutSine,
        PM_EaseInExpo,
        PM_EaseOutExpo,
        PM_EaseInOutExpo,
        PM_InCirc,
        PM_EaseOutCirc,
        PM_EaseInBounce,
        PM_EaseOutBounce,
        PM_EaseInOutBounce,
        PM_EaseInBack,
        PM_EaseOutBack,
        PM_EaseInOutBack,
        PM_EaseInElastic,
        PM_EaseOutElastic,
        PM_EaseInOutElastic,
    }
    public enum TweenTag
    {
         Tag_Child = 1 << 1,//包含子物体
    }
    public class FTweenTool
    {
        public FTweenEvent TweenNode { get; protected set; }

        public FTweenTool(FTweenEvent data)
        {
            TweenNode = data;
        }

        public FTweenTool SetDuration(float dur)
        {
            TweenNode.mDuration = dur;
            return this;
        }

        public FTweenTool SetMode(FTweenMode mode,int timece=-1)
        {
            TweenNode.mTweenMode = mode;
            TweenNode.mLoopTimes = timece;
            return this;
        }

        public FTweenTool SetEaseMode(Func<float, float> mode)
        {
            TweenNode.mCustonEaseModeFun = mode;
            return this;
        }

        public FTweenTool SetEaseMode(FEaseMode mode)
        {
            TweenNode.mEaseMode = mode;
            TweenNode.mCustonEaseModeFun = null;
            return this;
        }

        public FTweenTool SetDelay(float st,float et = 0)
        {
            TweenNode.mStartDelayTime = st;
            TweenNode.mTurnDelayTime = et;

            return this;
        }

        public FTweenTool SetOnStart(Action<int> callback)
        {
            TweenNode.mOnStart = callback;
            return this;
        }

        public FTweenTool SetOnComplete(Action<int> callback)
        {
            TweenNode.mOnComplete = callback;
            return this;
        }

        public FTweenTool SetTag(TweenTag tag)
        {
            TweenNode.mTag |= (int)tag;
            return this;
        }

        public FTweenTool SetOnUpdate(Action<float> callback)
        {
            TweenNode.mOnUpdate = callback;
            return this;
        }

        public void Stop()
        {
            TweenNode.StopTimer();
        }

        public void SetPause(bool isPause)
        {
            TweenNode.PauseTimer(isPause);
        }

        public void Reset()
        {
            TweenNode.ResetTimer();           
        }

        public float GetProgress()
        {
            return TweenNode.GetProgress();
        }

    }
    public class FTweenEvent : TimerEvent
    {
        internal float mDuration = 1;
        internal FTweenMode mTweenMode = FTweenMode.TM_Onece;
        internal FEaseMode mEaseMode = FEaseMode.PM_Line;
        internal Action<int> mOnStart;
        internal Action<int> mOnComplete;
        internal Action<float> mOnUpdate;
        internal int mTag = 0;
        internal FTweenTool mControl;
        internal float mStartDelayTime = 0;
        internal float mTurnDelayTime = 0;
        internal Func<float, float> mCustonEaseModeFun;
        protected float mCurDelayTime = 0;
        protected float mProgress = 0;
        protected float mDir = 1;
        protected float mSpeed;
        protected int mTimes = -1;
        protected bool mIsInit = false;
        protected Action<int> mActionModeFun;
        internal FTweenState mState = FTweenState.TS_Begin;
        public Func<float,float> mEaseModeFun;
        internal int mLoopTimes = -1;
        internal enum FTweenState
        {
            TS_Begin,
            TS_Init,
            TS_Delay,
            TS_Noraml,//正常
            TS_Delete,//待删除
        }
        public FTweenEvent()
        {
            mControl = new FTweenTool(this);
        }
        public float GetProgress()
        {
            return mProgress;
        }
        private void _ResetTween()
        {   
            mSpeed = 1.0f / mDuration;
            mDir = 1;
            mTimes = -1;
            mProgress = 0;
            SetNextDelayTime(mStartDelayTime);
            if (mTweenMode == FTweenMode.TM_Onece)
            {
                mActionModeFun = ActMode_Onece;
            }
            else if (mTweenMode == FTweenMode.TM_Loop)
            {
                mActionModeFun = ActMode_Loop;
            }
            else if (mTweenMode == FTweenMode.TM_YoYo)
            {
                mActionModeFun = ActMode_YoYo;
            }
            if (mCustonEaseModeFun != null)
            {
                mEaseModeFun = mCustonEaseModeFun;
            }
            else
            {
                switch (mEaseMode)
                {
                    case FEaseMode.PM_Line:
                        mEaseModeFun = PathMode_Line;
                        break;
                    case FEaseMode.PM_EaseInQuad:
                        mEaseModeFun = PathMode_EaseInQuad;
                        break;
                    case FEaseMode.PM_EaseOutQuad:
                        mEaseModeFun = PathMode_EaseOutQuad;
                        break;
                    case FEaseMode.PM_EaseInOutQuad:
                        mEaseModeFun = PathMode_EaseInOutQuad;
                        break;
                    case FEaseMode.PM_EaseInCubic:
                        mEaseModeFun = PathMode_EaseInCubic;
                        break;
                    case FEaseMode.PM_EaseInOutCubic:
                        mEaseModeFun = PathMode_EaseInOutCubic;
                        break;
                    case FEaseMode.PM_EaseInQuart:
                        mEaseModeFun = PathMode_EaseInQuart;
                        break;
                    case FEaseMode.PM_EaseOutQuart:
                        mEaseModeFun = PathMode_EaseOutQuart;
                        break;
                    case FEaseMode.PM_EaseInOutQuart:
                        mEaseModeFun = PathMode_EaseInOutQuart;
                        break;
                    case FEaseMode.PM_EaseInQuint:
                        mEaseModeFun = PathMode_EaseInQuint;
                        break;
                    case FEaseMode.PM_EaseOutQuint:
                        mEaseModeFun = PathMode_EaseOutQuint;
                        break;
                    case FEaseMode.PM_EaseInOutQuint:
                        mEaseModeFun = PathMode_EaseInOutQuint;
                        break;
                    case FEaseMode.PM_EaseInSine:
                        mEaseModeFun = PathMode_EaseInSine;
                        break;
                    case FEaseMode.PM_EaseOutSine:
                        mEaseModeFun = PathMode_EaseOutSine;
                        break;
                    case FEaseMode.PM_EaseInOutSine:
                        mEaseModeFun = PathMode_EaseInOutSine;
                        break;
                    case FEaseMode.PM_EaseInExpo:
                        mEaseModeFun = PathMode_EaseInExpo;
                        break;
                    case FEaseMode.PM_EaseOutExpo:
                        mEaseModeFun = PathMode_EaseInExpo;
                        break;
                    case FEaseMode.PM_EaseInOutExpo:
                        mEaseModeFun = PathMode_EaseInOutExpo;
                        break;
                    case FEaseMode.PM_InCirc:
                        mEaseModeFun = PathMode_InCirc;
                        break;
                    case FEaseMode.PM_EaseOutCirc:
                        mEaseModeFun = PathMode_EaseOutCirc;
                        break;
                    case FEaseMode.PM_EaseInBounce:
                        mEaseModeFun = PathMode_EaseInBounce;
                        break;
                    case FEaseMode.PM_EaseOutBounce:
                        mEaseModeFun = PathMode_EaseOutBounce;
                        break;
                    case FEaseMode.PM_EaseInOutBounce:
                        mEaseModeFun = PathMode_EaseInOutBounce;
                        break;
                    case FEaseMode.PM_EaseInBack:
                        mEaseModeFun = PathMode_EaseInBack;
                        break;
                    case FEaseMode.PM_EaseOutBack:
                        mEaseModeFun = PathMode_EaseOutBack;
                        break;
                    case FEaseMode.PM_EaseInOutBack:
                        mEaseModeFun = PathMode_EaseInOutBack;
                        break;
                    case FEaseMode.PM_EaseInElastic:
                        mEaseModeFun = PathMode_EaseInElastic;
                        break;
                    case FEaseMode.PM_EaseOutElastic:
                        mEaseModeFun = PathMode_EaseOutElastic;
                        break;
                    case FEaseMode.PM_EaseInOutElastic:
                        mEaseModeFun = PathMode_EaseInOutElastic;
                        break;
                }
            }
           
        }
        private void SetNextDelayTime(float time)
        {
            if(time > 0)
            {
                mCurDelayTime = time;
            }
            else if(time < 0)
            {
                mProgress += time * mDir * mSpeed * -1;
                mCurDelayTime = 0;
            }
            else
            {
                mCurDelayTime = 0;
            }
            mState = mCurDelayTime > 0 ? FTweenState.TS_Delay : FTweenState.TS_Noraml;
        }
        //运动模式
        private void ActMode_Onece(int timece)
        {
            mProgress = 1;
            if (mTurnDelayTime > 0.0001f && timece % 2 == 1)
            {
                SetNextDelayTime(mTurnDelayTime);
            }
            else
            {
                mState = FTweenState.TS_Delete;
            }
        }
        private void ActMode_Loop(int timece)
        {
            if (IsOver(timece))
            {
                return;
            }

            if (mTurnDelayTime > 0.0001f && timece % 2 == 1)
            {
                SetNextDelayTime(mTurnDelayTime);
                mProgress = 1;
            }
            else
            {
                mProgress -= (int)mProgress;
                SetNextDelayTime(mStartDelayTime);
            }
        }
        private bool IsOver(int timece)
        {
            if(mLoopTimes> 0 &&mLoopTimes <= timece)
            {
                mState = FTweenState.TS_Delete;
                return true;
            }
            return false;
        }
        private void ActMode_YoYo(int timece)
        {
            if(IsOver(timece))
            {
                return;
            }

            if (mProgress >= 1)
            {
                mDir = -1;
                mProgress = (1 - mProgress + ((int)mProgress));
                SetNextDelayTime(mTurnDelayTime);
            }
            else
            {
                mDir = 1;
                mProgress *= -1;
                mProgress -= (int)mProgress;
                SetNextDelayTime(mStartDelayTime);
            }
        }
        #region Ease路径
        private static float PathMode_Line(float value)
        {
            return value;
        }
        private static float PathMode_EaseInQuad(float value)
        {
            return value * value;
        }
        private static float PathMode_EaseOutQuad(float value)
        {
            return - value * (value - 2);
        }
        private static float PathMode_EaseInOutQuad(float value)
        {
            return value < 0.5 ? 2 * value * value : -1 + (4 - 2 * value) * value;
        }
        private static float PathMode_EaseInCubic(float value)
        {
            return value * value * value;
        }
        private static float PathMode_EaseOutCubic(float value)
        {
            value--;
            return (value * value * value + 1);
        }
        private static float PathMode_EaseInOutCubic(float value)
        {
            return value < 0.5 ? 4 * value * value * value : (value - 1) * (2 * value - 2) * (2 * value - 2) + 1;

        }
        private static float PathMode_EaseInQuart(float value)
        {
            return value * value * value * value;
        }
        private static float PathMode_EaseOutQuart(float value)
        {
            value--;
            return 1 - value * value * value * value;
        }
        private static float PathMode_EaseInOutQuart(float value)
        {
            return value < 0.5 ? 8 * value * value * value * value : 1 - 8 * (--value) * value * value * value;
        }
        private static float PathMode_EaseInQuint(float value)
        {
            return value * value * value * value * value;
        }
        private static float PathMode_EaseOutQuint(float value)
        {
            value--;
            return value * value * value * value * value + 1;
        }
        private static float PathMode_EaseInOutQuint(float value)
        {
            return value < 0.5 ? 16 * value * value * value * value * value : 1 + 16 * (--value) * value * value * value * value;
        }
        private static float PathMode_EaseInSine(float value)
        {
            return  1 - Mathf.Cos(value * (Mathf.PI * 0.5f));
        }
        private static float PathMode_EaseOutSine(float value)
        {
            return Mathf.Sin(value * (Mathf.PI * 0.5f));
        }
        private static float PathMode_EaseInOutSine(float value)
        {
            return - 0.5f * (Mathf.Cos(Mathf.PI * value) - 1);
        }
        private static float PathMode_EaseInExpo(float value)
        {
            return  Mathf.Pow(2, 10 * (value - 1));
        }
        private static float PathMode_EaseOutExpo(float value)
        {
            return 1-Mathf.Pow(2, -10 * value);
        }
        private static float PathMode_EaseInOutExpo(float value)
        {
            value /= .5f;
            if (value < 1) return 0.5f * Mathf.Pow(2, 10 * (value - 1));
            value--;
            return 0.5f * (-Mathf.Pow(2, -10 * value) + 2);
        }
        private static float PathMode_InCirc(float value)
        {
            return - (Mathf.Sqrt(1 - value * value) - 1);
        }
        private static float PathMode_EaseOutCirc(float value)
        {
            value--;
            return Mathf.Sqrt(1 - value * value);
        }
        private static float PathMode_EaseInOutCirc(float value)
        {
            value /= .5f;
            return -0.5f * (Mathf.Sqrt(1 - value * value) - 1);
        }
        private static float PathMode_EaseInBounce(float value)
        {
            return 1 - PathMode_EaseOutBounce(1 - value);
        }
        private static float PathMode_EaseOutBounce(float value)
        {
            value /= 1f;
            if (value < (1 / 2.75f))
            {
                return (7.5625f * value * value);
            }
            else if (value < (2 / 2.75f))
            {
                value -= (1.5f / 2.75f);
                return  (7.5625f * (value) * value + .75f);
            }
            else if (value < (2.5 / 2.75))
            {
                value -= (2.25f / 2.75f);
                return (7.5625f * (value) * value + .9375f);
            }
            else
            {
                value -= (2.625f / 2.75f);
                return (7.5625f * (value) * value + .984375f);
            }
        }
        private static float PathMode_EaseInOutBounce(float value)
        {
            if (value <  0.5f) return PathMode_EaseInBounce(value * 2) * 0.5f;
            else return PathMode_EaseOutBounce(value * 2-1) * 0.5f + 0.5f;
        }
        private static float PathMode_EaseInBack(float value)
        {
            float s = 1.70158f;
            return (value) * value * ((s + 1) * value - s);
        }
        private static float PathMode_EaseOutBack(float value)
        {
            float s = 1.70158f;
            value = (value) - 1;
            return ((value) * value * ((s + 1) * value + s) + 1);
        }
        private static float PathMode_EaseInOutBack(float value)
        {
            float s = 1.70158f;
            value /= .5f;
            if ((value) < 1)
            {
                s *= (1.525f);
                return 0.5f * (value * value * (((s) + 1) * value - s));
            }
            value -= 2;
            s *= (1.525f);
            return 0.5f * ((value) * value * (((s) + 1) * value + s) + 2);
        }
        private static float PathMode_EaseInElastic(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return 0;

            if ((value /= d) == 1) return 1;

            if (a == 0f || a < 1)
            {
                a = 1;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
            }

            return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
        }
        private static float PathMode_EaseOutElastic(float value)
        {
            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return 0;

            if ((value /= d) == 1) return 1;

            if (a == 0f || a < 1)
            {
                a = 1;
                s = p * 0.25f;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
            }

            return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + 1);
        }
        private static float PathMode_EaseInOutElastic(float value)
        {

            float d = 1f;
            float p = d * .3f;
            float s = 0;
            float a = 0;

            if (value == 0) return 0;

            if ((value /= d * 0.5f) == 2) return 1;

            if (a == 0f || a < 1)
            {
                a = 1;
                s = p / 4;
            }
            else
            {
                s = p / (2 * Mathf.PI) * Mathf.Asin(1 / a);
            }

            if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p));
            return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + 1;
        }
        #endregion
        private void SendMssage(int timece,bool isEnd)
        {
            if(mLoopTimes > 0&&timece != 0&&timece < mLoopTimes)
            {
                return;
            }
            if(isEnd)
            {
                if (mOnComplete != null)
                {
                    mOnComplete(timece);
                }
            }
            else
            {
                if (mOnStart != null)
                {
                    mOnStart(timece);
                }
            }
        }
        protected virtual void PlayReset()
        {

        }
        public override float PlayLogic(float timeDp)
        {
            if (mState == FTweenState.TS_Noraml)
            {
                mProgress += timeDp * mDir * mSpeed;
                if (mProgress < 0 || mProgress > 1)
                {
                    bool isEnd = mProgress >= 1;
                    mActionModeFun(mTimes + 1);
                    PlayUpdate(mEaseModeFun(Mathf.Clamp(mProgress,0.0f,1.0f)));
                    SendMssage(++mTimes, isEnd);
                }
                else
                {
                    PlayUpdate(mEaseModeFun(mProgress));
                }
                return 0.0f;
            }
            else if(mState == FTweenState.TS_Delay)
            {
                if (mCurDelayTime > 0)
                {
                    mCurDelayTime -= timeDp;
                    if (mCurDelayTime > 0)
                    {
                        return 0.0f;
                    }
                    else
                    {
                        timeDp = -mCurDelayTime;
                    }
                }
                mState = mIsInit?FTweenState.TS_Noraml:FTweenState.TS_Init;
                return PlayLogic(timeDp);
            }
            else if(mState == FTweenState.TS_Init)
            {
                _ResetOther();
                mState = FTweenState.TS_Noraml;
                return PlayLogic(timeDp);
            }
            else if(mState == FTweenState.TS_Begin)
            {
                _ResetTween();//重置数据
                if(mState != FTweenState.TS_Delay)
                {
                    _ResetOther();
                }
                SendMssage(++mTimes, false);
                return PlayLogic(timeDp);
            }
            else if (mState == FTweenState.TS_Delete)
            {
                return -1.0f;
            }
            return -1.0f;
        }
        public virtual void PlayFrame(float t)
        {

        }
        private void PlayUpdate(float t)
        {
            PlayFrame(t);
            if(mOnUpdate != null)
            {
                mOnUpdate(t);
            }
        }
        protected virtual void PlayInit()
        {

        }
        public sealed override void ResetData()
        {          
            base.ResetData();
            _ResetTween();//重置数据
            _ResetOther();
            SendMssage(++mTimes, false);
        }
        private void _ResetOther()
        {
            if (!mIsInit)
            {
                PlayInit();
                mIsInit = true;
            }
            PlayReset();
            PlayUpdate(mEaseModeFun(0));
        }
        public override bool ResetTimer()
        {
            if (state == EventState.ES_Stop)
            {
               return base.ResetTimer();
            }
            else
            {
                ResetData();
            }
            return true;
        }
    }
    //混合器
    public class FTweenMix
    {
        public bool IsFrist { get { return mIsFirst; } }
        public bool IsOver  { get { return mIsOver; } }
        private List<FTweenTool> mTweens = new List<FTweenTool>();
        private int mIndex = -1;
        private int mTimes = 0;
        private int mMaxTimes = -1;
        private int mDirIndex = 1;
        private Action<int> mOnComplete;
        private FTweenMode mMode = FTweenMode.TM_Onece;
        public bool mIsFirst = true;
        public bool mIsOver = false;
        public FTweenMix Append(FTweenTool tween)
        {
            InitTween(tween);
            mTweens.Add(tween);
            if (mIsOver)
            {
                mIndex = mTweens.Count -2;
                mIsOver = false;
                _Play();
            }
            return this;
        }
        public FTweenMix SetMode(FTweenMode mode, int timece = -1)
        {
            mMode = mode;
            mTimes = 0;
            mMaxTimes = timece;
            return this;
        }
        public FTweenMix SetOnComplete(Action<int> callback)
        {
            mOnComplete = callback;
            return this;
        }
        private bool IsTimerOver()
        {
            return mMaxTimes > 0 && mTimes >= mMaxTimes;
        }
        private void InitTween(FTweenTool t)
        {
            t.SetMode(mMode);
            t.TweenNode.mOnStart += (ti) =>
            {
                if (ti != 0)
                {
                    t.SetPause(true);
                    _Play();
                }
            };
            t.TweenNode.mOnComplete += (ti) =>
            {
                if (mMode != FTweenMode.TM_YoYo)
                {
                    if (t.TweenNode.mTurnDelayTime > 0.005)
                    {
                        if (ti % 2 == 1)
                        {
                            return;
                        }
                    }
                    else if (t.TweenNode.mTurnDelayTime > 0.001)
                    {
                        if (ti % 2 != 1)
                        {
                            return;
                        }
                    }
                }
                t.SetPause(true);
                _Play();
            };
            t.SetPause(true);
        }
        private void Init()
        {
            mIndex = -1;
            mDirIndex = 1;
            mTimes = 0;
            for (int i = 0; i < mTweens.Count; i++)
            {
                var t = mTweens[i];
                t.SetPause(i != 0);
            }
            _Play();
            mIsFirst = false;
        }
        private void _Play()
        {
            mIndex += mDirIndex;
            if (mTweens.Count > mIndex&&mIndex >= 0)
            {
                var t = mTweens[mIndex];
                t.SetPause(false);
            }
            else
            {
                mTimes++;                
                if(mIndex >= mTweens.Count)
                {
                    if(mMode == FTweenMode.TM_YoYo)
                    {
                        mDirIndex = -1;
                        mIndex = mTweens.Count;
                    }
                    else
                    {
                        mIndex = -1;
                    }
                }
                else if(mIndex < 0)
                {
                    mDirIndex = 1;
                    mIndex = -1;
                }

                if(mMaxTimes <= 0||IsTimerOver())
                {
                    if (mOnComplete != null)
                    {                       
                        mOnComplete(mTimes - 1);                       
                    }
                    if(mMode == FTweenMode.TM_Onece||mMaxTimes > 0)
                    {
                        Stop();
                        return;
                    }
                }

                _Play();
            }
        }
        public void Play()
        {
            if (!mIsFirst)
            {
                Reset();
            }
            else
            {
                if (mTweens.Count == 0)
                    return;              
                Init();                
            }
        }
        public void Stop()
        {
            mIsOver = true;
            for (int i = 0; i < mTweens.Count;i++)
            {
                mTweens[i].Stop();
            }
        }
        public void SetPause(bool isPause)
        {
            mTweens[mIndex].SetPause(isPause);
        }
        public void Reset()
        {
            mIsOver = false;
            for (int i = mTweens.Count-1; i >= 0; i--)
            {
                mTweens[i].Reset();
            }
            Timer_Frequency.SetTimer((f) =>
            {
                Init();
            }, 3);
        }
    }
    #endregion

    #region  扩展接口
    //自定义万能事件
    public class CustomFTween:FTweenEvent
    {
        private Action<float,CustomFTween> mCallBackFrame;
        public GameObject Ins { get; protected set; }

        public CustomFTween(GameObject go,Action<float,CustomFTween> callBack)
        {
            Ins = go;
            mCallBackFrame = callBack;
        }

        public override void PlayFrame(float t)
        {
            mCallBackFrame(t,this);
        }
    }
    internal class FTransformTween : FTweenEvent
    {
        protected Transform mTran;
        protected Vector3 mValue;
        protected Vector3 mEPValue;
        protected Vector3 mStartValue;
        private Action<float, FTransformTween> mCallBack;
        private FTranTweenType mType;
        private FTranXYZType mXYZ;
        private bool mIsRelative = false;
        internal enum FTranTweenType
        {
            TT_Move,
            TT_Rot,
            TT_Scale
        }
        internal enum FTranXYZType
        {
            F_X,
            F_Y,
            F_Z,
            F_XYZ,
        }
        public FTransformTween(Vector3 ep,Transform tra, FTranTweenType type,bool isRel, FTranXYZType xyz)
        {
            mTran = tra;
            mValue = ep;
            mType = type;
            mIsRelative = isRel;
            mXYZ = xyz;
        }

        private Vector3 _ComputerXYZ(Vector3 vec)
        {
            switch(mXYZ)
            {
                case FTranXYZType.F_X:
                    vec.y = 0;
                    vec.z = 0;
                    break;
                case FTranXYZType.F_Y:
                    vec.x = 0;
                    vec.z = 0;
                    break;
                case FTranXYZType.F_Z:
                    vec.x = 0;
                    vec.y = 0;
                    break;
                default:
                    break;
            }
            return vec;
        }

        protected override void PlayInit()
        {
           switch(mType)
            {
                case FTranTweenType.TT_Move:
                    mStartValue = mTran.localPosition;
                    mCallBack = MoveFun;
                    break;
                case FTranTweenType.TT_Rot:
                    mStartValue = mTran.localEulerAngles;
                    mCallBack = EulerFun;
                    break;
                case FTranTweenType.TT_Scale:
                    mStartValue = mTran.localScale;
                    mCallBack = ScaleFun;
                    break;
                default:break;
            }

            if (mIsRelative)
            {
                mEPValue = mValue;
                _ComputerXYZ(mEPValue);
            }
            else
            {
                mEPValue = mValue - mStartValue;
                _ComputerXYZ(mEPValue);
                if(mType == FTranTweenType.TT_Rot)
                {
                    mEPValue.x = _NormalAnagle(mEPValue.x);
                    mEPValue.y = _NormalAnagle(mEPValue.y);
                    mEPValue.z = _NormalAnagle(mEPValue.z);
                }
            }
        }

        private float _NormalAnagle(float x)
        {
            if(x >= -360&&x<=360)
            {
                if (x >= 180)
                {
                    x -= 360;
                }
                else if (x <= -180)
                {
                    x += 360;
                }
            }         
            return x;
        }

        public override void PlayFrame(float t)
        {
            mCallBack(t, this);
        }

        private static void MoveFun(float t, FTransformTween tra)
        {
            tra.mTran.localPosition = tra.mStartValue + tra.mEPValue * t;         
        }

        private static void EulerFun(float t, FTransformTween tra)
        {
            tra.mTran.localEulerAngles = tra.mStartValue + tra.mEPValue * t;
        }

        private static void ScaleFun(float t, FTransformTween tra)
        {
            tra.mTran.localScale = tra.mStartValue + tra.mEPValue * t;
        }
    }
    internal class FSpriteFadeTween : FTweenEvent
    {
        private SpriteRenderer mSprite;
        private SpriteRenderer[] mChilds;
        private Color mSC;
        private Color mEC;
        private float mSA;
        private float mEA;
        private bool mIsA = false;
        public FSpriteFadeTween(SpriteRenderer sprite, Color sc, Color ec)
        {
            mSC = sc;
            mEC = ec;
            mIsA = false;
            mSprite = sprite;
        }

        public FSpriteFadeTween(SpriteRenderer sprite, float sa, float ea)
        {
            mSA = sa;
            mEA = ea;
            mIsA = true;
            mSprite = sprite;
        }

        protected override void PlayInit()
        {
            if (FUniversalFunction.IsContainSameType(mTag, (int)TweenTag.Tag_Child))
            {
                mChilds = mSprite.gameObject.GetComponentsInChildren<SpriteRenderer>();
            }
        }

        public override void PlayFrame(float t)
        {

            if (mIsA)
            {
                Color c = mSprite.color;
                c.a = mSA + (mEA - mSA) * t;
                mSprite.color = c;
                if (mChilds != null)
                {
                    for (int i = 0; i < mChilds.Length; i++)
                    {
                        var tc = mChilds[i].color;
                        tc.a = c.a;
                        mChilds[i].color = tc;
                    }
                }
            }
            else
            {
                mSprite.color = mSC + (mEC - mSC) * t;
                if (mChilds != null)
                {
                    for (int i = 0; i < mChilds.Length; i++)
                    {
                        mChilds[i].color = mSprite.color;
                    }
                }
            }
        }
    }
    internal class FGraphicFadeTween : FTweenEvent
    {
        private MaskableGraphic mGraphic;
        private MaskableGraphic[] mChilds;
        private Color mSC;
        private Color mEC;
        private float mSA;
        private float mEA;
        private bool mIsA = false;
        public FGraphicFadeTween(MaskableGraphic sprite,float sa, float ea)
        {
            mSA = sa;
            mEA = ea;
            mIsA = true;
            mGraphic = sprite;
        }
        public FGraphicFadeTween(MaskableGraphic sprite, Color sc, Color ec)
        {
            mSC = sc;
            mEC = ec;
            mIsA = false;
            mGraphic = sprite;
        }
        protected override void PlayInit()
        {
            if (FUniversalFunction.IsContainSameType(mTag, (int)TweenTag.Tag_Child))
            {
                mChilds = mGraphic.gameObject.GetComponentsInChildren<MaskableGraphic>();
            }
        }
        public override void PlayFrame(float t)
        {
            if (mIsA)
            {
                Color c = mGraphic.color;
                c.a = mSA + (mEA - mSA) * t;
                mGraphic.color = c;
                if (mChilds != null)
                {
                    for (int i = 0; i < mChilds.Length; i++)
                    {
                        var tc = mChilds[i].color;
                        tc.a = c.a;
                        mChilds[i].color = tc;
                    }
                }
            }
            else
            {
                mGraphic.color = mSC + (mEC - mSC) * t;
                if (mChilds != null)
                {
                    for (int i = 0; i < mChilds.Length; i++)
                    {
                        mChilds[i].color = mGraphic.color;
                    }
                }
            }
        }
    }
    #endregion
}
