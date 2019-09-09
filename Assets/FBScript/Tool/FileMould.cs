//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections.Generic;
using System.Text;

namespace F2DEngine
{
    #region 基本结构
    public class MouldTool
    {
        #region 基础结构
        public class MouldPatch
        {
            public enum PatchType
            {
                PT_Value,//值
                PT_Analy,//解析
                PT_For,//for
                PT_Space,//空格
                PT_Back,//回退
                PT_DEFINE,//预编
                PT_LineHead,//头
            }
            public class MData
            {
                public PatchType type;
                public string strValue;
                public MouldPatch patchValue;
            }
            public List<MData> datas = new List<MData>();
            public void AddData(string txt, MouldPatch mp,PatchType type)
            {
                MData da = new MData();
                da.type = type;
                da.strValue = txt;
                da.patchValue = mp;
                datas.Add(da);
            }
            public MouldPatch mNextPactch;
            public MouldPatch mParent;
        }
        public class ValuePatch
        {
            public class VData
            {
                public string value;
                public FileMould mFileTool;
                public VData(string v, FileMould t)
                {
                    value = v;
                    mFileTool = t;
                }
            }
            public Dictionary<string, VData> mDataValues = new Dictionary<string, VData>();
            public List<ValuePatch> mNexts = new List<ValuePatch>();

            public ValuePatch Set(string key,string value)
            {
                return _Set(key, value, null);
            }
            public ValuePatch Set(string key, FileMould value)
            {
                return _Set(key, null, value);
            }

            public ValuePatch _Set(string key,string v, FileMould t)
            {
                mDataValues[key] = new VData(v,t);
                return this;
            }
            public ValuePatch _Create()
            {
                ValuePatch mv = new ValuePatch();
                mNexts.Add(mv);
                return mv;
            }
        }    
        protected Dictionary<string, ValuePatch> mValuePatchs = new Dictionary<string, ValuePatch>();
        protected Dictionary<string,bool> mDefines = new Dictionary<string,bool>();
        private MouldPatch mHeadPatch;
        /////////////////////////////
        private MouldPatch _PosPatch;
        public System.Action InitCallBack;
        public MouldTool()
        {
            mHeadPatch = new MouldPatch();
            _PosPatch = mHeadPatch;
        }     
        public MouldPatch GetHead()
        {
            return mHeadPatch;
        }
        public void ClearMould()
        {
            mHeadPatch = new MouldPatch();
            _PosPatch = mHeadPatch;
            ClearValue();
        }
        public void ClearValue()
        {
            mValueDataEditor = new EditorPro();
            mDefines.Clear();
            mValuePatchs.Clear();
        }
        private EditorPro mValueDataEditor = new EditorPro();
        public bool mIsNextLine = true;
        private class EditorPro
        {
            public string mSpace = "";
            public List<bool> mIsCanWirte = new List<bool>();
            public void _Space(int num, bool add = true)
            {
                int rn = add ? mSpace.Length + num : num;
                mSpace = rn > 0 ? " ".PadRight(rn) : "";
            }

            public void SetDef(bool isWrite)
            {
                mIsCanWirte.Add(isWrite);
            }
            public void SetElseDef()
            {
                mIsCanWirte[mIsCanWirte.Count - 1] = !mIsCanWirte[mIsCanWirte.Count - 1];
            }
            public void SetEnd()
            {
                mIsCanWirte.RemoveAt(mIsCanWirte.Count - 1);
            }
            public bool IsCanWrite()
            {
                for(int i = 0; i < mIsCanWirte.Count;i++)
                {
                    if (mIsCanWirte[i] == false)
                        return false;
                }
                return true;
            }
        }
        private void _Operation()
        {
           
        }
        #endregion

