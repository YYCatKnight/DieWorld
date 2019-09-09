using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class FCLoadingPlane : BasePlane
    {
        private static bool mIsFirst = true;
        private static LoadMode mCurLoadMode;
        private static LoadMode mLastLoadMode;
        public LoadPercent mLoadPercent;
        internal static FCLoadingPlane StartLoad(GameProgress gp, LoadMode ld)
        {
            return (FCLoadingPlane)FEngineManager.ShowWindos(ResConfig.FLOADINGPLANE, WinShowType.WT_NoClose,ld);
        }
        public override void Init(params object[] args)
        {
            mLoadPercent = new LoadPercent();
            mLastLoadMode = mCurLoadMode;
            mCurLoadMode = (LoadMode)args[0];
            EventManager.instance.Send(LoadSceneManager.LoadEvent,LoadSceneManager.LoadType.LT_Begin, mCurLoadMode);
            mCurLoadMode.PlayLoad(LoadSceneManager.LoadType.LT_Begin);
            BaseLoad bl = null;
            string planeTypeName = mCurLoadMode.PlaneName;
            planeTypeName = string.IsNullOrEmpty(planeTypeName)?ResConfig.FNORMALLOAD: planeTypeName;
            bl = FEngineManager.PoolObject<BaseLoad>(planeTypeName, this.gameObject);
            bl.GetComponent<RectTransform>().NormalRectTransform();
            bl.IsPool = bl.InitOpen(this, mCurLoadMode);
            StartCoroutine(PlayFun(mCurLoadMode.ToSceneName, bl));
        }


        IEnumerator PlayFun(GameProgress gp, BaseLoad bl)
        {
            ScriptsTime.BeginTag("_loadScene");
            mLoadPercent.GoOn(0, "");
            if (bl != null)
            {
                yield return bl.PlayStart();
            }
            else
            {
                yield return 0;
            }

            FEngineManager.CloseWindos();

            bool isPack = FEConfigure.mIsNoPack;
            if (mIsFirst)
            {
                mIsFirst = false;
                if (isPack)
                {
                    mLoadPercent.GoOn(0.3f);
                    BundleManager._isLoadComplete = true;
                }
                else
                {
                    ScriptsTime.BeginTag("_LoadAssetBundles");
                    //进场景第一次加载资源初始化
                    mLoadPercent.SetKD("Detection_Resources");
                    mLoadPercent.Reset("");
                    yield return BundleManager.instance.CopyStreamingAsset(mLoadPercent.CreateBranch(1), FEngineManager.GetFBEngine().GetBundleAskCall());
                    mLoadPercent.SetKD("Loading_Resources");
                    mLoadPercent.Reset("");
                    yield return BundleManager.instance.LoadAssetBundles("", mLoadPercent.CreateBranch(0.95f));
                    ScriptsTime._ShowTimeTag("_LoadAssetBundles", "加载资源消耗");
                }
                ScriptsTime.BeginTag("_LoadManager");
                yield return FEngineManager.GetFBEngine().LoadManager();
                ScriptsTime._ShowTimeTag("_LoadManager", "加载单件管理器");
            }


            string relaySceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            //逻辑处理
            EventManager.instance.Send(LoadSceneManager.LoadEvent, LoadSceneManager.LoadType.LT_BeginMask, mCurLoadMode);
            if (bl != null)
            {
                yield return bl.PlayResoureOver();
            }

            ScriptsTime.BeginTag("_UnLoadSysBundles");
            mLoadPercent.SetKD("LoadingScene");
            mLoadPercent.GoOn(0.7f);
            LoadSceneManager.instance.LoadDirectScene(GameProgress.GP_NONE);

            if (!isPack)
            {
                FEngineManager.GetFBEngine().PreLoadObject(relaySceneName, false);
            }

            //开启异步加载销毁
            // lua gc
            FEngineManager.GetFBEngine().LoadFrontScene(gp);

            ScriptsTime._ShowTimeTag("_UnLoadSysBundles", "卸载当前场景消耗");
         
            ScriptsTime.BeginTag("_LoadSceneAsy");
            yield return LoadSceneManager.instance.LoadSceneAsy(gp);
            ScriptsTime._ShowTimeTag("_LoadSceneAsy", "切换场景消耗");

            mLoadPercent.SetKD("Preloading");
            mLoadPercent.GoOn(0.9f);
            string newSceneName = LoadSceneManager.instance.GetSceneName(gp);
            if (!isPack)
            {
                Timer_Mix mix = FEngineManager.GetFBEngine().PreLoadObject(newSceneName, true);
                if (mix != null)
                {
                    ScriptsTime.BeginTag("_PreLoadObject");
                    if (!mix.IsOver)
                    {
                        var pre = mLoadPercent.CreateBranch(1, false);
                        pre.SetTimece(mix.MaxNum);
                        int lastNum = -1;
                        while (!mix.IsOver)
                        {
                            if (lastNum != mix.CurNum)
                            {
                                pre.GoOn(mix.CurNum.ToString() + "/" + mix.MaxNum.ToString());
                                lastNum = mix.CurNum;
                            }
                            yield return 0;
                        }
                    }
                    ScriptsTime._ShowTimeTag("_PreLoadObject", "预加载消耗");
                }
            }
            mLoadPercent.Over();
            if (mLastLoadMode != null)
            {
                mLastLoadMode.PlayLoad(LoadSceneManager.LoadType.Lt_Back);
            }
            mCurLoadMode.PlayLoad(LoadSceneManager.LoadType.Lt_EnterMask);
            EventManager.instance.Send(LoadSceneManager.LoadEvent, LoadSceneManager.LoadType.Lt_EnterMask, mCurLoadMode);
            if (bl != null)
            {
                yield return bl.PlayEnd();
                FEngineManager.DeletObject(bl);
            }
            else
            {
                yield return 0;
            }
            yield return 0;
            CloseMySelf(true);
            mCurLoadMode.PlayLoad(LoadSceneManager.LoadType.Lt_Enter);
            EventManager.instance.Send(LoadSceneManager.LoadEvent, LoadSceneManager.LoadType.Lt_Enter, mCurLoadMode);
            ScriptsTime._ShowTimeTag("_loadScene","加载场景"+ newSceneName+"总时间");
        }
    }
}
