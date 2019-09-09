using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using F2DEngine;
using UnityEditor.SceneManagement;
using System;
using System.Reflection;

using System.Collections;
using UnityEngine.UI;
using System.Xml;
using UnityEditor.Experimental.SceneManagement;

public class MyEdior
{

    #region 地图生成
    public class MapData
    {
        public int id;
        public int type;
        public int x;
        public int y;
    }

    public static int Fen(float r)
    {
        return (((int)(r * 10 + 0.5f)) / 10);
    }

    public static int ColorSwitch(Color c)
    {
        int r = Fen(c.r);
        int g = Fen(c.g);
        int b = Fen(c.b);
        int a = Fen(c.a);
        if (a != 0)
        {
            if (r == 1 && g == 1 && b == 1) return 1;
            if (r == 1 && g == 0 && b == 0) return 1;
            if (r == 1 && g == 1 && b == 0) return 2;
            if (r == 1 && g == 1 && b == 1) return 3;
            if (r == 1 && g == 1 && b == 0) return 4;
            if (r == 1 && g == 0 && b == 1) return 5;
            if (r == 0 && g == 1 && b == 1) return 6;
            if (r == 0 && g == 0 && b == 1) return 7;
        }
        return 8;
    }

    #endregion


    #region 通用操作事件
    [MenuItem("Assets/Excel数据生成")]
    private static void Assets_Excel()
    {
        var select = Selection.assetGUIDs;
        for (int i = 0; i < select.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(select[i]);
            int tempHead = path.LastIndexOf("/");
            int end = path.LastIndexOf(".xlsx");
            string excelName = "";
            if (tempHead != -1 && end != -1)
            {
                excelName = path.Substring(tempHead + 1, end - tempHead - 1);
            }
            if (excelName != "")
            {
                AssetEdior.CreateAutoExcel(excelName,path.Substring(0,tempHead));
            }
        }
    }

    [MenuItem("Excel/更新Excel数据")]
    public static void CreateAllAssetData()
    {
        AssetEdior.CreateAllAssetData();
    }

    //[MenuItem("FService/同步协议文件")]
    public static void UpdateCFSeriverPro()
    {
        string keyPath = "[server]";
        SelectPrefabEditor spe = new SelectPrefabEditor(".cs", keyPath, "", true);
        List<string> paths = new List<string>();
        for (int i = 0; i < spe.tempAllStrs.Length; i++)
        {
            string allPath = spe.tempAllStrs[i];
            int pos = allPath.IndexOf(keyPath);
            if (pos != -1)
            {
                string val = allPath.Substring(0, pos) + keyPath;
                if (!paths.Contains(val))
                {
                    paths.Add(val);
                }
            }
        }

        for (int i = 0; i < paths.Count; i++)
        {
            string tempPath = paths[i];
            int pos = tempPath.LastIndexOf("/");
            string exPath = "";
            if (pos != -1)
            {
                exPath = tempPath.Substring(pos);
            }
            MyEdior.CopyDirectory(Application.dataPath.Replace("/Assets", "/" + paths[i]), Application.dataPath.Replace("Client/FNEngine/Project/Assets", "CFServer/Server/F2DEngineServer/FEngine" + exPath), (f) =>
            {
                return (f.IndexOf(".meta") == -1);
            });
        }
    }


