//----------------------------------------------
//  F2DEngine: time: 2017.9  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
using System;

namespace F2DEngine
{
    public class FCMeshBody : FBodyBase
    {
        public class BodyPart
        {
            public List<CombineInstance> combines = new List<CombineInstance>();
            public List<Transform> bones = new List<Transform>();
            public List<Material> mats = new List<Material>();
        }

        private Dictionary<string, BodyPart> mParts = new Dictionary<string, BodyPart>();//装备信息
        private Transform[] mBones; //骨骼
        private GameObject mBody;
        private Dictionary<string, GameObject> mWeapons = new Dictionary<string, GameObject>();

        private FCAnimator mBodyAnimator;

        public override void Init()
        {
            if(mBodyAnimator == null)
            {
                mBodyAnimator = this.GetComponent<FCAnimator>();
                mBodyAnimator = mBodyAnimator == null ? this.GetComponentInChildren<FCAnimator>() : mBodyAnimator;
            }
            if(mBodyAnimator != null)
            {
                mBodyAnimator.Init();
            }
        }

        public override void SetColor(Color c)
        {
           
        }

        public override string GetAnimationName(int lay)
        {
            return mBodyAnimator.GetCurAnimationName();
        }
        public override int GetAnimationKey(string key)
        {
            return mBodyAnimator.GetKey(key);
        }
        public override void PlayAnimationKey(string key, int value)
        {
            mBodyAnimator.SetKey(key, value);
        }

        public override void PlayAnimation(string keyName, bool isLoop, int lay)
        {
            mBodyAnimator.PlayCrossFade(keyName, 0.2f);
        }
        public override void SetSpeed(float fps)
        {
            mBodyAnimator.SetSpeed(fps);
        }


        public override void AddEventName(string anmation, float pre, string key="")
        {
            mBodyAnimator.AddEventName(anmation,pre,key);
        }

        public override void RegEvent(Action<FAnimatorData> callBack)
        {
            mBodyAnimator.RegEvent(callBack);
        }



        public class AttachmentPair
        {
            public string partName;
            public string sourceName;
        }

        private void ChangeAttachment(AttachmentPair part)
        {
           
        }


        public void ChangeAttachments(List<AttachmentPair> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                ChangeAttachment(parts[i]);
            }
        }

        public void ChangeAttachment(string partName, string sourceName,bool isUpdate = true)
        {
            AttachmentPair ap = new AttachmentPair();
            ap.partName = partName;
            ap.sourceName = sourceName;
            ChangeAttachment(ap);
            if (isUpdate)
            {
                Debug.Log("暂时没有实现");
            }
        }

        public GameObject ChangeWeapon(string boneName,string sourceName)
        {
            Transform bone = GetBone(boneName);
            if(bone)
            {
                GameObject weapon = null;
                if (sourceName != "")
                {
                    weapon = FEngineManager.Create(sourceName, bone.gameObject);
                }

                if(mWeapons.ContainsKey(boneName))
                {
                    FEngineManager.Remove(mWeapons[boneName]);
                }
                mWeapons[boneName] = weapon;
                return weapon;
            }
            else
            {
                Debug.LogError(sourceName + "武器切换,骨骼:" + boneName + "没有找到");
            }
            return null;
        }

        public void CreateBones(string boneName)
        {
            mBody = FEngineManager.Create(boneName,this.gameObject);
            mBones = mBody.GetComponentsInChildren<Transform>(true);
            FEngineManager.AddComponent<SkinnedMeshRenderer>(mBody);
            mBodyAnimator = FEngineManager.AddComponent<FCAnimator>(mBody);
            Init();
        }
      
        private Transform GetBone(string name)
        {
            for (int i = 0; i < mBones.Length; i++)
            {
                if (mBones[i].name == name)
                    return mBones[i];
            }
            return null;
        }
    }
}
