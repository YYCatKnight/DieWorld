//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public class UnitObject : MonoBehaviour,UnitData
    {
        public string PoolName {get;set;}
        public bool IsPool { get; set;}
        public FLeaker Leaker { get; protected set; }//内存泄露监控


        //拿出缓存回调
        public virtual void ResetData()
        {
          
        }
        
        //内存泄露监控创建
        public void MonitorLeaker()
        {
            
        }


        //放入缓存执行回调
        public virtual void Clear()
        {

        }

        //得到能缓存的池数量
        public virtual int GetPoolCount()
        {
            return 20;
        }
    }

    public class FBaseAI:MonoBehaviour
    {

    }
}
