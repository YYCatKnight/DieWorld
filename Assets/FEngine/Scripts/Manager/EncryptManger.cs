//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace F2DEngine
{
    public class EncryptManger : ManagerTemplate<EncryptManger>
    {

        public static string nAllKey = "FEngine";


        public static void CreateEncryptFile(string fileName, string OutFileName)
        {
            FileStream sFile = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream sOutFile = new FileStream(OutFileName, FileMode.OpenOrCreate, FileAccess.Write);
            long size = sFile.Length;
            byte[] buffer = new byte[size];
            sFile.Read(buffer, 0, (int)size);
            sFile.Close();
            byte[] newData = EncryptString(buffer);
            sOutFile.Write(newData, 0, newData.Length);
            sOutFile.Close();
        }


        public static byte[] EncryptString(byte[] plainTextArray, string key = "")
        {

            string tempKey = key + nAllKey + "abcdefghijklmnopqrstuvwxyz1234567890";
            tempKey = tempKey.Substring(0, 32);
            byte[] Key = System.Convert.FromBase64String(tempKey);
            byte[] IV = System.Convert.FromBase64String(tempKey);
            // 建立一个MemoryStream，这里面存放加密后的数据流

            //DESCryptoServiceProvider DESalg = new DESCryptoServiceProvider();

            MemoryStream mStream = new MemoryStream();

            // 使用MemoryStream 和key、IV新建一个CryptoStream 对象

            CryptoStream cStream = new CryptoStream(mStream,

                new TripleDESCryptoServiceProvider().CreateEncryptor(Key, IV),

                CryptoStreamMode.Write);

            // 将加密后的字节流写入到MemoryStream

            cStream.Write(plainTextArray, 0, plainTextArray.Length);

            //把缓冲区中的最后状态更新到MemoryStream，并清除cStream的缓存区

            cStream.FlushFinalBlock();

            // 把解密后的数据流转成字节流

            byte[] ret = mStream.ToArray();

            // 关闭两个streams.

            cStream.Close();

            mStream.Close();

            return ret;

        }
        public static byte[] DecryptTextFromMemory(byte[] EncryptedDataArray, string key = "")
        {
            string tempKey = key + nAllKey + "abcdefghijklmnopqrstuvwxyz1234567890";
            tempKey = tempKey.Substring(0, 32);
            byte[] Key = System.Convert.FromBase64String(tempKey);
            byte[] IV = System.Convert.FromBase64String(tempKey);
            // 建立一个MemoryStream，这里面存放加密后的数据流

            MemoryStream msDecrypt = new MemoryStream(EncryptedDataArray);

            // 使用MemoryStream 和key、IV新建一个CryptoStream 对象

            CryptoStream csDecrypt = new CryptoStream(msDecrypt,

                new TripleDESCryptoServiceProvider().CreateDecryptor(Key, IV),

                CryptoStreamMode.Read);

            // 根据密文byte[]的长度（可能比加密前的明文长），新建一个存放解密后明文的byte[]

            byte[] DecryptDataArray = new byte[EncryptedDataArray.Length];

            // 把解密后的数据读入到DecryptDataArray

            csDecrypt.Read(DecryptDataArray, 0, DecryptDataArray.Length);

            msDecrypt.Close();

            csDecrypt.Close();

            return DecryptDataArray;

        }

    }
}
