//----------------------------------------------
//  F2DEngine: time: 2016.4  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
namespace F2DEngine
{
    [Obsolete("FCircleItem方法即将已过时，请使用FCScrollRect代替")]
    public class FCircleItem : UnitObject
    {
        public class CircleData
        {
            public GameObject go;
            private Component mComponent;
            public int index;
            public T GetComponent<T>() where T : Component
            {
                if (mComponent == null)
                    mComponent = go.GetComponent<T>();
                return (T)mComponent;
            }
        }
        public bool mIsCircle = true;
        private ScrollRect mScrollRect;
        public int WidthCount = 0;
        public int HightCount = 0;
        public Vector2 mSize;
        public Transform TempObject;
        public Func<int, CircleData, bool> mCallEvent;
        public string ResName;
        private Vector3 mStartPos;
        private Vector3 mEndPos;
        private Vector3 mWorldSzie;
        private bool isVertical = true;
        private Dictionary<int, List<CircleData>> mBuffs = new Dictionary<int, List<CircleData>>();
        private List<int> mIndexs = new List<int>();
        private RectTransform mContent;
        private Vector2 mDatilSize = Vector2.zero;
        private int mIndexPacge = 0;
        private Action<bool> mChangeCallFun;
        private FGroup<Transform> mGroupTransform;//缓存池
        private List<CircleData> mTempObject = new List<CircleData>();
        private Vector3 mContenPos = Vector3.zero;
        private Vector3 mTempPos = Vector3.zero;
        private float mLenEx = 0;
        private int mCurNum = 0;
        private bool mAutoLen = true;
        private bool mLoop = false;

        public CircleData this[int index]
        {
            get { return mTempObject[index]; }
        }
        private bool SentEvent(CircleData go, int i)
        {
            if (mLoop)
            {
                i = i < 0?((mCurNum - Mathf.Abs(i)%mCurNum)%mCurNum):(i % mCurNum);
            }

            go.index = i;
            if (mAutoLen && mCurNum <= i)
            {
                return false;
            }
            if (mCallEvent != null)
            {
                return mCallEvent(i, go);
            }
            return true;
        }

        public int GetMaxNum()
        {
            return mCurNum;
        }

        public void ResetActive()
        {
            for (int i = 0; i < mTempObject.Count; i++)
            {
                var t = mTempObject[i];
                t.go.SetActive(SentEvent(t, t.index));
            }
        }

        public void ResetActiveAndNum(int num)
        {
            SetIntNum(num);
            for (int i = 0; i < mTempObject.Count; i++)
            {
                var t = mTempObject[i];
                t.go.SetActive(SentEvent(t, t.index));
            }
        }

        public CircleData GetCircleData(GameObject go)
        {
            for (int i = 0; i < mTempObject.Count; i++)
            {
                var t = mTempObject[i];
                if (t.go == go)
                    return t;
            }
            return null;

        }

    public List<CircleData> GetAllObject()
        {
            return mTempObject;
        }


        public void SetExNum(float ex)
        {
            mLenEx = ex;
            SetIntNum(mCurNum);
        }
        public void SetIntNum(int num)
        {
            mCurNum = num;
            if (isVertical)
            {
                mContent.sizeDelta = new Vector2(mContent.sizeDelta.x, Mathf.Abs(mSize.y) * (1 + (num - 1) / WidthCount) + 10 + mLenEx);
            }
            else
            {
                mContent.sizeDelta = new Vector2(mLenEx + Mathf.Abs(mSize.x) * (1 + (num - 1) / HightCount), mContent.sizeDelta.y);
            }
        }


        //autolen,自动控制,TempObject锚点必须置顶,

