//----------------------------------------------
//  F2DEngine: time: 2016.7  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FCMainCanvas : FBaseController<FCMainCanvas>
    {
        public Vector2Int UISize { get; protected set; }
        public Vector2 UIUint { get; protected set; }
        private FCUniversalPanel mMainPlane;
        protected Camera mUICamera;
        private RectTransform mLowPlane;
        private RectTransform mUIPlane;
        private RectTransform HigPlane;
        private FGroup<RectTransform> mOtherPlane;
        protected override void Init()
        {
            DontDestroyOnLoad(this.gameObject);
            mMainPlane = this.GetComponent<FCUniversalPanel>();
            mUICamera = mMainPlane.GetFObject<Camera>("F_Camera");
            mLowPlane = mMainPlane.GetFObject<RectTransform>("F_LowPlane");
            mLowPlane.gameObject.SetActive(false);
            HigPlane = mMainPlane.GetFObject<RectTransform>("F_HigPlane");
            HigPlane.gameObject.SetActive(false);
            RectTransform rt = mMainPlane.GetFObject<RectTransform>("F_Plane");
            mOtherPlane = new FGroup<RectTransform>();
            mOtherPlane.Init(rt, (i, t) =>
            {
                t.name = ((LayerType)i).ToString();
                t.anchoredPosition3D = Vector3.zero;
                t.sizeDelta = Vector3.zero;
                t.gameObject.SetActive(false);
            });
            mOtherPlane.Refurbish((int)LayerType.LT_Hig);
            mUIPlane = mOtherPlane[(int)LayerType.LT_Normal];
        }
        private void Start()
        {
            var size = mUIPlane.transform.InverseTransformPoint(mUICamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height))) - mUIPlane.transform.InverseTransformPoint(mUICamera.ScreenToWorldPoint(Vector3.one));
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            UISize = new Vector2Int((int)size.x,(int)size.y);
            UIUint = new Vector2(size.x / Screen.width, size.y / Screen.height);
        }
        public void ShowLayer(bool isShow, string layName)
        {
            if (isShow)
            {
                mUICamera.cullingMask |= (1 << LayerMask.NameToLayer(layName));

            }
            else
            {
                mUICamera.cullingMask &= ~(1 << LayerMask.NameToLayer(layName));
            }
        }
        public Camera GetMianCamera()
        {
            return mUICamera;
        }
        public void SetLayer(LayerType layer,GameObject go)
        {
            RectTransform rt = go.GetComponent<RectTransform>();
            go.transform.SetParent(GetLayer(layer));
            if (rt != null)
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }
        private RectTransform _ComputerPlane(RectTransform plane)
        {
            plane.gameObject.SetActive(true);
            return plane;
        }
        public RectTransform GetLayer(LayerType type)
        {
            if(type == LayerType.LT_Low)
            {
                return _ComputerPlane(mLowPlane);
            }
            else if(type == LayerType.LT_Hig)
            {
                return _ComputerPlane(HigPlane);
            }
            return _ComputerPlane(mOtherPlane[(int)type]);
        }
    }
}
