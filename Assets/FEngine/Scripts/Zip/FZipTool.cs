//----------------------------------------------
//  F2DEngine: time: 2017.10  by fucong QQ:353204643
//----------------------------------------------
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace F2DEngine
{
    public class FZipTool
    {
        public static bool IsEnter = false;
        public static ZipThreadData ThreadUnZip(string ZipFile, string TargetDirectory, string Password, LoadPercent loadPercent  = null, bool OverWrite = true)
        {
            IsEnter = true;
            ZipThreadData ztd = new ZipThreadData();
            ztd.ZipFile = ZipFile;
            ztd.IsZipIng = true;
            ztd.TargetDirectory = TargetDirectory;
            ztd.Password = Password;
            ztd.loadPercent = loadPercent;
            ztd.OverWrite = OverWrite;
            Timer_Thread.SetTimer((t, r) =>
            {
                try
                {
                    UnZip(ztd.ZipFile, ztd.TargetDirectory, ztd.Password, ztd.loadPercent, ztd.OverWrite);
                    ztd.IsZipIng = false;
                }
                catch (System.Exception e)
                {
                    ztd.error = "解压线程出现错误:" + e.ToString();
                    ztd.loadPercent.Over("",1,ztd.error);
                    ztd.IsZipIng = false;
                }
                return -1;
            },0,null);
            return ztd;
        }

        public static void UnZip(string ZipFile, string TargetDirectory, string Password, LoadPercent loadPercent = null, bool OverWrite = true)
        {
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            if (loadPercent == null)
            {
                loadPercent = new LoadPercent();
            }

            loadPercent.SetKD("Decompression");
            //如果解压到的目录不存在，则报错
            FEPath.CreateDirectory(TargetDirectory);
            //目录结尾
            if (!TargetDirectory.EndsWith("/")) { TargetDirectory = TargetDirectory + "/"; }

            FileStream stream = File.OpenRead(ZipFile);
            using (ZipInputStream zipfiles = new ZipInputStream(stream))
            {
                zipfiles.Password = Password;
                ZipEntry theEntry;
                bool isShowPro = true;
                lock (loadPercent)
                {
                    loadPercent.SetTimece(stream.Length);
                    isShowPro = stream.Length > 1024 * 1024 * 5;
                }

                while ((theEntry = zipfiles.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;
                    long compressedSize = theEntry.CompressedSize;
                    if (pathToZip != "")
                        directoryName = FEPath.GetDirectoryName(pathToZip) + "/";

                    string fileName = FEPath.GetFileName(pathToZip);

                    FEPath.CreateDirectory(TargetDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(TargetDirectory + directoryName + fileName) && OverWrite) || (!File.Exists(TargetDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(TargetDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipfiles.Read(data, 0, data.Length);

                                    if (size > 0)
                                    {
                                        streamWriter.Write(data, 0, size);
                                    }
                                    else
                                    {
                                        lock (loadPercent)
                                        {
                                            var dataPro = loadPercent.GetPercent();
                                            if (isShowPro)
                                            {
                                                loadPercent.GoOn((dataPro.pre * 100).ToString("0") + "%", compressedSize);
                                            }
                                        }
                                        break;
                                    }
                                }
                                streamWriter.Close();
                            }
                        }
                        else
                        {
                            lock (loadPercent)
                            {
                                var dataPro = loadPercent.GetPercent();
                                if (isShowPro)
                                {
                                    loadPercent.GoOn((dataPro.pre * 100).ToString("0") + "%", compressedSize);
                                }
                            }
                        }
                    }
                }

                zipfiles.Close();
            }

            stream.Close();
            stream.Dispose();
            lock (loadPercent)
            {
                loadPercent.Over("");
            }
        }


        /// <param name="DirectoryToZip">需要压缩的文件夹（绝对路径）</param>
        /// <param name="ZipedPath">压缩后的文件路径（绝对路径）</param>
        /// <param name="ZipedFileName">压缩后的文件名称（文件名，默认 同源文件夹同名）</param>
        /// <param name="IsEncrypt">是否加密（默认 加密）</param>
        public static string ZipDirectory(string DirectoryToZip, string ZipedPath,string unEncryptKey = ".meta.delete", string ZipedFileName = "", bool IsEncrypt = true)
        {
            ZipConstants.DefaultCodePage = System.Text.Encoding.UTF8.CodePage;
            ZipedPath = FEPath.GetDirectoryName(ZipedPath);
            //如果目录不存在，则报错
            FEPath.CreateDirectory(DirectoryToZip);

            //文件名称（默认同源文件名称相同）
            string ZipFileName = string.IsNullOrEmpty(ZipedFileName) ? ZipedPath + "/" + new DirectoryInfo(DirectoryToZip).Name + ".zip" : ZipedPath + "/" + ZipedFileName + ".zip";

            using (System.IO.FileStream ZipFile = System.IO.File.Create(ZipFileName))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    if (IsEncrypt)
                    {
                        //压缩文件加密
                        s.Password = ResConfig.FZIPPASS;
                    }
                    string parentPath = FEPath.GetFileNameWithoutExtension(DirectoryToZip)+"/";
                    ZipSetp(DirectoryToZip, s, parentPath, unEncryptKey);
                }
            }
            return ZipFileName;
        }

        private static void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath,string unEncryptKey)
        {
            if (strDirectory[strDirectory.Length - 1] != '/')
            {
                strDirectory += "/";
            }
            //Crc32 crc = new Crc32();

            string[] filenames = FEPath.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {

                if (FEPath.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("/") + 1);
                    pPath += "/";
                    ZipSetp(file, s, pPath,unEncryptKey);
                }
                else // 否则直接压缩文件
                {
                    string exten = FEPath.GetExtension(file);

                    if (unEncryptKey== ""|| exten == ""||!unEncryptKey.Contains(exten))
                    {
                        //打开压缩文件
                        using (FileStream fs = File.OpenRead(file))
                        {
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, buffer.Length);
                            string fileName = parentPath + file.Substring(file.LastIndexOf("/") + 1);
                            ZipEntry entry = new ZipEntry(fileName);
                            entry.DateTime = DateTime.Now;
                            entry.Size = fs.Length;
                            fs.Close();
                            //crc.Reset();
                            //crc.Update(buffer);
                            //entry.Crc = crc.Value;
                            s.PutNextEntry(entry);
                            s.Write(buffer, 0, buffer.Length);
                        }

                        if (unEncryptKey.Contains(".delete"))
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
        }
    }
}
