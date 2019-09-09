//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FBaseController<T> : UnitObject where T : FBaseController<T>
    {

        public static T instance;

        void Awake()
        {
            instance = (T)this;
            Init();
        }

        protected virtual void Init()
        {

        }

        private void OnDestroy()
        {
            End();
            instance = null;
        }

        public virtual void End()
        {

        }
    }
}