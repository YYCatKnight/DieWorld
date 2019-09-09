//----------------------------------------------
//  F2DEngine: time: 2019.29  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class People_Profession_PoolProperty : BaseAssetProperty
{
   public string name;
}

public class People_Profession_PoolAsset : TemplateAsset<People_Profession_PoolAsset, People_Profession_PoolProperty>
{

}
