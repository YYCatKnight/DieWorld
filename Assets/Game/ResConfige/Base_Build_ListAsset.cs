//----------------------------------------------
//  F2DEngine: time: 2019.29  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class Base_Build_ListProperty : BaseAssetProperty
{
   public string name;  //名字
   public int[] all_params;  //参数
   public int need_coin;  //需求的灵石
   public string prefabObj;
   public string desc;  //描述
}

public class Base_Build_ListAsset : TemplateAsset<Base_Build_ListAsset, Base_Build_ListProperty>
{

}
