using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class Clipboard
{

#if UNITY_IPHONE && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void setIosClipboard(string content);

    [DllImport("__Internal")]
    private static extern void saveToKeychain(string content);

    [DllImport("__Internal")]
    private static extern string getFromKeychain();
#endif
    public static void SetIosClipboard(string content)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        setIosClipboard(content);
#endif
    }

    public static void SaveToKeychain(string account)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        saveToKeychain(account);
#endif
    }

    public static string GetFromKeychain()
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        return getFromKeychain();
#endif
        return "";
    }

}