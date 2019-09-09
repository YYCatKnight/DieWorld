//----------------------------------------------
// Friday Engine 2015-2019 Fu Cong QQ: 353204643
//----------------------------------------------
using F2DEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;

namespace FridayEditor
{
    //日志定向模块
    internal static class LogRedirection
    {
        private static readonly Regex LuaRegex = new Regex(@"[FXLua/]*FXLua/([a-zA-Z0-9]*?)\.lua:(\d+):");
        [OnOpenAsset(0)]
        private static bool OnOpenAsset(int instanceId, int line)
        {
            string selectedStackTrace = GetSelectedStackTrace();
            if(string.IsNullOrEmpty(selectedStackTrace))
            {
                //定向其他的
                string path = AssetDatabase.GetAssetPath(EditorUtility.InstanceIDToObject(instanceId));
                string name = Application.dataPath + "/" + path.Replace("Assets/", "");
                if (name.EndsWith(".lua"))
                {
                    InternalEditorUtility.OpenFileAtLineExternal(name,0);
                }
                return false;
            }

            return RedirectionLua(selectedStackTrace);
        }

        private static bool RedirectionLua(string selectedStackTrace)
        {
            if (selectedStackTrace.Contains("LuaException:"))
            {
                Match luamatch = LuaRegex.Match(selectedStackTrace);
                if (!luamatch.Success)
                {
                    return false;
                }
                else
                {
                    List<string> luas = SceneManager.instance.GetPathFiles(ResConfig.XLUAPATH, ".lua");
                    if (luas != null)
                    {
                        string luaName = luamatch.Groups[1].Value+ ".lua";
                        for (int i = 0; i < luas.Count; i++)
                        {
                            if (luas[i].EndsWith(luaName))
                            {
                                InternalEditorUtility.OpenFileAtLineExternal(luas[i], int.Parse(luamatch.Groups[2].Value));
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static string GetSelectedStackTrace()
        {
            Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;
            if (editorWindowAssembly == null)
            {
                return null;
            }

            System.Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");
            if (consoleWindowType == null)
            {
                return null;
            }

            FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
            if (consoleWindowFieldInfo == null)
            {
                return null;
            }

            EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;
            if (consoleWindow == null)
            {
                return null;
            }

            if (consoleWindow != EditorWindow.focusedWindow)
            {
                return null;
            }

            FieldInfo activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
            if (activeTextFieldInfo == null)
            {
                return null;
            }

            return (string)activeTextFieldInfo.GetValue(consoleWindow);
        }
    }
}
