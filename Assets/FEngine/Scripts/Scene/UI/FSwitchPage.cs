//----------------------------------------------
//  F2DEngine: time: 2017.11  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F2DEngine
{
    public class FSwitchPage : FUIObject
    {
        public FCommonBt nReduce;
        public FCommonBt nPlus;
        public InputField nInput;
        public Text nShowText;
        public Slider nSlider;
        public Text mSoldierNumber;
        public Text mSoldierTotal;
        private int mMin = 1;
        private int mMax = 30;
        public int mCurValue = 1;
        public System.Action<long> CallBack;
        public bool mIsIFChangeValue = true;
        [HideInInspector]
        private int mLastValue = -1;
        private int mSliderType = 0;
        private bool mInit = false;
        private void Start()
        {
            mInit = true;
            if (nSlider != null)
            {
                nSlider.onValueChanged.AddListener((f) =>
                {
                    int select = 0;
                    if (nSlider.wholeNumbers)
                    {
                        select = (int)f;
                    }
                    else
                    {
                        select = (int)((f * (mMax - mMin) + 0.9999f)) + mMin;
                    }

                    if (select != mCurValue && mSliderType == 0)
                    {
                        mSliderType = 1;
                        SetSelect(select);
                        mSliderType = 0;
                    }
                });
            }

            if (nReduce != null)
            {
                nReduce.nBtEvent = (f) =>
                {
                    SetSelect(mCurValue - 1);
                };
            }

            if (nPlus != null)
            {
                nPlus.nBtEvent = (f) =>
                {
                    SetSelect(mCurValue + 1);
                };
            }

            if (nInput != null && mIsIFChangeValue)
            {
                nInput.onValueChanged.AddListener((f) =>
                {
                    int select = 0;
                    if (int.TryParse(f, out select))
                    {
                        if (select != mCurValue)
                        {
                            SetSelect(select);
                        }
                    }
                    else
                    {
                        SetSelect(mCurValue);
                    }
                });
            }

            SetSlier();
            UpdateValue();
        }

        private void SetSlier()
        {
            if (nSlider != null)
            {
                if (mMax - mMin <= -1)
                {
                    nSlider.minValue = mMin;
                    nSlider.maxValue = mMax;
                    nSlider.wholeNumbers = true;
                }
                else
                {
                    nSlider.minValue = 0;
                    nSlider.maxValue = 1;
                    nSlider.wholeNumbers = false;
                }
            }
        }

        public void SetInfo(int min, int max, int selectIndex = -1)
        {
            mMax = Mathf.Max(min,max);
            mMin = min;
            mCurValue = selectIndex < 0 ? mCurValue : selectIndex;

            SetSlier();
            UpdateValue();
        }


        public int GetValue()
        {
            return mCurValue;
        }
        public void SetSelect(int selectIndex,bool callback = false)
        {
            SetInfo(mMin, mMax, selectIndex);
            if(callback)
            {
                _UpdateSlider();
            }
        }

        public void SetMax(int max)
        {
            SetInfo(mMin, max);
        }

        public void SetMin(int min)
        {
            SetInfo(min, mMax);
        }

        private void _UpdateSlider()
        {
            if (nSlider != null)
            {
                if (nSlider.wholeNumbers)
                {
                    nSlider.value = mCurValue;
                }
                else
                {
                    if (mMax == mMin)
                    {
                        if (0 == mMax)
                        {
                            nSlider.value = 0;
                        }
                        else
                        {
                            nSlider.value = 1;
                        }
                    }
                    else
                    {
                        nSlider.value = (mCurValue - mMin) / (float)(mMax - mMin);
                    }
                }
            }
        }
        /// <summary>
        /// 只赋值组件
        /// </summary>
        public void SetComponentValue()
        {
            if (nShowText)
                nShowText.text = mCurValue.ToString() + "/" + mMax.ToString();
            if (mSoldierNumber)
            {
                mSoldierNumber.text = mCurValue.ToString();
            }
            if (mSoldierTotal)
            {
                mSoldierTotal.text = (mMax - mCurValue).ToString ();
            }
        }
        private void UpdateValue()
        {
          
            if (!mInit)
                return;
            mCurValue = Mathf.Clamp(mCurValue, mMin, mMax);
            SetComponentValue();
            if (nInput != null)
            {
                nInput.text = mCurValue.ToString();
            }
            if (nReduce != null)
            {
                nReduce.gameObject.ChangeDrak(mCurValue == mMin);
            }
            if (nPlus != null)
            {
                nPlus.gameObject.ChangeDrak(mCurValue == mMax);
            }
            
            if (nSlider != null)
            {
                if (mSliderType == 0)
                {
                    mSliderType = 1;
                    _UpdateSlider();
                     mSliderType = 0;
                }
            }
            if (CallBack != null)
            {
                if (mLastValue != mCurValue)
                {
                    mLastValue = mCurValue;
                    CallBack(mCurValue);
                }
            }
        }
    }
}
