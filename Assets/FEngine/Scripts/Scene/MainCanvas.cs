//----------------------------------------------
//  F2DEngine: time: 2016.7  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class MainCanvas : FCMainCanvas
    {
        protected override void Init()
        {
            base.Init();
            //MirrorFlipCamera(GetMianCamera());
            FUniversalPanel main = this.GetComponent<FUniversalPanel>();
            var canvas = main.GetFObject<UnityEngine.UI.CanvasScaler>("F_Canvas");
            canvas.matchWidthOrHeight = 0;
        }

        void MirrorFlipCamera(Camera camera)
        {
            Matrix4x4 mat = camera.projectionMatrix;
            mat *= Matrix4x4.Scale(new Vector3(-1, 1, 1));
            camera.projectionMatrix = mat;
        }
    }
}