    [MenuItem("Assets/FEngine/配置表安全检测")]
    private static void CheckExcel()
    {
        var select = Selection.assetGUIDs;
        for (int i = 0; i < select.Length; i++)
        {
            var path = AssetDatabase.GUIDToAssetPath(select[i]);
            int tempHead = path.LastIndexOf("/");
            int end = path.LastIndexOf("Asset.cs");
            if (tempHead != -1 && end != -1)
            {
                string tempPath = path + ".pp";
                string[] lines = File.ReadAllLines(path);
                if (File.Exists(tempPath))
                {
                    File.Delete(path);
                    File.Move(tempPath, path);
                }
                else
                {
                    for (int l = 0; l < lines.Length; l++)
                    {
                        var tl = lines[l];
                        if (tl.IndexOf(" class ") == -1 && tl.IndexOf(" readonly ") == -1 && tl.IndexOf("(") == -1)
                        {
                            if (tl.IndexOf("public") != -1)
                            {
                                lines[l] = tl.Replace("public", "public readonly") + "  //___________检测修改";
                            }
                        }
                    }
                    File.Move(path, tempPath);
                    File.WriteAllLines(path, lines, System.Text.Encoding.UTF8);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    //[MenuItem("FEngine/强制添加脚本")]
    public static void ForceAddScript()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayProgressBar("Hold on", path, (float)(i + 1) / guids.Length);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var sliders = go.GetComponentsInChildren<Slider>(true);
            bool hasModify = false;
            for (int t = 0; t < sliders.Length; t++)
            {
                if (null == sliders[t].gameObject.GetComponent<DragUI>())
                {
                    sliders[t].gameObject.AddComponent<DragUI>();
                    hasModify = true;
                }
            }
            var srs = go.GetComponentsInChildren<ScrollRect>();
            for (int t = 0; t < srs.Length; t++)
            {
                if (srs[t].horizontal == true && null == srs[t].gameObject.GetComponent<DragUI>())
                {
                    srs[t].gameObject.AddComponent<DragUI>();
                    hasModify = true;
                }
            }
            if (hasModify)
            {
                EditorUtility.SetDirty(go);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log("脚本添加完成" + guids.Length);
    }

    //[MenuItem("FEngine/替换字体")]
    public static void ReplaceFont()
    {
        var ngo = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Text.prefab");//目标字体
        Font toFont = ngo.GetComponent<Text>().font;
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        for(int i = 0; i < guids.Length;i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            EditorUtility.DisplayProgressBar("Hold on", path, (float)(i + 1) / guids.Length);
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            Text[] texts = go.GetComponentsInChildren<Text>(true);
            bool hasModifyFont = false;
            for (int t = 0; t < texts.Length;t++)
            {
                Text text = texts[t];
                if(text.font == null||text.font.name == "HSMedium")
                {
                    text.font = toFont;
                    hasModifyFont = true;
                }
                if(text.fontStyle == FontStyle.Bold||text.fontStyle == FontStyle.BoldAndItalic)
                {
                    text.fontStyle = FontStyle.Normal;
                    hasModifyFont = true;
                }
            }
            if(hasModifyFont)
            {
                EditorUtility.SetDirty(go);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log("字体替换完成,共替换"+ guids.Length);
    }

    [MenuItem("Assets/FEngine/检查贴图Tight使用情况")]
    public static void CheckTight()
    {
        SelectPrefabEditor spe = new SelectPrefabEditor(".png", "/Assetbundle/Effect/Frame/Raw", "", true);
        for (int i = 0; i < spe.tempAllStrs.Length; i++)
        {
            TextureImporter import = AssetImporter.GetAtPath(spe.tempAllStrs[i]) as TextureImporter;
            if (import != null)
            {
                if (import.textureType == TextureImporterType.Sprite)
                {
                    TextureImporterSettings set = new TextureImporterSettings();
                    import.ReadTextureSettings(set);
                    set.spriteMeshType = SpriteMeshType.Tight;
                    import.SetTextureSettings(set);
                    import.SaveAndReimport();
                }
            }
        }
    }


    [MenuItem("Assets/FEngine/检查贴图使用情况")]
    public static void CheckUI()
    {
        string[] tempPath = Selection.assetGUIDs;
        if (tempPath.Length != 0)
        {

            List<string> lookFile = new List<string>();
            for (int i = 0; i < tempPath.Length; i++)
            {
                lookFile.Add(AssetDatabase.GUIDToAssetPath(tempPath[i]));
            }

            Dictionary<string, List<string>> mPaths = new Dictionary<string, List<string>>();
            string[] prfabsPath = AssetDatabase.FindAssets("t:Prefab");
            for (int i = 0; i < prfabsPath.Length; i++)
            {
                string realPath = AssetDatabase.GUIDToAssetPath(prfabsPath[i]);
                string[] paths = AssetDatabase.GetDependencies(realPath);
                for (int p = 0; p < paths.Length; p++)
                {
                    List<string> t = null;
                    if (!mPaths.TryGetValue(paths[p], out t))
                    {
                        t = new List<string>();
                        mPaths[paths[p]] = t;
                    }
                    t.Add(realPath);
                }
            }

            for (int i = 0; i < lookFile.Count; i++)
            {
                List<string> t = null;
                if (mPaths.TryGetValue(lookFile[i], out t))
                {
                    if (t.Count == 1)
                    {
                        if (t[0] == lookFile[i])
                        {
                            UnityEngine.Debug.Log("未被引用物件:" + lookFile[i]);
                            continue;
                        }
                    }
                    UnityEngine.Debug.Log("<color=#AAAA00>引用物件:" + lookFile[i] + "</color>");
                    UnityEngine.Debug.Log("<color=#FF0000>***************************************************</color>");
                    for (int xx = 0; xx < t.Count; xx++)
                    {
                        UnityEngine.Debug.Log(t[xx]);
                    }
                    UnityEngine.Debug.Log("<color=#FF0000>***************************************************</color>");
                }
                else
                {
                    UnityEngine.Debug.Log("未被引用物件:" + lookFile[i]);
                }
            }

        }
    }

    private class LuaScriptAsset : UnityEditor.ProjectWindowCallback.EndNameEditAction
    {
        private class LuaMould:FBaseMould
        {
            protected override void OnInit(MouldTool tool)
            {
                tool.WRITE("--XLUA 热更新[name]脚本 time:[time]  by auto");
                tool.WRITE("");
                tool.WRITE("--------------快捷入口------------------------------------------");
                tool.WRITE("  local helper = require('BaseLua/FHelper')");
                tool.WRITE("  local util = require('BaseLua/util')");
                tool.WRITE("  local F2D = CS.F2DEngine");
                tool.WRITE("  local FLua = CS.F2DEngine.SceneLuaManager");
                tool.WRITE("  local FScene = CS.F2DEngine.SceneManager");
                tool.WRITE("  local FComm = CS.F2DEngine.FCommonFunction");
                tool.WRITE("  local Unity = CS.UnityEngine");
                tool.WRITE("  local UI = CS.UnityEngine.UI");
                tool.WRITE("----------------------------------------------------------------");
                tool.WRITE("  print('修补[name]脚本')");
                tool.WRITE("");
                tool.WRITE("  local  funs = {}");
                tool.WRITE("");
                tool.WRITE("");
                tool.WRITE("  funs['函数名XX'] = function(this)");
                tool.WRITE("  ----函数实现");
                tool.WRITE("  print('修补函数XX')");
                tool.WRITE("  this:XX()");
                tool.WRITE("");
                tool.WRITE("  end");
                tool.WRITE("");
                tool.WRITE("");
                tool.WRITE("  xlua.private_accessible(CS.[name])");
                tool.WRITE("  util.hotfix_ex(CS.[name],'XX',funs['XX'])");
            }
        }
        
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            LuaMould lua = new LuaMould();
            lua.Set("time", System.DateTime.Now.ToString("yyyy.dd"));
            lua.Set("name", FEPath.GetFileName(pathName));
            lua.Build(pathName + ".lua", FFilePath.FP_Abs);
            AssetDatabase.Refresh();
        }
    }

    [MenuItem("Assets/Create/Lua Script", false, 31)]
    private static void CreateLuaFile()
    {
        var select = Selection.assetGUIDs;
        if (select.Length == 1)
        {
            var path = AssetDatabase.GUIDToAssetPath(select[0]);
            if (!AssetDatabase.IsValidFolder(path))
            {
                path = FEPath.GetDirectoryName(path);
            }

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, new LuaScriptAsset(), path + "/" + "NewLuaScript", EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D, null);
        }
    }

    [MenuItem("GameObject/FEngine/创建BasePeople")]
    static void CreateBasePeople()
    {
        var go = new GameObject("new_BasePeople",typeof(FBasePeople));
    }

    [MenuItem("GameObject/FEngine/创建UI画布")]
    static void CreateUI()
    {
       GameObject main =  SceneManager.instance.Create(ResConfig.FMAINUISCENE);
       main.name = "EditorCanvas";
       MainCanvas msui = main.GetComponent<MainCanvas>();
       if(msui != null)
       {
            GameObject.DestroyImmediate(msui);
       }

        main.AddComponent<EditorMainCanvas>();

        FCUniversalPanel fup = main.GetComponent<FCUniversalPanel>();
        if (fup != null)
        {
            GameObject.DestroyImmediate(fup);
        }

    }

    [MenuItem("GameObject/FEngine/创建FUniversalItem")]
    static void CreateUniversalItem()
    {
        var go = new GameObject("FUniversalItem", typeof(FUniversalItem));
        
    }

    [MenuItem("Novice/新手编辑")]
    static void NoviceWindow()
    {
        if (Application.isPlaying)
        {
            if (UIManager.instance.GetActiveWindos(ResConfig.FNOVICEEDITORPLANE) != null)
            {
                UIManager.instance.CloseWindos(ResConfig.FNOVICEEDITORPLANE);
            }
            else
            {
                UIManager.instance.ShowWindos(ResConfig.FNOVICEEDITORPLANE+"[nc]");
            }
        }
        
    }
    #endregion

    #region 通用绘制函数

    public class DrawMono
    {
        public UnityEngine.Object mObj;
        private SerializedObject mSerializedObject;
        private List<SerializedProperty> mDrawProperty = new List<SerializedProperty>();
        public DrawMono(UnityEngine.Object obj)
        {
            mObj = obj;
            mSerializedObject = new SerializedObject(obj);
            var t = obj.GetType();
            System.Reflection.FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
            {
                var tt = fields[i];
                if (!CheckType(tt))
                {
                    continue;
                }

                if (!tt.IsDefined(typeof(HideInInspector), false))
                {
                    mDrawProperty.Add(mSerializedObject.FindProperty(tt.Name));
                }
            }
        }
        public void Draw()
        {
            if (!Application.isPlaying)
            {
                for (int i = 0; i < mDrawProperty.Count; i++)
                {
                    var pro = mDrawProperty[i];
                    if (pro != null)
                    {
                        EditorGUILayout.PropertyField(mDrawProperty[i], true, null);
                    }
                }
                mSerializedObject.ApplyModifiedProperties();
            }
        }
        private static bool CheckType(FieldInfo field)
        {
            if (!field.IsPublic || field.IsStatic || field.IsLiteral | field.IsNotSerialized)
                return false;
            return true;
        }
    }

    public class DrawConverter:BaseConverter
    {
        private static DrawConverter ins = new DrawConverter();

        public static void DrawObject(object obj, List<string> tags)
        {
            ins._DrawObject(obj, tags);
        }

        private List<string> mTags;
        private  void _DrawObject(object obj, List<string> tags)
        {
            BytesPack pack = new BytesPack();
            Serialize(obj,pack);
            pack.BeginSeek();
            mTags = tags;
            Deserialize(pack,obj.GetType(),obj);
        }
 
        protected override object _Deserialize(string name, FFieldInfo info, SerializeTool tool, object obj = null)
        {
            SetName(name,info);
            switch(info.FType)
            {
                case FFieldType.F_Int:
                    return EditorGUILayout.IntField(tool.PopInt(name));
                case FFieldType.F_Bool:
                    return EditorGUILayout.Toggle(tool.PopBool(name));
                case FFieldType.F_Byte:
                    return (byte)EditorGUILayout.IntField(tool.PopByte(name));
                case FFieldType.F_Enum:
                    return EditorGUILayout.IntField(tool.PopInt(name));
                case FFieldType.F_Float:
                    return EditorGUILayout.FloatField(tool.PopFloat(name));
                case FFieldType.F_Long:
                    return EditorGUILayout.LongField(tool.PopLong(name));
                case FFieldType.F_String:
                    return EditorGUILayout.TextField(tool.PopString(name));
                default:
                    break;
            }
            return base._Deserialize(name, info,tool,obj);
        }

        protected override object _DeserializeArray(string name,FFieldInfo info, SerializeTool tool, bool isArray)
        {

            IList list = null;
            int count = tool.PopArrayHead();
            EditorGUILayout.LabelField("数组个数:"+count);
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


            if (isArray)
            {
                for (int i = 0; i < count; i++)
                {
                    tool.ArrayIndexToName(i);
                    list[i] = _Deserialize("  元素" +i.ToString(), info.GenericType1, tool, null);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    tool.ArrayIndexToName(i);
                    list.Add(_Deserialize("  元素" + i.ToString(), info.GenericType1, tool, null));
                }
            }

            EditorGUILayout.LabelField("---------------数组结束--------------------");

            if (GUILayout.Button("添加元素"))
            {
                if (isArray)
                {
                    Array newArray = Array.CreateInstance(info.GenericType1.ClassType, count + 1);
                    if (count != 0)
                    {
                        Array.Copy((Array)list, newArray, count);
                    }
                    list = newArray;
                    list[count] = CreateInstance(info.GenericType1);
                }
                else
                {
                    if (count == 0)
                    {
                        var listType = typeof(List<>).MakeGenericType(info.GenericType1.ClassType);
                        list = (IList)Activator.CreateInstance(listType);
                    }
                    list.Add(CreateInstance(info.GenericType1));
                }
            }
            if (count != 0)
            {
                if (GUILayout.Button("删除元素"))
                {
                    if (isArray)
                    {
                        if (count != 0)
                        {
                            Array newArray = Array.CreateInstance(info.GenericType1.ClassType, count - 1);
                            if (count - 1 > 0)
                            {
                                Array.Copy((Array)list, newArray, count - 1);
                            }
                            list = newArray;
                        }
                    }
                    else
                    {
                        if (count != 0)
                        {
                            list.RemoveAt(count - 1);
                        }
                    }
                }
            }
            return list;
        }

        protected override object _DeserializeDic(string name, FFieldInfo info, SerializeTool tool)
        {

            IDictionary list =  (IDictionary)base._DeserializeDic(name,info, tool);
            EditorGUILayout.LabelField("---------------字典结束--------------------");
            int count = list.Count;
            if (GUILayout.Button("添加元素"))
            {
                if (count == 0)
                {
                    Type[] tempTypes = info.ClassType.GetGenericArguments();
                    var listType = typeof(Dictionary<,>).MakeGenericType(tempTypes);
                    list = (IDictionary)Activator.CreateInstance(listType);
                }
                object key = CreateInstance(info.GenericType1);
                list[key] = CreateInstance(info.GenericType2);
            }
            if (count != 0)
            {
                if (GUILayout.Button("删除元素"))
                {
                    if (count != 0)
                    {
                        list.Remove(CreateInstance(info.GenericType1));
                    }
                }
            }
            return list;
        }

        private object CreateInstance(FFieldInfo info)
        {
            if(info.FType == FFieldType.F_String)
            {
                return "";
            }
            return Activator.CreateInstance(info.ClassType);
        }

        private static void SetName(string name,FFieldInfo fileInfo)
        {
            if (fileInfo.Info != null)
            {
                var lent = fileInfo.Info.GetCustomAttributes(typeof(FRenameAttr), false);
                if (lent.Length > 0)
                {
                    EditorGUILayout.LabelField(((FRenameAttr)lent[0]).nName);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                EditorGUILayout.LabelField(name);
            }
        }

        protected override bool CheckType(FieldInfo field)
        {
            return (base.CheckType(field)&& IsCanDrawInInspector(field,mTags));
        }

        private  bool IsCanDrawInInspector(FieldInfo fileInfo, List<string> tags)
        {
            if (tags != null && tags.Count != 0)
            {
                var tagAtt = fileInfo.GetCustomAttributes(typeof(FTagAttr), true);
                if (tagAtt != null && tagAtt.Length != 0)
                {
                    FTagAttr att = (FTagAttr)tagAtt[0];
                    if (att.StrBuffs != null && att.StrBuffs.Length != 0)
                    {
                        bool isDraw = false;
                        for (int i = 0; i < att.StrBuffs.Length; i++)
                        {
                            if (tags.Contains(att.StrBuffs[i]))
                            {
                                isDraw = true;
                                break;
                            }
                        }
                        if (!isDraw)
                        {
                            return false;
                        }
                    }
                }
            }
            return (fileInfo.GetCustomAttributes(typeof(HideInInspector), true).Length == 0);
        }

    }

    //绘制委托函数类
    public static System.Delegate DrawDelegate(object ins,Delegate callObject,Type classType,Type outType,Type returnType,params Type[]paras)
    {
        var mode = classType.GetMethods();
        List<MethodInfo> methods = new List<MethodInfo>();

        for (int i = 0; i < mode.Length; i++)
        {
            var mo = mode[i];
            var obj = mo.GetCustomAttributes(typeof(HideInInspector),true);
            if ((obj==null||obj.Length == 0)&&mo.IsPublic && mo.ReturnParameter.ParameterType == returnType)
            {
                var parame = mo.GetParameters();

                if (parame.Length == paras.Length)
                {
                    bool IsFit = true;
                    for(int p = 0; p < parame.Length;p++)
                    {
                        if(parame[p].ParameterType != paras[p])
                        {
                            IsFit = false;
                            break;
                        }
                    }
                    if (IsFit)
                    {
                        methods.Add(mo);
                    }
                }
            }
        }

        string selectName = "";
        if (callObject != null)
        {
            var motd = callObject.Method;
            selectName = motd.Name;
        }

        int index = 0;
        List<string> methodName = new List<string>();
        methodName.Add("Null");
        FCommonFunction.SetList(methods, (f, i) =>
        {
            if (f.Name == selectName)
            {
                index = i + 1;
            }
            methodName.Add(f.Name);
        });

        int lastIndex = index;
        index = EditorGUILayout.Popup(index, methodName.ToArray());

        if (index != lastIndex)
        {
            if (index == 0)
            {
                return null;
            }
            else
            {   
                return System.Delegate.CreateDelegate(outType,ins, methods[index - 1]);
            }
        }

        return callObject;
    }
    #endregion

    #region 其他
    //保存,加载函数
    public static string SaveFilePanel(string ex)
    {
        FSaveHandle ste = FSaveHandle.Create("SavePanel", FFilePath.FP_Cache, FOpenType.OT_Txt);
        string temp = Application.dataPath;
        if (ste.IsLoad)
        {
            temp = ste.GetContext();
        }
        string path = EditorUtility.SaveFilePanel("保存状态", temp, "temp_0", ex);
        if (path == "")
            return "";
        FSaveHandle ddd = FSaveHandle.Create("SavePanel", FFilePath.FP_Cache, FOpenType.OT_Write | FOpenType.OT_Txt);
        ddd.SetContext(path);
        ddd.Save();
        return path;
    }

    public static string OpenFilePanel(string ex)
    {
        FSaveHandle ste = FSaveHandle.Create("SavePanel", FFilePath.FP_Cache, FOpenType.OT_Txt);
        string temp = Application.dataPath;
        if (ste.IsLoad)
        {
            temp = ste.GetContext();
        }
        string path = EditorUtility.OpenFilePanel("打开状态", temp, ex);
        if (path == "")
            return "";
        FSaveHandle ddd = FSaveHandle.Create("SavePanel", FFilePath.FP_Cache, FOpenType.OT_Write | FOpenType.OT_Txt);
        ddd.SetContext(path);
        ddd.Save();
        return path;
    }

    //最后不带“/”
    public static void CopyDirectory(string sourceDirectory, string destDirectory, Func<string, bool> callBack = null)
    {
        //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
        FEPath.CreateDirectory(sourceDirectory);
        FEPath.CreateDirectory(destDirectory);
        //拷贝文件
        copyFile(sourceDirectory, destDirectory, false, callBack);

        //拷贝子目录       
        //获取所有子目录名称
        string[] directionName = FEPath.GetDirectories(sourceDirectory);

        foreach (string directionPath in directionName)
        {
            //根据每个子目录名称生成对应的目标子目录名称
            string directionPathTemp = destDirectory + "/" + directionPath.Substring(sourceDirectory.Length + 1);

            //递归下去
            CopyDirectory(directionPath, directionPathTemp, callBack);
        }
    }

    public static void DeletFile(string path)
    {
        FCommonFunction.DeletFile(path);
    }

    public static int GetTimeVersion()
    {
        var dn = DateTime.Now;
        int timeVersion = 1000000000 + ((DateTime.Now.Year + 1) % 10) * 100000000 + int.Parse(DateTime.Now.ToString("MMddHHmm"));
        return timeVersion;
    }

    public static void copyFile(string sourceDirectory, string destDirectory, bool isAuto = false, Func<string, bool> callBack = null)
    {
        if (isAuto)
        {
            string path = FEPath.GetDirectoryName(destDirectory);
            FEPath.CreateDirectory(path);
        }

        //获取所有文件名称
        string[] fileName = FEPath.GetFiles(sourceDirectory);

        foreach (string filePath in fileName)
        {
            if (callBack != null)
            {
                if (!callBack(filePath))
                {
                    continue;
                }
            }
            //根据每个文件名称生成对应的目标文件名称
            string filePathTemp = destDirectory + "/" + filePath.Substring(sourceDirectory.Length + 1);

            //若不存在，直接复制文件；若存在，覆盖复制
            if (File.Exists(filePathTemp))
            {
                File.Copy(filePath, filePathTemp, true);
            }
            else
            {
                File.Copy(filePath, filePathTemp);
            }
        }
    }

    public static void KeepScene()
    {
        if (GUI.changed)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
        // if (EditorGUI.EndChangeCheck())
        //EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }

    #endregion
}

public class StateEditor : Editor
{
     
   public class FEngineEitor:FBEngine
   {
        public MonoBehaviour GetMonoInstance()
        {
            return null;
        }

        public bool GetIsNoPackMode()
        {
            return true;
        }

        public T GetNoPackObject<T>(string fileName) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (GetIsNoPackMode())
            {
                return (T)EditorScripts.GetNoPackObject<T>(fileName);
            }
#endif
            return null;
        }

        public ZipThreadData ThreadUnZip(string ZipFile, string TargetDirectory, string Password, LoadPercent loadPercent = null, bool OverWrite = true)//解压Zip
        {
            return null;
        }

        public System.Collections.IEnumerator LoadManager()
        {
            yield break;
        }

        public void SettingGM(GM.GMFunAction action)
        {

        }

        public void LoadFrontScene(GameProgress gp)
        {

        }

        public FNetMsgCore GetNetMsgCore()
        {
            return null;
        }

        public BundleManager.BundleAskCall GetBundleAskCall()
        {
            return null;
        }


        public Timer_Mix PreLoadObject(string id, bool isLoad)
        {
            return null;
        }

        public FESetting GetFESetting()
        {
            return FESetting.FE_NoPack;
        }

        public string GetEditorPath(string fileName)
        {
#if UNITY_EDITOR
            if (GetIsNoPackMode())
            {
                string tempPath = EditorScripts.GetExistPath(fileName);
                if (tempPath != "")
                {
                    return tempPath;
                }
            }
#endif
            return fileName;
        }
    }

    [InitializeOnLoadMethod]
    static void NewButton()
    {
        if (!Application.isPlaying)
        {
            //注册创建函数
            if (FEngineManager.GetFBEngine() == null)
            {
                FEngineManager.SetEngine(new FEngineEitor());
            }
        }

        EditorApplication.playModeStateChanged += (f) =>
        {
            if (f == PlayModeStateChange.EnteredEditMode)
            {
                FEngineManager.SetEngine(new FEngineEitor());
            }
        };


        //加工按钮
        EditorApplication.hierarchyChanged += () =>
        {
            var go = Selection.activeGameObject;
            if(go != null&& go.GetComponent<UnityEngine.UI.Button>()!= null)
            {
                if(go.GetComponent<FCommonBt>() == null)
                {
                    go.AddComponent<FCommonBt>();
                }
            }
            if (go != null && go.GetComponent<UnityEngine.UI.Slider>() != null)
            {
                var sld = go.GetComponent<UnityEngine.UI.Slider>();

                //loginLoad 强制不加
                var isLoginUI = false;
                var p = sld.transform.parent;
                while (null != p)
                {
                    if (p.name.Contains("LoginLoad"))
                    {
                        isLoginUI = true;
                        break;
                    }
                    p = p.parent;
                }

                if (null != sld.handleRect 
                    && sld.handleRect.parent.gameObject.activeSelf 
                    && !isLoginUI
                    && go.GetComponent<DragUI>() == null)
                {
                    go.AddComponent<DragUI>();
                }
            }
            if (go != null && go.GetComponent<UnityEngine.UI.ScrollRect>() != null)
            {
                if (null == go.GetComponent<MonitorScrollRect>())
                {
                    go.AddComponent<MonitorScrollRect>();
                }
                if (go.GetComponent<UnityEngine.UI.ScrollRect>().horizontal == true && go.GetComponent<DragUI>() == null)
                {
                    go.AddComponent<DragUI>();
                }
            }
        };
    }

}

