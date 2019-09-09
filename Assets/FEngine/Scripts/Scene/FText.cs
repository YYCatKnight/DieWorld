//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace F2DEngine
{
    public class FText : FUIObject
    {
        private static int mLuangeTimes = 0;
        private int mTimes = -1;
        public string key = "";
        public Text mText; 
        void OnEnable()
        {
            if(key != ""&& mTimes != mLuangeTimes)
            {
                mTimes = mLuangeTimes;
                if(mText == null)
                {
                    mText = this.GetComponent<Text>();
                }
                if(mText != null)
                {
                    mText.text = key;
                }
            }
        }

        public void SetKey(string k)
        {
            key = k;
            mTimes = -1;
            OnEnable();
        }
        public static void RestText()
        {
            mLuangeTimes++;
        }
    }
}
