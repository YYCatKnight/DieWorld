//----------------------------------------------
//  F2DEngine: time: 2016.8  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;

namespace F2DEngine
{
    public class FAniEvent : UnitObject
    {
        public const string EVENTNAME = "_MyAnimatorEvent";
        public Action<string> CallEvent;
        private void _MyAnimatorEvent(string other)
        {
            if (CallEvent != null)
            {
                CallEvent(other);
            }
        }
    }
}
