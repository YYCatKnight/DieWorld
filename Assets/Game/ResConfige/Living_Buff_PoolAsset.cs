//----------------------------------------------
//  F2DEngine: time: 2019.30  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class Living_Buff_PoolProperty : BaseAssetProperty
{
   public int buff_type;  //buff类别
   public int level;  //buff等级
   public int[] all_params;  //string
}

public class Living_Buff_PoolAsset : TemplateAsset<Living_Buff_PoolAsset, Living_Buff_PoolProperty>
{

}
