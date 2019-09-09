//----------------------------------------------
//  F2DEngine: time: 2017.3  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class BaseLoad : UnitObject
    {
        protected FCLoadingPlane mMainPlane;
        protected LoadMode mTool;
        public bool  InitOpen(FCLoadingPlane lp, LoadMode tool)
        {
            mMainPlane = lp;
            mTool = tool;
            return Init();
        }
        public virtual  IEnumerator PlayStart()
        {
            yield return 0;
        }

		public virtual IEnumerator PlayResoureOver()
		{
			yield return 0;
		}
        public virtual IEnumerator PlayEnd()
        {
            yield return 0;
        }


        //是否使用缓冲池
        public virtual bool  Init()
        {
            return false;
        }
    }
}
