//----------------------------------------------
//  F2DEngine: time: 2018.3  by fucong QQ:353204643
//----------------------------------------------
using System.Collections.Generic;
using System.Diagnostics;

namespace F2DEngine
{
    public class ScriptsTime
    {
        private static Dictionary<string,long> mWatchTimes = new Dictionary<string, long>();
        private const string _SHOWCOLORBASE_TIME = "<color=#226600>[ScriptsTime:({0})]:{1}</color>";
        private const string _SHOWCOLORBASE_DEBUG = "<color=#226600>[Debug]:{0}</color>";
        private const string SHOWTIMEENDDEC_TIME = "<color=#AAAA00>[ScriptsTime:({0})]:{1}</color>";
        private const string SHOWTIMEENDDEC_DEBUG = "<color=#AAAA00>[Debug]:{0}</color>";
        private const string DefaultStr = "ScriptsTime_Time";
        private static bool  mDebug = true;
        private static Stopwatch mStopwatch = null;

        public static long GetTimes()
        {
            if(mStopwatch == null)
            {
                mStopwatch = new Stopwatch();
                mStopwatch.Start();
            }
            return mStopwatch.ElapsedMilliseconds;
        }
        public static void Begin()
        {
            _Begin(DefaultStr);
        }

        public static void ShutDebug()
        {
            mDebug = false;
        }

        public static void ShowTime(string dec="")
        {
            _ShowTime(dec, DefaultStr, SHOWTIMEENDDEC_TIME);
        }

        public static void BeginTag(string tag)
        {
            _Begin(tag);
        }

        public static void ShowTimeTag(string tag, string dec = "")
        {
            _ShowTime("TAG(" + tag + "):" + dec, tag, SHOWTIMEENDDEC_TIME);
        }

        public static void Debug(string str)
        {
            UnityEngine.Debug.Log(string.Format(SHOWTIMEENDDEC_DEBUG, str));
        }

        internal static void _ShowTimeTag(string tag, string dec = "")
        {
            if (mDebug)
            {
                _ShowTime("TAG(" + tag + "):" + dec, tag, _SHOWCOLORBASE_TIME);
            }
        }

        internal static void _Debug(string str)
        {
            if(mDebug)
            {
                UnityEngine.Debug.Log(string.Format(_SHOWCOLORBASE_DEBUG,str));
            }
        }

        private static string FormatEnd(string time,string dec,string all)
        {
            return string.Format(all, time,dec);
        }

        private static void _ShowTime(string dec,string tag,string show)
        {
            long times = 0;
            long curTime = GetTimes();
            if(mWatchTimes.TryGetValue(tag, out times))
            {
                double time = (curTime - times) / 1000.0f;
                UnityEngine.Debug.Log(FormatEnd(time.ToString(), dec, show));
            }
        }

        private static void _Begin(string tage)
        {
            mWatchTimes[tage] = GetTimes();
        }
    }
}
