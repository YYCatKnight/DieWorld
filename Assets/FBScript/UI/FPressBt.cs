//----------------------------------------------
//  F2DEngine: time: 2016.2  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace F2DEngine
{
    public class FPressBt:FUIObject,IPointerDownHandler,IPointerUpHandler,IPointerExitHandler
    {
        public Action<double, bool> CallAction;
        private bool mCycle;
        private float mTime;
        private Timer_Logic mTimer;
        public bool IsSend { get; protected set; }
        private bool mIsPress = true;

        public void SetIsPress(bool isPress)
        {
            mIsPress = isPress;
        }
        public void SetCycle(bool cycle, float time)
        {
            mCycle = cycle;
            mTime = time;
        }

        protected void OnDestroy()
        {
            StopTimer();
        }

        private float Press(Timer_Logic timer)
        {
            IsSend = true;
            CallAction(timer.totalTime, true);
            if (!mCycle)
                return -1;
            if (mTime == 0)
                return 0;
            return mTime;
        }

        private void Exit()
        {
            if (mTimer != null)
            {
                CallAction(mTimer.totalTime, false);
                StopTimer();
            }
        }

        private void StopTimer()
        {
            if(mTimer != null)
            {
                mTimer.StopTimer();
                mTimer = null;
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            IsSend = false;
            if(!mIsPress)
            {
               return;
            }

            StopTimer();
            mTimer = Timer_Logic.SetTimer(Press, mTime,this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Exit();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit();
        }
    }
}