        public void Init(Func<int, CircleData, bool> callEvent, int num,bool autoLen = true,bool isLoop = false)
        {

            mLoop = isLoop;
            if(mLoop)
            {
                this.GetComponent<ScrollRect>().movementType = ScrollRect.MovementType.Unrestricted;
                autoLen = false;
                mIsCircle = true;
            }

            mAutoLen = autoLen;
            mSize = new Vector2(Mathf.Abs(mSize.x), Math.Abs(mSize.y)*-1);

            mCallEvent = callEvent;
            if (mIsCircle)
            {
                mChangeCallFun = ChangeItemEx;
            }
            else
            {
                mChangeCallFun = mAsyncChange;
            }
            mScrollRect = this.gameObject.GetComponent<ScrollRect>();
            if (mContent == null)
            {
                mContent = mScrollRect.content;
                mContenPos = mContent.transform.position;
                mTempPos = TempObject.transform.position;
                //锚点置顶,防止拖动范围变大时,位置发生变化
                var tempRect = TempObject.GetComponent<RectTransform>();
                tempRect.anchorMin = new Vector2(0, 1);
                tempRect.anchorMax = new Vector2(0, 1);
                TempObject.transform.position = mTempPos;
            }
            else
            {
                mContent.transform.position = mContenPos;
                mTempObject[0].go.transform.position = mTempPos;
            }

            mIndexPacge = 0;
            if (mScrollRect != null)
            {
                isVertical = mScrollRect.vertical;
            }
            else
            {
                Debug.LogError("未找到ScrollRect");
            }

            SetIntNum(num);

            if (TempObject != null)
            {
                /*
                List<Transform> tempGos = new List<Transform>();
                if (mTempObject.Count == 0)
                {
                    tempGos.Add(TempObject);
                    //mTempObject.Add(new CircleData() { go = TempObject.gameObject });  
                }
                else
                {
                    for (int j = 0; j < mTempObject.Count; j++)
                    {
                        tempGos.Add(mTempObject[j].go.transform);
                    }
                }
                */

                if(mGroupTransform == null)
                {
                    mGroupTransform = new FGroup<Transform>();
                    if (string.IsNullOrEmpty(ResName))
                    {
                        mGroupTransform.Init(TempObject, GLayPosition.Create(mSize.x, mSize.y, WidthCount));
                    }
                    else
                    {
                        mGroupTransform.Init(ResName,TempObject.gameObject, GLayPosition.Create(mSize.x, mSize.y, WidthCount));
                    }
                }
                mGroupTransform.Refurbish(WidthCount * HightCount);
                //SceneManager.CloneObjectList(mSize, tempGos, WidthCount * HightCount, WidthCount);
                int maxCount = isVertical ? HightCount : WidthCount;
                mIndexs.Clear();
                mBuffs.Clear();
                for (int i = 0; i < maxCount; i++)
                {
                    mBuffs[i] = new List<CircleData>();
                    mIndexs.Add(i);
                }

                int allNum = WidthCount * HightCount;
                for (int i = 0; i < allNum; i++)
                {
                    //mTempObject[i].name = i.ToString();
                    CircleData cd = null;
                    if (mTempObject.Count > i)
                    {
                        cd = mTempObject[i];
                    }
                    else
                    {
                        cd = new CircleData();
                        cd.go = mGroupTransform[i].gameObject;
                        mTempObject.Add(cd);
                    }
                    if (isVertical)
                    {
                        mBuffs[i / WidthCount].Add(cd);
                    }
                    else
                    {
                        mBuffs[i % WidthCount].Add(cd);
                    }
                    if (!SentEvent(mTempObject[i], i))
                    {
                        mTempObject[i].go.SetActive(false);
                    }
                }


                mStartPos = mTempObject[0].go.transform.position;
                mEndPos = mTempObject[allNum - 1].go.transform.position;

                mWorldSzie = (mEndPos - mStartPos);
                mDatilSize = mSize;
                if (isVertical)
                {
                    mWorldSzie.y = mWorldSzie.y / (HightCount - 1);
                    mWorldSzie.x = 0;
                    mDatilSize.x = 0;
                    mDatilSize.y *= -1;
                }
                else
                {
                    mWorldSzie.x = mWorldSzie.x / (WidthCount - 1);
                    mWorldSzie.y = 0;
                    mDatilSize.y = 0;
                }

                mEndPos = mStartPos;
                mStartPos -= mWorldSzie * 3.0f;
                mEndPos -= (mWorldSzie / 2.0f);

            }
        }


        public void DeletePool()
        {
            if (mGroupTransform != null)
            {
                mGroupTransform.PushPool();
            }
        }

        void Update()
        {
            if (mCallEvent != null)
            {
                CheckItem();
            }
        }


