//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace F2DEngine
{
    public class StringSerialize
    {
        public static string Serialize(object ob)
        {
            StringPack pack = new StringPack();
            SerializeConverter.Serialize(ob, pack);
            return pack.ToString();
        }

        public static object Deserialize(string str, Type type)
        {
            StringPack pack = new StringPack(str.Replace(" ", ""));
            return SerializeConverter.Deserialize(pack, type);
        }

        public static void Deserialize(string str, object obj)
        {
            StringPack pack = new StringPack(str.Replace(" ", ""));
            SerializeConverter.Deserialize(pack, obj);
        }

        public static T Deserialize<T>(string str)
        {
            StringPack pack = new StringPack(str);
            return SerializeConverter.Deserialize<T>(pack);
        }
    }
    public class BytesSerialize
    {
        public static void  Serialize(object ob, BytesPack bp)
        {
            SerializeConverter.Serialize(ob, bp);
        }
        public static BytesPack Serialize(object ob)
        { 
            BytesPack pack = new BytesPack();
            SerializeConverter.Serialize(ob, pack);
            return pack;
        }
        public static object Deserialize(BytesPack pack, Type type)
        {
            return SerializeConverter.Deserialize(pack, type);
        }
        public static void Deserialize(BytesPack pack, object obj)
        {
            SerializeConverter.Deserialize(pack, obj);
        }
        public static T Deserialize<T>(BytesPack pack)
        {
            return SerializeConverter.Deserialize<T>(pack);
        }

    }
    public interface SerializeTool
    {
        //序列化接口
        void StartSerialize();
        void EndSerialize();
        void PushArrayHead(int len);
        void PushBool(string name, bool value);
        void PushByte(string name, byte value);
        void PushInt(string name, int value);
        void PushFloat(string name, float value);
        void PushLong(string name, long value);
        void PushString(string name, string value);
        SerializeTool PushComplexBegin(string name,FFieldInfo info);//开始序列复杂结构
        void PushComplexEnd(string name, FFieldInfo info,SerializeTool tool);//结束序列复杂结构
        string ArrayIndexToName(int index);//数组转名字
        bool PushArray(string name, FFieldInfo info, IList list, bool isArray);
        void PushArrayEnd(string name, FFieldInfo info, IList list, bool isArray);
        void PushField(string name, FFieldInfo info,object obj);
        bool PushAutoCreate(string name,FFieldInfo info);

        //反序列化接口
        void StartDeserialize();
        void EndDeserialize();
        int PopArrayHead();
        bool PopBool(string name);
        byte PopByte(string name);
        int PopInt(string name);
        float PopFloat(string name);
        long PopLong(string name);
        string PopString(string name);
        object PopArray(string name, FFieldInfo info, IList list, bool isArray,out bool result);
        void PopArrayEnd(string name, FFieldInfo info, IList list, bool isArray);
        SerializeTool PopComplexBegin(string name, FFieldInfo info);
        void PopComplexEnd(string name, FFieldInfo info, SerializeTool tool);
        void PopField(string name, FFieldInfo info);
        bool PopAutoCreate(string name, FFieldInfo info);
    }
    public class  SerializeConverter
    {
        private static BaseConverter mConverter = new BaseConverter(); 
        #region 通用接口     
        public static void Serialize(object ob, SerializeTool tool)
        {
            mConverter.Serialize(ob, tool);
        }
        public static object Deserialize(SerializeTool tool, Type type)
        {
            return mConverter.Deserialize(tool,type,null);
        }

        public static T Deserialize<T>(SerializeTool tool)
        {
            return (T)Deserialize(tool, typeof(T));
        }
        //只能序列化类结构
        public static void Deserialize(SerializeTool tool, object obj)
        {
            mConverter.Deserialize(tool, obj.GetType(), obj);
        }

        #endregion    
    }
    public class BaseConverter
    {
        public void Serialize(object ob, SerializeTool tool)
        {
            tool.StartSerialize();
            FFieldInfo main = new FFieldInfo();
            main.GenericType1 = main;
            _AnalyzeType(ob.GetType(), main);
            _Serialize(null, main, ob, tool);
            tool.EndSerialize();
        }
        public  object Deserialize(SerializeTool tool,Type type,object obj)
        {
            tool.StartDeserialize();
            FFieldInfo main = new FFieldInfo();
            main.GenericType1 = main;
            _AnalyzeType(type, main);
            var value = _Deserialize(null, main, tool, obj);
            tool.EndDeserialize();
            return value;
        }

        #region 序列化
        protected virtual void _SerializeArray(string name, FFieldInfo info, IList list, SerializeTool tool,bool isArray)
        {
            if (list == null)
            {
                tool.PushArrayHead(0);
                return;
            }
            int count = list.Count;
            tool.PushArrayHead(count);
            if(tool.PushArray(name,info,list,isArray))
            {
                return;
            }

            switch (info.GenericType1.FType)
            {
                case FFieldType.F_Int:
                    if (isArray)
                    {
                        int[] int_values = (int[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushInt(tool.ArrayIndexToName(i), int_values[i]);
                        }
                    }
                    else
                    {
                        List<int> int_values = (List<int>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushInt(tool.ArrayIndexToName(i), int_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_Bool:
                    if (isArray)
                    {
                        bool[] bool_values = (bool[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushBool(tool.ArrayIndexToName(i), bool_values[i]);
                        }
                    }
                    else
                    {
                        List<bool> bool_values = (List<bool>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushBool(tool.ArrayIndexToName(i), bool_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_Byte:
                    if (isArray)
                    {
                        byte[] byte_values = (byte[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushByte(tool.ArrayIndexToName(i), byte_values[i]);
                        }
                    }
                    else
                    {
                        List<byte> byte_values = (List<byte>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushByte(tool.ArrayIndexToName(i), byte_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_Enum:
                    if (isArray)
                    {
                        int[] enum_values = (int[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushInt(tool.ArrayIndexToName(i), enum_values[i]);
                        }
                    }
                    else
                    {
                        List<int> enum_values = (List<int>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushInt(tool.ArrayIndexToName(i), enum_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_Float:
                    if (isArray)
                    {
                        float[] float_values = (float[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushFloat(tool.ArrayIndexToName(i), float_values[i]);
                        }
                    }
                    else
                    {
                        List<float> float_values = (List<float>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushFloat(tool.ArrayIndexToName(i), float_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_Long:
                    if (isArray)
                    {
                        long[] long_values = (long[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushLong(tool.ArrayIndexToName(i), long_values[i]);
                        }
                    }
                    else
                    {
                        List<long> long_values = (List<long>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushLong(tool.ArrayIndexToName(i), long_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_String:
                    if (isArray)
                    {
                        string[] string_values = (string[])list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushString(tool.ArrayIndexToName(i), string_values[i]);
                        }
                    }
                    else
                    {
                        List<string> string_values = (List<string>)list;
                        for (int i = 0; i < count; i++)
                        {
                            tool.PushString(tool.ArrayIndexToName(i), string_values[i]);
                        }
                    }
                    break;
                case FFieldType.F_List:
                case FFieldType.F_Array:
                case FFieldType.F_Dic:
                case FFieldType.F_Class:
                    for (int i = 0; i < count; i++)
                    {
                        _Serialize(tool.ArrayIndexToName(i), info.GenericType1, list[i], tool);
                    }
                    break;
                default:
                    break;
            }
            tool.PushArrayEnd(name, info, list, isArray);
        }
        protected virtual void _SerializeDic(string name, FFieldInfo info, IDictionary list, SerializeTool tool)
        {
            if (list == null)
            {
                tool.PushArrayHead(0);
                return;
            }
            int count = list.Count;
            tool.PushArrayHead(count * 2);
            var keys = list.Keys;
            var values = list.Values;
            object[] keyList = new object[count];
            object[] valueList = new object[count];
            int index = 0;
            foreach (var k in keys)
            {
                keyList[index++] = k;
            }

            index = 0;

            foreach (var k in values)
            {
                valueList[index++] = k;
            }

            for (int i = 0; i < count; i++)
            {
                _Serialize(tool.ArrayIndexToName(i * 2), info.GenericType1, keyList[i], tool);
                _Serialize(tool.ArrayIndexToName(i * 2 + 1), info.GenericType2, valueList[i], tool);
            }
        }
        protected virtual void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            object value = info.GetVlaue(obj);
            //value = info.Info != null ? info.Info.GetValue(obj) : obj;
            tool.PushField(name, info, value);
            if (info.FunAction != null)
            {
                info.FunAction._Serialize(name, info, value, tool);
            }
            else
            {
                switch (info.FType)
                {
                    case FFieldType.F_Int:
                        tool.PushInt(name, (int)value);
                        break;
                    case FFieldType.F_Bool:
                        tool.PushBool(name, (bool)value);
                        break;
                    case FFieldType.F_Byte:
                        tool.PushByte(name, (byte)value);
                        break;
                    case FFieldType.F_Enum:
                        tool.PushInt(name, (int)value);
                        break;
                    case FFieldType.F_Float:
                        tool.PushFloat(name, (float)value);
                        break;
                    case FFieldType.F_Long:
                        tool.PushLong(name, (long)value);
                        break;
                    case FFieldType.F_String:
                        tool.PushString(name, (string)value);
                        break;
                    case FFieldType.F_List:
                        var listTool = tool.PushComplexBegin(name, info);
                        _SerializeArray(name, info, (IList)value, listTool, false);
                        tool.PushComplexEnd(name, info, listTool);
                        break;
                    case FFieldType.F_Array:
                        var arrayTool = tool.PushComplexBegin(name, info);
                        _SerializeArray(name, info, (IList)value, arrayTool, true);
                        tool.PushComplexEnd(name, info, arrayTool);
                        break;
                    case FFieldType.F_Dic:
                        var dicTool = tool.PushComplexBegin(name, info);
                        _SerializeDic(name, info, (IDictionary)value, dicTool);
                        tool.PushComplexEnd(name, info, dicTool);
                        break;
                    case FFieldType.F_Class:
                        var temp = value;
                        if (temp == null)
                        {
                            if (tool.PushAutoCreate(name, info))
                            {
                                temp = System.Activator.CreateInstance(info.GenericType1.ClassType);
                            }
                            else
                            {
                                return;
                            }
                        }
                        var classTool = tool.PushComplexBegin(name, info);
                        for (int i = 0; i < info.GenericType1.MaxChildCount; i++)
                        {
                            var child = info.GenericType1.Childs[i];
                            _Serialize(child.Name, child, temp, classTool);
                        }
                        tool.PushComplexEnd(name, info, classTool);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region 反序列化
        protected virtual object _DeserializeArray(string name,FFieldInfo info, SerializeTool tool, bool isArray)
        {
            IList list = null;

            int count = tool.PopArrayHead();
            if (isArray)
            {
                list = Array.CreateInstance(info.GenericType1.ClassType, count);
            }
            else
            {
                Type[] tempTypes = info.ClassType.GetGenericArguments();
                var listType = typeof(List<>).MakeGenericType(tempTypes);
                list = (IList)Activator.CreateInstance(listType);
            }
            bool result = false;
            object obj = tool.PopArray(name, info, list, isArray,out result);
            if(result)
            {
                return obj;
            }

            switch (info.GenericType1.FType)
            {
                case FFieldType.F_Int:
                    if (isArray)
                    {
                        int[] int_values = (int[])list;
                        for (int i = 0; i < count; i++)
                        {
                            int_values[i] = tool.PopInt(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<int> int_values = (List<int>)list;
                        for (int i = 0; i < count; i++)
                        {
                            int_values.Add(tool.PopInt(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_Bool:
                    if (isArray)
                    {
                        bool[] bool_values = (bool[])list;
                        for (int i = 0; i < count; i++)
                        {
                            bool_values[i] = tool.PopBool(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<bool> bool_values = (List<bool>)list;
                        for (int i = 0; i < count; i++)
                        {
                            bool_values.Add(tool.PopBool(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_Byte:
                    if (isArray)
                    {
                        byte[] byte_values = (byte[])list;
                        for (int i = 0; i < count; i++)
                        {
                            byte_values[i] = tool.PopByte(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<byte> byte_values = (List<byte>)list;
                        for (int i = 0; i < count; i++)
                        {
                            byte_values.Add(tool.PopByte(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_Enum:
                    if (isArray)
                    {
                        int[] enum_values = (int[])list;
                        for (int i = 0; i < count; i++)
                        {
                            enum_values[i] = tool.PopInt(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<int> enum_values = (List<int>)list;
                        for (int i = 0; i < count; i++)
                        {
                            enum_values.Add(tool.PopInt(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_Float:
                    if (isArray)
                    {
                        float[] float_values = (float[])list;
                        for (int i = 0; i < count; i++)
                        {
                            float_values[i] = tool.PopFloat(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<float> float_values = (List<float>)list;
                        for (int i = 0; i < count; i++)
                        {
                            float_values.Add(tool.PopFloat(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_Long:
                    if (isArray)
                    {
                        long[] long_values = (long[])list;
                        for (int i = 0; i < count; i++)
                        {
                            long_values[i] = tool.PopLong(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<long> long_values = (List<long>)list;
                        for (int i = 0; i < count; i++)
                        {
                            long_values.Add(tool.PopLong(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_String:
                    if (isArray)
                    {
                        string[] string_values = (string[])list;
                        for (int i = 0; i < count; i++)
                        {
                            string_values[i] = tool.PopString(tool.ArrayIndexToName(i));
                        }
                    }
                    else
                    {
                        List<string> string_values = (List<string>)list;
                        for (int i = 0; i < count; i++)
                        {
                            string_values.Add(tool.PopString(tool.ArrayIndexToName(i)));
                        }
                    }
                    break;
                case FFieldType.F_List:
                case FFieldType.F_Array:
                case FFieldType.F_Dic:
                case FFieldType.F_Class:
                    if (isArray)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            list[i] = _Deserialize(tool.ArrayIndexToName(i), info.GenericType1, tool, null);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < count; i++)
                        {
                            list.Add(_Deserialize(tool.ArrayIndexToName(i), info.GenericType1, tool, null));
                        }
                    }
                    break;
                default:
                    break;
            }
            tool.PopArrayEnd(name, info, list, isArray);
            return list;
        }
        protected virtual object _DeserializeDic(string name,FFieldInfo info, SerializeTool tool)
        {
            Type[] tempTypes = info.ClassType.GetGenericArguments();
            var listType = typeof(Dictionary<,>).MakeGenericType(tempTypes);
            IDictionary list = (IDictionary)Activator.CreateInstance(listType);
            int count = tool.PopArrayHead();
            var keyType = tempTypes[0];
            var valueType = tempTypes[1];

            for (int i = 0; i < count; i++)
            {
                object keyValue = _Deserialize(tool.ArrayIndexToName(i), info.GenericType1, tool, null);
                i++;
                object valueValue = _Deserialize(tool.ArrayIndexToName(i), info.GenericType2, tool, null);
                list.Add(keyValue, valueValue);
            }
            return list;
        }
        protected virtual object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            tool.PopField(name,info);
            if(info.FunAction != null)
            {
               return  info.FunAction._Deserialize(name, info, tool, obj);
            }
            else
            {
                switch (info.FType)
                {
                    case FFieldType.F_Int:
                        return tool.PopInt(name);
                    case FFieldType.F_Bool:
                        return tool.PopBool(name);
                    case FFieldType.F_Byte:
                        return tool.PopByte(name);
                    case FFieldType.F_Enum:
                        return tool.PopInt(name);
                    case FFieldType.F_Float:
                        return tool.PopFloat(name);
                    case FFieldType.F_Long:
                        return tool.PopLong(name);
                    case FFieldType.F_String:
                        return tool.PopString(name);
                    case FFieldType.F_List:
                        var listTool = tool.PopComplexBegin(name, info);
                        var array = _DeserializeArray(name, info, listTool, false);
                        tool.PopComplexEnd(name, info, listTool);
                        return array;
                    case FFieldType.F_Array:
                        var arryTool = tool.PopComplexBegin(name, info);
                        var list = _DeserializeArray(name, info, arryTool, true);
                        tool.PopComplexEnd(name, info, arryTool);
                        return list;
                    case FFieldType.F_Dic:
                        var dicTool = tool.PopComplexBegin(name, info);
                        var dic = _DeserializeDic(name, info, dicTool);
                        tool.PopComplexEnd(name, info, dicTool);
                        return dic;
                    case FFieldType.F_Class:
                        if (obj == null)
                        {
                            if (tool.PopAutoCreate(name, info))
                            {
                                obj = System.Activator.CreateInstance(info.GenericType1.ClassType);
                            }
                            else
                            {
                                return null;
                            }
                        }

                        var classTool = tool.PopComplexBegin(name, info);
                        for (int i = 0; i < info.GenericType1.MaxChildCount; i++)
                        {
                            var child = info.GenericType1.Childs[i];
                            object t = _Deserialize(child.Name, child, classTool, null);
                            if (t != null)
                            {
                                child.SetValue(obj, t);
                                //child.Info.SetValue(obj, t);
                            }
                        }
                        tool.PopComplexEnd(name, info, classTool);
                        return obj;
                    default:
                        break;
                }
            }           
            return null;
        }
        #endregion

        #region 解析结构
        private static Int_FieldAction mIntAction =  new Int_FieldAction();
        private static Enum_FieldAction mEnumAction = new Enum_FieldAction();
        private static Bool_FieldAction mBoolAction = new Bool_FieldAction();
        private static Byte_FieldAction mByteAction = new Byte_FieldAction();
        private static Float_FieldAction mFloatAction = new Float_FieldAction();
        private static Long_FieldAction mLongAction = new Long_FieldAction();
        private static String_FieldAction mStringAction =  new String_FieldAction();
        //-------------------------------------------------------------------------------------
        private Dictionary<Type, FFieldInfo> mLoadField = new Dictionary<Type, FFieldInfo>();
        protected virtual void _AnalyzeType(Type FieldType, FFieldInfo info)
        {
            info.ClassType = FieldType;
            if (FieldType == typeof(bool))
            {
                info.FType = FFieldType.F_Bool;
                info.FunAction = mBoolAction;
            }
            else if (FieldType == typeof(byte))
            {
                info.FType = FFieldType.F_Byte;
                info.FunAction = mByteAction;
            }
            else if (FieldType == typeof(int))
            {
                info.FType = FFieldType.F_Int;
                info.FunAction = mIntAction;
            }
            else if (FieldType.IsEnum)
            {
                info.FType = FFieldType.F_Enum;
                info.FunAction = mEnumAction;
            }
            else if (FieldType == typeof(float))
            {
                info.FType = FFieldType.F_Float;
                info.FunAction = mFloatAction;
            }
            else if (FieldType == typeof(long))
            {
                info.FType = FFieldType.F_Long;
                info.FunAction = mLongAction;
            }
            else if (FieldType == typeof(string))
            {
                info.FType = FFieldType.F_String;
                info.FunAction = mStringAction;
            }
            else if (FieldType.IsGenericType)
            {
                if (FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type[] tempTypes = FieldType.GetGenericArguments();
                    FFieldInfo genericType = new FFieldInfo();
                    info.FType = FFieldType.F_List;
                    info.GenericType1 = genericType;
                    _AnalyzeType(tempTypes[0], genericType);

                }
                else if (FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type[] tempTypes = FieldType.GetGenericArguments();
                    FFieldInfo genericType1 = new FFieldInfo();
                    FFieldInfo genericType2 = new FFieldInfo();
                    info.FType = FFieldType.F_Dic;
                    info.GenericType1 = genericType1;
                    info.GenericType2 = genericType2;
                    _AnalyzeType(tempTypes[0], genericType1);
                    _AnalyzeType(tempTypes[1], genericType2);

                }
                else
                {
                    _AnalyzeClassType(FieldType, info);
                }
            }
            else
            {
                Type subType = FieldType.GetElementType();
                if (subType == null)
                {
                    _AnalyzeClassType(FieldType, info);
                }
                else if (FieldType.IsArray)
                {
                    FFieldInfo genericType = new FFieldInfo();
                    info.FType = FFieldType.F_Array;
                    info.GenericType1 = genericType;
                    _AnalyzeType(subType, genericType);
                }
            }
        }
        protected virtual void _AnalyzeClassType(Type FieldType, FFieldInfo info)
        {

            info.FType = FFieldType.F_Class;
            FFieldInfo genericType1 = null;
            lock (mLoadField)
            {
                if (mLoadField.TryGetValue(FieldType, out genericType1))
                {
                    info.GenericType1 = genericType1;
                }
                else
                {
                    genericType1 = new FFieldInfo();
                    info.GenericType1 = genericType1;
                    mLoadField[FieldType] = genericType1;
                    _AnalyzeFilelds(FieldType, genericType1);
                }
            }
        }
        protected virtual void _AnalyzeFilelds(Type FieldType, FFieldInfo info)
        {
            info.ClassType = FieldType;
            if (FieldType.IsDefined(typeof(FPropertySerialize), false))
            {
                PropertyInfo[] proInfos = FieldType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                info.Childs = new FFieldInfo[proInfos.Length];
                info.MaxChildCount = 0;
                foreach (PropertyInfo pro in proInfos)
                {
                    if (!(pro.CanWrite&&pro.CanRead))
                    {
                        continue;
                    }
                    FFieldInfo subInfo = new FFieldInfo();
                    subInfo.IsField = false;
                    subInfo.proInfo = pro;
                    subInfo.Name = pro.Name;
                    info.Childs[info.MaxChildCount++] = subInfo;
                    _AnalyzeType(pro.PropertyType, subInfo);
                }
            }
            else
            {
                FieldInfo[] fields = FieldType.GetFields(BindingFlags.Public | BindingFlags.Instance);
                info.Childs = new FFieldInfo[fields.Length];
                info.MaxChildCount = 0;
                foreach (FieldInfo field in fields)
                {
                    if (!CheckType(field))
                    {
                        continue;
                    }

                    FFieldInfo subInfo = new FFieldInfo();
                    subInfo.IsField = true;
                    subInfo.Info = field;
                    subInfo.Name = field.Name;
                    info.Childs[info.MaxChildCount++] = subInfo;
                    _AnalyzeType(field.FieldType, subInfo);
                }
            }
        }
        protected virtual bool CheckType(FieldInfo field)
        {
            if (field.IsNotSerialized)
                return false;
            return true;
        }
        #endregion
    }
    public enum FFieldType
    {
        F_Int,
        F_Bool,
        F_Byte,
        F_Enum,
        F_Float,
        F_Long,
        F_String,
        F_List,
        F_Array,
        F_Dic,
        F_Class,
    }
    public interface FieldAciton
    {
        void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool);
        object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null);
    }
    public class Int_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushInt(name, (int)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopInt(name);
        }
    }
    public class Enum_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushInt(name, (int)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopInt(name);
        }
    }
    public class Bool_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushBool(name, (bool)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopBool(name);
        }
    }
    public class Byte_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushByte(name, (byte)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopByte(name);
        }
    }
    public class Float_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushFloat(name, (float)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopFloat(name);
        }
    }
    public class Long_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushLong(name, (long)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopLong(name);
        }
    }
    public class String_FieldAction : FieldAciton
    {
        public void _Serialize(string name, FFieldInfo info, object obj, SerializeTool tool)
        {
            tool.PushString(name, (string)obj);
        }
        public object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            return tool.PopString(name);
        }
    }
    public class FFieldInfo
    {
        public bool IsField = true;
        public bool IsCom = false;
        public FFieldType FType;
        public FieldAciton FunAction;
        public string Name;
        public FieldInfo Info;
        public PropertyInfo proInfo;
        public Type ClassType;
        public FFieldInfo GenericType1;
        public FFieldInfo GenericType2;
        public FFieldInfo[] Childs;
        public int MaxChildCount = 0;
        private INamedMemberAccessor mProAccessor;
        public object GetVlaue(object obj)
        {
            //if (IsField)
            //{
            //    value = Info != null ? Info.GetValue(obj) : obj;
            //}
            if (proInfo == null && Info == null)
                return obj;

            if (IsCom)
            {
                if (mProAccessor == null)
                {
                    _CreateAccessor(obj.GetType(), ClassType);
                }
                return mProAccessor.GetValue(obj);
            }
            else
            {
                return IsField ? Info.GetValue(obj) : proInfo.GetValue(obj, null);
            }
        }
        private void _CreateAccessor(Type mian, Type obj)
        {
            if (mProAccessor == null)
            {
                mProAccessor = Activator.CreateInstance(typeof(PropertyAccessor<,>).MakeGenericType(mian, obj), this) as INamedMemberAccessor;
            }
        }
        public void SetValue(object main, object obj)
        {
            if (proInfo == null && Info == null)
                return;

            if (IsCom)
            {
                if (mProAccessor == null)
                {
                    _CreateAccessor(main.GetType(), ClassType);
                }
                mProAccessor.SetValue(main, obj);
            }
            else
            {
                if (IsField)
                {
                    Info.SetValue(main, obj);
                }
                else
                {
                    proInfo.SetValue(main, obj, null);
                }
            }
        }
    }
    internal interface INamedMemberAccessor
    {
        object GetValue(object instance);
        void SetValue(object instance, object newValue);
    }
    internal class PropertyAccessor<T,P> : INamedMemberAccessor
    {
        private Func<T, P> GetValueDelegate;
        private Action<T, P> SetValueDelegate;
        private FFieldInfo mFFieldInfo;
        public PropertyAccessor(FFieldInfo info)
        {
            mFFieldInfo = info;
        }

        public object GetValue(object instance)
        {
            if(GetValueDelegate == null)
            {
                CreateGetter();
            }
            return GetValueDelegate((T)instance);
        }

        public void SetValue(object instance, object newValue)
        {
            if(SetValueDelegate == null)
            {
                CreateSetter();
            }
            SetValueDelegate((T)instance, (P)newValue);
        }

        private void  CreateGetter()
        {
            if (mFFieldInfo.IsField)
            {
                DynamicMethod setterMethod = new DynamicMethod(string.Empty, typeof(P), new Type[1] { typeof(T) }, true);
                ILGenerator gen = setterMethod.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, mFFieldInfo.Info);
                gen.Emit(OpCodes.Ret);
                GetValueDelegate = (Func<T, P>)setterMethod.CreateDelegate(typeof(Func<T, P>));
            }
            else
            {
                GetValueDelegate = (Func<T, P>)Delegate.CreateDelegate(typeof(Func<T, P>), mFFieldInfo.proInfo.GetGetMethod());
            }
        }

        private void CreateSetter()
        {
            if (mFFieldInfo.IsField)
            {
               
                DynamicMethod setterMethod = new DynamicMethod(string.Empty, typeof(void), new Type[2] { typeof(T), typeof(P) }, true);
                ILGenerator gen = setterMethod.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, mFFieldInfo.Info);
                gen.Emit(OpCodes.Ret);
                SetValueDelegate = (Action<T, P>)setterMethod.CreateDelegate(typeof(Action<T, P>));
            }
            else
            {
                SetValueDelegate = (Action<T, P>)Delegate.CreateDelegate(typeof(Action<T, P>), mFFieldInfo.proInfo.GetSetMethod());
            }
        }

    }
    #region 扩展接口  
    public class StringAnalyisis
    {
        public class StringPatch
        {
            public int sIndex;//标记起始位置
            public int startIndex;//真实其实位置
            public bool isSetName = false;//是否设置key了
            public int count;//字符数量
            public StringPatch parent;//父节点
            public Dictionary<string, StringPatch> Vluaes = new Dictionary<string, StringPatch>();
            private int mPosIndex = 0;
            public void Clear()
            {
                sIndex = 0;
                count = 0;
                startIndex = 0;
                isSetName = false;
                parent = null;
                mPosIndex = 0;
                Vluaes.Clear();
            }


            public StringPatch GetPatchByName(string name)
            {
                StringPatch tv = null;
                Vluaes.TryGetValue(name, out tv);
                return tv;
            }

            public StringPatch GetAutoPatch(string name)
            {
                StringPatch tv = null;
                if (isSetName)
                {
                    Vluaes.TryGetValue(name, out tv);
                }
                else
                {
                    Vluaes.TryGetValue(mPosIndex.ToString(), out tv);
                    mPosIndex++;
                }
                return tv;
            }
        }
        private const char KF = '{';
        private const char KE = '}';
        private const char DH = ',';
        public void Reset()
        {
            mHeadPath = null;
            mPosPath = null;
            mArrayIndex = 0;
        }
        private StringPatch _Analyisis(StringPatch curencrypt, string value, int i, int sOffset, bool start)
        {
            StringPatch nld = null;
            int sKey = i - sOffset;
            int lastKey = 0;
            while (i > sKey)
            {
                lastKey = sKey;
                int nextIndex = value.IndexOf(DH, lastKey, i - lastKey);

                if (nextIndex != -1)
                {
                    sKey = nextIndex;
                }
                else
                {
                    sKey = i;
                }

                nld = new StringPatch();
                if (start)
                {
                    nld.sIndex = i;
                }
                nld.parent = curencrypt;

                int twoIndex = value.IndexOf('=', lastKey, sKey - lastKey);
                if (twoIndex != -1)
                {
                    curencrypt.Vluaes[value.Substring(lastKey, twoIndex - lastKey)] = nld;
                    curencrypt.isSetName = true;
                    nld.startIndex = twoIndex + 1;
                    nld.count = sKey - twoIndex - 1;
                }
                else
                {
                    curencrypt.Vluaes[curencrypt.Vluaes.Count.ToString()] = nld;
                    nld.startIndex = lastKey;
                    nld.count = sKey - lastKey;
                }
                sKey++;
            }
            return nld;
        }
        private StringPatch Analyisis(string value)
        {
            StringPatch head = new StringPatch();
            StringPatch pos = head;
            int sOffset = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char tchar = value[i];
                if (tchar == KF)
                {
                    StringPatch nld = null;                  
                    if(sOffset != 0)
                    {
                        var dValue = _Analyisis(pos, value, i, sOffset, true);
                        if (dValue != null)
                        {
                            if (value[i - 1] == '=')
                            {
                                nld = dValue;
                            }
                        }
                    }
                    if(nld == null)
                    {
                        nld = new StringPatch();
                        nld.sIndex = i;
                        pos.Vluaes[pos.Vluaes.Count.ToString()] = nld;
                    }
                    nld.parent = pos;
                    pos = nld;
                    sOffset = 0;
                }
                else if (tchar == KE)
                {
                    pos.startIndex = pos.sIndex + 1;
                    pos.count = i - pos.sIndex - 1;
                    _Analyisis(pos, value, i, sOffset, false);
                    //新增加，下一个如果为,则i++,
                    if (i + 1 < value.Length)
                    {
                        if (value[i + 1] == DH)
                        {
                            i++;
                        }
                    }
                    pos = pos.parent;
                    sOffset = 0;
                }
                else
                {
                    sOffset++;
                }
            }
            return head;
        }
        private StringPatch mHeadPath;
        private StringPatch mPosPath;
        private string mValue;
        private int mArrayIndex = 0;
        public void Encrypt(string value)
        {
            mValue = value;
            mHeadPath = Analyisis(value);
            mPosPath = mHeadPath;
        }
        public string GetValue(string name)
        {
            StringPatch tv = GetPatch(name);
            return GetPatchValue(tv);
        }
        public int GetPatchNum()
        {
            return mPosPath.Vluaes.Count;
        }
        private StringPatch GetPatch(string name)
        {
            if(name == null)
            {
                name = mArrayIndex.ToString();
            }
            return mPosPath.GetAutoPatch(name);
        }
        public void MoveNextPath(string name)
        {
            var dat = GetPatch(name);
            if (dat == null)
            {
                dat = new StringPatch();
                dat.parent = mPosPath;
            }
            mPosPath = dat;
        }
        public void MoveParent()
        {
            mPosPath = mPosPath.parent;
        }
        public string SetArrayIndex(int index)
        {
            mArrayIndex = index;
            return null;
        }
        public string GetPatchValue(StringPatch patch)
        {
            if (patch != null)
            {
                return mValue.Substring(patch.startIndex, patch.count);
            }
            return null;
        }
        public StringPatch GetHead()
        {
            return mHeadPath;
        }
        public StringPatch GetPos()
        {
            return mPosPath;
        }
    }
    public class StringPack : SerializeTool
    {   
        public StringPack()
        {

        }
        public StringPack(string value)
        {
            mValue = value;
        }
        private string mValue;
        private  StringAnalyisis mEncrypt = new StringAnalyisis();

        //反序列化---------------------------------------------------------
        public void StartDeserialize()
        {
            mEncrypt.Reset();
            mEncrypt.Encrypt(mValue);
        }

        public void EndDeserialize()
        {
          
        }

        public string ArrayIndexToName(int index)
        {
            mEncrypt.SetArrayIndex(index);
            return null;
        }

        public int PopArrayHead()
        {
            return mEncrypt.GetPatchNum();
        }

        public bool PopBool(string name)
        {
            var data = mEncrypt.GetValue(name);
            bool result = false;
            if(bool.TryParse(data,out result))
            {
                return result;
            }
            return true;
        }

        public byte PopByte(string name)
        {
            var data = mEncrypt.GetValue(name);
            byte result = 0;
            if(byte.TryParse(data,out result))
            {
                return result;
            }
            return 0;
        }

        public SerializeTool  PopComplexBegin(string name, FFieldInfo info)
        {
            mEncrypt.MoveNextPath(name);
            return this;
        }

        public void PopComplexEnd(string name, FFieldInfo info,SerializeTool tool)
        {
            mEncrypt.MoveParent();
        }

        public float PopFloat(string name)
        {
            var data = mEncrypt.GetValue(name);
            float result = 0;
            if (float.TryParse(data, out result))
            {
                return result;
            }
            return 0;
        }

        public int PopInt(string name)
        {
            var data = mEncrypt.GetValue(name);
            int result = 0;
            if (int.TryParse(data, out result))
            {
                return result;
            }
            return 0;
        }

        public long PopLong(string name)
        {
            var data = mEncrypt.GetValue(name);
            long result = 0;
            if (long.TryParse(data, out result))
            {
                return result;
            }
            return 0;
        }

        public string PopString(string name)
        {
            var data = mEncrypt.GetValue(name);
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            return data;
        }

        private const string DLK = "={";
        private const string RKD = "},";
        private const string DD = "=";
        private const string DH = ",";
        private const string KK = "{";
        private  StringBuilder mBuildBuff;

        public override string ToString()
        {
            if (mValue != null)
            {
                return mValue;
            }
            else
            {
                return "";
            }
        }

        public void StartSerialize()
        {
            mBuildBuff = new StringBuilder(256);
        }

        public void EndSerialize()
        {
            removeEnd();
            mValue = mBuildBuff.ToString();
        }

        public void PushArrayHead(int len)
        {
            
        }

        public void PushBool(string name, bool value)
        {
            SetFormat(mBuildBuff, name, value.ToString());
        }

        public void PushByte(string name, byte value)
        {
            SetFormat(mBuildBuff, name, value.ToString());
        }

        public SerializeTool PushComplexBegin(string name,FFieldInfo info)
        {
            if (name == null)
            {
                mBuildBuff.Append(KK);
            }
            else
            {
                mBuildBuff.Append(name);
                mBuildBuff.Append(DLK);
            }
            return this;
        }

        public void PushComplexEnd(string name,FFieldInfo info,SerializeTool tool)
        {
            removeEnd();
            mBuildBuff.Append(RKD);
        }

        protected virtual void removeEnd()
        {
            int endIndex = mBuildBuff.Length - 1;
            if (endIndex > 0)
            {
                if (mBuildBuff[endIndex] == ',')
                {
                    mBuildBuff.Length = mBuildBuff.Length - 1;
                }
            }
        }

        public void PushFloat(string name, float value)
        {
            SetFormat(mBuildBuff, name, value.ToString());
        }

        public void PushInt(string name, int value)
        {
            SetFormat(mBuildBuff, name,value.ToString());
        }

        public void PushLong(string name, long value)
        {
            SetFormat(mBuildBuff, name, value.ToString());
        }

        public void PushString(string name, string value)
        {
            SetStringFormat(mBuildBuff,name,value);
        }

        protected virtual void SetStringFormat(StringBuilder builder,string name, string value)
        {
            if (name == null)
            {
                builder.Append(KK);
                builder.Append(value);
                builder.Append(RKD);
            }
            else
            {
                builder.Append(name);
                builder.Append(DLK);
                builder.Append(value);
                builder.Append(RKD);
            }
        }

        protected virtual void SetFormat(StringBuilder builder, string name, string value)
        {
            if (name == null)
            {
                builder.Append(value);
                builder.Append(DH);
            }
            else
            {
                builder.Append(name);
                builder.Append(DD);
                builder.Append(value);
                builder.Append(DH);
            }
        }

        public bool PushArray(string name, FFieldInfo info, IList list, bool isArray)
        {
            return false;
        }

        public void PushArrayEnd(string name, FFieldInfo info, IList list, bool isArray)
        {
            
        }

        public object PopArray(string name, FFieldInfo info, IList list, bool isArray,out bool result)
        {
            result = false;
            return null;
        }

        public void PopArrayEnd(string name, FFieldInfo info, IList list, bool isArray)
        {
            
        }

        public void PushField(string name, FFieldInfo info, object obj)
        {
            
        }

        public void PopField(string name, FFieldInfo info)
        {
            
        }

        public bool PushAutoCreate(string name, FFieldInfo info)
        {
            return true;
        }

        public bool PopAutoCreate(string name, FFieldInfo info)
        {
            return true;
        }
    }
    public class BytesPack : SerializeTool
    {
        protected byte[] _ByteBuf;
        protected int _streamLen = 0;
        protected int _position = 0;
        public void CreateBytes(int len = 1024)
        {
            _ByteBuf = new byte[1024*1024*10];
            _position = 0;
            _streamLen = 0;
        }

        public void Reset()
        {
            _position = 0;
            _streamLen = 0;
        }

        public  BytesPack()
        {

        }
        public bool IsOver()
        {
            return _position >= _streamLen;
        }

        public void BeginSeek()
        {
            _position = 0;
        }

        public void CreateReadBytes(byte[] buf,int offset = 0)
        {
            _ByteBuf = buf;
            _position = offset;
            _streamLen = buf.Length;
        }

        public int GetStreamLen()
        {
            return _streamLen;
        }
        public byte[] GetStream()
        {
            return _ByteBuf;
        }

        public byte[] GetRealByte()
        {
            byte[] re = new byte[_streamLen];
            Array.Copy(_ByteBuf, re, _streamLen);
            return re;
        }

        public void StartSerialize()
        {
            if (_ByteBuf == null)
            {
                CreateBytes();

            }
        }

        public void EndSerialize()
        {
           
        }

        public void PushBytes(byte[] bytes)
        {
            PushInt("",bytes.Length);
            _PushByteArray(bytes);
        }

        private void _AutoCheckLen(int len = 0)
        {
            
        }

        private void _PushByteArray(byte[] sourceBytes)
        {
            int offset = _streamLen;
            int len = sourceBytes.Length;
            _AutoCheckLen(len);
            Array.Copy(sourceBytes, 0, _ByteBuf, _streamLen, len);
            _streamLen = offset + len;
        }

        public  string ArrayIndexToName(int index)
        {
            return null;
        }

        public int PopArrayHead()
        {
            return PopInt("");
        }

        public bool PopBool(string name)
        {
            byte ret = _ByteBuf[_position++];
            return (ret == 1);
        }

        public byte PopByte(string name)
        {
            byte ret = _ByteBuf[_position];
            _position += 1;
            return ret;
        }

        public SerializeTool  PopComplexBegin(string name, FFieldInfo info)
        {
            return this;
        }
        public void PopComplexEnd(string name, FFieldInfo info, SerializeTool tool)
        {

        }

        public float PopFloat(string name)
        {
            float ret = BitConverter.ToSingle(_ByteBuf, _position); 
            _position += 4;
            return ret;
        }

        public int PopInt(string name)
        {
            int ret = BitConverter.ToInt32(_ByteBuf, _position);
            _position += 4;
            return ret;
        }

        public long PopLong(string name)
        {
            long ret = BitConverter.ToInt64(_ByteBuf, _position);
            _position += 8;
            return ret;
        }

        public string PopString(string name)
        {
            int len = PopInt(null);
            var str = System.Text.Encoding.UTF8.GetString(_ByteBuf, _position, len);
            _position += len;
            return str;
        }

        public byte[] PopBytes()
        {
            int len = PopInt(null);
            return _PopByteArray(len);
        }

        private byte[] _PopByteArray(int Length)
        {
            byte[] ret = new byte[Length];
            Array.Copy(_ByteBuf, _position, ret, 0, Length);
            //提升位置
            _position += Length;
            return ret;
        }

        public void StartDeserialize()
        {

        }

        public void EndDeserialize()
        {
           
        }

        /// //////////////////////////////////////////////////////////////
        public void PushArrayHead(int len)
        {
            PushInt(null,len);
        }

        public void PushBool(string name, bool value)
        {
            _AutoCheckLen();
            _ByteBuf[_streamLen++] = (byte)(value ? 1 : 0);
        }

        public void PushByte(string name, byte value)
        {
            _AutoCheckLen();
            _ByteBuf[_streamLen++] = value;
        }

        public SerializeTool PushComplexBegin(string name,FFieldInfo info)
        {
            return this;
        }

        public void PushComplexEnd(string name,FFieldInfo info,SerializeTool tool)
        {

        }

        public  void PushFloat(string name, float value)
        {
            _AutoCheckLen();
            byte[] bufs = BitConverter.GetBytes(value);
            _ByteBuf[_streamLen++] = bufs[0];
            _ByteBuf[_streamLen++] = bufs[1];
            _ByteBuf[_streamLen++] = bufs[2];
            _ByteBuf[_streamLen++] = bufs[3];
        }

        public  void PushInt(string name, int value)
        {
            _AutoCheckLen();
            //fixed (byte* b = &_ByteBuf[_streamLen])
            //{
            //    *((int*)b) = value;
            //}
            //_streamLen += 4;

            _ByteBuf[_streamLen++] = (byte)(value >> 0);
            _ByteBuf[_streamLen++] = (byte)(value >> 8);
            _ByteBuf[_streamLen++] = (byte)(value >> 16);
            _ByteBuf[_streamLen++] = (byte)(value >> 24);

            //byte[] bufs = BitConverter.GetBytes(value);
            //_ByteBuf[_streamLen++] = bufs[0];
            //_ByteBuf[_streamLen++] = bufs[1];
            //_ByteBuf[_streamLen++] = bufs[2];
            //_ByteBuf[_streamLen++] = bufs[3];
        }

        public  void PushLong(string name, long value)
        {
            _AutoCheckLen();
            //fixed (byte* b = &_ByteBuf[_streamLen])
            //{
            //    *((long*)b) = value;
            //}
            //_streamLen += 8;
            _ByteBuf[_streamLen++] = (byte)(value >> 0);
            _ByteBuf[_streamLen++] = (byte)(value >> 8);
            _ByteBuf[_streamLen++] = (byte)(value >> 16);
            _ByteBuf[_streamLen++] = (byte)(value >> 24);
            _ByteBuf[_streamLen++] = (byte)(value >> 32);
            _ByteBuf[_streamLen++] = (byte)(value >> 40);
            _ByteBuf[_streamLen++] = (byte)(value >> 48);
            _ByteBuf[_streamLen++] = (byte)(value >> 56);

            //byte[] bufs = BitConverter.GetBytes(value);
            //_ByteBuf[_streamLen++] = bufs[0];
            //_ByteBuf[_streamLen++] = bufs[1];
            //_ByteBuf[_streamLen++] = bufs[2];
            //_ByteBuf[_streamLen++] = bufs[3];
            //_ByteBuf[_streamLen++] = bufs[4];
            //_ByteBuf[_streamLen++] = bufs[5];
            //_ByteBuf[_streamLen++] = bufs[6];
            //_ByteBuf[_streamLen++] = bufs[7];
        }

        public void PushString(string name, string value)
        {
            if (value == null)
            {
                value = "";
            }
            int len = value.Length;
            int size = System.Text.Encoding.UTF8.GetBytes(value,0,len, _ByteBuf,_streamLen+4);
            PushInt("", size);
            _streamLen += size;
        }

        public bool PushArray(string name, FFieldInfo info, IList list, bool isArray)
        {
            return false;
        }

        public void PushArrayEnd(string name, FFieldInfo info, IList list, bool isArray)
        {
            
        }

        public object PopArray(string name, FFieldInfo info, IList list, bool isArray, out bool result)
        {
            result = false;
            return null;
        }

        public void PopArrayEnd(string name, FFieldInfo info, IList list, bool isArray)
        {
            
        }

        public void PushField(string name, FFieldInfo info, object obj)
        {
           
        }

        public void PopField(string name, FFieldInfo info)
        {
            
        }

        public bool PushAutoCreate(string name, FFieldInfo info)
        {
            return true;
        }

        public bool PopAutoCreate(string name, FFieldInfo info)
        {
            return true;
        }
    }
    #endregion

}
