//----------------------------------------------
//  F2DEngine: time: 2017.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace F2DEngine
{
    public class FTog : FUIObject
    {
        private Toggle mToggle;
        public Action<bool, FTog> CallBack;
        public Text nText;
        public bool SendFalseEvent = false;
        private void Awake()
        {
            mToggle = this.GetComponent<Toggle>();
            mToggle.onValueChanged.AddListener(Click);
        }
        public Toggle GetToggle { get { return mToggle; } }
        public void SetIsCanClick(bool isClick)
        {
            mToggle.interactable = isClick;
        }
        
        public void SetName(string name)
        {
            nText.text = name;
        }

        public void SetTextColor(string color)
        {
            Color nColor;
            ColorUtility.TryParseHtmlString(color,out nColor);
            if (nColor != null)
            {
                nText.color = nColor;
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            if (nText == null)
            {
                nText = this.GetComponentInChildren<Text>();
            }
        }
#endif

        private void Click(bool isTog)
        {
            if (CallBack != null)
            {
                if (SendFalseEvent || isTog)
                {
                    CallBack(isTog, this);
                }
            }
        }

        public void SetIsOn(bool isTog)
        {
            if (mToggle.isOn && isTog)
            {
                Click(true);
            }
            mToggle.isOn = isTog;
        }

        public bool GetIsOn()
        {
            return mToggle.isOn;
        }
    }
}
