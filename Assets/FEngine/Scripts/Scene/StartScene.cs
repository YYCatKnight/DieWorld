//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;

namespace F2DEngine
{
    public class StartScene : FSceneTemplate<StartScene>
    {
        public override void Begin(params object[] obj)
        {
            UIManager.instance.ShowWindos(ResConfig.FMENUPLANE);
        }
    }
}
