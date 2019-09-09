//----------------------------------------------
//  F2DEngine: time: 2017.5  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class TestScene : FSceneTemplate<TestScene>
    {
        public string mPlaneName = "";
        private BasePlane mBasePlane;
        public override void Begin(params object[] obj)
        {
            if(!string.IsNullOrEmpty(mPlaneName))
            {
                mBasePlane =  UIManager.instance.ShowWindos(mPlaneName);
            }
        }

        public override void EndScene()
        {
            if (mBasePlane != null)
            {
                UIManager.instance.CloseWindos(mBasePlane);
            }
        }
    }
}
