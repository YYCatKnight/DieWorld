using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using F2DEngine;
using System;

public abstract class FEngineDrawerEditor<T> : PropertyDrawer where T : FEngineAttribute
{
    protected T TargetAttribute { get { return (T)attribute; } }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        SerializedPropertyType st = GetIsPropertyType();
        if (st != SerializedPropertyType.Generic && property.propertyType != st)
        {
            EditorGUI.LabelField(position, label.text, "类型使用错误");
        }
        else
        {
            OnGUIEX(position, property, label);
        }
    }

    public virtual void OnGUIEX(Rect position, SerializedProperty property, GUIContent label)
    {

    }

    public virtual SerializedPropertyType GetIsPropertyType()
    {
        return SerializedPropertyType.Generic;
    }

}


[CustomPropertyDrawer(typeof(FIntAttr))]
public class FIntAttrDrawer : FEngineDrawerEditor<FIntAttr>
{
    public override void OnGUIEX(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
    {
        property.intValue =  (int)EditorGUI.Slider(position, label.text+"  ("+ TargetAttribute.minNum.ToString()+"~"+ TargetAttribute.maxNum.ToString()+")",property.intValue, TargetAttribute.minNum, TargetAttribute.maxNum);
    }

    public override SerializedPropertyType GetIsPropertyType()
    {
        return SerializedPropertyType.Integer;
    }
}

[CustomPropertyDrawer(typeof(FFloatAttr))]
public class FFloatAttrDrawer : FEngineDrawerEditor<FFloatAttr>
{
    public override void OnGUIEX(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
    {
        property.floatValue = EditorGUI.Slider(position, label.text + "  (" + TargetAttribute.minNum.ToString() + "~" + TargetAttribute.maxNum.ToString() + ")", property.floatValue, TargetAttribute.minNum, TargetAttribute.maxNum);
    }

    public override SerializedPropertyType GetIsPropertyType()
    {
        return SerializedPropertyType.Float;
    }
}


[CustomPropertyDrawer(typeof(FStringAttr))]
public class FStringAttrDrawer : FEngineDrawerEditor<FStringAttr>
{
    public int GetSelectIndex(string name)
    {
        for(int i = 0; i < TargetAttribute.StrBuffs.Length;i++)
        {
            if (TargetAttribute.StrBuffs[i] == name)
                return i;
        }
        return 0;
    }

    public override SerializedPropertyType GetIsPropertyType()
    {
        return SerializedPropertyType.String;
    }

    public override void OnGUIEX(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
    {
        int index = GetSelectIndex(property.stringValue);
        index =  EditorGUI.Popup(position,label.text, index, TargetAttribute.StrBuffs);
        if(index < TargetAttribute.StrBuffs.Length)
        {
            property.stringValue = TargetAttribute.StrBuffs[index];
        }
    }
}

[CustomPropertyDrawer(typeof(FRenameAttr))]
public class FRenameAttrDrawer : FEngineDrawerEditor<FRenameAttr>
{
    public override SerializedPropertyType GetIsPropertyType()
    {
        return SerializedPropertyType.Generic;
    }

    private Dictionary<string, string> SetUpCustomEnumNames(SerializedProperty property, string[] enumNames)
    {
        Dictionary<string, string> tempNames = new Dictionary<string, string>();
        System.Type enumType = fieldInfo.FieldType;
        if(enumType.IsGenericType)
        {
            Type[] tempTypes = enumType.GetGenericArguments();
            enumType = tempTypes[0];
        }
        else if(enumType.IsArray)
        {
            enumType = enumType.GetElementType();
        }

        foreach (string e in enumNames)
        {
            var field = enumType.GetField(e);
            if (field == null) continue;

            FRenameAttr[] attrs = (FRenameAttr[])field.GetCustomAttributes(typeof(FRenameAttr), false);

            if (attrs.Length == 0)
            {
                tempNames[e] = e;
            }
            else
            {
                if (!tempNames.ContainsKey(e))
                {
                    foreach (FRenameAttr labelAttribute in attrs)
                    {
                       tempNames.Add(e,labelAttribute.nName);
                    }
                }
            }
        }
        return tempNames;
    }


    public override void OnGUIEX(UnityEngine.Rect position, SerializedProperty property, UnityEngine.GUIContent label)
    {

        string tempName = TargetAttribute.nName;
        Type enumType = fieldInfo.FieldType;
        if (enumType.IsGenericType||enumType.IsArray)
        {
            tempName = label.text;
        }
        //枚举重新
        if (property.propertyType == SerializedPropertyType.Enum)
        {
            Dictionary<string, string> tempBuffs = SetUpCustomEnumNames(property, property.enumNames);
            string[] tempItems = new string[tempBuffs.Count];
            tempBuffs.Values.CopyTo(tempItems,0);
            property.enumValueIndex = EditorGUI.Popup(position, tempName, property.enumValueIndex, tempItems);
        }
        else
        {
            EditorGUI.PropertyField(position, property, new GUIContent(tempName));
        }
    }

}

