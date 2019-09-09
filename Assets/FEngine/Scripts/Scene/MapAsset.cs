//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;

[Serializable]
public class MapProperty : BaseAssetProperty
{
    public int serverId;
    public int buildId;
    public int x;
    public int y;
}

public class MapAsset : TemplateAsset<MapAsset, MapProperty>
{
    private Dictionary<int, MapProperty> mPros = new Dictionary<int, MapProperty>();

    public MapProperty GetMap(int id)
    {
        MapProperty pro = null;
        if (mPros.TryGetValue(id, out pro))
        {
            return pro;
        }
        return null;
    }
   
    protected override MapProperty ShowNoLog(string only_id)
    {
        return null;
    }
}