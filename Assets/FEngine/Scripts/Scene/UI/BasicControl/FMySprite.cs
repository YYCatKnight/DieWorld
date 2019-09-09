//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace F2DEngine
{
    public class FSpriteAnimation:UnitData 
    {
        protected FMySprite mFms;
        protected float mPlayTime = 0;
        protected float mDeltaSpeedTime = 1;


        public void SetDeltaTime(float speedTime)
        {
            mDeltaSpeedTime = speedTime;
        }
        public void  Init(FMySprite fms)
        {
            mFms = fms;
            Begin();
        }

        public virtual void Begin()
        {

        }
        public bool PlayFrame()
        {        
            mPlayTime += Time.deltaTime*mDeltaSpeedTime;
            return Play();
        }

        public virtual bool Play()
        {
            return false;
        }
        public virtual void End()
        {

        }
    }

    //飞X
    public class MySprFlyX : FSpriteAnimation
    {
        private List<Vector3> mDirList = new List<Vector3>();
        private Color mColor = Color.white;
        private float mSpeedTime = 5;
        private float mFlyTime = 1;
        private bool mIsDir = false;

        private List<float> mCurTimes = new List<float>();
        public MySprFlyX(float timeDp, float flySpeed, bool IsDri)
        {
            mSpeedTime = timeDp;
            mIsDir = IsDri;
            mFlyTime = flySpeed;

        }
        public override void Begin()
        {
            mFms.InitAndReset(mIsDir);
            if (mDirList.Count == 0)
            {
                //float DpTime = mSpeedTime / mFms.maxX;
                for (int x = 0; x < mFms.maxX; x++)
                {
                    for (int y = 0; y < mFms.maxY; y++)
                    {
                        float rot = Random.Range(0, 3.15f);
                        float num = Random.Range(50.0f, 400.0f);
                        mDirList.Add(new Vector3(Mathf.Sin(rot) * num, Mathf.Cos(rot) * num, 0));
                        mCurTimes.Add(0);
                    }
                }
            }
        }


        public override bool Play()
        {
            if (mPlayTime > mSpeedTime+mFlyTime)
                return false;

            int curPlayId = (int)(mFms.maxX * mPlayTime / (mSpeedTime));

            for (int y = 0; y < mFms.maxY; y++)
            {
                FMySubSprite fms = mFms.getFMySunSprite(0, y);
                for (int x = 0; x < mFms.maxX; x++)
                {
                   if (curPlayId < x)
                        break;

                   float tt = mCurTimes[x];
                    if(y == 0)
                    {
                        tt += Time.deltaTime;
                        mCurTimes[x] = tt;
                    }
                        
                    if (tt > mFlyTime + 0.1f)
                        continue;

                    if (tt > mFlyTime)
                    {
                        tt = mFlyTime;
                    }

                    mColor.a = 1 - tt / mFlyTime;
                    float nn = tt / mFlyTime;
                    if (!mIsDir)
                    {
                        mColor.a = 1 - mColor.a;
                        nn = 1 - nn;
                    }

                    mColor.a *= mColor.a;

                    fms.TransRelativePos(x, y, mDirList[x * mFms.maxY + y] * nn, new Vector3(mColor.a, mColor.a, 1));
                    fms.TransRelativeColor(x, y, mColor);
                }
              
            }
            mFms.UpdateMesh();
            return true;
        }

        public override void End()
        {
            mFms.InitAndReset(!mIsDir);
        }
    }


    //飞Y
    public class MySprFlyY:FSpriteAnimation
    {
        private List<Vector3> mDirList = new List<Vector3>();
        private Color mColor = Color.white;
        private float mSpeedTime = 5;
        private float mFlyTime = 1;
        private bool mIsDir = false;

        private List<float> mCurTimes = new List<float>();
        public MySprFlyY(float timeDp,float flySpeed,bool IsDri)
        {
            mSpeedTime = timeDp;
            mIsDir = IsDri;
            mFlyTime = flySpeed;
           
        }
        public override void Begin()
        {
            mFms.InitAndReset(mIsDir);
            if (mDirList.Count == 0)
            {
                //float DpTime = mSpeedTime / mFms.maxX;
                for (int x = 0; x < mFms.maxX; x++)
                {
                    for (int y = 0; y < mFms.maxY; y++)
                    {
                        float rot = Random.Range(-1.5f,1.5f);
                        float num = Random.Range(50.0f, 400.0f);
                        mDirList.Add(new Vector3(Mathf.Sin(rot) * num, Mathf.Cos(rot) * num, 0));
                        mCurTimes.Add(0);
                    }
                }
            }
        }


        public override bool Play()
        {
            if (mPlayTime > mSpeedTime)
                return false;

            int curPlayId = (int)(mFms.maxY * mPlayTime / (mSpeedTime-mFlyTime));

            int yy = mFms.maxY;
            int xx = mFms.maxX;

            for (int y = 0; y < yy; y++)
            {
                if (mIsDir)
                {
                    if (curPlayId < y)
                        break;
                }
                else
                {
                  if (curPlayId < mFms.maxY - y)
                        continue;
                }
                float tt = mCurTimes[y] + Time.deltaTime;
                mCurTimes[y] = tt;
                FMySubSprite fms = mFms.getFMySunSprite(0, y);

                if (tt > mFlyTime + 0.1f)
                    continue;
                if(tt > mFlyTime)
                {
                    tt = mFlyTime;
                }
                mColor.a = 1 - tt / mFlyTime;
                float nn = tt / mFlyTime;
                if (!mIsDir)
                {
                    mColor.a = 1 - mColor.a;
                    nn = 1 - nn;
                }

                mColor.a *= mColor.a;

                for (int x = 0; x < xx; x++)
                {
                    fms.TransRelativePos(x, y, mDirList[x * mFms.maxY + y] * nn, new Vector3(mColor.a, mColor.a, 1));
                    fms.TransRelativeColor(x, y, mColor);
                }
            }
            mFms.UpdateMesh();
            return true;
        }

        public override void End()
        {
            mFms.InitAndReset(!mIsDir);
        }
    }

    //爆炸类
    public class MySprBOMB: FSpriteAnimation
    {
        private List<Vector3> mDirList = new List<Vector3>();
        private Color mColor = Color.white;
        private float mSpeedTime = 5;
        private bool mIsDir = false;
        public MySprBOMB(float timeDp,bool IsDri)
        {
            mSpeedTime = timeDp;
            mIsDir = IsDri;
           
        }
        public override void Begin()
        {
            if (mDirList.Count == 0)
            {
                for (int x = 0; x < mFms.maxX; x++)
                {
                    for (int y = 0; y < mFms.maxY; y++)
                    {
                        float rot = Random.Range(0, 6.28f);
                        float num = Random.Range(50.0f, 500.0f);
                        mDirList.Add(new Vector3(Mathf.Sin(rot) * num, Mathf.Cos(rot) * num, 0));
                    }
                }
            }
        }

        public override bool Play()
        {
            if (mPlayTime > mSpeedTime)
                return false;

            mColor.a = 1 - mPlayTime / mSpeedTime;
            float nn = mPlayTime / mSpeedTime;
            if (!mIsDir)
            {
                mColor.a = 1 - mColor.a;
                nn = 1 - nn;
            }
         
            mColor.a *= mColor.a;
            for (int y = 0; y < mFms.maxY; y++)
            {
                FMySubSprite fms = mFms.getFMySunSprite(0, y);
                for (int x = 0; x < mFms.maxX; x++)
                {
                    fms.TransRelativePos(x, y, mDirList[x * mFms.maxY + y] * nn, new Vector3(mColor.a, mColor.a, 1));
                    fms.TransRelativeColor(x, y, mColor);
                }
            }

            mFms.UpdateMesh();
            return true;
        }

        public override void End()
        {
            mFms.InitAndReset(!mIsDir);
        }
    }
    public class FMySprite : FUIObject
    {
        private int mMaxPoint = 1000;
        public int maxX = 1;
        public int maxY = 1;
        private GameObject mHostGameObject;
        public System.Action<FMySprite> nCallFinshiEven;
        private List<FMySubSprite> mMySubSprs = new List<FMySubSprite>();
        private FMySubSprite.MeshMat mMeshMat;

        private string mMatName = "";
        
        public void SetMatName(string name)
        {
            mMatName = name;
        }
        public void PlayAnimation(FSpriteAnimation fsa)
        {
         
            StopAllCoroutines();
            StartCoroutine(PlayFun(fsa));
        }

        IEnumerator PlayFun(FSpriteAnimation fsa)
        {
            fsa.Init(this);
            yield return 0;
            while(fsa.PlayFrame())
            {
                yield return 0;
            }
            yield return 0;
            fsa.End();
            if (nCallFinshiEven != null)
            {
                nCallFinshiEven(this);
            }
        }
      
        public void CreateClip(int sx = 0, int sy = 0)
        {
            if(sx*sy != 0)
            {
                maxX = sx;
                maxY = sy;
            }
     
            int hightNum = mMaxPoint / maxX;
            if (hightNum == 0)
                hightNum = 1;
            int rowNum = maxY / hightNum;
            int lastRow = maxY % hightNum;
  
            for (int i = 0; i < rowNum;i++)
            {
               CreateFmySun(0,hightNum*i,maxX,hightNum, mHostGameObject);
            }

            if (lastRow > 0)
            {
                CreateFmySun(0, hightNum * rowNum, maxX, lastRow, mHostGameObject);
            }
        }
        private FMySubSprite CreateFmySun(int x, int y,int cx,int cy, GameObject go)
        {
            GameObject cloneGo = new GameObject();
            cloneGo.layer = this.gameObject.layer;
            cloneGo.name = "SubSprite";
            cloneGo.transform.parent = this.transform;
            cloneGo.transform.localPosition = Vector3.zero;
            cloneGo.transform.localRotation = Quaternion.identity;
            cloneGo.transform.localScale = Vector3.one;
            FMySubSprite fms = SceneManager.instance.AddComponent<FMySubSprite>(cloneGo);
            fms.SetDefalueMaterial(mMatName);
            mMySubSprs.Add(fms);
            fms.SetMeshMat(mMeshMat);
            fms.CreateClip(x, y, cx, cy, maxX, maxY,go);
            mMeshMat = fms.GetMesh();
            return fms;
        }

        public void SetHostGameObject(GameObject go)
        {
            mHostGameObject = go;
        }
        
        public override void Clear()
        {

            for (int i = 0; i < mMySubSprs.Count; i++)
            {
                mMySubSprs[i].Clear();
            }
        }

        public void InitAndReset(bool res = true)
        {
            if(res)
            {
                Reset();
            }
            else
            {
                Clear();
            }
        }
        public void Reset()
        {
            for (int i = 0; i < mMySubSprs.Count; i++)
            {
                mMySubSprs[i].Reset();
            }
        }
        public void  UpdateMesh()
        {
            for(int i = 0; i < mMySubSprs.Count;i++)
            {
                mMySubSprs[i].UpdateMesh();
            }
        }

        public FMySubSprite Transpos(int x, int y, Vector3 pos, Vector3 s)
        {
            FMySubSprite fms = getFMySunSprite(x, y);
            fms.TransRelativePos(x, y, pos, s);
            return fms;
        }

        public FMySubSprite TransColor(int x, int y, Color c)
        {
            FMySubSprite fms = getFMySunSprite(x, y);
            fms.TransRelativeColor(x, y, c);
            return fms;

        }
        public FMySubSprite  getFMySunSprite(int x, int y)
        {
            int sSize = y * maxX + x;
            for(int i = 0; i <mMySubSprs.Count;i++)
            {
                FMySubSprite fss = mMySubSprs[i];
                int curNum = fss.GetClipNum();
                if(sSize < curNum)
                {
                    return fss;
                }
                else
                {
                    sSize -= curNum;
                }
            }
            return null;
        }
    }
}
