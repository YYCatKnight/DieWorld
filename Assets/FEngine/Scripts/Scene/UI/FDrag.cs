
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class FDrag : MonoBehaviour,IPointerDownHandler, IDragHandler, IPointerUpHandler,IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action<PointerEventData> CallEvent;
    public Func<PointerEventData,DragType,bool> DragEvent;
    private Vector2 mDownPos;
    private bool mIsDrag = false;
    private bool mIsCanDrag = false;
    private bool mIsFirst = true;

    public enum DragType
    {
        DT_First,
        DT_Normal,
        DT_End
    }

    public bool GetIsDrag()
    {
        return mIsDrag;
    }
    //当鼠标按下时调用 接口对应  IPointerDownHandler
    public void OnPointerDown(PointerEventData eventData)
    {
        mIsDrag = false;
        mIsCanDrag = true;
        mIsFirst = true;
    }

    //当鼠标拖动时调用   对应接口 IDragHandler
    public void OnDrag(PointerEventData eventData)
    {
        if (mIsCanDrag)
        {
            mIsCanDrag = SentDrag(eventData, mIsFirst?DragType.DT_First:DragType.DT_Normal);
            mIsFirst = false;
            mIsDrag = mIsCanDrag;
            if(!mIsCanDrag)
            {
                SentDrag(eventData, DragType.DT_End);
            }
        }
    }

    //当鼠标抬起时调用  对应接口  IPointerUpHandler
    public void OnPointerUp(PointerEventData eventData)
    {
        if (mIsDrag != mIsCanDrag)
        {
            if (CallEvent != null)
            {
                CallEvent(eventData);
            }
        }
        else if(mIsCanDrag)
        {
            mIsDrag = false;
            SentDrag(eventData, DragType.DT_End);
        }
        mIsDrag = false;
    }


    private bool SentDrag(PointerEventData ped, DragType IsDrg)
    {
        if(DragEvent != null)
        {
           return DragEvent(ped, IsDrg);
        }
        return false;
    }


    //当鼠标结束拖动时调用   对应接口  IEndDragHandler
    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    //当鼠标进入图片时调用   对应接口   IPointerEnterHandler
    public void OnPointerEnter(PointerEventData eventData)
    {
       
    }

    //当鼠标退出图片时调用   对应接口   IPointerExitHandler
    public void OnPointerExit(PointerEventData eventData)
    {
      
    }
}
