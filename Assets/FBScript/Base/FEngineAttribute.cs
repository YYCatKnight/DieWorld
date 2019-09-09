//----------------------------------------------
//  F2DEngine: time: 2017.2  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class FEngineAttribute : PropertyAttribute
    {

    }

    //基本属性结构
    public class FPropertySerialize: Attribute
    {
      
    }

    public class FMethodAtt : Attribute
    {
        public string nName;
        public string[] nTags;
        public FMethodAtt(string name,params string[]tags)
        {
            nName = name;
            nTags = tags;
        }

        public bool IsHaveTag(string tag)
        {
            if(nTags != null&&nTags.Length > 0)
            {
                for(int i = 0; i < nTags.Length;i++)
                {
                    if (nTags[i] == tag)
                        return true;
                }
                return false;
            }
            return true;
        }
    }

    public class FMachineAttribute : Attribute
    {
        public string nName;
        public string[] nDelegates;
        public int mMask;
        public FMachineAttribute(string name, FMask mask = FMask.Mask0, params string[] delegates)
        {
            nName = name;
            nDelegates = delegates;
            mMask = (int)mask;
        }
    }

    public class FAnimatorAttr: FEngineAttribute
    {

    }

    /// 整数范围控制
    public class FIntAttr:FEngineAttribute
    {
        public int minNum;
        public int maxNum;

        public FIntAttr(int _minNum = 0,int _maxNum = 100)
        {
            minNum = _minNum;
            maxNum = _maxNum;
        }
    }

    ////浮点范围控制
    public class FFloatAttr : FEngineAttribute
    {
        public float minNum;
        public float maxNum;

        public FFloatAttr(float _min = 0, float _max = 1)
        {
            minNum = _min;
            maxNum = _max;
        }
    }

    ////字符数组
    public class FStringAttr:FEngineAttribute
    {
        public string[] StrBuffs;
        public FStringAttr(params string[] str)
        {
            StrBuffs = str;
        }

        public int GetIndex(string key)
        {
            for(int i = 0; i < StrBuffs.Length;i++)
            {
                if (StrBuffs[i] == key)
                    return i;
            }
            return 0;
        }
    }

    ////属性改名,包括枚举
    public class FRenameAttr : FEngineAttribute
    {
        public string nName;
        public FRenameAttr(string name = "")
        {
            nName = name;
        }
    }

    ///属性标记
    public class FTagAttr:FEngineAttribute
    {
        public string[] StrBuffs;
        public FTagAttr(params string[] str)
        {
            StrBuffs = str;
        }
    }
}
