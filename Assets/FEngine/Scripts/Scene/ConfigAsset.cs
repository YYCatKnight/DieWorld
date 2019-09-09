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
    public class ConfigProperty : BaseAssetProperty
    {
        public string className;
    }


    public class ConfigAsset : TemplateAsset<ConfigAsset, ConfigProperty>
    {
    }

}