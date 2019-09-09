using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using F2DEngine;

[CustomEditor(typeof(BasePlane), true)]
public class BasePlaneEditor : Editor
{

    protected BasePlane np;

    // 添加UniversalPanel组件的GameObject被选中时触发该函数
    private static string[] AnimationName = new string[]{"无","Open"};
    void OnEnable()
    {
        np = target as BasePlane;
        Init();
    }

    public virtual void Init()
    {

    }


    private string GetAnimationName(int index)
    {
        if (index == 0)
            return "";
        return AnimationName[index];
    }

    private int GetAnimationIndex(string name)
    {
        for(int i = 0; i < AnimationName.Length;i++)
        {
            if(AnimationName[i] == name)
            {
                return i;
            }
        }
        return 0;
    }

    // 重写Inspector检视面板
    public override void OnInspectorGUI()
    {
        OnBeginGUI();
        np.nUiType = (UIWIND_TYPE)EditorGUILayout.EnumPopup("界面类型", np.nUiType);
        np.RefreshType = (UIRefresh_Type)EditorGUILayout.EnumPopup("刷新界面", np.RefreshType);
        np.LayerType = (LayerType)EditorGUILayout.EnumPopup("层级", np.LayerType);
        np.UsePool = EditorGUILayout.Toggle("使用缓存池", np.UsePool);
        np.StarAnimation = GetAnimationName(EditorGUILayout.Popup("打开动画", GetAnimationIndex(np.StarAnimation), AnimationName));
        np.CloseAnimation = GetAnimationName(EditorGUILayout.Popup("关闭动画", GetAnimationIndex(np.CloseAnimation), AnimationName));
        OnEndGUI();
    }

    protected virtual void OnBeginGUI()
    {

    }

    protected virtual void OnEndGUI()
    {

    }
}
