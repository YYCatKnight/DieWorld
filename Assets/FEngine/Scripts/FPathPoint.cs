//----------------------------------------------
//  F2DEngine: time: 2018.7  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace F2DEngine
{
    public class FPathPoint : UnitObject
    {
        //基础数据
        [Serializable]
        public class PointData
        {
            public Vector3 pos;
            public List<int> mIds = new List<int>();
        }

        public List<PointData> pathData = new List<PointData>();


        public PointData GetPathById(int id)
        {
            return pathData[id];
        }

        public PointData RegPoint(Vector3 pos)
        {
            PointData pd = new PointData();
            pd.pos = pos;
            pathData.Add(pd);
            return pd;
        }

        public PathControl GetPathControl(int sId,int eId)
        {
            PathControl pc = new PathControl(FindWay(sId, eId));
            return pc;
        }

        public int GetIdByPos(Vector3 pos)
        {
            float len = 99999;
            int id = 0;
            for(int i = 0; i < pathData.Count;i++)
            {
                float dis = Vector3.Distance(pos, pathData[i].pos);
                if(dis < len)
                {
                    id = i;
                    len = dis;
                }
            }
            return id;
        }

        public PathControl GetPathControl(Vector3 sP,Vector3 eP)
        {
            return GetPathControl(GetIdByPos(sP), GetIdByPos(eP));
        }

        //开启列表
        List<int> Open_List = new List<int>();

        //关闭列表
        List<int> Close_List = new List<int>();

        //父节点列表
        Dictionary<int,int> Father_Dic = new Dictionary<int,int>();

        //base A星算法
        private List<PointData> FindWay(int sId,int eId)
        {
            List<PointData> Path = new List<PointData>();
            Open_List.Clear();
            Father_Dic.Clear();
            Close_List.Clear();
            Open_List.Add(sId);

            while (true)
            {
                if (Open_List.Count == 0)
                    return Path;
                int id = Open_List[0];
                if (id == eId)
                    break;
                var data = pathData[id];
                if (!Close_List.Contains(id))
                {
                    for (int i = 0; i < data.mIds.Count; i++)
                    {
                        int tId = data.mIds[i];
                        if (!Open_List.Contains(tId) && !Close_List.Contains(tId))
                        {
                            Father_Dic[tId] = id;
                            Open_List.Add(tId);
                        }
                    }
                }
                Open_List.Remove(id);
                Close_List.Add(id);
            }

            int pathId = eId;
            while (true)
            {
                Path.Insert(0, pathData[pathId]);
                if(pathId == sId||!Father_Dic.TryGetValue(pathId,out pathId))
                {
                    break;
                }
            }
            return Path;
        }



        //路径
        public class PathControl
        {
            public enum PathType
            {
                PT_Noraml,
                PT_Back,
            }
            public List<PointData> mPoint = new List<PointData>();
            public Action<int> CallBack;
            private int mPosId = 0;
            public float mCurPos = 0;
            public Vector3 mDir;
            public float mLen;
            public PathType type;
            private int mLastId = -1;
            public PathControl(List<PointData> pd)
            {
                mPoint = pd;
            }

            public Vector3 GetNextDir()
            {
                var sP = mPoint[mPosId];
                var eP = mPoint[mPosId + 1];
                var dir = (eP.pos - sP.pos).normalized;
                return dir;
            }

            public float GetMoveLen()
            {
                var sP = mPoint[mPosId];
                var eP = mPoint[mPosId + 1];
                return Vector3.Distance(sP.pos, eP.pos);
            }

            public Vector3 MovePostion(float len)
            {
                Vector3 tempPos = Vector3.zero;
                var sP = mPoint[mPosId];
                var eP = mPoint[mPosId + 1];
                if (mLastId != mPosId)
                {
                    mLastId = mPosId;
                    mDir = (eP.pos - sP.pos).normalized;
                    mLen = Vector3.Distance(sP.pos, eP.pos);
                }

                mCurPos += len;
                if(mCurPos >= mLen)
                {
                    tempPos = eP.pos;
                    mPosId++;

                    int id = 0;
                    if(mPosId == mPoint.Count-1)
                    {
                        if(type == PathType.PT_Noraml)
                        {

                        }
                        else if(type == PathType.PT_Back)
                        {
                            mPoint.Reverse();
                        }
                        mPosId = 0;
                      
                        mPosId = 0;
                        id = 1;
                    }
                    mCurPos -= mLen;

                    if (CallBack != null)
                    {
                        CallBack(id);
                    }
                }
                else
                {
                    tempPos = sP.pos + mDir * mCurPos;
                }
                return tempPos;
            }
        }
    }
}
