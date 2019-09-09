//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    [Serializable]
    public class NpcProperty : BaseAssetProperty
    {
        public string name;
        public string talkId;
        public int blood;
        public int define;
        public int attack;
        public string[] dataEx;
    }


    public class NpcAsset : TemplateAsset<NpcAsset, NpcProperty>
    {
    }

}