//----------------------------------------------
//  F2DEngine: time: 2019.29  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class People_Talent_PoolProperty : BaseAssetProperty
{
   public string name;
   public string desc;
   public string buff_list;
   public string all_params;
}

public class People_Talent_PoolAsset : TemplateAsset<People_Talent_PoolAsset, People_Talent_PoolProperty>
{

}