        #region 编写模板
        public void WRITE(string txt,string l1 = "[",string r2 = "]")
        {
            string tempStr = txt;
            if (mIsNextLine)
            {
                _PosPatch.AddData("", null, MouldPatch.PatchType.PT_LineHead);
            }
            while (true)
            {
                int starPos = tempStr.IndexOf(l1);
                int endPos = tempStr.IndexOf(r2);
                if (starPos != -1 && endPos != -1)
                {
                    _PosPatch.AddData(tempStr.Substring(0, starPos), null, MouldPatch.PatchType.PT_Value);
                    _PosPatch.AddData(tempStr.Substring(starPos + l1.Length, endPos - starPos - l1.Length), null, MouldPatch.PatchType.PT_Analy);
                    tempStr = tempStr.Substring(endPos + r2.Length);
                }
                else
                {
                    _PosPatch.AddData(tempStr, null, MouldPatch.PatchType.PT_Value);
                    break;
                }
            }
            _PosPatch.AddData("\n", null, MouldPatch.PatchType.PT_Value);
            mIsNextLine = true;
            _Operation();
        }     
        public void FOR(string key)
        {          
            MouldPatch patch = new MouldPatch();
            _PosPatch.AddData(key, patch, MouldPatch.PatchType.PT_For);
            patch.mParent = _PosPatch;
            _PosPatch = patch;
            _Operation();
        }
        public void END_FOR()
        {
            _PosPatch = _PosPatch.mParent;
            _Operation();
        }
        public void DEFINE(string key,bool isDef = true)
        {
            _PosPatch.AddData("0/"+key + "/" + isDef.ToString(), null, MouldPatch.PatchType.PT_DEFINE);
            _Operation();
        }
        public void BACK(int len=1)
        {
            _PosPatch.AddData(len.ToString(), null, MouldPatch.PatchType.PT_Back);
            _Operation();
        }
        public void ELSEDEF()
        {
            _PosPatch.AddData("1",null, MouldPatch.PatchType.PT_DEFINE);
            _Operation();
        }
        public void END_DEFINE()
        {
            _PosPatch.AddData("2", null, MouldPatch.PatchType.PT_DEFINE);
            _Operation();
        }
        public void SPACE(int num,bool add = true)
        {
            if(!mIsNextLine)
            {
                if(num > 0)
                {
                    _PosPatch.AddData(" ".PadRight(num), null, MouldPatch.PatchType.PT_Value);
                }              
            }
            _PosPatch.AddData(num.ToString()+"/"+add.ToString(), null, MouldPatch.PatchType.PT_Space);
            _Operation();
        }      
        #endregion

        #region 模板参数设置
        public void Set(string key,string value)
        {
            ValuePatch mv = new ValuePatch();
            mv.Set(key,value);
            mValuePatchs[key] = mv;
        }
        public void Set(string key, FileMould value)
        {
            ValuePatch mv = new ValuePatch();
            mv.Set(key, value);
            mValuePatchs[key] = mv;
        }
        public void SetDefine(string key,bool isDef = true)
        {
            mDefines[key] = isDef;
        }
        public ValuePatch CreateSet(string key)
        {
            ValuePatch mv = null;
            if (!mValuePatchs.TryGetValue(key, out mv))
            {
                mv = new ValuePatch();
                mValuePatchs[key] = mv;
            }
           return mv._Create();
        }
        #endregion

