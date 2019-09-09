//----------------------------------------------
//  F2DEngine: time: 2017.9  by fucong QQ:353204643
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F2DEngine
{

    public class ScrollEndEvent : FUIObject, IEndDragHandler, IDragHandler//, IBeginDragHandler, IDragHandler
    {
        private ScrollRect mScrollRect;
        private RectTransform mContent;
        private Vector3 mBaseVec;

        public System.Action<PointerEventData> EndBottomEvent;
        public System.Action<PointerEventData> EndTopEvent;
        public System.Action<PointerEventData> DragBottomEvent;
        public System.Action<PointerEventData> DragTopEvent;

        public void Awake()
        {
            mScrollRect = this.GetComponent<ScrollRect>();
            mContent = mScrollRect.content;
            mBaseVec = mContent.localPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (mScrollRect.horizontal)
            {
                float width = mContent.localPosition.x + mScrollRect.rectTransform().sizeDelta.x;
                if (width > mContent.sizeDelta.x && DragBottomEvent != null)
                {
                    DragBottomEvent(eventData);
                }
            }
            else
            {
                float height = mContent.localPosition.y + mScrollRect.rectTransform().sizeDelta.y;
                if (height > mContent.sizeDelta.y && DragBottomEvent != null)
                {
                    DragBottomEvent(eventData);
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(mScrollRect == null)
            {
                return;
            }

            if(mScrollRect.horizontal)
            {
                float width = Mathf.Abs(mContent.localPosition.x) + transform.rectTransform().rect.width;
                if (width > mContent.sizeDelta.x && EndTopEvent != null)
                {
                    EndTopEvent(eventData);
                }

                width = mContent.localPosition.x + mScrollRect.rectTransform().sizeDelta.x;

                if (width > mContent.sizeDelta.x && EndBottomEvent != null)
                {
                    EndBottomEvent(eventData);
                }
            }
            else
            {
                float height = Mathf.Abs(mContent.localPosition.y) + transform.rectTransform().rect.height;
                if (height > mContent.sizeDelta.y && EndTopEvent != null)
                {
                    EndTopEvent(eventData);
                }
                height = mContent.localPosition.y + mScrollRect.transform.rectTransform().sizeDelta.y;
                if (height > mContent.sizeDelta.y && EndBottomEvent != null)
                {
                    EndBottomEvent(eventData);
                }
            }
        }
    }
}
