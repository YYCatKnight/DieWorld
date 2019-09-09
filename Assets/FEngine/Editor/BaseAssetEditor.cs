using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using F2DEngine;

[CustomEditor(typeof(BaseAsset),true)]
public class BaseAssetEditor : Editor
{
    private string mFindName = "";
    private List<SerializedProperty> mFindPros = new List<SerializedProperty>();
    private bool mIsShowAll = false;
    private IList mMainList;
    void OnEnable()
    {
        if (target != null)
        {
            var listPro = target.GetType().GetField("ProList");
            if (listPro != null)
            {
                var list = listPro.GetValue(target);
                if (list != null)
                {
                    mMainList = (IList)list;
                }
            }
        }
    }

    
    public override void OnInspectorGUI()
    {
        if (mMainList != null)
        {
            mIsShowAll = EditorGUILayout.Toggle("数据量:" + mMainList.Count.ToString() + "  全显示", mIsShowAll);
            if(mIsShowAll)
            {
                base.OnInspectorGUI();
            }
            else
            {
                EditorGUILayout.LabelField("查找数据,=精确查找");
                mFindName = EditorGUILayout.TextField(mFindName);
                if(GUILayout.Button("查找"))
                {
                    mFindPros.Clear();
                    if (!string.IsNullOrEmpty(mFindName))
                    {
                        var pro = serializedObject.FindProperty("ProList");
                        if (pro != null)
                        {
                            string tempName = mFindName;
                            bool isExact = tempName[0] == '=';
                            if(isExact)
                            {
                                tempName = tempName.Substring(1);
                            }

                            if (!string.IsNullOrEmpty(tempName))
                            {
                                for (int i = 0; i < mMainList.Count; i++)
                                {
                                    var d = (BaseAssetProperty)mMainList[i];
                                    if ((d.Only_id.IndexOf(tempName,System.StringComparison.OrdinalIgnoreCase) != -1 &&!isExact)||(d.Only_id == tempName))
                                    {
                                        SerializedProperty st = pro.GetArrayElementAtIndex(i);
                                        if (st != null)
                                        {
                                            mFindPros.Add(st);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if(mFindPros.Count > 0)
                {
                    EditorGUILayout.LabelField("查找结果");
                    for (int i = 0; i < mFindPros.Count;i++)
                    {
                        EditorGUILayout.PropertyField(mFindPros[i], true, null);                        
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
        else
        {
            base.OnInspectorGUI();
        }
    }
}
