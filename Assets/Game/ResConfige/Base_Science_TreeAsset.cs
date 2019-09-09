//----------------------------------------------
//  F2DEngine: time: 2019.29  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class Base_Science_TreeProperty : BaseAssetProperty
{
   public int science_type;  //科技类别ID
   public int level;  //等级
   public int type;  //科技类别
   public int need_coin;  //需要的灵石
   public int max_level;  //最大等级
   public int[] all_params;  //参数
   public string father_node;  //父节点
   public int need_father_level;  //需要父节点达到的等级
   public int[] node_pos;  //节点位置
   public string name;  //名字
   public string desc;  //描述
}

public class Base_Science_TreeAsset : TemplateAsset<Base_Science_TreeAsset, Base_Science_TreeProperty>
{

}
