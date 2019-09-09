//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;

namespace F2DEngine
{
    public class FUIObject : UnitObject
    {
        public void DestroyMySelf()
        {
            Destroy(this.gameObject);
        }
    }
}
