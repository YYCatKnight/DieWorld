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
    public class PreBundleProperty : BaseAssetProperty
    {
        public List<string> PreFiles;//预制文件
        public List<string> PrePahts;//预制路径
    }


    public class PreBundleAsset : TemplateAsset<PreBundleAsset, PreBundleProperty>
    {
        protected override PreBundleProperty ShowNoLog(string only_id)
        {
            return null;
        }
    }
}
