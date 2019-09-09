using UnityEngine;
using System.Collections;
using System;
using F2DEngine;
using System.Collections.Generic;

namespace F2DEngine
{
    public class GM
    {
        public delegate void GMFunAction(string key, Func<string, string, bool> callEvent, string dec = "");
        private static Dictionary<string, Func<string, string, bool>> mGMAction = new Dictionary<string, Func<string, string, bool>>();
        private static Dictionary<string, string> mGMShow = new Dictionary<string, string>();
        private static string GMKEY = "gm/";
        private static void RegisterGM(string key, Func<string, string, bool> callEvent, string dec = "")
        {
            mGMAction[key.ToLower()] = callEvent;
            mGMShow[key.ToLower()] = dec;
        }

        public enum GMResult
        {
            GM_None,
            GM_Fail,
            GM_Success,
        }
        public static GMResult ResetGM(string key, int maxSize = 99999)
        {
            if (key.Length > 3 && key.Length < maxSize)
            {
                if (key.Substring(0, GMKEY.Length).ToLower() == GMKEY)
                {
                    string tempStr = key.Substring(GMKEY.Length);
                    return Encrypt(tempStr);
                }
            }
            return GMResult.GM_None;
        }

        public static Dictionary<string, string> GetShow()
        {
            return mGMShow;
        }

        private static GMResult Encrypt(string key)
        {
            int index = key.IndexOf("/");
            string action = "";
            string valuestr = "";
            if (index != -1)
            {
                action = key.Substring(0, index).ToLower();
                valuestr = key.Substring(index + 1);
            }
            else
            {
                action = key.ToLower();
            }
            if (mGMAction.ContainsKey(action))
            {
                var act = mGMAction[action];
                if (act != null)
                {
                    if (act(action, valuestr))
                    {
                        return GMResult.GM_Success;
                    }
                }
            }
            return GMResult.GM_Fail;
        }

        //通用GM
        public static void GMPlay()
        {
            if (mGMAction.Count != 0)
                return;

            //////////////////////////////////////////
            RegisterGM("#log", (f, e) =>
            {
                MyLog.SetLog();
                return true;
            }, "显示日志系统");

            FEngineManager.GetFBEngine().SettingGM(RegisterGM);
        }
    }
}
