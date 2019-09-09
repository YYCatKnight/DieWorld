//----------------------------------------------
//  F2DEngine: time: 2018.9  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    //核心驱动类
    public interface FBEngine
    {
        MonoBehaviour GetMonoInstance();//得到实体
        T GetNoPackObject<T>(string fileName) where T : Object;//不打包加载设置
        ZipThreadData ThreadUnZip(string ZipFile, string TargetDirectory, string Password,LoadPercent fpd = null, bool OverWrite = true);//解压Zip
        IEnumerator LoadManager();//加载管理器
        Timer_Mix PreLoadObject(string id, bool isLoad);//预加载操作
        string GetEditorPath(string fileName);//得到编辑路径
        void SettingGM(GM.GMFunAction action);//设置gm
        void LoadFrontScene(GameProgress gp);
        FESetting GetFESetting();//配置
        FNetMsgCore GetNetMsgCore();//协议核心
        BundleManager.BundleAskCall GetBundleAskCall();//资源更新询问回调
    }
}
