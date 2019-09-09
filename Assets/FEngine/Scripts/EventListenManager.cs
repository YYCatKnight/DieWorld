using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
public class EventListenManager
{
    public static void Send(string key, params object[] objs)
    {
        EventManager.instance.SendDelay(key, objs);
    }
}
