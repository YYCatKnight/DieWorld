//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public class FLoadingPlane : FCLoadingPlane
    {
        public override void Init(params object[] args)
        {
            if(this.transform.childCount >=2)
            {
                var child = this.transform.GetChild(1);
                if (child != null)
                {
                    if (child.gameObject.name == ResConfig.FFRISTLOAD)
                    {
                        SceneManager.instance.Remove(child.gameObject);
                    }
                }
            }
            base.Init(args);
        }
    }
}
