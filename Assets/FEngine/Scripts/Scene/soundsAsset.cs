//----------------------------------------------
//  F2DEngine: time: 2017.9  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

[Serializable]
public class soundsProperty : BaseAssetProperty
{
    public string name;
    public float time;
    public float volume;
    public float starttime;
    public bool isloop;
}

public class soundsAsset : TemplateAsset<soundsAsset, soundsProperty>
{

}
