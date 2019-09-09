//----------------------------------------------
//  F2DEngine: time: 2019.05  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;

public class local_langAsset : TemplateAsset<local_langAsset, Lange_ListProperty> 
{
    protected override Lange_ListProperty ShowNoLog(string only_id)
    {
        return null;
    }

    public string GetValue(string key)
    {
        Lange_ListProperty lp = GetProperty(key);
        if (lp != null)
        {
            return Lange_ListAsset.LangCallBack(lp);
        }
        return "»±…Ÿ@@@@:[" + key + "]";
    }
}
