//----------------------------------------------
//  F2DEngine: time: 2016.3  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace F2DEngine
{

 
    public class TouchController : FBaseController<TouchController>
    {
        public enum TouchType
        {
            TT_NONE,
            TT_KEY,
            TT_MOVE,
        }

        public class TouchData
        {
            public TouchType tt;
            public Action<TouchData> nTouchEvent;
            public Vector3 touchPos;
            public int index;
        }

        private Dictionary<TouchType, TouchData> mTouchEvents = new Dictionary<TouchType, TouchData>();
        public List<FCommonBt> nButtons = new List<FCommonBt>();
       
        protected override void Init()
        {
            FCommonFunction.SetList(nButtons, (f, i) => 
            {
                f.nBtEvent = (t) =>
                {
                    if (mTouchEvents.ContainsKey(TouchType.TT_KEY))
                    {
                        TouchData td = mTouchEvents[TouchType.TT_KEY];
                        td.index = i;
                        td.nTouchEvent(td);
                    }
                };
            });
        }

        //脚本拖入
        public void JoyStickControlMove(Vector2 direction)
        {
            TouchData td = mTouchEvents[TouchType.TT_MOVE];
            td.touchPos = direction;
            td.nTouchEvent(td);
        }

        protected void _CallTouchEvent(TouchType tt, Action<TouchData> touchEvent)
        {
            TouchData td = new TouchData();
            td.tt = tt;
            td.nTouchEvent = touchEvent;
            mTouchEvents[tt] = td;
        }

        public static void CallTouchEvent(TouchType tt, Action<TouchData> touchEvent)
        {
            if(TouchController.instance == null)
            {
                GameObject go = SceneManager.instance.Create(ResConfig.CC_JOYCONTROLLER);
                go.GetComponent<RectTransform>().anchorMax = Vector2.one;
                MainCanvas.instance.SetLayer(0, go);              

            }
            TouchController.instance._CallTouchEvent(tt, touchEvent);
        }

        public static void DesytoryTouch()
        {
            if(TouchController.instance != null)
            {
                SceneManager.instance.DeletObject(TouchController.instance);
            }
        }

    }
}
