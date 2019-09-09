//----------------------------------------------
//  F2DEngine: time: 2018.11  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F2DEngine
{
    public class FlowTips : BasePlane
    {
        private FGroup<FUniversalPanel> mGroups = new FGroup<FUniversalPanel>();
        private Queue<TipsManger.Normal_TipParams> mParmas = new Queue<TipsManger.Normal_TipParams>();
        private Vector3 mStartPos;
        public override void Init(params object[] o)
        {
            var item = mMainPlane.GetFObject<FUniversalPanel>("F_Item");
            mStartPos = item.transform.localPosition;
            mGroups.Init(item);
            openPool(OpenType.OT_None, o);
            StartCoroutine(PlayFun());
        }
        
        private void TipsEnd(int t)
        {
            mGroups.Remove(1,0);
        }

        IEnumerator PlayFun()
        {
            WaitForSeconds waifTime = new WaitForSeconds(0.3f);
            while (mGroups.Count != 0 || mParmas.Count > 0)
            {
                while (mParmas.Count > 0)
                {
                    var tips = mParmas.Dequeue();
                    mGroups.Add(1);
                    var item = mGroups.Last;
                    var text = item.GetFObject<Text>("S_Text");
                    text.text = tips.Context;
                    var back = item.GetFObject<RectTransform>("S_Back");
                    back.sizeDelta = new Vector2(580, text.preferredHeight * 1.5f + (text.preferredHeight < 30 ? 15 : 0));
                    Color startColor = Color.white;
                    startColor.a = 0;
                    back.GetComponent<MaskableGraphic>().FOFade(0, 1).SetDuration(1.0f).SetTag(TweenTag.Tag_Child).SetEaseMode(FEaseMode.PM_EaseOutQuart).Reset();
                    back.transform.localPosition = mStartPos;
                    back.FOMove(mStartPos + new Vector3(0,150, 0)).SetDuration(1.5f).SetOnComplete(TipsEnd).SetEaseMode(FEaseMode.PM_EaseOutQuart);
                    yield return waifTime;
                }
                yield return waifTime;
            }
            CloseMySelf();
        }

        public override void openPool(OpenType type, params object[] o)
        {
            mParmas.Enqueue((TipsManger.Normal_TipParams)o[0]);
        }
    }
}
