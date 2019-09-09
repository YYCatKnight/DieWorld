using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

namespace F2DEngine
{
    public class UIManager : ManagerTemplate<UIManager>
    {
        #region UI界面管理
        protected override void OnInit()
        {
            FEngineManager.ShowWindosCall = HandWindos;
        }

        //[nc]表示不关闭任何窗口,[pool]使用池存储界面,[back]界面关闭时回退,[reset]重刷界面
        public BasePlane ShowWindos(string name, params object[] arg)
        {
            List<string> keys = FUniversalFunction.GetChunk(name, "[", "]");
            name = keys[0];
            WinShowType type = WinShowType.WT_Normal;
            if (keys.Contains("nc"))
            {
                type |= WinShowType.WT_NoClose;
            }
            if (keys.Contains("pool"))
            {
                type |= WinShowType.WT_Pool;
            }
            if (keys.Contains("back"))
            {
                type |= WinShowType.WT_Back;
            }
            if (keys.Contains("reset"))
            {
                type |= WinShowType.WT_Reset;
            }
            return ShowWindos(name, type, arg);
        }
        public const string ShowWindowEvent = "ShowWindowEvent";
        public BasePlane ShowWindos(string name, WinShowType type = WinShowType.WT_Normal, params object[] arg)
        {
            var bp = FEngineManager.ShowWindos(name, type, arg);
            EventListenManager.Send(ShowWindowEvent);
            //HandWindos(bp);
            /*
            if (bp != null)
            {
                bp.transform.localEulerAngles = new Vector3(0, 180, 0);
                var texts = bp.GetComponentsInChildren<Text>(true);
                for (int i = 0; i < texts.Length; i++)
                {
                    texts[i].MirrorText();
                }
            }
            */
            return bp;
        }

        public int isExtraHight = -1;
        public void HandWindos(BasePlane bp)
        {
            if (isExtraHight > 0)
            {
                if (bp != null)
                {
                    if (bp.nUiType == UIWIND_TYPE.UI_NORMAL || bp.nUiType == UIWIND_TYPE.UI_SOLID)
                    {
                        HandWindowEx(bp);
                    }
                    else if (bp.nUiType == UIWIND_TYPE.UI_SOLF)
                    {
                        if (bp.PoolName == ResConfig.FNOVICEPLANE)
                        {
                            HandWindowEx(bp);
                        }
                    }

                }
            }

            if (bp != null)
            {
                if (bp.nUiType == UIWIND_TYPE.UI_TEMP)
                {
                    var t = bp.transform.localPosition;
                    t.z = -15;
                    bp.transform.localPosition = t;
                }
            }
            /*
            if(FEngine.instance.IsPad)
            {
                if (bp != null)
                {
                    if (bp.Ipad)
                    {
                        float s = 1.7778f * Screen.width / Screen.height;
                        RectTransform rectTransform = (bp.transform as RectTransform);
                        float w = rectTransform.rect.width * (1.0f/s-1);
                        float h = rectTransform.rect.height * (1.0f/s-1);
                        bp.transform.localScale = new Vector3(s, s,1);
                        rectTransform.sizeDelta = new Vector2(w,h);
                    }
                }
            }
            */
        }

        public void HandWindowEx(BasePlane bp)
        {
            RectTransform rectTransform = (bp.transform as RectTransform);
            rectTransform.offsetMin = new Vector2(0, isExtraHight);
            rectTransform.offsetMax = new Vector2(0, -isExtraHight);
        }

        public void CloseWindos(BasePlane p)
        {
            FEngineManager.CloseWindos(p);
        }


        public void ClearBackWindos()
        {
            FEngineManager.ClearBackWindos();
        }

        public void ResetWindos(string keyName)
        {
            FEngineManager.ResetWindos(keyName);
        }

        public void CloseWindos(string name = "", bool isAuto = true)
        {
            FEngineManager.CloseWindos(name, isAuto);
        }

        public bool IsActiveWindos(UIWIND_TYPE type)
        {
            return FEngineManager.IsActiveWindos(type);
        }
        public BasePlane GetActiveWindos(string name)
        {
            return FEngineManager.GetActiveWindos(name);
        }
        #endregion

        public void ShowSceneLayer(bool isShow)
        {
            ShowLayer(isShow, UIPlane.MainPlane);
            MainCanvas.instance.ShowLayer(isShow, "N_UI");
        }

        public void ShowLayer(bool isShow, string mainPlane = UIPlane.MainPlane)
        {
            var plane = GetActiveWindos(mainPlane);
            if (plane != null)
            {
                if (!isShow)
                {
                    plane.transform.localPosition = new Vector3(9999, 9999, 0);
                }
                else
                {
                    plane.transform.localPosition = new Vector3(0, 0, 0);
                }
            }
        }
    }
}