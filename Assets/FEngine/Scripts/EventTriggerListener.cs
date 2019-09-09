using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, ISelectHandler, IUpdateSelectedHandler// UnityEngine.EventSystems.EventTrigger
{
    public delegate void VoidDelegate(GameObject go);
    public delegate void ParamsDelegate(GameObject go, params object[] args);
    public delegate void TimeDelegate(GameObject go,float time,bool IsStop);


    public ParamsDelegate onParamsClick;

	public VoidDelegate onClick;
	public VoidDelegate onDown;
	public VoidDelegate onEnter;
	public VoidDelegate onExit;
	public VoidDelegate onUp;
    public VoidDelegate onSelect;
    public VoidDelegate onUpdateSelect;
    private TimeDelegate _onPress;
    public TimeDelegate onPress
    {
        get
        {
            return _onPress;
        }
        set
        {
            if (GetComponent<Button>() == null)
            {
                Debug.LogError("这个事件只支持Button");
            }
            _onPress = value;
        }
    }


    public object param;
    public object[] args;

    public void SetParams(params object[] _args)
    {
        args = _args;
    }

	static public EventTriggerListener Get (GameObject go)
	{
		EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
		if (listener == null) listener = go.AddComponent<EventTriggerListener>();
		return listener;
	}


    public void OnPointerClick(PointerEventData eventData)
	{
		if(onClick != null) 	onClick(gameObject);

        if (onParamsClick != null) onParamsClick(gameObject, args);
	}
	public  void OnPointerDown (PointerEventData eventData){
        StartPress();
		if(onDown != null) onDown(gameObject);
	}
	public  void OnPointerEnter (PointerEventData eventData){
		if(onEnter != null) onEnter(gameObject);
	}
	public  void OnPointerExit (PointerEventData eventData){
		if(onExit != null) onExit(gameObject);
	}
	public  void OnPointerUp (PointerEventData eventData)
	{
	    StopPress();
        if (onUp != null) onUp(gameObject);
	}
	public void OnSelect (BaseEventData eventData){
		if(onSelect != null) onSelect(gameObject);
	}

    /// <summary>
    /// 仅支持button
    /// </summary>
    /// <param name="eventData"></param>
	public  void OnUpdateSelected (BaseEventData eventData){
        OnPress();
		if(onUpdateSelect != null) onUpdateSelect(gameObject);
	}
    
    private bool isOnPress;
    private float pressTime;

    private void StartPress()
    {
        isOnPress = true;
        pressTime = 0;
    }
    /// <summary>
    /// 仅支持button
    /// </summary>
    private void OnPress()
    {
        if (isOnPress)
        {
            pressTime += Time.deltaTime;
            if (onPress != null) onPress(gameObject, pressTime, false);
        }
    }
    private void StopPress()
    {
        isOnPress = false;
        pressTime = 0;
        if (onPress != null) onPress(gameObject, pressTime,true);
    }
}