        public void CheckItem()
        {
            if (isVertical)
            {
                if (mBuffs[mIndexs[0]][0].go.transform.position.y > mStartPos.y)
                {
                    mChangeCallFun(true);
                }
                else if (mBuffs[mIndexs[0]][0].go.transform.position.y < mEndPos.y)
                {
                    mChangeCallFun(false);
                }
            }
            else
            {
                if (mBuffs[mIndexs[0]][0].go.transform.position.x < mStartPos.x)
                {
                    mChangeCallFun(true);
                }
                else if (mBuffs[mIndexs[0]][0].go.transform.position.x > mEndPos.x)
                {
                    mChangeCallFun(false);
                }
            }
        }



        private void mAsyncChange(bool create)
        {
            
            if (!create && mIndexPacge == 0)
            {
                return;
            }

            if (create)
            {
                List<CircleData> end = mBuffs[mIndexs[mIndexs.Count - 1]];
                var newDataList = new List<CircleData>();
                mBuffs[mIndexs.Count] = newDataList;
                mIndexs.Add(mIndexs.Count);
                
                for(int i = 0; i < end.Count;i++)
                {
                    var cd = end[i];
                    
                    CircleData newCD = null;
                    int realIndex = GetIndex(mIndexPacge, i,true);
                    mGroupTransform.Add(1);
                    if (mTempObject.Count > realIndex)
                    {
                        newCD = mTempObject[realIndex];
                    }
                    else
                    {
                        //GameObject clone = SceneManager.CloneObject(cd.go);
                       
                        GameObject clone = mGroupTransform.Last.gameObject;
                        clone.transform.position = cd.go.transform.position + mWorldSzie;
                        newCD = new CircleData();
                        newCD.go = clone;
                        mTempObject.Add(newCD);
                    }

                    newDataList.Add(newCD);

                    if (SentEvent(newCD, realIndex))
                    {
                        newCD.go.SetActive(true);
                    }
                    else
                    {
                        newCD.go.SetActive(false);
                    }
                }
                mIndexs[0]++;
                mIndexPacge += 1;
            }
        }


        private int GetIndex(int pacge,int index,bool isAdd)
        {
            if (isVertical)
            {
                return isAdd?((pacge + HightCount) * WidthCount + index): ((pacge - 1) * WidthCount + index);
            }
            else
            {
                return isAdd ? ((pacge + WidthCount) * HightCount + index) : ((pacge - 1) * HightCount + index);
            }
        }


        public void ChangeItemEx(bool isAdd)
        {
            if (!isAdd)
            {
                if (!mLoop)
                {
                    return;
                }
            }

            List<CircleData> one = isAdd ? mBuffs[mIndexs[0]] : mBuffs[mIndexs[mIndexs.Count - 1]];
            List<CircleData> end = isAdd ? mBuffs[mIndexs[mIndexs.Count - 1]] : mBuffs[mIndexs[0]];
            int curADD = isAdd ? 1 : -1;
            for (int i = 0; i < one.Count; i++)
            {
                one[i].go.transform.position = end[i].go.transform.position + mWorldSzie * curADD;
                int realIndex = GetIndex(mIndexPacge, i, isAdd);

                if (SentEvent(one[i], realIndex))
                {
                    one[i].go.SetActive(true);
                }
                else
                {
                    one[i].go.SetActive(false);
                }
            }
            UpdateContent(isAdd);
            mIndexPacge += curADD;
        }

        private void UpdateContent(bool isAdd)
        {
            if (isAdd)
            {
                //mContent.sizeDelta = new Vector2(mContent.sizeDelta.x + mDatilSize.x, mContent.sizeDelta.y + mDatilSize.y);
                int temp = mIndexs[0];
                mIndexs.RemoveAt(0);
                mIndexs.Add(temp);
            }
            else
            {
                //mContent.sizeDelta = new Vector2(mContent.sizeDelta.x - mDatilSize.x, mContent.sizeDelta.y - mDatilSize.y);
                int temp = mIndexs[mIndexs.Count - 1];
                mIndexs.RemoveAt(mIndexs.Count - 1);
                mIndexs.Insert(0, temp);
            }
        }
    }
}
