//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;


namespace F2DEngine
{
    public class FCMusciObject : UnitObject
    {
        public enum MusicType
        {
            Music,//音乐
            Sound,//音效
        }
        public AudioSource nAudioSource;
        internal MusicType mMusicType = 0;
        private float mCurValue = 1;
        public System.Action<FCMusciObject> nCallBack;
        internal float GetCurValue()
        {
            return mCurValue;
        }
        internal void SetCurValue(float v)
        {
            mCurValue = v;
        }
        internal void PlaySound(string name,AudioClip ac, float timeDp, float value = 1, float starttimeDp = 0,bool isLoop = false)
        {
            if (timeDp == 0 && !isLoop)
            {
                timeDp = ac.length - starttimeDp;
            }
            nAudioSource.clip = ac;
            nAudioSource.Play();
            nAudioSource.volume = value*mCurValue;
            nAudioSource.time = starttimeDp;
            nAudioSource.loop = isLoop;
            StartCoroutine(PlayFun(timeDp,name));
        }

        IEnumerator PlayFun(float timeDp,string name)
        {
            if (timeDp > 0)
            {
                yield return new WaitForSeconds(timeDp);
                StopSound();
            }
            yield return 0;
        }

        public void StopSound()
        {
            FEngineManager.StopSound(this);
        }

        public override void Clear()
        {
            if(nCallBack != null)
            {
                nCallBack(this);
                nCallBack = null;
            }
        }
    }
}
