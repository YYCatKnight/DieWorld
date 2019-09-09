using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStaticMgr
{
    private static List<IDataStatic> m_dataList = new List<IDataStatic>();

    public static void SetPlayerInfo()
    {
        //foreach(var e in m_dataList)
        //{
        //    e.SetPlayerInfo()
        //}
    }

    public static void OnEnterBase()
    {
        foreach (var e in m_dataList)
        {
            e.OnEnterBaseScene(DataStaticConst.EnterBase);
        }
    }

    public static void StartGame()
    {
        foreach(var e in m_dataList)
        {
            e.StartGameSuccess(DataStaticConst.StartGame);
        }
    }

    public static void DownloadResSuccess()
    {
        foreach(var e in m_dataList)
        {
            e.DownloadResSuccess(DataStaticConst.DownloadRes);
        }
    }

    public static void LoginGame()
    {
        foreach(var e in m_dataList)
        {
            e.LoginGame(DataStaticConst.LoginGame);
        }
    }
}
