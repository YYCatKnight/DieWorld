//----------------------------------------------
//  F2DEngine: time: 2016.2  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



namespace F2DEngine
{
    public class LogoScene : FSceneTemplate<LogoScene>
    {
        public GameObject nLog;
        public RawImage[] nBackTexs;
        public FCommonBt[] nLogButtone;
        private List<FMySprite> mFmys = new List<FMySprite>();

        public override void Begin(params object[] obj)
        {
            nLogButtone[0].nBtEvent = ClickD;
            nLog.SetActive(true);
            for (int j = 0; j < nBackTexs.Length; j++)
            {
                nBackTexs[j].gameObject.SetActive(false);
                FMySprite fms = null;
                fms = nBackTexs[j].gameObject.CreateMySprite();
                int higNum = 100;
                if(j == 1)
                {
                    higNum = 60;
                }
                else if(j == 2)
                {
                    higNum = 110;
                }
                fms.CreateClip(50,higNum);
                mFmys.Add(fms);
            }
            StartCoroutine(PlayFun());
        }

        private void ClickD(FCommonBt bt)
        {
            nLogButtone[1].nBtEvent = ClickO;
        }

        private void ClickO(FCommonBt bt)
        {
            MyLog.SetLog();
        }
        IEnumerator PlayFun()
        {
            
            for (int i = 0; i < mFmys.Count; i++)
            {
                mFmys[i].PlayAnimation(new MySprBOMB(4,false));
            }
            yield return new WaitForSeconds(6);
            SceneManager.instance.PlayMusic("Log");
            for (int i = 0; i < mFmys.Count; i++)
            {
                float timeTp = 1;
                if(i == 1)
                {
                    timeTp = 0.3f;
                }
                else if(i == 2)
                {
                    timeTp = 1.5f;
                }
                mFmys[i].PlayAnimation(new MySprFlyX(timeTp,0.8f,true));
                yield return new WaitForSeconds(timeTp);
            }
            SceneManager.instance.StopSound("Log");
            yield return new WaitForSeconds(1.0f);
            LoadSceneManager.instance.LoadScene(GameProgress.GP_Menu);
        }
    }
}
