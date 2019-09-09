//----------------------------------------------
//  F2DEngine: time: 2017.3  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class NormalLoad : BaseLoad
    {
        public Image Image;
        public FAnimator mAni;
        //private static int mIndex = -1;
        public override bool Init()
        {
            //int range = Random.Range(2, 4);
            //if(range == mIndex)
            //{
            //    range++;
            //    if(range >3)
            //    {
            //        range = Random.Range(1,3);
            //    }
            //}
            //mIndex = range;
            //if (LoadSceneManager.instance.GameActiveScene == GameProgress.GP_City)
            //{
            //    Image.SwitchSprite("UI2/Texture/Loading_02");
            //}
            //else
            //{
            //    Image.SwitchSprite("UI2/Texture/Loading_02");
            //}
            //Image.SwitchSprite("UI2/Texture/Loading_0" + mIndex.ToString());
            return true;
        }

        public override IEnumerator PlayStart()
        {
          //  SceneManager.instance.PlaySoundByID("16076");
            mAni.Play("Close");
            yield return new WaitForSeconds(0.55f);
            Image.color = Color.white;
            yield return 0;
        }

        public override IEnumerator PlayEnd()
        {
            yield return 0;
        }
    }
}
