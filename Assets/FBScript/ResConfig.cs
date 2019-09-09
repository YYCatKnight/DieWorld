using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class ResConfig
    {

        //FZip压缩密码
        public const string FZIPPASS = "1989514";

        //FZip后缀
        public const string FZIPNAMEEX = ".zip";


        public const string FCACHEFILE = "Cache_File";//缓存区文件配置

        //C#服务器配置
        public const int C_HEAD = 0x00F0F0F0;

        //服务器
        //public const string SEREVERCONTROL = "ServerControl";
        // CommonControl

        public const string CC_FENGINE = "CommonControl/FEngine";

        public const string CC_FMERGESPRITE = "CommonControl/FMergeSprite";

        public const string CC_FUNIVERSALPANEL = "CommonControl/FUniversalPanel";
        public const string CC_MUSICAUDION = "CommonControl/MusciObject";

        public const string CC_FPEOPLEAI = "CommonControl/FPeopleAI";
        public const string CC_FBASEAI = "CommonControl/FBaseAI";

        public const string CC_LOG = "CommonControl/Log";

        public const string CC_NORMALTIPS = "CommonControl/NormalTips";

        public const string CC_FLOWTIPS = "CommonControl/FlowTips";
        //网络延迟等待界面
        public const string CC_WAITPLANE = "CommonControl/WaitPlane";

        //JoyControl
        public const string CC_JOYCONTROLLER = "CommonControl/TouchController";

        // TKControl
        public const string TK_FANIMATIONBODY = "TKControl/FAnimationBody_";

        //局域网广播事件
        public const string LOCALPROTOCOLE = "LocalProtocol";
        //网络事件
        public const string PROTOCOLEVENT = "ProtocolEvent_";

        //音乐路径
        public const string MUSIC_PATH = "Music/";

        //asset路径
        public const string ASSET_PAHT = "f_AssetData";

        //Astar*物体路径
        public const string ASTAR_PAHT = "StarMapPrefab";

        //默认材质
        public const string MATERIAL_DEFAULT = "Material/FDefaultMaterial";

        //场景变化材质
        public const string MATERIAL_SCENECOLOR = "Material/FSceneColored";

        //特效材质
        public const string MATERIAL_EFFECT = "Material/FEffectMaterial";

        //变灰材质
        public const string MATERIAL_GREY = "Material/Grey";

        //扭曲材质
        public const string MATERIAL_HEAT = "Material/HeatDistortion";

        //X-Ray材质
        public const string MATERIAL_X_RAY = "Material/X-Ray";

        //白线材质
        public const string MATERIAL_LINE = "Material/DefaultLine";

        //目标UGUI图集
        public const string UGUIFILE = "FAtlas";

        //目标Assetbundle
        public const string ASSETBUNDLE = "Assetbundle";

        //目的Assetbundle文件
        public const string FDESASSETBUNDLE = "FAssetbundle";

        //Assetbundle 文件配置
        public const string ASSETBUNDLECONFIFILE = "MyAssetbundleConfig";

        //Assetbundle 后缀文件
        public const string ASSETBUNDLESUFFIXES = ".unity3d";

        //StreamingAssets配置
        public const string STREAMINGASSETSCONFIG = "FStreamingAssets";

        public const string MicroBundleName = "MicroBundle";

        //UGUI转换后的图集
        //public const string UGUIEND = "FUI";

        //配置保存名称
        public const string CONFIGSETTING = "ConfigSetting";

        //视频播放界面
        public const string FVideoPlayer = "CommonControl/VideoPlayerPlane";

        //Loading界面
        public const string FLOADINGPLANE = "CommonControl/FLoadingPlane";
        public const string FNORMALLOAD = "CommonControl/NormalLoad";
        public const string FFRISTLOAD = "CommonControl/LoginLoad";

        //MainUIScene画布
        public const string FMAINUISCENE = "CommonControl/MainCanvas";

        //MENU设置界面
        public const string FMENUPLANE = "CommonControl/MenuPlane";

        //examples界面
        public const string FEXAMPLESPLANE = "CommonControl/ExamplesPlane";

        //新手引导界面
        public const string FNOVICEPLANE = "CommonControl/NovicePlane";

        //新手引导编辑界面
        public const string FNOVICEEDITORPLANE = "CommonControl/NoviceEditor";

        //聊天表情
        public const string FCHATICON = "CommonControl/ChatIcon";
        //聊天超链接
        public const string FCHATLINK = "CommonControl/ChatLink";

        //EXCEL 文件
        public const string EXCELFILEPATH = "Excel";

        //协议文件
        public const string FPROREADPATH = "FPro";
        public const string FPROOUTPATH = "FCSPro";

        //Arpg 文件
        public const string ARPGEX = "arpg";


        //fcsm 状态机
        public const string FCSM = "fcsm";

        //不打包配置
        public const string NOPACKPATH = "FConfigSetting/NoPackSetting";//不打包配置文件
        public const string PACKCOLORPATH = "FConfigSetting/PackRuleSetting";//打包规则

        //行动树 文件
        public const string BEHAVIORTREE = "btree";

        //nav配置文件
        public const string NAVEMESEX = "fnav";
        //xlua基础配置
        public const string XLUASET = "fxluamain";
        //xlua更新模块
        public const string XLUAPATH = "FXLua";

        //四叉树场景分割
        public const string SEPARATETREEEX = "sptree";

    }
}
