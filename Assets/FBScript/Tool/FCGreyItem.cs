//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace F2DEngine
{
    public class FCGreyItem : UnitObject
    {
        public enum GreyType
        {
            GT_None = 0,
            GT_Sprite = 1<<4,//2DSprite
            GT_ForbidButton = 1<<5,//禁止按钮
        }
        private List<GreyTool> mToos = new List<GreyTool>();
        private interface GreyTool
        {
            void Clear();
            void Grey(Material mat);
            void Dark(Color color);
        }
        private class UIGrey: GreyTool
        {
            private MaskableGraphic mGrap;
            private Color mColor;
            public UIGrey(MaskableGraphic grap)
            {
                mGrap = grap;
                mColor = grap.color;
            }

            public void Clear()
            {
                mGrap.color = mColor;
                mGrap.material = null;
            }

            public void Dark(Color color)
            {
                mColor = mGrap.color;
                color.a = mColor.a;
                mGrap.color = color;
            }

            public void Grey(Material mat)
            {
                mColor = mGrap.color;
                if (mGrap is Text)
                {
                    mGrap.color = Color.gray;
                }
                else
                {
                    mGrap.material = mat;
                }
            }
        }
        private class ButtonGrey:GreyTool
        {
            private Button mUIButton;
            private bool mInteractable = false;
            public ButtonGrey(Button grap)
            {
                mUIButton = grap;
                mInteractable = grap.interactable;
            }

            public void Clear()
            {
                mUIButton.interactable = mInteractable;
            }

            public void Dark(Color color)
            {
                mInteractable = mUIButton.interactable;
                mUIButton.interactable = false;
            }

            public void Grey(Material mat)
            {
                mInteractable = mUIButton.interactable;
                mUIButton.interactable = false;
            }
        }
        private class SpriteGrey:GreyTool
        {
            private SpriteRenderer mSprite;
            private Color mColor;
            public SpriteGrey(SpriteRenderer spr)
            {
                mSprite = spr;
                mColor = mSprite.color;
            }

            public void Clear()
            {
                mSprite.material = new Material(Shader.Find("Sprites/Default"));
                mSprite.color = mColor;
            }

            public void Dark(Color color)
            {
                mColor = mSprite.color;
                color.a = mColor.a;
                mSprite.color = color;
            }

            public void Grey(Material mat)
            {
                mColor = mSprite.color;
                mSprite.material = mat;
            }
        }
        public override void Clear()
        {
            for (int i = 0; i < mToos.Count;i++)
            {
                mToos[i].Clear();
            }
            mTag = 0;
        }
        private void SetDrak(Color color)
        {
            for (int i = 0; i < mToos.Count; i++)
            {
                mToos[i].Dark(color);
            }
        }
        private void SetGray(Material mat)
        {
            for (int i = 0; i < mToos.Count; i++)
            {
                mToos[i].Grey(mat);
            }
        }
        private int mTag = 0;//记录行为
        private GreyType mGreyType = GreyType.GT_None;
        private void PlayGray(int tag,GreyType type,Material mat,Color color)
        {
            Init(type);
            if(mTag == tag)
            {
                return;
            }
            Clear();
            if(mat == null)
            {
                SetDrak(color);
            }
            else
            {
                SetGray(mat);
            }
            mTag = tag;
        }
        private void Init(GreyType type)
        {
            if(mToos.Count != 0)
            {
                if(type != mGreyType)
                {
                    Clear();
                    mToos.Clear();
                }
            }

            if(mToos.Count == 0)
            {
                mGreyType = type;
                bool IsSpr = FUniversalFunction.IsContainSameType((int)type, (int)GreyType.GT_Sprite);
                if (IsSpr)
                {
                    SpriteRenderer[] sprs = this.gameObject.GetComponentsInChildren<SpriteRenderer>(true);
                    for (int i = 0; i < sprs.Length; i++)
                    {
                        SpriteGrey gr = new SpriteGrey(sprs[i]);
                        mToos.Add(gr);
                    }
                }
                else
                {
                    MaskableGraphic[] maskGraps = this.gameObject.GetComponentsInChildren<MaskableGraphic>(true);
                    for (int i = 0; i < maskGraps.Length; i++)
                    {
                        UIGrey gr = new UIGrey(maskGraps[i]);
                        mToos.Add(gr);
                    }

                    bool GreyButton = FUniversalFunction.IsContainSameType((int)type, (int)GreyType.GT_ForbidButton);
                    if (GreyButton)
                    {
                        Button[] buttons = this.gameObject.GetComponentsInChildren<Button>(true);
                        for (int i = 0; i < buttons.Length; i++)
                        {
                            ButtonGrey gr = new ButtonGrey(buttons[i]);
                            mToos.Add(gr);
                        }
                    }
                }              
            }
        }
        public void PlayDrak(Color color,GreyType type)
        {
            PlayGray(1,type, null, color);
        }
        public void PlayGray(Material mat,GreyType type)
        {
            PlayGray(2,type,mat,Color.white);
        }
    }
}
