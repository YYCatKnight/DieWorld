//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

namespace F2DEngine
{
    public class Tips : BasePlane
    {
        protected static List<TipsManger.Normal_TipParams> tempObject = new List<TipsManger.Normal_TipParams>();
        private GameObject mItem;
        private Queue<Transform> mQueue=new Queue<Transform>();
        private float mContinueTime = 0.2f;//多个tip时，弹出的间隔
        private float mDistanceTime = 2.0f;//每一个tip位移的时长
        private float mFadeTime = 0.4f;//渐隐时长
        public override void Init(params object[] o)
        {
            mItem = mMainPlane.GetFObject("F_Back");
            mItem.SetActive(false);
            openPool(OpenType.OT_Normal, o);
            StartCoroutine(PlayFun());
        }

        public override void openPool(OpenType type, params object[] o)
        {
            tempObject.Add((TipsManger.Normal_TipParams)o[0]);
        }

        IEnumerator PlayFun()
        {
            int index = 0;
            while (true)
            {
                while (tempObject.Count > 0)
                {
                    index++;
                    Transform item;
                    if (mQueue.Count == 0)
                    {
                        item = SceneManager.CloneObject<Transform>(mItem);
                    }
                    else
                    {
                        item = mQueue.Dequeue();
                    }
                    item.SetAsLastSibling();
                    item.gameObject.SetActive(true);
                    Text sText = item.GetComponent<FUniversalPanel>().GetFObject<Text>("S_Text");
                    sText.text = tempObject[0].Context;
                    item.localPosition = Vector3.zero;
                    sText.FOFade(1,0).SetDuration(mFadeTime).SetDelay(mDistanceTime- mFadeTime);
                    item.GetComponent<Image>().FOFade(1,0).SetDuration(mFadeTime).SetDelay(mDistanceTime - mFadeTime);
                    item.FOMove(item.transform.localPosition+new Vector3(0,300,0)).SetDuration(mDistanceTime).SetEaseMode(FEaseMode.PM_EaseInOutSine).SetOnComplete((c) => 
                    {
                        item.gameObject.SetActive(false);
                        sText.color = new Color(1, 1, 1, 1);
                        item.GetComponent<Image>().color = new Color(1,1,1,1);
                        mQueue.Enqueue(item);
                        index--;
                    });                  
                    tempObject.RemoveAt(0);
                    yield return new WaitForSeconds(mContinueTime);

                }

                while(index != 0)
                {
                    yield return 0;
                    if (tempObject.Count != 0)
                    {
                        break;
                    }
                }
                if (tempObject.Count == 0)
                {
                    CloseMySelf();
                    break;
                }
            }
            
        }
        
    }
}
