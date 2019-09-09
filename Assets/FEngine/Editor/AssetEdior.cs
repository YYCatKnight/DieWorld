using ExcelDataReader;
using F2DEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AssetEdior
{

    public class ExcelAsset
    {
        public class ExcelPath
        {
            public string FilePath { get; protected set; }
            public string ClassName { get; protected set; }
            public string AssetOutPath { get; protected set; }
            public string CSPath { get; protected set; }
            public string DirPath { get; protected set; }
            public ExcelPath(string excelName, string path = null)
            {
                if (string.IsNullOrEmpty(path))
                {
                    FilePath = Application.dataPath + "/" + ResConfig.EXCELFILEPATH + "/" + excelName + ".xlsx";
                }
                else
                {
                    FilePath = path + "/" + excelName + ".xlsx";
                }
                ClassName = excelName.Replace("Asset", "") + "Asset";
                DirPath = "Assets/" + ResConfig.ASSETBUNDLE + "/" + ResConfig.ASSET_PAHT + "[" + FPackageEditor.F_EX + "]";
                AssetOutPath = DirPath + "/" + ClassName + ".asset";
                CSPath = Application.dataPath + "/Game/ResConfige";
            }
        }
        private List<Dictionary<string, string>> mDatas = new List<Dictionary<string, string>>();//内容
        public ExcelPath mPath;
        private List<string> mName = new List<string>();//名字
        private List<string> mContexts = new List<string>();//描述
        private List<string> mTypes = new List<string>();//类型
        private List<string> mOthers = new List<string>();//其他
        private BaseAsset mValue;
        public ExcelAsset(string excelName, string path = null)
        {
            mPath = new ExcelPath(excelName, path);
            var ass = Assembly.Load("Assembly-CSharp");
            var type = ass.GetType(mPath.ClassName);
            if (type != null)
            {
                mValue = (BaseAsset)ScriptableObject.CreateInstance(type);
            }
        }
        private List<string> _GetRow(IExcelDataReader excelReader)
        {
            List<string> rowList = new List<string>();
            for (int i = 0; i < excelReader.FieldCount; i++)
            {
                string value = excelReader.IsDBNull(i) ? "" : excelReader[i].ToString();
                value = value.Trim();
                rowList.Add(value);
            }
            return rowList;
        }
        private void _LoadExcel()
        {
            FileStream stream = File.Open(mPath.FilePath, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream);
             
            if (excelReader.Read())
            {
                mName = _GetRow(excelReader);
                mName[0] = "Only_id";
                if (excelReader.Read())
                {
                    mContexts = _GetRow(excelReader);
                    if (excelReader.Read())
                    {
                        //mTypes = _GetRow(excelReader);
                        if (excelReader.Read())
                        {
                            mOthers = _GetRow(excelReader);
                            while (excelReader.Read())
                            {
                                //正文内容
                                Dictionary<string, string> dics = new Dictionary<string, string>();
                                List<string> temps = _GetRow(excelReader);
                                for (int i = 0; i < mName.Count; i++)
                                {
                                    if (temps.Count > i)
                                    {
                                        dics[mName[i]] = temps[i];
                                    }
                                    else
                                    {
                                        dics[mName[i]] = "";
                                    }
                                }
                                if (!string.IsNullOrEmpty(dics["Only_id"]))
                                {
                                    mDatas.Add(dics);
                                }
                            }
                        }
                    }
                }
            }
            excelReader.Close();
        }
        public void BuildCSFile()
        {
            if (mDatas.Count == 0)
            {
                _LoadExcel();
            }
            Debug.LogError("自动生成：" + mPath.ClassName + "类,请重新打包");
            List<VariableData> varTypes = _GetVariable();
            string nameSpace = null;
            string jxClass = mPath.ClassName;
            int index = mPath.ClassName.IndexOf(".");
            if (index != -1)
            {
                nameSpace = mPath.ClassName.Substring(0, index);
                jxClass = mPath.ClassName.Substring(index + 1);
            }

            AssetFile aFile = new AssetFile(jxClass.Substring(0, jxClass.Length - "Asset".Length));
            if (!string.IsNullOrEmpty(nameSpace))
            {
                aFile.SetNamespace(nameSpace);
            }

            for (int i = 0; i < varTypes.Count; i++)
            {
                var variable = varTypes[i];
                if (!string.IsNullOrEmpty(variable.name))
                {
                    string ex = !string.IsNullOrEmpty(variable.context) ? ("  //" + variable.context) : "";
                    if (!string.IsNullOrEmpty(variable.typeName))
                    {
                        aFile.SetVariable(variable.typeName, variable.name + ";" + ex);
                    }
                    else
                    {
                        StringAnalyisis sa = new StringAnalyisis();
                        sa.Encrypt("{" + ComputerString(variable.typeValue, false) + "}");
                        sa.MoveNextPath("0");
                        var posIndex = sa.GetPos();
                        List<string> fTypeFiles = new List<string>();
                        bool isSameType = true;
                        for (int p = 0; p < posIndex.Vluaes.Count; p++)
                        {
                            string value = sa.GetValue(p.ToString());
                            var fType = _StringTryBasicTypeName(value);
                            fTypeFiles.Add(fType);
                            if (fTypeFiles[0] != fType)
                            {
                                isSameType = false;
                            }
                        }
                        if (fTypeFiles.Count == 0)
                        {
                            aFile.SetVariable("string", variable.name + ";" + ex);
                        }
                        else if (fTypeFiles.Count == 1)
                        {
                            aFile.SetVariable(fTypeFiles[0], variable.name + ";" + ex);
                        }
                        else
                        {
                            if (isSameType)
                            {
                                //数组
                                if (fTypeFiles[0] != "string")
                                {
                                    //必定为数组
                                    aFile.SetVariable(fTypeFiles[0] + "[]", variable.name + ";" + ex);
                                }
                                else
                                {
                                    //超级复杂结构||类结构[]||string[]
                                    string allValue = sa.GetPatchValue(posIndex);
                                    if (allValue.IndexOf("{") == -1)
                                    {
                                        //字符数组
                                        aFile.SetVariable("string[]", variable.name + ";" + ex);
                                    }
                                    else
                                    {
                                        isSameType = true;
                                        List<string> Last = null;
                                        for (int c = 0; c < posIndex.Vluaes.Count; c++)
                                        {
                                            List<string> tempStrs = new List<string>();
                                            var vPatch = posIndex.GetPatchByName(c.ToString());
                                            if (vPatch != null)
                                            {
                                                foreach (var k in vPatch.Vluaes)
                                                {
                                                    tempStrs.Add(_StringTryBasicTypeName(sa.GetPatchValue(k.Value)));
                                                }
                                            }
                                            if (Last != null)
                                            {
                                                if (tempStrs.Count != Last.Count)
                                                {
                                                    isSameType = false;
                                                }
                                                else
                                                {
                                                    for (int s = 0; s < Last.Count; s++)
                                                    {
                                                        if (Last[s] != tempStrs[s])
                                                        {
                                                            isSameType = false;
                                                        }
                                                    }
                                                }

                                            }
                                            Last = tempStrs;
                                        }
                                        if (isSameType)
                                        {
                                            //类数组
                                            aFile.SetVariable(variable.name + "_Data[]", variable.name + ";" + ex);
                                            for (int p = 0; p < Last.Count; p++)
                                            {
                                                Last[p] = Last[p] + " param_" + p.ToString();
                                            }
                                            aFile.SetSubClass(variable.name + "_Data", Last.ToArray());
                                        }
                                        else
                                        {
                                            //超级复杂结构,不解析,给用户自定义
                                            aFile.SetVariable(variable.name + "_Data", variable.name + ";" + ex);
                                            string[] pars = new string[fTypeFiles.Count];
                                            for (int p = 0; p < pars.Length; p++)
                                            {
                                                pars[p] = "string" + " param_" + p.ToString();
                                            }
                                            aFile.SetSubClass(variable.name + "_Data", pars);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //类结构
                                aFile.SetVariable(variable.name + "_Data", variable.name + ";" + ex);
                                string[] pars = new string[fTypeFiles.Count];
                                for (int p = 0; p < pars.Length; p++)
                                {
                                    pars[p] = fTypeFiles[p] + " param_" + p.ToString();
                                }
                                aFile.SetSubClass(variable.name + "_Data", pars);
                            }
                        }
                    }
                }
            }

            FEPath.CreateDirectory(mPath.CSPath);
            aFile.Build(mPath.CSPath + "/" + jxClass + ".cs");
        }
        private class VariableData
        {
            public string name;
            public string typeValue;
            public string typeName;
            public string context;
            public string other;
        }
        private List<VariableData> _GetVariable()
        {
            List<VariableData> result = new List<VariableData>();
            string[] firsts = new string[mName.Count];//第一个最长的真实数据
            for (int n = 0; n < mName.Count; n++)
            {
                firsts[n] = "";
            }
            for (int i = 0; i < mDatas.Count; i++)
            {
                var data = mDatas[i];
                for (int n = 0; n < mName.Count; n++)
                {
                    var key = mName[n];
                    string value = "";
                    if (!data.TryGetValue(key, out value))
                    {
                        value = "";
                    }
                    if (firsts[n] == null || firsts[n].Length < value.Length)
                    {
                        firsts[n] = value;
                    }
                }
            }

            for (int i = 1; i < mName.Count; i++)
            {
                VariableData evt = new VariableData();
                evt.name = mName[i];
                evt.context = mContexts.Count > i ? mContexts[i].Replace(" ", "").Replace("\r", "").Replace("\n", "") : "";
                evt.other = mOthers.Count > i ? mOthers[i] : "";
                evt.typeName = mTypes.Count > i ? mTypes[i].ToLower() : "";
                evt.typeValue = firsts.Length > i ? firsts[i] : "";
                result.Add(evt);
            }
            return result;
        }
        private string mError = null;
        public void Deserialize()
        {
            if (mValue == null)
                return;
            if (mDatas.Count == 0)
            {
                _LoadExcel();
            }
            int errorLine = 4;
            try
            {
                var ass = Assembly.Load("Assembly-CSharp");
                object assetObject = mValue;
                //if (assetObject == null)
                //{
                //    assetObject = ScriptableObject.CreateInstance(ass.GetType(mPath.ClassName));
                //}
                FieldInfo field = assetObject.GetType().GetField("ProList");
                Type[] tempTypes = field.FieldType.GetGenericArguments();
                var listType = typeof(List<>).MakeGenericType(tempTypes);
                IList list = (IList)Activator.CreateInstance(listType);
                var subType = ass.GetType(tempTypes[0].FullName);

                ////特殊需求
                List<string> mArrayName = new List<string>();
                FieldInfo[] subFields = subType.GetFields();
                for (int i = 0; i < subFields.Length; i++)
                {
                    var sf = subFields[i];
                    var f = sf.FieldType;
                    Type t = null;
                    if (f.IsGenericType && f.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        Type[] xxx = f.GetGenericArguments();
                        t = xxx[0];

                    }
                    else if (f.IsArray)
                    {
                        t = f.GetElementType();
                    }
                    if (t != null && !t.IsPrimitive && t != typeof(string))
                    {
                        mArrayName.Add(sf.Name);
                    }
                }

                StringBuilder StrBuf = new StringBuilder();
                for (int i = 0; i < mDatas.Count; i++)
                {
                    Dictionary<string, string> tempBuff = mDatas[i];
                    object subTobject = System.Activator.CreateInstance(subType);
                    StrBuf.Length = 0;
                    StrBuf.Append("{");
                    foreach (var k in tempBuff)
                    {
                        ////特殊需求

                        string realyValue = ComputerString(k.Value, false);
                        if (!realyValue.Contains("|"))
                        {
                            if (mArrayName.Contains(k.Key))
                            {
                                if (realyValue != "")
                                {
                                    if (!realyValue.Contains("{"))
                                    {
                                        realyValue = "{" + realyValue + "}";
                                    }
                                }
                            }
                        }

                        StrBuf.Append(k.Key + "=" + "{" + realyValue + "},");
                    }
                    if (StrBuf.Length > 2)
                    {
                        //去掉逗号
                        StrBuf.Length--;
                    }
                    StrBuf.Append("}");
                    errorLine++;

                    StringSerialize.Deserialize(StrBuf.ToString(), subTobject);
                    list.Add(subTobject);
                }
                field.SetValue(assetObject, list);
            }
            catch (System.Exception e)
            {
                mError = (mPath.ClassName + "错误行数：[" + errorLine.ToString() + "]---其他:" + e.ToString());
            }
        }
        public void BuildAsset()
        {
            if (mValue != null)
            {
                if (File.Exists(mPath.FilePath))
                {
                    if (mDatas.Count == 0)
                    {
                        Deserialize();
                    }

                    FEPath.CreateDirectory(mPath.DirPath);

                    if (!string.IsNullOrEmpty(mError))
                    {
                        Debug.LogError(mError);
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(mValue as UnityEngine.Object, mPath.AssetOutPath);
                        AssetDatabase.SaveAssets();
                        ScriptsTime.Debug(mPath.ClassName + ":导出成功");
                    }
                }
                else
                {
                    Debug.LogError("路径：" + mPath.FilePath + "不存在");
                }
            }
            else
            {
                Debug.LogError(mPath.ClassName + ":类型不存在");
            }
        }
        public string GetError()
        {
            return mError;
        }
        private string _StringTryBasicTypeName(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                int data_int = 0;
                float data_float = 0;
                bool data_bool = false;
                if (int.TryParse(key, out data_int))
                {
                    return "int";
                }
                else if (key.IndexOf(".") != -1 && float.TryParse(key, out data_float))
                {
                    return "float";
                }
                else if (bool.TryParse(key, out data_bool))
                {
                    return "bool";
                }
            }
            return "string";
        }
        public void AutoBuild()
        {
            if (mValue == null)
            {
                BuildCSFile();
            }
            else
            {
                BuildAsset();
            }
        }

        private string ComputerString(string str, bool isLang)
        {
            if (str.Contains("|"))
            {
                return "{" + str.Replace("|", "},{") + "}";
            }
            if (isLang)
            {
                return ComputerColor(str.Replace(@"\n", "\n"));
            }
            return str;
        }
    }

    private static readonly Regex s_Regex = new Regex(@"(.*?)#([0-9])(.+?)#", RegexOptions.Singleline); //图文混排
    private static string ComputerColor(string str)
    {
        int index = str.IndexOf('#');
        if (index != -1)
        {
            StringBuilder builder = new StringBuilder();
            int sIndex = 0;
            //正则表达式
            foreach (Match match in s_Regex.Matches(str))
            {
                //FRegex
                Match act = match;
                builder.Append(match.Groups[1].ToString());
                int type = int.Parse(match.Groups[2].ToString());
                string color = "<color=#ffffffff>";
                switch (type)
                {
                    case 0:
                        color = "<color=#c14f4fff>";
                        break;
                    case 1:
                        color = "<color=#f49a21ff>";
                        break;
                    case 2:
                        color = "<color=#f5f0b7ff>";
                        break;
                    case 3:
                        color = "<color=#86b964ff>";
                        break;
                    case 4:
                        color = "<color=#29a6e7ff>";
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                }
                builder.Append(color);
                builder.Append(match.Groups[3].ToString());
                builder.Append("</color>");
                sIndex = match.Index + match.Length;
            }
            builder.Append(str.Substring(sIndex));
            return builder.ToString();
        }
        return str;
    }


    public static void CreateAutoExcel(string excelName, string path)
    {
        ExcelAsset ex = new ExcelAsset(excelName, path);
        try
        {
            ex.AutoBuild();
            if (!string.IsNullOrEmpty(ex.GetError()))
            {
                Debug.LogError("导表失败:" + ex.GetError());
                EditorUtility.DisplayDialog("导表错误", ex.GetError(), "确定");
            }
            else
            {
                AssetDatabase.Refresh();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("导表失败:" + e.ToString());
            EditorUtility.DisplayDialog("导表错误", ex.mPath.ClassName + ":" + e.ToString(), "确定");
        }
    }

    public static bool CreateAllAssetData()
    {
        if (Application.isPlaying || EditorApplication.isCompiling)
        {
            Debug.LogError("导表失败:运行中无法导表");
            return false;
        }
        try
        {
            TimerController.EditorClear();
            EditorUtility.DisplayCancelableProgressBar("导表中...", "正在计算配置", 0);
            ScriptsTime.Begin();
            SelectPrefabEditor spe = new SelectPrefabEditor(".xlsx", "/" + ResConfig.EXCELFILEPATH + "/", "", false);
            List<ExcelAsset> allExcel = new List<ExcelAsset>();
            var ass = Assembly.Load("Assembly-CSharp");
            for (int i = 0; i < spe.tempAllStrs.Length; i++)
            {
                string excelName = spe.tempAllStrs[i].Replace(".xlsx", "");
                string ClassName = excelName.Replace("Asset", "") + "Asset";
                var type = ass.GetType(ClassName);
                if (type != null)
                {
                    ExcelAsset ex = new ExcelAsset(excelName);
                    allExcel.Add(ex);
                }
            }

            int allCount = allExcel.Count;
            int maxNum = allExcel.Count;
            string error = null;
            for (int i = 0; i < 2; i++)
            {
                Timer_Thread.SetTimer((t, r) =>
                {
                    ExcelAsset asset = null;
                    lock (allExcel)
                    {
                        if (allExcel.Count > 0)
                        {
                            asset = allExcel[0];
                            allExcel.RemoveAt(0);
                        }
                    }
                    try
                    {
                        if (asset != null)
                        {

                            asset.Deserialize();
                            r.callBack = (arg1, arg2) =>
                            {
                                try
                                {
                                    EditorUtility.DisplayCancelableProgressBar("导表中...", "正在导表:" + asset.mPath.ClassName + "(" + (allCount - maxNum).ToString() + "/" + allCount.ToString() + ")", (allCount - maxNum) / (float)allCount);
                                    asset.BuildAsset();
                                    maxNum--;
                                    if (!string.IsNullOrEmpty(asset.GetError()))
                                    {
                                        if (error == null)
                                        {
                                            error = asset.GetError();
                                        }
                                        maxNum = 0;
                                    }
                                }
                                catch (System.Exception e)
                                {
                                    maxNum = 0;
                                    error = asset.mPath.ClassName + ":未知错误:" + e.ToString();
                                }
                            };
                            return 0;
                        }
                    }
                    catch (System.Exception e)
                    {
                        maxNum = 0;
                        error = asset.mPath.ClassName + ":未知错误:" + e.ToString();
                    }
                    return -1;
                }, 0, null);
            }

            while (maxNum > 0 && error == null)
            {
                System.Threading.Thread.Sleep(100);
                TimerController.EditorUpdate();
            }
            EditorUtility.ClearProgressBar();
            if (error == null)
            {
                ScriptsTime.ShowTime("导表共用时间");
                Debug.Log("导表成功");
            }
            else
            {
                Debug.LogError("导表失败:" + error);
                return false;
            }
            AssetDatabase.Refresh();
            return true;
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError("导表失败:" + e.ToString());
            EditorUtility.DisplayDialog("导表错误", e.ToString(), "确定");
            return false;
        }
    }
}
