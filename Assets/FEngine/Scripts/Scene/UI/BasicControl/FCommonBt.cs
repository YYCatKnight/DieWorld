//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace F2DEngine
{

    public enum FButtonType 
    {
        BT_NORMAL = 0,
        BT_CLIP,
    }
    public class FCommonBt : FUIObject
    {
        public FButtonType nBtType = FButtonType.BT_NORMAL;
        public Button nBt;
        public Text nText;
        public FText nFText;
        private Color mTextColor;
        public Action<FCommonBt> nBtEvent;
        private Dictionary<FButtonType, UnityEngine.Events.UnityAction> mCallEvent = new Dictionary<FButtonType, UnityEngine.Events.UnityAction>();
        [HideInInspector]
        [SerializeField]
        public object nCallData;
        #region CLIP按钮 
        public Image nClipImage; 
        public enum ClipType
        {
            CT_NORMAL,
            CT_DONCLICK,
            CT_CDOVER
        }
        [HideInInspector]
        [SerializeField]
        public ClipType mClipType = ClipType.CT_NORMAL;

        [HideInInspector]
        [SerializeField]
        public float mNextCD = 0;
        private bool isCanClick = true;
        private bool isMastRest = false;
        private float mCDTime = 2.0f;
        #endregion

        private FPressBt mPressBt;
        public void SetPress(float time,bool circle,Action<double,bool> CallBack)
        {
            mPressBt = SceneManager.instance.AddComponent<FPressBt>(this.gameObject);
            mPressBt.SetCycle(circle, time);
            mPressBt.CallAction = CallBack;
        }
 

#if UNITY_EDITOR
        private void Reset()
        {
            if(nBt == null)
            {
                nBt = this.GetComponent<Button>();
            }
            if(nText == null)
            {
                nText = this.GetComponentInChildren<Text>();
            }
        }
#endif
        void Start()
        {
            mCallEvent[FButtonType.BT_NORMAL] = ClickBTEvent;
            mCallEvent[FButtonType.BT_CLIP] = ClickClipEvnent;
            mTextColor = nText != null ? nText.color : Color.white;
            nBt.onClick.AddListener(mCallEvent[nBtType]);
        }

        public void SetTextColor(Color c)
        {
            if(nText != null)
            {
                nText.color = c;
            }
        }
        public void SetIsActive(bool isActive)
        {
            nBt.interactable = isActive;
            Color a = isActive?mTextColor:Color.grey;
            if(mPressBt != null)
            {
                mPressBt.SetIsPress(isActive);
            }
            SetTextColor(a);
        }
        public void setSpriteName(string name)
        {
            nBt.image.SwitchSprite(name);
        }

        public void setName(string Name)
        {
            nText.text = Name;
        }

        public void SetKeyName(string keyName)
        {
            nFText.SetKey(keyName);
        }
        public void ClickClipEvnent()
        {
            mNextCD = mCDTime;
            if (nBtEvent != null)
            {
                mClipType = isCanClick ? ClipType.CT_NORMAL : ClipType.CT_DONCLICK;
                SendBtMessage(mClipType);
                if (isCanClick && mNextCD != 0)
                {
                    StartCoroutine(PlayFun(mNextCD));
                }
            }
        }

        IEnumerator PlayFun(float cd)
        {
            isMastRest = false;
            isCanClick = false;
            nClipImage.fillAmount = 1;
            yield return 0;
            while (nClipImage.fillAmount > 0.01f)
            {
                nClipImage.fillAmount -= 1 / cd * Time.deltaTime;
                yield return 0;
                if (isMastRest)
                {
                    break;
                }
            }
            isMastRest = false;
            isCanClick = true;
            nClipImage.fillAmount = 0;
            SendBtMessage(ClipType.CT_CDOVER);
        }

        private void SendBtMessage(ClipType ct = ClipType.CT_NORMAL)
        {
            //if (mPressBt != null && mPressBt.IsSend)
                //return;
            if (nBtEvent != null)
            {
                mClipType = ct;
                nBtEvent(this);
            }
        }
        //普通按钮事件
        public void ClickBTEvent()
        {
            SendBtMessage();
        }

        //默认点击
        public void AutoClick()
        {
            ExecuteEvents.Execute<IPointerClickHandler>(nBt.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
            ExecuteEvents.Execute<ISubmitHandler>(nBt.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }
}
