using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using F2DEngine;

[CustomEditor(typeof(SpriteTexture))]
public class SpriteTextureEditor : Editor
{
    private SpriteTexture TI;
    void OnEnable()
    {
        TI = target as SpriteTexture;
        if(TI.SpriteRenderer == null)
        {
            TI.SpriteRenderer = TI.GetComponent<SpriteRenderer>();
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUI.changed)
        {
            UpdateTexture();
        }
    }

    private void UpdateTexture()
    {
        TI.ResetTexture();
    }
}
