using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace F2DEngine
{
    public class VideoPlayerPlane : BasePlane
    {
        private VideoPlayer mVideoPlayer;
        private FCommonBt mSkipBt;
        private float mShowTime;
        private RawImage mImage;
        private bool mSkiping = false;
        private RenderTexture newTexture;
        private Text mDec;
        private AudioSource mSource;
        private int mCurTimeDec = 0;
        private TimeDec mLastTimeDec;
        private class TimeDec
        {
            public float stime;
            public float etime;
            public string keys;
        }


        private List<TimeDec> mTimeDec = new List<TimeDec>();
        private bool mIsPlay = false;
        private void SetVideoTime(string key,float st,float et)
        {
            TimeDec td = new TimeDec();
            td.keys = key;
            td.stime = st;
            td.etime = et;
            mTimeDec.Add(td);
        }

        public override void Init(params object[] o)
        {


            //SetVideoTime("Video01", 0,7.43f);
            //SetVideoTime("Video02", 7.43f,11.43f);
            //SetVideoTime("Video03", 11.43f,15.31f);
            //SetVideoTime("Video04", 15.31f,23.48f);
            //SetVideoTime("Video05", 23.48f,28.14f);
            //SetVideoTime("Video06", 28.14f,36.36f);
            //SetVideoTime("Video07", 36.36f,41.48f);

            SceneManager.instance.StopSound(SceneManager.MianMusicGroup);

            SetVideoTime("Video01", 1, 8.2f);
            SetVideoTime("Video02", 9.1f, 16.0f);
            SetVideoTime("Video02_1",17.0f, 29.0f);
            SetVideoTime("Video03", 30.0f, 38.5f);
            SetVideoTime("Video03_1",38.6f, 46.23f);
            SetVideoTime("Video04", 47.1f, 57.5f);
            SetVideoTime("Video04_1",57.6f, 63.2f);
            SetVideoTime("Video05", 64.0f, 71.0f);
            SetVideoTime("Video06", 72.0f, 77.07f);
            SetVideoTime("Video06_1", 78.0f,84.07f);
            SetVideoTime("Video07", 85.1f, 90.0f);

            string videoName = (string)o[0];
            mVideoPlayer = mMainPlane.GetFObject<VideoPlayer>("F_VideoPlayer");
            mImage = mMainPlane.GetFObject<RawImage>("F_RawImage");
            newTexture = new RenderTexture(512,512,16,RenderTextureFormat.ARGB32);
            newTexture.name = "video" + GetInstanceID();
            newTexture.isPowerOfTwo = true;
            newTexture.hideFlags = HideFlags.DontSave;
            mImage.texture = newTexture;
            mVideoPlayer.targetTexture = newTexture;
            var v = mImage.rectTransform.sizeDelta;
            v.y = mImage.rectTransform.rect.width * 1024/768;
            mImage.rectTransform.sizeDelta = v;
            //VideoClip clipv = FEngineManager.LoadPrefab<VideoClip>(videoName);
            //mVideoPlayer.clip = clipv;
            mSource = mVideoPlayer.GetComponent<AudioSource>();
            mVideoPlayer.SetTargetAudioSource(0, mSource);
            mVideoPlayer.Play();        
            mVideoPlayer.errorReceived += (f, t) =>
            {
                Debug.LogError(t);
                CloseMySelf();
            };
            mVideoPlayer.loopPointReached += (f) =>
            {
                if (!mSkiping)
                {
                    StartCoroutine(PlayFun());
                }
            };

            mSkipBt = mMainPlane.GetFObject<FCommonBt>("F_Skip");
            mSkipBt.gameObject.SetActive(true);
            mSkipBt.nBtEvent = (f) =>
            {
                if (!mSkiping)
                {
                    StartCoroutine(PlayFun());
                }
            };
            mDec = mMainPlane.GetFObject<Text>("F_Dec");
            mDec.text = "";
        }

        IEnumerator PlayFun()
        {
            mSkiping = true;
            mSkipBt.gameObject.SetActive(false);
            Color color = mImage.color;
            color.a = 1;
            
            while (color.r > 0)
            {
                float dp = Time.deltaTime * 2.0f;
                color.r -= dp;
                color.g -= dp;
                color.b -= dp;
                mDec.color = color;
                mImage.color = color;
                mSource.volume = color.r;
                yield return 0;
            }
            mImage.color = color;
            yield return 0;
            if (!mIsPlay)
            {
#if FACEBOOK_SDK
                AppsFlyerMgr.LookVideo(9);
#if UNITY_ANDROID
                FireBaseMgr.LookVideo(9);
#endif
#endif
                mIsPlay = true;
                Handheld.PlayFullScreenMovie("null.mp4", Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.Fill);
            }
            yield return 0;
            yield return 0;
            yield return new WaitForSeconds(0.3f);
            mImage.color = Color.black;

            yield return 0;

            CloseMySelf();
        }
        private void Update()
        {
            if(mLastTimeDec!= null)
            {
                if(mLastTimeDec.etime < mVideoPlayer.time)
                {
                    mDec.text = "";
                    mLastTimeDec = null;
                }
            }
            if (mLastTimeDec == null)
            {
                if (mTimeDec.Count > mCurTimeDec)
                {
                    var tt = mTimeDec[mCurTimeDec];
                    if (tt.stime <= mVideoPlayer.time)
                    {
                        mDec.text = tt.keys;
                        mCurTimeDec++;
                        mLastTimeDec = tt;
                    }
                }
            }

            if (!mSkiping)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    mSkipBt.gameObject.SetActive(true);
                    mShowTime = 3;
                }
                if (mSkipBt.gameObject.activeInHierarchy && (mShowTime -= Time.deltaTime) < 0)
                {
                    mSkipBt.gameObject.SetActive(true);
                }
            }
        }
        public override void Clear()
        {
            if (FEngine.mBackDown != null)
            {
                SceneManager.instance.PlayMusic("COE_Back", SceneManager.MianMusicGroup);
            }
            else
            {
                SceneManager.instance.PlaySoundByID("16003", SceneManager.MianMusicGroup);
            }

            if (mVideoPlayer.isPlaying)
            {
                mVideoPlayer.Stop();
            }
            if (newTexture != null)
            {
                newTexture.Release();
            }
        }
    }
}
