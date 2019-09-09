using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using F2DEngine;

public class ThroughUI :UnitObject, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [FRenameAttr("是否全穿透")]
    public bool IsAll = false;
    //监听按下
    public void OnPointerDown(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerDownHandler);
    }

    //监听抬起
    public void OnPointerUp(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerUpHandler);
    }

    //监听点击
    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.submitHandler);
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }


    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function)
        where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        GameObject current = data.pointerCurrentRaycast.gameObject;
        for (int i = 0; i < results.Count; i++)
        {
            var tG = results[i].gameObject;
            if (current != tG && tG != this.gameObject)
            {
                ExecuteEvents.Execute(tG, data, function);
                if (!IsAll)
                {
                    break;
                }
            }
        }
    }
}
