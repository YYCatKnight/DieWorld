//----------------------------------------------
//  F2DEngine: time: 2017.5  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    [ExecuteInEditMode]
    public class FEffectScene :UnitObject
    {
        [FRenameAttr("场景变颜色")]
        public Color nColor;
        private Material mMaterial;
        void Start()
        {
            SetDefaultMaterial();
        }

        private void SetDefaultMaterial()
        {
            mMaterial = new Material(SceneManager.LoadPrefab<Material>(ResConfig.MATERIAL_SCENECOLOR));
            mMaterial.SetColor("_NormalColor", nColor);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
#if UNITY_EDITOR
            SetDefaultMaterial();
#endif
            Graphics.Blit(source, destination, mMaterial);
        }
    }
}
