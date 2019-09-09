//-----------------------------------------------
// Friday Engine 2015-2019 Fu Cong QQ: 353204643
//-----------------------------------------------
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
namespace F2DEngine
{
    //循环列表,扩展功能
    public class FCScrollRect : UnitObject, IEndDragHandler, IBeginDragHandler
    {
        public class ScrollItem
        {
            public ScrollItem(GameObject go)
            {
                m_GameObject = go;
            }
            private GameObject m_GameObject;
            public GameObject gameObject { get { return m_GameObject; } }
            private int m_Index;
            public int Index { get { return m_Index; } }
            private int m_GroupIndex;
            public int GroupIndex { get { return m_GroupIndex; } }
            private Component m_Component;
            public T GetComponent<T>() where T : Component
            {
                if (m_Component == null)
                    m_Component = m_GameObject.GetComponent<T>();
                return (T)m_Component;
            }

            internal void SetIndexAndGroup(int index, int group)
            {
                m_Index = index;
                m_GroupIndex = group;
            }
        }
        private ScrollRect mScrollRect;
        private RectTransform mContent;
        private FGroup<Transform> mGroups;//缓存池
        private List<ScrollItem> mItems = new List<ScrollItem>();
        private Action<ScrollItem> m_CenterAction;
        public Action<ScrollItem> UpdateEvent;
        public Action<ScrollItem> CenterEvent
        {
            get { return m_CenterAction; }
            set
            {
                m_CenterAction = value;
                m_IsCenter = m_CenterAction != null;
            }
        }
        private int m_CacheLine = 1;//缓存池大小
        private int m_GroupNum = 1;//群体个数
        private int m_ActionNum = 0;//有效个体
        private int m_MaxLine = 1;//最大条数
        private Vector2 m_Rect;
        private float m_GroupDis;
        private float m_SpaceDis;
        private float m_SpaceOffset = 0;
        private Vector3 m_CenterPosion = Vector3.zero;//中心位置
        private ScrollItem m_CenterItem;
        private int m_LastCenterGroup = -99999;
        private ScrollItem m_TempCenterItem;
        private Vector3 m_GPSCenterPosion;
        private float m_CenterOffset = 0;
        private float m_DefaultCenterOffset = 0;
        private bool mIsUseSelect = false;
        private bool mIsMoveState = false;
        private int mStartGroup = 0;
        private Vector3 mVelocityPos = Vector3.zero;
        private ScrollType mModeType = ScrollType.Repeat;
        private bool m_IsVertical;
        private Vector3 mStartContenPos = Vector3.zero;
        private Vector3 mStartItemPos = Vector3.zero;
        private Component mComponent;
        private int m_LastIndex = -99999;
        private int m_MaxNum = 0;
        private bool m_IsCenter = false;//显示中心
        private bool mIsInit = false;
        private FGLayPosition m_GLayPosition;
        private Action<float> m_UpdateFun;
        private IScrollEx m_ScrollEx;
        public enum ScrollType
        {
            None,
            Repeat,//重复利用
            Asy,//异步
            Circle,//循环
        }
        public FCScrollRect SetCache(int count)
        {
            m_CacheLine = count;
            return this;
        }
        public FCScrollRect SetGroup(int count)
        {
            m_GroupNum = count;
            return this;
        }
        public FCScrollRect SetOffset(float offset)
        {
            m_SpaceOffset = offset;
            return this;
        }
        public FCScrollRect SetComponent(Component com)
        {
            mComponent = com;
            return this;
        }
        public FCScrollRect SetMode(ScrollType type)
        {
            mModeType = type;
            return this;
        }
        public FCScrollRect SetScroll(IScrollEx scrollEx)
        {
            m_ScrollEx = scrollEx;
            return this;
        }
        public FCScrollRect SetCenterOffset(float offset)
        {
            m_CenterOffset = offset;
            return this;
        }
        public FCScrollRect SetCount(int count, int startGroup = 0)
        {
            m_MaxNum = count;
            mStartGroup = startGroup;
            mIsInit = false;
            return this;
        }
        public ScrollItem GetSelectGroup()
        {
            if (m_IsCenter)
            {
                return m_CenterItem;
            }
            else
            {
                int index = 0;
                if (m_IsVertical)
                {
                    //反即是正
                    index =(int)((mContent.transform.localPosition.y - mStartContenPos.y - m_CenterOffset) / m_SpaceDis);
                }
                else
                {
                    //正即是反
                    index = (int)((mStartContenPos.x + m_CenterOffset-mContent.transform.localPosition.x) / m_SpaceDis);
                }
                for(int i = 0; i < mItems.Count;i++)
                {
                    if(mItems[i].GroupIndex == index)
                    {
                        return mItems[i];
                    }
                }
            }
            return null;
        }     
        public void MoveToNext(int offset)
        {
            if(m_CenterItem != null)
            {
                SelectGroup(m_CenterItem.GroupIndex + offset,false);
            }
        }
        public List<ScrollItem> GetAllItem()
        {
            return mItems;
        }
        public void SelectGroup(int startGroup, bool immediately = true)
        {
            if (mIsInit)
            {
                if (mModeType == ScrollType.Repeat || mModeType == ScrollType.Circle)
                {
                }
                else
                {
                    if (startGroup < 0)
                    {
                        startGroup = m_MaxLine - 1;
                    }
                    startGroup = Mathf.Clamp(startGroup, 0, m_MaxLine - 1);
                }
                var desContent = mContent.localPosition;
                if (m_IsVertical)
                {
                    //反即是正
                    desContent.y = m_SpaceDis * startGroup + mStartContenPos.y + m_CenterOffset + m_DefaultCenterOffset;
                }
                else
                {
                    //正即是反
                    desContent.x = mStartContenPos.x - m_SpaceDis * startGroup + m_CenterOffset + m_DefaultCenterOffset;
                }
                mIsUseSelect = true;
                mIsMoveState = !immediately;
                if (immediately)
                {
                    mContent.transform.localPosition = desContent;
                    Update();
                }
                else
                {
                    m_GPSCenterPosion = desContent;
                }
            }
            else
            {
                mStartGroup = startGroup;
            }
        }
        private void Begin()
        {
            if (!mIsInit)
            {
                mIsInit = true;
                SetStart(m_MaxNum, mStartGroup);

            }
        }
        private void SetStart(int count, int startGroup = 0)
        {
            mScrollRect = this.gameObject.GetComponent<ScrollRect>();
            if (mScrollRect != null)
            {
                if (mContent == null)
                {
                    //初始化
                    mContent = mScrollRect.content;

                    if (mComponent == null)
                    {
                        mComponent = mContent.GetChild(0);
                    }
                    mStartContenPos = mContent.transform.localPosition;
                    mStartItemPos = mComponent.transform.position;
                    //锚点置顶,防止拖动范围变大时,位置发生变化
                    var tempRect = mComponent.GetComponent<RectTransform>();
                    tempRect.anchorMin = new Vector2(0, 1);
                    tempRect.anchorMax = new Vector2(0, 1);
                    mComponent.transform.position = mStartItemPos;
                    mStartItemPos = mComponent.transform.localPosition;
                    m_Rect = new Vector2(tempRect.sizeDelta.x, tempRect.sizeDelta.y);

                    var tempScroll = mScrollRect.GetComponent<RectTransform>();
                    m_CenterPosion = Vector3.zero;
                    m_IsVertical = mScrollRect.vertical;
                    mGroups = new FGroup<Transform>();
                    mGroups.Init(mComponent.transform);
                    var tempPos = mContent.transform.position;
                    tempPos = mScrollRect.transform.InverseTransformPoint(tempPos);
                    if (m_IsVertical)
                    {
                        m_SpaceDis = m_Rect.y;
                        m_GroupDis = m_Rect.x;
                        m_CenterPosion.x = tempPos.x;
                    }
                    else
                    {
                        m_SpaceDis = m_Rect.x;
                        m_GroupDis = m_Rect.y;
                        m_CenterPosion.y = tempPos.y;
                    }

                    if (m_IsCenter)
                    {
                        var offset = mScrollRect.transform.TransformPoint(m_CenterPosion);
                        offset = mContent.transform.InverseTransformPoint(offset);
                        m_DefaultCenterOffset = (m_IsVertical ? (offset.y - mStartItemPos.y) : (offset.x - mStartItemPos.x));
                    }

                    m_GLayPosition = FGLayPosition.Create(m_SpaceDis, m_IsVertical, m_GroupNum, m_GroupDis);
                    m_GLayPosition.SetStart(mStartItemPos);

                    if (mModeType == ScrollType.Repeat)
                    {
                        m_UpdateFun = UpdateRepeat;
                    }
                    else if (mModeType == ScrollType.Asy)
                    {
                        m_UpdateFun = UpdateAsy;
                    }
                    else if (mModeType == ScrollType.Circle)
                    {
                        m_UpdateFun = UpdateCircle;
                        mScrollRect.movementType = ScrollRect.MovementType.Unrestricted;
                    }
                    else
                    {
                        m_UpdateFun = UpdateNull;
                    }
                }
                else
                {
                    //复位
                    mContent.transform.localPosition = mStartContenPos;
                }
                m_LastIndex = -99999;//重置
                m_LastCenterGroup = -99999;
                //配置滑动长度
                var scrollTransform = mScrollRect.GetComponent<RectTransform>();
                Vector3[] sizeVector3 = new Vector3[4];
                scrollTransform.GetLocalCorners(sizeVector3);
                float sizeX = Mathf.Abs(sizeVector3[0].x - sizeVector3[2].x);
                float sizeY = Mathf.Abs(sizeVector3[0].y - sizeVector3[2].y);

                if (m_IsVertical)
                {
                    if (m_CacheLine <= 1)
                    {
                        SetCache(Mathf.Clamp((int)(sizeY / m_SpaceDis) + 2, 4, 999999));
                    }
                    mContent.sizeDelta = new Vector2(mContent.sizeDelta.x, m_SpaceDis * (1 + (count - 1) / m_GroupNum) + m_SpaceOffset);
                }
                else
                {
                    if (m_CacheLine <= 1)
                    {
                        SetCache(Mathf.Clamp((int)(sizeX / m_SpaceDis) + 2, 4, 999999));
                    }
                    mContent.sizeDelta = new Vector2(m_SpaceOffset + m_SpaceDis * (1 + (count - 1) / m_GroupNum), mContent.sizeDelta.y);
                }
                m_ActionNum = m_GroupNum * m_CacheLine;
                int rNum = mModeType == ScrollType.Circle ? m_ActionNum : Mathf.Min(m_ActionNum, count);
                mGroups.Refurbish(rNum);
                mItems.Clear();
                foreach (var k in mGroups)
                {
                    ScrollItem sItem = new ScrollItem(k.gameObject);
                    mItems.Add(sItem);
                }
                m_MaxNum = count;
                m_MaxLine = 1 + (count - 1) / m_GroupNum;
                SelectGroup(startGroup);
                if(m_ScrollEx!= null)
                {
                    m_ScrollEx.Init(this);
                }
            }
            else
            {
                Debug.LogError("ScrollRect未找到");
            }
        }
        private void Update()
        {
            Begin();

            if (m_IsVertical)
            {  
                //反即是正
                float sIndex = (mContent.localPosition.y - mStartContenPos.y) / m_SpaceDis;
                UpdateItems(sIndex);

            }
            else
            {
                //正即是反
                float sIndex = (mStartContenPos.x - mContent.localPosition.x) / m_SpaceDis;
                UpdateItems(sIndex);
            }
            if (mIsMoveState)
            {
                mContent.transform.localPosition = Vector3.SmoothDamp(mContent.transform.localPosition, m_GPSCenterPosion, ref mVelocityPos, 0.15f);
                if (Vector3.Distance(mContent.transform.localPosition, m_GPSCenterPosion) < 5)
                {
                    mIsMoveState = false;
                }
            }
            else
            {
                if (m_IsCenter)
                {
                    UpdateCenter();
                }
            }
            if(m_ScrollEx != null)
            {
                m_ScrollEx.Update();
            }
        }
        private void UpdateItems(float floatIndex)
        {
            m_UpdateFun(floatIndex);
        }
        private void UpdateNull(float floatIndex)
        {

        }
        private void UpdateItem(float floatIndex, bool isCircle)
        {
            int desIndex = 0;
            if (floatIndex > 0.5f)
            {
                desIndex = (int)(floatIndex - 0.5f);
            }
            else
            {
                desIndex = (int)(floatIndex - 1.5f);
            }

            int offset = desIndex - m_LastIndex;

            if (offset != 0)
            {
                if (!isCircle)
                {
                    desIndex = Mathf.Clamp(desIndex, 0, m_MaxLine);
                }
                offset = desIndex - m_LastIndex;
                if (offset != 0)
                {
                    //大于总长度,全刷新
                    int sIndex = 0;
                    int eIndex = 0;
                    if (offset >= m_CacheLine || offset <= -m_CacheLine)
                    {
                        //全刷新
                        sIndex = desIndex;
                        eIndex = desIndex + m_CacheLine;
                    }
                    else if (offset > 0)
                    {
                        //如果是下拉,根据尾部更新
                        sIndex = m_LastIndex + m_CacheLine;
                        eIndex = desIndex + m_CacheLine;
                    }
                    else if (offset < 0)
                    {
                        //如果是上拉,根据头部更新
                        sIndex = desIndex;
                        eIndex = m_LastIndex;
                    }
                    int cid = 0;
                    for (int page = sIndex; page < eIndex; page++)
                    {
                        for (int n = 0; n < m_GroupNum; n++)
                        {
                            cid = page * m_GroupNum + n;
                            int relayId = cid;
                            int itemIndex = cid;
                            if (isCircle)
                            {
                                if (cid < 0)
                                {
                                    relayId = m_MaxNum - Mathf.Abs(cid) % m_MaxNum;
                                    itemIndex = mItems.Count - Mathf.Abs(cid) % mItems.Count;
                                }
                                relayId = relayId % m_MaxNum;
                            }

                            if (m_MaxNum > relayId)
                            {
                                var com = mItems[itemIndex % mItems.Count];
                                m_GLayPosition.HandTool(cid, com.gameObject.transform);
                                com.SetIndexAndGroup(relayId, page);
                                UpdateEvent(com);
                            }
                        }
                    }
                    m_LastIndex = desIndex;
                }
            }
        }
        private void UpdateRepeat(float floatIndex)
        {
            UpdateItem(floatIndex, false);
        }
        private void UpdateAsy(float floatIndex)
        {
            int desIndex = (int)(floatIndex - 0.5f) + m_CacheLine;
            if (desIndex > m_LastIndex)
            {
                desIndex = Mathf.Clamp(desIndex, 0, m_MaxLine);
                int count = Mathf.Clamp(m_LastIndex, 0, m_MaxLine);
                int cid = 0;
                for (int page = count; page < desIndex + 1; page++)
                {
                    for (int n = 0; n < m_GroupNum; n++)
                    {
                        cid = page * m_GroupNum + n;
                        if (m_MaxNum > cid)
                        {
                            ScrollItem com = null;
                            if (cid < mItems.Count)
                            {
                                com = mItems[cid];
                            }
                            else
                            {
                                mGroups.Add(1);
                                com = new ScrollItem(mGroups.Last.gameObject);
                                mItems.Add(com);
                            }
                            m_GLayPosition.HandTool(cid, com.gameObject.transform);
                            com.SetIndexAndGroup(cid, page);
                            UpdateEvent(com);
                        }
                    }
                }
                m_LastIndex = desIndex;
            }
        }
        private void UpdateCircle(float floatIndex)
        {
            UpdateItem(floatIndex, true);
        }
        private void UpdateCenter()
        {
            if (mIsUseSelect)
            {
                float sp = 0;
                if (m_IsVertical)
                {
                    sp = Mathf.Abs(mScrollRect.velocity.y);
                }
                else
                {
                    sp = Mathf.Abs(mScrollRect.velocity.x);
                }

                if (sp < 100)
                {
                    if (m_TempCenterItem == null)
                    {
                        GpsItem(null);
                    }
                    else
                    {
                        mContent.transform.localPosition = Vector3.SmoothDamp(mContent.transform.localPosition, m_GPSCenterPosion, ref mVelocityPos, 0.15f);
                        if (Vector3.Distance(mContent.transform.localPosition, m_GPSCenterPosion) < 5)
                        {
                            mIsUseSelect = false;
                            m_TempCenterItem = null;
                            mScrollRect.velocity = Vector3.zero;
                            mContent.transform.localPosition = m_GPSCenterPosion;
                            if (m_LastCenterGroup != m_CenterItem.GroupIndex)
                            {
                                m_LastCenterGroup = m_CenterItem.GroupIndex;
                                CenterEvent(m_CenterItem);
                            }
                        }
                    }
                }
            }
        }
        private void GpsItem(ScrollItem item)
        {
            //重新定位选择物体
            mScrollRect.velocity = Vector3.zero;
            m_GPSCenterPosion = mScrollRect.transform.TransformPoint(m_CenterPosion);
            m_GPSCenterPosion = mContent.transform.InverseTransformPoint(m_GPSCenterPosion);
            if (item == null)
            {
                float dis = Mathf.Infinity;
                for (int i = 0; i < mItems.Count; i++)
                {
                    float tempDis = 0;
                    var it = mItems[i];
                    if (m_IsVertical)
                    {
                        tempDis = Mathf.Abs(m_GPSCenterPosion.y - it.gameObject.transform.localPosition.y+ m_CenterOffset);
                    }
                    else
                    {
                        tempDis = Mathf.Abs(m_GPSCenterPosion.x - it.gameObject.transform.localPosition.x+ m_CenterOffset);
                    }

                    if (m_CenterItem != null && (m_CenterItem == it || m_CenterItem.GroupIndex == it.GroupIndex))
                    {
                        tempDis += m_SpaceDis / 2.0f;
                    }
                    if (tempDis < dis)
                    {
                        item = it;
                        dis = tempDis;
                    }
                }
            }

            if (m_IsVertical)
            {
                m_GPSCenterPosion.y = mContent.transform.localPosition.y + m_GPSCenterPosion.y - item.gameObject.transform.localPosition.y + m_CenterOffset;
            }
            else
            {
                m_GPSCenterPosion.x = mContent.transform.localPosition.x + m_GPSCenterPosion.x - item.gameObject.transform.localPosition.x + m_CenterOffset;
            }
            m_CenterItem = item;
            m_TempCenterItem = item;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsUseSelect = false;
            m_TempCenterItem = null;
        }
        public void OnEndDrag(PointerEventData eventData)
        {
            mIsUseSelect = true;
        }
    }

