//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class BackRunObject : FUIObject
    {
        public float mOffset = 0.1f;
        public GameObject nCamera;
        public GameObject nBackObject;
        public Vector3 nNextPos = new Vector3(100, 0, 0);
        public Vector3 mAutoSpeed;
        private Vector3 nLastPos;
        [FFloatAttr(0,1)]
        public float mDp = 0.0f;
        private List<GameObject> mBackList = new List<GameObject>();
        void Start()
        {
            if(nCamera == null)
            {
                nCamera = Camera.main.gameObject;
            }

            if(nBackObject == null)
            {
                nBackObject = this.transform.GetChild(0).gameObject;
            }
            mBackList.Add(nBackObject);
            if (nNextPos.x > -1)
            {
                GameObject clone = SceneManager.CloneObject(nBackObject, nBackObject.transform.parent.gameObject);
                clone.transform.localPosition += nNextPos;
                mBackList.Add(clone);
            }

            if (mBackList.Count == 2)
            {
                nLastPos = nCamera.transform.position;
            }
        }


        void LateUpdate()
        {
            Vector3 tempPos = (nCamera.transform.position - nLastPos) * (1 - mDp) + mAutoSpeed * Time.deltaTime;
            nLastPos = nCamera.transform.position;
            tempPos.y = 0;
            tempPos.z = 0;
            this.gameObject.transform.position += tempPos;

            if (mBackList.Count == 2)
            {
                if (nCamera.transform.position.x > mBackList[1].gameObject.transform.position.x+ mOffset)
                {
                    mBackList[0].gameObject.transform.position = mBackList[1].gameObject.transform.position;
                    mBackList[1].gameObject.transform.localPosition += nNextPos;

                }
                else if (nCamera.gameObject.transform.position.x < mBackList[0].gameObject.transform.position.x+ mOffset)
                {
                    mBackList[1].gameObject.transform.position = mBackList[0].gameObject.transform.position;
                    mBackList[0].gameObject.transform.position -= nNextPos;
                    
                }
            }
        }
    }

}