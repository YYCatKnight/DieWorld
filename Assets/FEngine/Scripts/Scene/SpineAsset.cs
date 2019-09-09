//----------------------------------------------
//  F2DEngine: time: 2016.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace F2DEngine
{
    [Serializable]
    public class SkinBaseData
    {
        public string slotName;
        public string equipName;
        public string alasName;
        public string GloabData;
        public string slotQquipName; //要替换装备名称
    }

    [Serializable]
    public class SpineProperty : BaseAssetProperty
    {
        public string Skeleton;
        public SkinBaseData[] SkinData;
        public string DefaultSkin;
    }


    public class SpineAsset : TemplateAsset<SpineAsset, SpineProperty>
    {

    }
}
