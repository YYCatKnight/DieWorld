//----------------------------------------------
//  F2DEngine: time: 2019.30  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class Living_Buff_TypeProperty : BaseAssetProperty
{
   public string buff_name;  //buff名字
   public string buff_desc;  //buff描述
   public int add_type;  //叠加状态
   public int show_type; //显示状态
   public string icon;  //显示图片
}

public class Living_Buff_TypeAsset : TemplateAsset<Living_Buff_TypeAsset, Living_Buff_TypeProperty>
{

}
