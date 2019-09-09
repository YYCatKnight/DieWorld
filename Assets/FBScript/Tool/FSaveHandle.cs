//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace F2DEngine
{
    #region 基础结构
    public enum FFilePath
    {
        FP_Abs,
        FP_Cache,
        FP_Relative,
    }

    public enum FOpenType
    {   
        OT_ReadWrite = 0,
        OT_Write = 1<<1,
        OT_Binary = 1 << 2,
        OT_Encrypt = 1<< 3,
        OT_Txt = 1 << 4,
    }

    public class FileTool : UnitData
    {
        public bool IsLoad { get { return mIsLoad; } }
        protected bool mIsLoad = false;
        protected string mFilePath;
        protected FOpenType mFOpenType = FOpenType.OT_ReadWrite;

        protected bool IsHaveSameType(FOpenType main, FOpenType use)
        {
            return ((int)(main & use)) != 0;
        }

        public bool Open(string fileName, FFilePath pathType, FOpenType ot = FOpenType.OT_ReadWrite)
        {
            mFOpenType = ot;
            mFilePath = FEngineManager.ConvertPath(fileName, pathType);
            Init();
            if (!IsHaveSameType(mFOpenType,FOpenType.OT_Write))
            {
                mIsLoad = File.Exists(mFilePath);
                return mIsLoad ? ReadFile() : false;
            }
            return false;
        }

        private void _CreateDirectory()
        {
            string path = FEPath.GetDirectoryName(mFilePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        protected virtual bool Init()
        {
            return true;
        }

        protected virtual bool ReadFile()
        {
            return true;
        }

        protected virtual void SaveFile()
        {

        }

        public void SaveAs(string fileName, FFilePath pathType)
        {
            mFilePath = FEngineManager.ConvertPath(fileName, pathType);
            Save();
        }

        public void Save()
        {
            _CreateDirectory();
            SaveFile();
        }
    }
    #endregion

    #region 扩展结构
    public class FSaveHandle : FileTool
    {
        private static string _saveObjectPath = "FEngine";
        public static string SaveMainPath { get{return _saveObjectPath; } set { _saveObjectPath = value; }}
        private Dictionary<string, BytesPack> mDataPacks = new Dictionary<string, BytesPack>();//混合数据
        private string mContext="";
        private byte[] mBytes;
        private bool mIsTxtMode = false;
        private bool mIsBinary = false;
        private bool mIsEncrypt = false;

        public static FSaveHandle Create(string fileName, FFilePath pathType, FOpenType ot = FOpenType.OT_ReadWrite)
        {
            FSaveHandle sh = new FSaveHandle();
            sh.Open(fileName, pathType, ot);
            return sh;
        }

        public FSaveHandle()
        {

        }  

        public void SetPack(BytesPack pack)
        {
            _ReadPack(pack);
        }

        protected override bool Init()
        {
            mIsTxtMode = IsHaveSameType(mFOpenType, FOpenType.OT_Txt);
            mIsBinary = IsHaveSameType(mFOpenType, FOpenType.OT_Binary);
            return true;
        }

        private void  _ReadFile()
        {
            if (mIsTxtMode)
            {
                if (mIsBinary)
                {
                    mBytes = File.ReadAllBytes(mFilePath);
                }
                else
                {
                    mContext = File.ReadAllText(mFilePath, System.Text.Encoding.Default);
                }
            }
            else
            {
                FileStream fs = new FileStream(mFilePath, FileMode.Open, FileAccess.Read);
                fs.Seek(0, SeekOrigin.Begin);
                var pack = new BytesPack();
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                pack.CreateReadBytes(bytes);
                _ReadPack(pack);
            }
        }

        private void _ReadPack(BytesPack pack)
        {
            mIsBinary = pack.PopBool("");
            mIsEncrypt = pack.PopBool("");
            while (!pack.IsOver())
            {
               string fileName = pack.PopString("");
               BytesPack pb = new BytesPack();
               byte[] bytes = pack.PopBytes();
               if (mIsEncrypt)
               {
                  FUniversalFunction.EncryptBytes(bytes, 0);
               }
               pb.CreateReadBytes(bytes);
               mDataPacks[fileName] = pb;
            }
        }

        protected override bool ReadFile()
        {
            _ReadFile();
            return true;
        }

        protected override void SaveFile()
        {
            if (mIsTxtMode)
            {
                if (mIsBinary)
                {
                    File.WriteAllBytes(mFilePath,mBytes);
                }
                else
                {
                    File.WriteAllText(mFilePath, mContext, System.Text.Encoding.Default);
                }
            }
            else
            {
                FileStream fs = new FileStream(mFilePath, FileMode.Create, FileAccess.Write);
                fs.Seek(0, SeekOrigin.Begin);
                fs.WriteByte((byte)(mIsBinary?1:0));
                fs.WriteByte((byte)0);
                foreach (var k in mDataPacks)
                {
                    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(k.Key);
                    fs.Write(System.BitConverter.GetBytes(bytes.Length), 0, 4);
                    fs.Write(bytes, 0, bytes.Length);
                    int len = k.Value.GetStreamLen();
                    fs.Write(System.BitConverter.GetBytes(len), 0, 4);
                    bytes = k.Value.GetStream(); 
                    fs.Write(bytes, 0, len);
                }
                fs.Close();
            }
        }

        public object GetObject(System.Type type,string keyName = null)
        {
            return _GetObject(null, type, keyName);
        }

        public T GetObject<T>(string keyName = null)
        {
            return (T)GetObject(typeof(T), keyName);
        }

        private object _GetObject(object obj,System.Type type,string keyName)
        {
            if (mIsTxtMode)
            {
                if (obj == null)
                {
                    return StringSerialize.Deserialize(mContext, type);
                }
                else
                {
                    StringSerialize.Deserialize(mContext, obj);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(keyName))
                {
                    keyName = type.Name;
                }

                BytesPack pack = null;
                if (mDataPacks.TryGetValue(keyName, out pack))
                {
                    if (mIsBinary)
                    {
                        if (obj == null)
                        {
                           return BytesSerialize.Deserialize(pack,type);
                        }
                        else
                        {
                            BytesSerialize.Deserialize(pack, obj);
                        }

                    }
                    else
                    {
                        string str = System.Text.Encoding.UTF8.GetString(pack.GetStream());
                        if (obj == null)
                        {
                            return StringSerialize.Deserialize(str, type);
                        }
                        else
                        {
                            StringSerialize.Deserialize(str, obj);
                        }
                    }
                }
            }
            return null;
        }

        public void FromObject(object obj,string keyName = null)
        {
            _GetObject(obj, obj.GetType(), keyName);
        }

        public void PushObject(object obj,string keyName = null)
        {
           if(mIsTxtMode)
           {
                mContext = StringSerialize.Serialize(obj);
            }
           else
           {
                if(string.IsNullOrEmpty(keyName))
                {
                    keyName = obj.GetType().Name;
                }
                BytesPack pack = null;
                if (mIsBinary)
                {
                    pack = BytesSerialize.Serialize(obj);
                   
                }
                else
                {
                    string temps = StringSerialize.Serialize(obj);
                    pack = new BytesPack();
                    pack.CreateReadBytes(System.Text.Encoding.UTF8.GetBytes(temps));
                }
                mDataPacks[keyName] = pack;
            }
        }

        public string GetContext()
        {
            return mContext;
        }

        public byte[] GetBytes()
        {
            return mBytes;
        }

        public void SetContext(string txt)
        {
            mContext = txt;
            mIsTxtMode = true;
        }

        public void SetBytes(byte[]bytes)
        {
            if (mIsBinary&&mIsTxtMode)
            {
                mBytes = bytes;
            }
            else
            {
                var pack = new BytesPack();
                pack.CreateReadBytes(bytes);
                _ReadPack(pack);
            }
        }
    }
    public class Save_Object
    {
        private string mLocalPath = null;
        private FSaveHandle mFileHandle;
        protected void SetLocalPath(string pathFile)
        {
            if (mLocalPath == null)
            {
                mLocalPath = FSaveHandle.SaveMainPath + "/" + pathFile + "_" + this.GetType().Name;
            }
        }

        protected string GetLocalPath()
        {
            if (mLocalPath == null)
            {
                SetLocalPath("");
            }
            return mLocalPath;
        }

        public bool ReadFile(string path = "")
        {
            SetLocalPath(path);
            return _ReadFile(GetLocalPath());
        }    

        public void SaveFile()
        {
            _SaveFile(GetLocalPath());
        }

        protected virtual FOpenType GetOpenType()
        {
            return FOpenType.OT_ReadWrite;
        }

        protected virtual bool _ReadFile(string realPath)
        {
            mFileHandle = new FSaveHandle();
            mFileHandle.Open(realPath, FFilePath.FP_Cache, GetOpenType()&(~FOpenType.OT_Write));
            if(mFileHandle.IsLoad)
            {
                mFileHandle.FromObject(this);
                return true;
            }
            return false;
        }

        protected virtual void _SaveFile(string realPath)
        {
            if(mFileHandle == null)
            {
                mFileHandle = new FSaveHandle();
                mFileHandle.Open(realPath, FFilePath.FP_Cache,GetOpenType()|FOpenType.OT_Write);
            }
            mFileHandle.PushObject(this);
            mFileHandle.Save();
        }
    }

    #endregion

}