        #region 模板生成
        public void MergeMould(MouldTool parent)
        {
            foreach (var k in parent.mValuePatchs)
            {
                if(!mValuePatchs.ContainsKey(k.Key))
                {
                    mValuePatchs[k.Key] = k.Value;
                }
            }
            foreach (var k in parent.mDefines)
            {
                if (!mDefines.ContainsKey(k.Key))
                {
                    mDefines[k.Key] = k.Value;
                }
            }
        }
        public string Build()
        {
            StringBuilder builer = new StringBuilder();
            _Build(mHeadPatch, builer,null);
            return builer.ToString();
        }
        protected void _Build(MouldPatch patch,StringBuilder builder,List<ValuePatch> valPatch)
        {
            for (int i = 0; i < patch.datas.Count;i++)
            {
                var data = patch.datas[i];
                if(data.type == MouldPatch.PatchType.PT_DEFINE)
                {
                    string[] tempStrs = data.strValue.Split('/');
                    if(tempStrs[0] == "0")
                    {
                        bool res = false;
                        if (!mDefines.TryGetValue(tempStrs[1], out res))
                        {
                            res = false;
                        }
                        mValueDataEditor.SetDef(res == bool.Parse(tempStrs[2]));
                    }
                    else if(tempStrs[0] == "1")
                    {
                        mValueDataEditor.SetElseDef();
                    }
                    else
                    {
                        mValueDataEditor.SetEnd();
                    }
                }
                if (!mValueDataEditor.IsCanWrite())
                    continue;
                if(data.type == MouldPatch.PatchType.PT_LineHead)
                {
                    builder.Append(mValueDataEditor.mSpace);
                }
                else if(data.type == MouldPatch.PatchType.PT_Space)
                {
                    string[] temps = data.strValue.Split('/');
                    mValueDataEditor._Space(int.Parse(temps[0]),bool.Parse(temps[1]));
                }
                else if(data.type == MouldPatch.PatchType.PT_Back)
                {
                    builder.Length -= int.Parse(data.strValue);
                }
                else if(data.type == MouldPatch.PatchType.PT_Value)
                {
                    builder.Append(data.strValue);
                }
                else if(data.type == MouldPatch.PatchType.PT_Analy)
                {
                    ValuePatch.VData vData = null;
                    if (valPatch != null)
                    {
                        for (int v = 0; v < valPatch.Count; v++)
                        {
                            if (valPatch[v].mDataValues.TryGetValue(data.strValue, out vData))
                            {
                                break;
                            }
                        }

                    }
                    if (vData == null)
                    {
                        ValuePatch vp = null;
                        if (mValuePatchs.TryGetValue(data.strValue, out vp))
                        {
                            vp.mDataValues.TryGetValue(data.strValue, out vData);
                        }
                    }

                    if (vData != null)
                    {
                        if (vData.mFileTool != null)
                        {
                            //子模块编译
                            var tool = vData.mFileTool.GetTool();
                            tool.mIsNextLine = false;
                            tool.mValueDataEditor._Space(mValueDataEditor.mSpace.Length);
                            vData.mFileTool.Init();
                            tool._Build(tool.GetHead(), builder, valPatch);
                            builder.Length--;
                        }
                        else
                        {
                            builder.Append(vData.value);
                        }
                    }
                    else
                    {
                        builder.Append("[" + data.strValue + "]");
                    }
                }
                else if(data.type == MouldPatch.PatchType.PT_For)
                {
                    ValuePatch vp = null;
                    if (mValuePatchs.TryGetValue(data.strValue, out vp))
                    {
                        for (int v = 0; v < vp.mNexts.Count; v++)
                        {
                            ValuePatch vPatch = vp.mNexts[v];
                            List<ValuePatch> vPatchList = new List<ValuePatch>();
                            vPatchList.Add(vPatch);
                            if(valPatch != null)
                            {
                                vPatchList.AddRange(valPatch);
                            }
                            _Build(data.patchValue,builder, vPatchList);
                        }
                    }
                }
            }
        }     
        #endregion

    }

    public interface FileMould
    {
        void Init();
        MouldTool GetTool();
    }

