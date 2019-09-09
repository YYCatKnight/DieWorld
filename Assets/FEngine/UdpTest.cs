using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using F2DEngine;

public class FoxUdp : FNetHead
{
    public string key;
    public string json;

    public int GetCore()
    {
        return 0;
    }

    public int GetMaxSize()
    {
        return 1;
    }
}

public class UdpTest
{ 
    public static void SentMsg(string key, FNetHead json)
   {
#if UNITY_EDITOR
        if (FEngine.instance.UDPOPEN)
        {
            FoxUdp fu = new FoxUdp();
            fu.key = key;
            fu.json = StringSerialize.Serialize(json);
            Send(fu);
        }
#endif
    }
    private static bool mIsInit = false;
    private static void Send(FoxUdp udp)
    {
        if(!mIsInit)
        {
            mIsInit = true;
        }
    }
}
