//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    //通用模块
    public class UniversalModel
    {
        public List<UnitPool> mPools = new List<UnitPool>();
        public FGroup<T> Create<T>(string resName, GameObject parent,FGroupTool tool = null,Action<int, T> call = null) where T : Component
        {
            var g = new FGroup<T>();
            g.Init(resName, parent,tool,call);
            mPools.Add(g);
            return g;
        }

        public void Clear()
        {
            for(int i = 0; i < mPools.Count;i++)
            {
                mPools[i].PushPool();
            }
            mPools.Clear();
        }
    }
}
