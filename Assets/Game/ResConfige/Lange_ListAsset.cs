//----------------------------------------------
//  F2DEngine: time: 2019.29  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using F2DEngine;


[Serializable]
public class Lange_ListProperty : BaseAssetProperty
{
   public string key;  //索引
   public string chinese;  //中文
   public string english;  //英文
}

public class Lange_ListAsset : TemplateAsset<Lange_ListAsset, Lange_ListProperty>
{
    public static LanguageType Language { get; protected set; }
    public static Func<Lange_ListProperty, string> LangCallBack;

    public static void SetLanguageType(LanguageType type)
    {
        Language = type;
        LangCallBack = ChinaCallBack;
        switch(type)
        {
            case LanguageType.None:
            case LanguageType.SimplifiedChinese:
                LangCallBack = ChinaCallBack;
                break;
            case LanguageType.English:
                LangCallBack = EnglishCallBack;
                break;
        }
    }

    public string GetValue(string key)
    {
        if (key != null)
        {
            Lange_ListProperty lp = GetProperty(key);
            if (lp != null)
            {
                string endStr = LangCallBack(lp);
                if (string.IsNullOrEmpty(endStr))
                {
                    Debug.Log("[" + key + "]的语言为空");
                    return key;
                }
                else
                {
                    return endStr;
                }
            }
            else
            {
                string endStr = "缺少" + "[" + key + "]";
                return endStr;
            }
        }
        else
        {
            Debug.LogError("语言获取传入的key为null");
            return "NULL";
        }
    }

    protected static string ChinaCallBack(Lange_ListProperty pro)
    {
        return pro.chinese;
    }

    protected static string EnglishCallBack(Lange_ListProperty pro)
    {
        return pro.english;
    }
}
