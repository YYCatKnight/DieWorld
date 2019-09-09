//----------------------------------------------
//  F2DEngine: time: 2017.9  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

namespace F2DEngine
{
    public class F3DMeshBody : MonoBehaviour
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

        private void ApplyAttachments()
        {
            SkinnedMeshRenderer sm = mBody.GetComponent<SkinnedMeshRenderer>();
            List<CombineInstance> coms = new List<CombineInstance>();
            List<Transform> bones = new List<Transform>();
            List<Material> mats = new List<Material>();

            foreach (var k in mParts)
            {
                var part = k.Value;
                coms.AddRange(part.combines);
                bones.AddRange(part.bones);
                mats.AddRange(part.mats);
            }
            sm.sharedMesh = new Mesh();
            sm.sharedMesh.CombineMeshes(coms.ToArray(), false, false);
            sm.bones = bones.ToArray();
            sm.materials = mats.ToArray();
        }

        public class AttachmentPair
        {
            public string partName;
            public string sourceName;
        }

        private void ChangeAttachment(AttachmentPair part)
        {
            BodyPart bp = new BodyPart();
            GameObject partObject = SceneManager.LoadPrefab<GameObject>(part.sourceName);
            SkinnedMeshRenderer[] smrs = partObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < smrs.Length; i++)
            {
                SkinnedMeshRenderer sm = smrs[i];

                //骨骼
                for (int b = 0; b < sm.bones.Length; b++)
                {
                    string boneName = sm.bones[b].name;
                    Transform bone = GetBone(boneName);
                    if (bone)
                    {
                        bp.bones.Add(bone);
                    }
                    else
                    {
                        Debug.LogError(part.partName + "部位,切换装备:" + part.sourceName + "没有找到" + boneName + "骨骼");
                    }
                }


                //mesh
                for (int s = 0; s < sm.sharedMesh.subMeshCount; s++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = sm.sharedMesh;
                    ci.subMeshIndex = s;
                    bp.combines.Add(ci);
                }

                //mat
                for (int m = 0; m < sm.sharedMaterials.Length; m++)
                {
                    bp.mats.Add(sm.sharedMaterials[m]);
                }

            }
            mParts[part.partName] = bp;
        }


        public void ChangeAttachments(List<AttachmentPair> parts)
        {
            for (int i = 0; i < parts.Count; i++)
            {
                ChangeAttachment(parts[i]);
            }
            ApplyAttachments();
        }

        public void ChangeAttachment(string partName, string sourceName)
        {
            AttachmentPair ap = new AttachmentPair();
            ap.partName = partName;
            ap.sourceName = sourceName;
            ChangeAttachment(ap);
            ApplyAttachments();
        }

        public GameObject ChangeWeapon(string boneName,string sourceName)
        {
            Transform bone = GetBone(boneName);
            if(bone)
            {
                GameObject weapon = null;
                if (sourceName != "")
                {
                    weapon = SceneManager.instance.Create(sourceName, bone.gameObject);
                }

                if(mWeapons.ContainsKey(boneName))
                {
                    SceneManager.instance.Remove(mWeapons[boneName]);
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
            mBody = SceneManager.instance.Create(boneName,this.gameObject);
            mBones = mBody.GetComponentsInChildren<Transform>(true);
            SceneManager.instance.AddComponent<SkinnedMeshRenderer>(mBody);
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