    public interface IScrollEx
    {
        void Init(FCScrollRect scrollRect);
        void Update();
    }

    public class FGLayPosition : FGroupTool
    {
        private Vector3 mStartPos;
        private bool m_IsVertical = false;
        private float m_GroupDis = 0;
        private float m_SpaceDis = 0;
        private int m_GroupNum = 1;

        public static FGLayPosition Create(float spaceDis, bool isVertical = false, int groupNum = 1, float groupDis = 0)
        {
            FGLayPosition lp = new FGLayPosition();
            lp.SetSpace(spaceDis).SetVerical(isVertical).SetGroup(groupNum, groupDis);
            return lp;
        }

        public FGLayPosition SetVerical(bool isVerical)
        {
            m_IsVertical = isVerical;
            return this;
        }

        public FGLayPosition SetGroup(int num, float f)
        {
            m_GroupNum = num;
            m_GroupDis = f;
            return this;
        }

        public FGLayPosition SetSpace(float f)
        {
            m_SpaceDis = f;
            return this;
        }

        public FGLayPosition SetStart(Vector3 pos)
        {
            mStartPos = pos;
            return this;
        }


        public void Init(string name, Component obj)
        {
            if (obj != null)
            {
                mStartPos = obj.transform.localPosition;
            }
            else
            {
                mStartPos = Vector3.zero;
            }
        }

        public void HandTool(int index, Component obj)
        {
            float space = 0;
            float gdis = 0;
            if (m_GroupNum <= 1)
            {
                space = index * m_SpaceDis;
            }
            else
            {
                if (index < 0)
                {
                    int xx = (index + 1 - m_GroupNum) / m_GroupNum;
                    int yy = (m_GroupNum - Mathf.Abs(index) % m_GroupNum) % m_GroupNum;
                    space = xx * m_SpaceDis;
                    gdis = yy * m_GroupDis;
                }
                else
                {
                    int xx = index / m_GroupNum;
                    int yy = index % m_GroupNum;
                    space = xx * m_SpaceDis;
                    gdis = yy * m_GroupDis;
                }

            }
            Vector3 pos = mStartPos;
            if (m_IsVertical)
            {
                //垂直             
                pos.x += gdis;
                pos.y -= space;
            }
            else
            {
                pos.x += space;
                pos.y -= gdis;
            }
            obj.transform.localPosition = pos;
        }
    }


    #region 扩展接口
    public class ScaleScrollEx : IScrollEx
    {
        public void Init(FCScrollRect scrollRect)
        {
            
        }

        public void Update()
        {
            
        }
    }

    #endregion
}