    public class FBaseMould: FileMould
    {
        private MouldTool mTool = new MouldTool();      
        private bool mIsInit = false;
        public void Init()
        {
            if (!mIsInit)
            {
                if(mInitCall!= null)
                {
                    mTool.MergeMould(mParent.mTool);
                    mInitCall(mTool);
                }
                OnInit(mTool);
            }
            mIsInit = true;
        }
        protected virtual void  OnInit(MouldTool tool)
        {

        }      
        public  MouldTool GetTool()
        {
            return mTool;
        }
        public void Set(string key, string value)
        {
            mTool.Set(key, value);
        }
        public void Set(string key, FileMould value)
        {
            mTool.Set(key, value);
        }
        public void SetDefine(string key, bool isDef = true)
        {
            mTool.SetDefine(key, isDef);
        }
        public MouldTool.ValuePatch CreateSet(string key)
        {
           return  mTool.CreateSet(key);
        }
        public void Build(string path, FFilePath type = FFilePath.FP_Abs)
        {
            Init();
            FSaveHandle handle = FSaveHandle.Create(path, type, FOpenType.OT_Txt);
            handle.SetContext(mTool.Build());
            handle.Save();
        }
        protected System.Action<MouldTool> mInitCall;
        protected FBaseMould mParent;
        protected FBaseMould CreateSubMould(System.Action<MouldTool> callBack)
        {
            return CreateSubMould<FBaseMould>(callBack);
        }
        
        protected T CreateSubMould<T>(System.Action<MouldTool> callBack) where T : FBaseMould,new()
        {
            T subMould = new T();
            subMould.mInitCall = callBack;
            subMould.mParent = this;
            return subMould;
        }
    }
    #endregion

    #region 扩展接口


    //数据表设置
    public class AssetFile: FBaseMould
    {
        private FBaseMould CreateSubClass()
        {
            FBaseMould subMould = CreateSubMould((tool) =>
            {
                tool.WRITE("[Serializable]");
                tool.WRITE("public class [SubProperty]");
                tool.WRITE("{");
                tool.FOR("SubVariable");
                tool.WRITE("    public [subName];");
                tool.END_FOR();
                tool.WRITE("}");
            });
            return subMould;
        }
        protected override void OnInit(MouldTool tool)
        {
            tool.WRITE("//----------------------------------------------");
            tool.WRITE("//  F2DEngine: time: [time]  by fucong QQ:353204643");
            tool.WRITE("//----------------------------------------------");
            tool.WRITE("using UnityEngine;");
            tool.WRITE("using System.Collections;");
            tool.WRITE("using System.Collections.Generic;");
            tool.WRITE("using System;");
            tool.WRITE("using F2DEngine;");
            tool.WRITE("");
            tool.DEFINE("NameSpack");
            tool.WRITE("namespace [NameSpace]");
            tool.WRITE("{");
            tool.SPACE(4);
            tool.END_DEFINE();
            tool.FOR("SubClass");
            tool.WRITE("[ClassKey]");
            tool.END_FOR();
            tool.WRITE("");
            tool.WRITE("[Serializable]");
            tool.WRITE("public class [Property]Property : BaseAssetProperty");
            tool.WRITE("{");
            tool.FOR("Variable");
            tool.WRITE("   public [type] [typename]");
            tool.END_FOR();
            tool.WRITE("}");
            tool.WRITE("");
            tool.WRITE("public class [Property]Asset : TemplateAsset<[Property]Asset, [Property]Property>");
            tool.WRITE("{");
            tool.WRITE("");
            tool.WRITE("}");
            tool.DEFINE("NameSpack");
            tool.SPACE(-4);
            tool.WRITE("}");
            tool.END_DEFINE();
        }
        public void SetNamespace(string sp)
        {
            SetDefine("NameSpack");
            Set("NameSpace",sp);
        }
        public AssetFile(string name)
        {
            Set("Property", name);
            Set("time", System.DateTime.Now.ToString("yyyy.dd"));
        }     
        public void SetVariable(string type,string name)
        {
            CreateSet("Variable").Set("type", type).Set("typename", name);
        }
        public void SetSubClass(string className,params string[]name)
        {
            var tool = CreateSubClass();
            tool.Set("SubProperty", className);
            for (int i = 0; i < name.Length; i++)
            {
                tool.CreateSet("SubVariable").Set("subName", name[i]);
            }
            CreateSet("SubClass").Set("ClassKey", tool);
        }

    }

    #endregion
}
