//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace F2DEngine
{
    public enum FESetting
    {
        FE_NONE = 0,
        FE_Log = 1 << 1,//日志
        FE_HideLog = 1 <<2,//隐形日志
        FE_NoPack = 1 << 3,//不打包模式
        FE_AutoUnload = 1 << 4,//异步释放资源
        FE_Leaker = 1 << 5,//内存监控模式(比较消耗,开启需谨慎)
    }

    //基本配置
    public static class FEConfigure
    {    
        private static FESetting mSetting;
        private static bool _log = false;
        public static bool mIsLog { get { return _log; } }
        private static bool _hidelog = false;
        public static bool mIsHideLog { get { return _hidelog; } }
        private static bool _NoPack = false;
        public static bool mIsNoPack { get { return _NoPack; } }
        private static bool _Unload = false;
        public static bool mIsAutoUnload { get { return _Unload; } }
        private static bool _Leaker = false;
        public static bool mIsLeaker { get { return _Leaker; } }

        public static void SetConfig(FESetting set)
        {
            mSetting = set;
            _UpdateSetting();
        }


        private static void _UpdateSetting()
        {
            _log = IsHaveSameType(mSetting, FESetting.FE_Log);
            _hidelog = IsHaveSameType(mSetting, FESetting.FE_HideLog);
            _NoPack = IsHaveSameType(mSetting, FESetting.FE_NoPack);
            _Unload = IsHaveSameType(mSetting, FESetting.FE_AutoUnload);
        }

        private static bool IsHaveSameType(FESetting main, FESetting use)
        {
            return ((int)(main & use)) != 0;
        }
    }
}
