using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteTexture : UnitObject
    {
        [HideInInspector]
        [SerializeField]
        public SpriteRenderer SpriteRenderer;
        public Texture2D Texture;
        private Sprite mSprite;

        void Awake()
        {
            if (mSprite == null && Texture != null && SpriteRenderer != null)
            {
                mSprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), new Vector2(0.5f, 0.5f));
                mSprite.name = Texture.name;
                SpriteRenderer.sprite = mSprite;
            }
        }
        public void ResetTexture()
        {
            if(SpriteRenderer != null)
            {
                if(Texture != null)
                {
                    if(mSprite == null||mSprite.name != Texture.name)
                    {
                        mSprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), new Vector2(0.5f, 0.5f));
                        mSprite.name = Texture.name;
                        SpriteRenderer.sprite = mSprite;
                    }
                }
                else
                {
                    mSprite = null;
                }
                SpriteRenderer.sprite = mSprite;
            }
        }
    }
}
