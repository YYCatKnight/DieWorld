//----------------------------------------------
//  F2DEngine: time: 2017.2  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FSpineBody : FBodyBase
    {
        /*
        public class SpineAnimation
        {
            private SkeletonAnimation animation;
            private SkeletonGraphic graphic;
            private ISkeletonComponent ISkeleton;
            private IAnimationStateComponent IState;
            public void SetSpineAnimation(SkeletonAnimation ske, SkeletonGraphic gr)
            {
                animation = ske;
                graphic = gr;
                if (animation != null)
                {
                    ISkeleton = animation;
                    IState = animation;
                }
                else
                {
                    ISkeleton = graphic;
                    IState = graphic;
                }
            }

            public Transform transform
            {
                get
                {
                    if (animation)
                    {
                        return animation.transform;
                    }
                    else
                    {
                        return graphic.transform;
                    }
                }
            }

            public Skeleton skeleton
            {
                get
                {
                    return ISkeleton.Skeleton;
                }
            }
            public Spine.AnimationState state
            {
                get { return AnimationState; }
            }

            public Spine.Skeleton Skeleton
            {
                get
                {
                    return ISkeleton.Skeleton;
                }
            }

            public Spine.AnimationState AnimationState
            {
                get
                {
                    return IState.AnimationState;
                }
            }

            public SkeletonDataAsset skeletonDataAsset
            {
                get
                {
                    return ISkeleton.SkeletonDataAsset;
                }
            }

            public float timeScale
            {
                get
                {
                    if (animation)
                    {
                        return animation.timeScale;
                    }
                    else
                    {
                        return graphic.timeScale;
                    }
                }
                set
                {
                    if (animation)
                    {
                        animation.timeScale = value; ;
                    }
                    else
                    {
                        graphic.timeScale = value;
                    }
                }
            }
        }

        private SpineAnimation nBodyAnimatror;
        private Action<FAnimatorData> mCallBack;
        private List<FAnimatorData> mFAnimatorDataList = new List<FAnimatorData>();
        private Dictionary<string, SlotRegionPair> mSlotRegionPairs = new Dictionary<string, SlotRegionPair>();
        private StateTimeEvent mStateTimeEvent;
        private float mCurTimeScale = 0;
        private bool nIsInitEvent = false;
        private Skin mSkin;


        public SpineAnimation GetSpineAnimation()
        {
            return nBodyAnimatror;
        }
        public class SlotRegionPair
        {
            public string slot;
            public string equipName;
            public string atlas;
            public string slotQquipName;
            public SlotOffse offse;

            public class SlotOffse
            {
                public float x;
                public float y;
                public float rot;
            }
        }

        public override void Init()
        {
            if (nBodyAnimatror == null)
            {
                nBodyAnimatror = new SpineAnimation();
                nBodyAnimatror.SetSpineAnimation(this.GetComponent<SkeletonAnimation>(), this.GetComponent<SkeletonGraphic>());
                mStateTimeEvent = new StateTimeEvent(UpdateTimeState);
            }
        }


        public void ResumPlay()
        {
            if (nBodyAnimatror.AnimationState.TimeScale == 0)
            {
                nBodyAnimatror.AnimationState.TimeScale = mCurTimeScale;
            }
        }

        public void PausePlay()
        {
            mCurTimeScale = nBodyAnimatror.AnimationState.TimeScale;
            nBodyAnimatror.AnimationState.TimeScale = 0;
        }

        private void UpdateTimeState(StateTimeEvent.STimeData st, StateTimeEvent.STimeType type)
        {
            if (type == StateTimeEvent.STimeType.ST_Normal)
            {
                if (mCallBack != null)
                {
                    mCallBack((FAnimatorData)st.ex);
                }
            }
        }

        public void UpdateAnyTime(float timeDp)
        {
            nBodyAnimatror.state.Update(timeDp);
            nBodyAnimatror.state.Apply(nBodyAnimatror.skeleton);
            nBodyAnimatror.skeleton.UpdateWorldTransform();
        }

        public override void PlayUpdate(float dp)
        {
            mStateTimeEvent.PlayLogic(dp);
        }

        private bool _regsionEvent()
        {
            if (nIsInitEvent || nBodyAnimatror.state == null)
                return false;
            nBodyAnimatror.state.Start += (f) =>
            {
                TrackEntryDelegate(0, f);
            };
            nBodyAnimatror.state.Interrupt += (f) =>
            {
                TrackEntryDelegate(1, f);
            };
            nBodyAnimatror.state.End += (f) =>
            {
                TrackEntryDelegate(2, f);
            };
            nBodyAnimatror.state.Dispose += (f) =>
            {
                TrackEntryDelegate(3, f);
            };
            //nBodyAnimatror.state.Complete += (f) =>
            //{
            //    TrackEntryDelegate(100, f);
            //};

            nBodyAnimatror.state.Event += (f, e) =>
            {
                TrackEntryDelegate(-1, f, e.Data.Name);
            };
            nIsInitEvent = true;
            return true;
        }
        private void Start()
        {

            _regsionEvent();
        }


        public override void SetColor(Color c)
        {
            nBodyAnimatror.skeleton.r = c.r;
            nBodyAnimatror.skeleton.g = c.g;
            nBodyAnimatror.skeleton.b = c.b;
            nBodyAnimatror.skeleton.a = c.a;
        }

        public void SetColor(Color c, string slotName)
        {
            var slot = nBodyAnimatror.skeleton.FindSlot(slotName);
            if (slot != null)
            {
                slot.r = c.r;
                slot.g = c.g;
                slot.b = c.b;
                slot.a = c.a;
            }
        }

        public override string GetAnimationName(int lay = 0)
        {
            TrackEntry te = nBodyAnimatror.state.GetCurrent(lay);
            if (te != null)
            {
                return te.Animation.Name;
            }
            return "";
        }
        //public override int GetAnimationKey(string key)
        //{
        //    return 0;
        //}
        //public override void PlayAnimationKey(string key, int value)
        //{

        //}

        public void PlayAnimation(int lay, string animationName, bool isLoop = true)
        {
            nBodyAnimatror.AnimationState.SetAnimation(0, animationName, isLoop);
        }


        public override void PlayAnimation(string keyName, bool isLoop, int lay)
        {
            nBodyAnimatror.AnimationState.SetAnimation(lay, keyName, isLoop);
        }
        public override void SetSpeed(float fps)
        {
            nBodyAnimatror.timeScale = fps;
        }

        public override void AddEventName(string anmation, float len, string key)
        {
            for (int i = 0; i < mFAnimatorDataList.Count; i++)
            {
                FAnimatorData d = mFAnimatorDataList[i];
                if (d.Key == key && d.Name == anmation)
                {
                    return;
                }
            }
            FAnimatorData fd = new FAnimatorData(key,anmation,len,null);
            mFAnimatorDataList.Add(fd);
        }

        public override void RegEvent(Action<FAnimatorData> callBack)
        {
            mCallBack = callBack;
        }


        private void TrackEntryDelegate(int type, Spine.TrackEntry trackEntry, string ex = "")
        {
            string anmaitonName = trackEntry.animation.name;
            if (type == -1)
            {
                anmaitonName = ex;
                type = 0;
            }

            for (int i = 0; i < mFAnimatorDataList.Count; i++)
            {
                FAnimatorData d = mFAnimatorDataList[i];
                if (d.Name == anmaitonName)
                {
                    if (type == 0)
                    {
                        mStateTimeEvent.AddState(d.Key, d.Percent * trackEntry.animation.duration).ex = d;
                    }
                    else
                    {
                        mStateTimeEvent.Reset(d.Key);
                    }
                }
            }
        }




        public Vector3 GetBoneWorldPos(string boneName)
        {
            var bone = nBodyAnimatror.skeleton.FindBone(boneName);
            float x = bone.worldX;
            float y = bone.worldY;
            Vector3 pos = new Vector3(x, y, 0);
            pos = this.transform.TransformPoint(pos);
            return pos;
        }
        public Vector3 GetBoneWorldRot(string boneName)
        {
            var bone = nBodyAnimatror.skeleton.FindBone(boneName);
            float x = bone.WorldRotationX;
            Vector3 worldRotation = nBodyAnimatror.transform.rotation.eulerAngles;
            Vector3 rot = new Vector3(worldRotation.x, worldRotation.y, x + worldRotation.z);
            return rot;
        }

        //特有接口

        public void SetSkin(string id)
        {
            SpineProperty pro = SpineAsset.instance.GetProperty(id);
            if (pro != null)
            {
                //切换皮肤
                if (!string.IsNullOrEmpty(pro.DefaultSkin))
                {
                    ChangeSkin(pro.DefaultSkin);
                }


                if (pro.SkinData != null)
                {
                    List<SlotRegionPair> slots = new List<SlotRegionPair>();

                    for (int i = 0; i < pro.SkinData.Length; i++)
                    {
                        var data = pro.SkinData[i];
                        if (string.IsNullOrEmpty(data.slotName))
                        {
                            continue;
                        }
                        SlotRegionPair srp = new SlotRegionPair();
                        srp.atlas = data.alasName;
                        srp.equipName = data.equipName;
                        srp.slot = data.slotName;
                        srp.slotQquipName = data.slotQquipName;

                        if (!string.IsNullOrEmpty(data.GloabData))
                        {
                            GlobalDataProperty gdp = GlobalDataAsset.instance.GetProperty(data.GloabData);
                            if (gdp != null)
                            {
                                srp.offse = gdp.GetValue<SlotRegionPair.SlotOffse>();
                            }
                        }
                        slots.Add(srp);
                    }
                    ChangeAttachments(slots);
                }

            }
        }

        public void ChangeSkin(string skinName)
        {
            //切换套装方法1
            //nBodyAnimatror.initialSkinName = skinName;
            //nBodyAnimatror.Initialize(true);

            //切换套装方法2
            nBodyAnimatror.Skeleton.SetSkin(skinName);
            nBodyAnimatror.Skeleton.SetSlotsToSetupPose();

        }


        private void _BeignSkin()
        {
            mSkin = nBodyAnimatror.skeleton.UnshareSkin(true, false, nBodyAnimatror.AnimationState);
        }

        private void _EndSkin()
        {
            if (mSkin != null)
            {
                nBodyAnimatror.skeleton.SetSkin(mSkin);
                nBodyAnimatror.skeleton.SetToSetupPose();
                mSkin = null;
            }
        }

        //添加部件
        private void addAttachments(SlotRegionPair srp)
        {
            var newSkin = mSkin;
            mSlotRegionPairs[srp.slot] = srp;
            AtlasAsset atlasasset = null;
            if (string.IsNullOrEmpty(srp.atlas))
            {
                atlasasset = nBodyAnimatror.skeletonDataAsset.atlasAssets[0];
            }
            else
            {
                atlasasset = SceneManager.LoadAsset<AtlasAsset>(srp.atlas);
            }

            Atlas at = atlasasset.GetAtlas();
            float scale = nBodyAnimatror.skeletonDataAsset.scale;
            int slotIndex = nBodyAnimatror.skeleton.FindSlotIndex(srp.slot);
            var region = at.FindRegion(srp.equipName);
            if (region == null)
            {
                Debug.LogError("换装失败--!图集:[" + atlasasset.name + "]" + srp.equipName + "没有找到");
                return;
            }
            var attachment = region.ToRegionAttachment(srp.equipName, scale);
            attachment.SetRotation(srp.offse.rot);
            attachment.SetPositionOffset(srp.offse.x, srp.offse.y);
            attachment.UpdateOffset();
            string name = string.IsNullOrEmpty(srp.slotQquipName) ? srp.slot : srp.slotQquipName;
            newSkin.AddAttachment(slotIndex, name, attachment);
        }

        //移除部件
        private void _DeleteAttachments(string slotName)
        {
            var rSlot = nBodyAnimatror.skeleton.FindSlot(slotName);
            if (rSlot != null)
            {
                rSlot.Attachment = null;
            }
        }

        public void DeletAttachments(string slotName = "")
        {
            if (slotName == "")
            {
                foreach (var k in mSlotRegionPairs)
                {
                    _DeleteAttachments(k.Key);
                }
                mSlotRegionPairs.Clear();
            }
            else
            {
                _DeleteAttachments(slotName);
            }
        }


        public void ChangeAttachments(string slotName, string equipName, string atlasName)
        {
            _BeignSkin();
            SlotRegionPair sp = new SlotRegionPair();
            sp.equipName = equipName;
            sp.slot = slotName;
            sp.atlas = atlasName;
            addAttachments(sp);
            _EndSkin();
        }

        public void ChangeAttachments(List<SlotRegionPair> srps)
        {
            _BeignSkin();
            for (int i = 0; i < srps.Count; i++)
            {
                addAttachments(srps[i]);
            }
            _EndSkin();
        }

        public void ApplyAttachments()
        {
            _BeignSkin();
            foreach (var k in mSlotRegionPairs)
            {
                addAttachments(k.Value);
            }
            _EndSkin();
        }*/
    }
    
}
