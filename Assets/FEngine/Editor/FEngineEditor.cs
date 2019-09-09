using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using F2DEngine;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(FEngine))]
public class ClientEditor : Editor
{
    private FEngine np;

    // 添加UniversalPanel组件的GameObject被选中时触发该函数
    void OnEnable()
    {
        np = target as FEngine;
    } 

    // 重写Inspector检视面板
    public override void OnInspectorGUI()
    {      
        np.IsNoPack = EditorGUILayout.Toggle("不打包模式(打一次包可用)", np.IsNoPack);
        np.DebugType = (FEngine.FDebugType)EditorGUILayout.EnumPopup("Debug类型", np.DebugType);
        MyEdior.KeepScene();
    }
}
