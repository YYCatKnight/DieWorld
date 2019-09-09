using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace F2DEngine
{
    //文件下载控件
    
    public class FDownloader : UnitData
    {
        private static List<FDownloader> mCreateTools = new List<FDownloader>();
        public  int Timeout { get; set; }
        public const string POST = "POST";
        public const string GET = "GET";
        public enum DownState
        {
            None,
            Wait,
            Success,
            Fail,
        }
        public string Md5Tag { get; set; }//md5标记,用于下载记录
        public long TotalLength { get; internal set; }
        public long DowningLength { get; protected set; }
        public float Progress
        {
            get
            {
                if (TotalLength > 0)
                {
                    return (float)(DowningLength / (double)TotalLength);
                }
                else
                {
                    return 0;
                }
            }
        }
        public string Method { get;protected set; }
        public string URL { get; protected set; }
        public string ContentType { get; set; }
        public string FilePath
        {
            protected get { return _filePath; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _filePath = null;
                    mFileMode = false;
                }
                else
                {
                    _filePath = value;
                    mFileMode = true;
                }
            }
        }//文件保存
        public string Error { get; protected set; }
        public DownState State { get; protected set; }
        public byte[] DownBytes{ get; protected set;}
        public byte[] RequestBytes { get; protected set; }
        public string DownString
        {
            get { return System.Text.Encoding.UTF8.GetString(DownBytes);}
        }
        public void Send(Action<FDownloader> callBack)
        {
            if (State == DownState.Wait)
            {
                Debug.LogError("正在下载中,无法重复下载");
            }
            else
            {
                if(!string.IsNullOrEmpty(FilePath))
                {
                    string path = FEPath.GetDirectoryName(FilePath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                State = DownState.Wait;
                Error = null;
                mTool.Init(this);
                mTool.Send(callBack);
            }
        }      
        public IEnumerator Send()
        {
            if (State == DownState.Wait)
            {
                Debug.LogError("正在下载中,无法重复下载");
                yield break;
            }
            else
            {
                bool isWait = true;
                Send((f) =>
                {
                    isWait = false;
                });
                WaitForSeconds wait = new WaitForSeconds(0.3f);
                while (isWait)
                {
                    yield return wait;
                }
            }
        }
        protected FDownloader(string url)
        {
            Method = POST;
            URL = url;
            State = DownState.None;
            DowningLength = 0;
            mCreateTools.Add(this);
        }
        public static FDownloader CreateUnityDown(string url, string method = null)
        {
            FDownloader http = new FDownloader(url);
            if (method != null)
            {
                http.Method = method;
            }
            http.mTool = new UnityDownloader();
            http.mTool.Init(http);
            return http;
        }
        public void SetRequestStream(byte[]bytes)
        {
            RequestBytes = bytes;
        }
        public void Clear()
        {
            DownBytes = null;
            RequestBytes = null;
            if(mTool != null)
            {
                mTool.Close();
                mTool = null;
            }
            if(mBreakStream != null)
            {
                mBreakStream.Close();
                mBreakStream = null;
            }
            mCreateTools.Remove(this);
        }
        private Stream mBreakStream;//断点续传
        private string _filePath;       
        internal DownTool mTool;
        internal bool mFileMode = false;

        private string _GetTempPathName()
        {
            if(string.IsNullOrEmpty(Md5Tag))
            {
                return FilePath + ".temp";
            }
            else
            {
                return FilePath + "_" + Md5Tag + ".temp";
            }
        }
        internal void  CreateStream()
        {
            if (mBreakStream == null)
            {
                mBreakStream = new FileStream(_GetTempPathName(), FileMode.OpenOrCreate);
                //续传减一个很小的数,防止下载完成
                mBreakStream.Seek(mBreakStream.Length, SeekOrigin.Current);
            }
        }
        internal long GetBreakStreamCount()
        {
            if(mBreakStream != null)
            {
                return mBreakStream.Position;
            }
            return 0;
        }
        internal void WriteStream(byte[]bytes,int offset,int count)
        {
            if (mBreakStream != null)
            {
                mBreakStream.Write(bytes, offset, count);
                DowningLength = mBreakStream.Length;
            }
        }
        internal void  HandleStream()
        {
            if(mBreakStream != null)
            {
                FileStream fs = (FileStream)mBreakStream;
                fs.Close();
                fs.Dispose();
                //改名
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
                File.Move(_GetTempPathName(), FilePath);
                mBreakStream = null;
                State = DownState.Success;
            }
            else
            {
                State = DownState.Fail;
            }
        }
        internal void SetError(string error)
        {
            Error = error;
            State = DownState.Fail;
        }
        internal static void ClearDownTool()
        {
           
        }
        internal void ExceptionCode(int code)
        {

        }
    }
    #region 扩展接口
    public interface DownTool
    {
        void Init(FDownloader setting);
        void Send(Action<FDownloader> callBack);
        void Close();
    }
    internal class UnityDownloader : DownTool
    {
        private FDownloader mDownloader;
        private UnityWebRequest mUnityWeb;
        public void Init(FDownloader setting)
        {
            mDownloader = setting;
        }

        public void Close()
        {

        }
        public void Send(Action<FDownloader> callBack)
        {
            Timer_Coroutine.SetTimer(SendDownloader(callBack), null);
        }

        public class MyUnityDownloadHandler : DownloadHandlerScript
        {
            private FDownloader mDownloader;
            public MyUnityDownloadHandler(FDownloader loader)
            {
                mDownloader = loader;
            }

            protected override bool ReceiveData(byte[] data, int dataLength)
            {
                if (mDownloader != null)
                {
                    mDownloader.WriteStream(data, 0, dataLength);
                }
                return true;
            }
        }

        private IEnumerator SendDownloader(Action<FDownloader> callBack)
        {
            using (var www = new UnityWebRequest(mDownloader.URL, mDownloader.Method))
            {
                mUnityWeb = www;
                mDownloader.CreateStream();
                www.downloadHandler = new MyUnityDownloadHandler(mDownloader);
                www.SetRequestHeader("Content-Type", string.IsNullOrEmpty(mDownloader.ContentType) ? "application/x-www-form-urlencoded" : mDownloader.ContentType);
                long startIndex = mDownloader.GetBreakStreamCount();
                if (startIndex > 0)
                {
                    www.SetRequestHeader("Range", "bytes=" + startIndex + "-");
                }
                www.SendWebRequest();
                yield return 0;

                if (mDownloader.TotalLength <= 0)
                {
                    string lengthstr = www.GetResponseHeader("Content-Length");
                    while (string.IsNullOrEmpty(lengthstr) && !www.isDone && !www.isHttpError && !www.isNetworkError)
                    {
                        lengthstr = www.GetResponseHeader("Content-Length");
                        yield return 0;
                    }
                    if (!string.IsNullOrEmpty(lengthstr))
                    {
                        mDownloader.TotalLength = long.Parse(lengthstr)+ startIndex;
                    }
                }


                //int index = 0;
                while (!www.isDone && !www.isHttpError && !www.isNetworkError)
                {
                    //if (www.downloadHandler != null && www.downloadHandler.data != null && www.downloadHandler.data.Length > index)
                    //{
                    //    var len = www.downloadHandler.data.Length - index;
                    //    mDownloader.WriteStream(www.downloadHandler.data, index, len);
                    //    index += len;
                    //}
                    yield return new WaitForSeconds(1.0f);
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    mDownloader.ExceptionCode((int)www.responseCode);
                    mDownloader.SetError(www.error);
                }
                else
                {
                    mDownloader.HandleStream();
                }
                mUnityWeb = null;
                callBack(mDownloader);
            }
        }
    }
    #endregion
}
