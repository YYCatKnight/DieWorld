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
    public class VersionProperty : BaseAssetProperty
    {
        public string versionId;
        public List<string> rosourceName;
    }


    public class VersionAsset : TemplateAsset<VersionAsset, VersionProperty>
    {
    }

}