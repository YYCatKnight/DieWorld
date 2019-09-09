using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class IPv6SupportMidleware
{
#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);

#endif

    //"192.168.1.1&&ipv4"
    private static string GetIPv6(string ip, string port)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        string mIPv6 = getIPv6(ip, port);
        return mIPv6;
#else
        return ip + "&&ipv4";
#endif
    }

    public static void getIPType(string ip, string port, out string newIP, out AddressFamily ipType)
    {
        ipType = AddressFamily.InterNetwork;
        newIP = ip;
        try
        {
            string mIPv6 = GetIPv6(ip, port);
            if (!string.IsNullOrEmpty(mIPv6))
            {
                string[] m_StrTemp = Regex.Split(mIPv6, "&&");
                if (m_StrTemp != null && m_StrTemp.Length >= 2)
                {
                    string IPType = m_StrTemp[1];
                    if (IPType == "ipv6")
                    {
                        newIP = m_StrTemp[0];
                        ipType = AddressFamily.InterNetworkV6;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("GetIPv6 error:" + e);
        }
    }
}