//----------------------------------------------
//  F2DEngine: time: 2017.4  by fucong QQ:353204643
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace F2DEngine
{
    public class FCutString
    { 
        private const string RegexHead = @"<link=(.+?)=(.+?)/>|<color=#(.+?)>(.+?)</color>|<image=(.+?)>";
        public static FCutString Create(string txt)
        {
            FCutString cs = new FCutString(txt);
            cs.Init();
            return cs;
        }
        public CutTool HeadCut { get; protected set; }
        private string mValue;
        public FCutString(string txt)
        {
            mValue = txt;
        }
        public void Init()
        {
            HeadCut = new CutTool();
            HeadCut.Init(mValue);
        }
        public string GetString(int len)
        {
            int outlen = 0;
            return HeadCut.GetSubString(len, out outlen);
        }
        public string GetString(int id, out int len)
        {
            return HeadCut.GetSubString(id, out len);
        }

        public string GetBareString()
        {
            return HeadCut.GetBareString();
        }

        public class NormalCut : CutTool
        {
            private string mDec;
            public void SetInfo(string txt)
            {
                mDec = txt;
            }

            protected override string HandleString(string txt)
            {
                return txt;
            }

            protected override string GetOnlyString(int index, out int outLen)
            {
                outLen = 0;
                if (mDec.Length > index)
                {
                    outLen = index;
                    return mDec.Substring(0, index);
                }
                else
                {
                    outLen = mDec.Length;
                    return mDec;
                }
            }
            protected override void Create()
            {

            }

            protected override string GetBare()
            {
                return mDec;
            }
        }
        public class LinkCut : CutTool
        {
            private const string LinkFormat = "<link={0}={1}/>";
            public string mAllName;
            public string mLinkKey;
            public string mLinkDec;

            public void SetInfo(string all, string linkKey, string linkDec)
            {
                mAllName = all;
                mLinkKey = linkKey;
                mLinkDec = linkDec;
                mValue = mLinkDec;
            }

            protected override string HandleString(string txt)
            {
                return string.Format(LinkFormat, mLinkKey, txt);
            }


            protected override string GetOnlyString(int index, out int outLen)
            {
                outLen = 0;
                if (mLinkDec.Length > index)
                {
                    outLen = index;
                    return mLinkDec.Substring(0, index);
                }
                else
                {
                    outLen = mLinkDec.Length;
                    return mAllName;
                }
            }


        }
        public class ImageCut : CutTool
        {
            private string mAllName;
            public void SetInfo(string all, string Key, string Dec)
            {
                mAllName = all;
            }

            protected override string GetOnlyString(int index, out int outLen)
            {
                outLen = 1;
                return mAllName;
            }

            protected override string GetBare()
            {
                return "邉";
            }
        }
        public class ColorCut : CutTool
        {
            private const string ColorFormat = "<color=#{0}>{1}</color>";
            private string mAllName;
            private string mColorKey;
            private string mColorDec;
            public void SetInfo(string all, string Key, string Dec)
            {
                mAllName = all;
                mColorKey = Key;
                mColorDec = Dec;
                mValue = Dec;
            }

            protected override string HandleString(string txt)
            {
                return string.Format(ColorFormat, mColorKey, txt);
            }


            protected override string GetOnlyString(int index, out int outLen)
            {
                outLen = 0;
                if (mColorDec.Length > index)
                {
                    outLen = index;
                    return HandleString(mColorDec.Substring(0, index));
                }
                else
                {
                    outLen = mColorDec.Length;
                    return mAllName;
                }
            }

        }
        public class CutTool
        {
            public List<CutTool> mCutDatas = new List<CutTool>();
            protected string mValue = "";
            //加工
            protected virtual string HandleString(string txt)
            {
                return txt;
            }

            protected virtual string GetOnlyString(int index, out int outLen)
            {
                outLen = 0;
                return "";
            }
            protected virtual string GetBare()
            {
                return "";
            }

            public  string GetBareString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(GetBare());
                for(int i = 0; i < mCutDatas.Count;i++)
                {
                    builder.Append(mCutDatas[i].GetBareString());
                }
                return builder.ToString();
            }

            public string GetSubString(int index, out int outLen)
            {
                outLen = 0;
                string tempName = "";
                if (mCutDatas.Count != 0)
                {
                    StringBuilder builder = new StringBuilder();
                    int tempIndex = index;
                    for (int i = 0; i < mCutDatas.Count; i++)
                    {
                        if (tempIndex < 0)
                            break;
                        int chileLen = 0;
                        builder.Append(mCutDatas[i].GetSubString(tempIndex, out chileLen));
                        tempIndex -= chileLen;
                        outLen += chileLen;
                        if (tempIndex <= 0)
                        {
                            break;
                        }
                    }
                    tempName = HandleString(builder.ToString());
                }
                else
                {
                    tempName = GetOnlyString(index, out outLen);
                }
                return tempName;
            }

            protected virtual void Create()
            {
                if (mValue != "")
                {
                    Init(mValue);
                }
            }

            public void Init(string txt)
            {
                Regex re = new Regex(RegexHead);
                MatchCollection gc = re.Matches(txt);
                int indexText = 0;
                foreach (Match match in gc)
                {
                    NormalCut ntbd = new NormalCut();
                    ntbd.SetInfo(txt.Substring(indexText, match.Index - indexText));
                    indexText = match.Index + match.Length;
                    mCutDatas.Add(ntbd);
                    ntbd.Create();
                    string allName = match.Groups[0].ToString();
                    if (allName.Length > 8)
                    {
                        if (allName.IndexOf("<link=", 0, 6) != -1)
                        {
                            LinkCut ltd = new LinkCut();
                            ltd.SetInfo(allName, match.Groups[1].ToString(), match.Groups[2].ToString());
                            mCutDatas.Add(ltd);
                            ltd.Create();
                        }
                        else if (allName.IndexOf("<color=", 0, 7) != -1)
                        {
                            ColorCut ltd = new ColorCut();
                            ltd.SetInfo(allName, match.Groups[3].ToString(), match.Groups[4].ToString());
                            mCutDatas.Add(ltd);
                            ltd.Create();
                        }
                        else if (allName.IndexOf("<image=", 0, 7) != -1)
                        {
                            ImageCut ltd = new ImageCut();
                            ltd.SetInfo(allName, "", "");
                            mCutDatas.Add(ltd);
                            ltd.Create();
                        }
                    }
                }
                NormalCut temp = new NormalCut();
                temp.SetInfo(txt.Substring(indexText, txt.Length - indexText));
                mCutDatas.Add(temp);
                temp.Create();
            }
        }
    }
}
