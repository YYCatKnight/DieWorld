//----------------------------------------------
//  F2DEngine: time: 2017.4  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace F2DEngine
{
    public class ScreenImage : UnitObject
    {
        private Sprite m2D;
        public void Start()
        {
            PlayScreen(MainCanvas.instance.GetMianCamera());
        }
        public void PlayScreen(Camera ca)
        {
            StopAllCoroutines();
            Image image = this.GetComponent<Image>();
            if (image == null)
                return;
            Color temp = image.color;
            image.color = new Color(0,0,0,0);
            RectTransform rt = this.GetComponent<RectTransform>();

            Vector3 tempCenter = ca.WorldToScreenPoint(rt.position);
                  
            StartCoroutine(CaptureByCamera(ca, new Rect(0,0,Screen.width, Screen.height),(texture) => 
            {
                m2D = Sprite.Create(texture, new Rect(tempCenter.x- rt.sizeDelta.x/2, tempCenter.y - rt.sizeDelta.y/2, rt.sizeDelta.x, rt.sizeDelta.y), Vector2.zero);
                image.sprite = m2D;
                image.color = temp;
            }));   
        }

      

        private IEnumerator CaptureByCamera(Camera mCamera, Rect mRect,Action<Texture2D> callBack)
        {
            //等待渲染线程结束  
            yield return new WaitForEndOfFrame();
            //初始化RenderTexture  
            RenderTexture mRender = new RenderTexture((int)mRect.width, (int)mRect.height, 0);
            //设置相机的渲染目标  
            mCamera.targetTexture = mRender;
            //开始渲染  
            mCamera.Render();

            //激活渲染贴图读取信息  
            RenderTexture.active = mRender;

            Texture2D mTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            //读取屏幕像素信息并存储为纹理数据  
            mTexture.ReadPixels(mRect, 0, 0);
            //应用  
            mTexture.Apply();

            //释放相机，销毁渲染贴图  
            mCamera.targetTexture = null;
            RenderTexture.active = null;
            GameObject.Destroy(mRender);
            callBack(mTexture);
        }
    }
}
