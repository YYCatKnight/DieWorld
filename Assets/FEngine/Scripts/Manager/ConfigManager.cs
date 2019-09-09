//----------------------------------------------
//  F2DEngine: time: 2016.3  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace F2DEngine
{
    public enum LanguageType
    {
        None = 0,
        English,            //1英语
        SimplifiedChinese, //7简体中文
    }

    public class OptionStatus : Save_Object
    {
        public class Data
        {
            public string key = "";
            public bool flag = true;
            public string languageUI = "";
        }
        public List<Data> datas = new List<Data>();

        public Data GetData(string key)
        {
            for (int n = 0; n < datas.Count; n++)
            {
                if (datas[n].key == key)
                    return datas[n];
            }
            return new Data();
        }

        public void SaveData(string key,bool flag)
        {
            Data data = new Data();
            data.key = key;
            data.flag = flag;
            datas.Add(data);
        }

        public void SaveData(string key, string languageUI)
        {
            Data data = GetData(key);
            if (data.key != null)
            {
                data.key = key;
                data.languageUI = languageUI;
                datas.Add(data);
            }
            else
            {
                Data _data = new Data();
                _data.key = key;
                _data.languageUI = languageUI;
                datas.Add(_data);
            }
        }
    }

    public class ConfigManager : ManagerTemplate<ConfigManager>
    {
        public class ConfigInfo: Save_Object
        {
            public LanguageType mLanguage = LanguageType.SimplifiedChinese;
            public float mVideoSize = 1.0f;
            public float mMusicSize = 1.0f;
            public string mPlayName = "";
            public int mPlayTimes = 0;
        }

        private ConfigInfo mConfigInfo;
        private OptionStatus mOptionStatus = new OptionStatus();

        public bool nIsFirstTime = false;
        protected override void OnInit()
        {
            SceneManager.instance.Init();
            BundleManager.instance.Init();
            AnimationManager.instance.Init();
            EventManager.instance.Init();
            UIManager.instance.Init();
            LoadSceneManager.instance.Init();
            
            ReadConfig();
            var uiRoot = SceneManager.instance.Create(ResConfig.FMAINUISCENE);
            uiRoot.name = "MainCanvas";
            if (uiRoot)
            {
                uiRoot.transform.position = Vector3.up * 5000;
            }
        }

        public IEnumerator LoadManager()
        {
            FEngine.instance.InitManager();
            yield return 0;
        }

        public void SetLanguage(LanguageType type)
        {
            mConfigInfo.mLanguage = type;
            FText.RestText();
            Lange_ListAsset.SetLanguageType(type);
        }

        public LanguageType GetLanguage()
        {
            return mConfigInfo.mLanguage;
        }

        private void ReadConfig()
        {
            LoadConfig();
        }

        public void SaveConfig()
        {
            if (mConfigInfo != null)
            {
                mConfigInfo.SaveFile();
            }
        }
        public void SetNewGame()
        {

        }
        public void SetMusic(float size)
        {
            mConfigInfo.mMusicSize = size;
            SceneManager.instance.SetMusicVolume(size);
        }

        public string GetPlayName()
        {
            return mConfigInfo.mPlayName;
        }
        public float GetMusic()
        {
            return mConfigInfo.mMusicSize;
        }

        public void SetPlayTimece(int times)
        {
            mConfigInfo.mPlayTimes = times;
        }
        public int GetPlayTimece()
        {
            return mConfigInfo.mPlayTimes;
        }

        public void SetVideo(float size)
        {
            mConfigInfo.mVideoSize = size;
            SceneManager.instance.SetSoundVolume(size);
        }

        public float GetVideo()
        {
            return mConfigInfo.mVideoSize;
        }

        public OptionStatus.Data GetOptionData(string key)
        {
            return mOptionStatus.GetData(key);
        }

        public void SaveOptionData(string key, bool flag)
        {
            mOptionStatus.SaveData(key, flag);
            mOptionStatus.SaveFile();
        }

        public void SaveOptionData(string key, string ui)
        {
            mOptionStatus.SaveData(key, ui);
            mOptionStatus.SaveFile();
        }


        private void LoadConfig()
        {
            mConfigInfo = new ConfigInfo();
            if (!mConfigInfo.ReadFile())
            {
                nIsFirstTime = true;
                switch(Application.systemLanguage)
                {
                    case SystemLanguage.ChineseSimplified:
                    case SystemLanguage.Chinese:
                        mConfigInfo.mLanguage = LanguageType.SimplifiedChinese;
                        break;
                    default:
                        mConfigInfo.mLanguage = LanguageType.English;
                        break;
                }
            }
            SetLanguage(mConfigInfo.mLanguage);
            SetVideo(mConfigInfo.mVideoSize);
            SetMusic(mConfigInfo.mMusicSize);
        }

        /// <summary>
        /// AIHelp语言
        /// </summary>
        public string GetLanguageAlias()
        {
            string alias = "en";
            switch (mConfigInfo.mLanguage)
            {
                case LanguageType.SimplifiedChinese:
                    alias = "zh_CN";
                    break;
                case LanguageType.English:
                    alias = "en";
                    break;
                default:
                    break;
            }

            return alias;
        }
    }
}
