//----------------------------------------------
//  F2DEngine: time: 2015.10  by fucong QQ:353204643
//----------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public static class FUniversalFunction
    {
        //简单加密
        private static byte[] mCode = new byte[] 
        {
            1,4,5,44,32,9,90,32,34,21,45,5,93,23,22,11,55,66,77,88,99,55,54,65,32,111,78,97,21,4,6,8,44,90,
            3,6,7,17,13,15,35,34,21,7,5,8,9,46,67,21,71,13,45,13,6,33,36,4,6,45,77,88,21,34,67,83,8,1,32
        };

        public  static void EncryptBytes(byte[]bytes,int offset)
        {
            int maxLen = mCode.Length;
            for (int i = offset; i < bytes.Length;i++)
            {
                bytes[i] = (byte)(bytes[i]^mCode[i% maxLen]); 
            }
        }

        public static void  LeakerGameObject(GameObject go)
        {
           
        }

        public static System.Reflection.Assembly GetAssembly()
        {
           return  System.Reflection.Assembly.Load("Assembly-CSharp");
        }

        public static Type GetType(string typeName)
        {
            return System.Reflection.Assembly.Load("Assembly-CSharp").GetType(typeName);
        }

        public static List<Type> GetAssemblyType(Type type)
        {
            List<Type> tempTypes = new List<Type>();
            var types = System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes();
            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];
                if (t.IsSubclassOf(type))
                {
                    tempTypes.Add(t);
                }
            }
            return tempTypes;
        }

        public static  List<string> GetChunk(string key, string k1, string k2)
        {
            List<string> chunks = new List<string>();
            string tempStr = key;
            while (true)
            {
                int starPos = tempStr.IndexOf(k1);
                int endPos = tempStr.IndexOf(k2);
                if (starPos != -1 && endPos != -1)
                {
                    string str = tempStr.Substring(starPos + k1.Length, endPos - starPos - k1.Length);
                    chunks.Add(str);
                    tempStr = tempStr.Substring(0, starPos) + tempStr.Substring(endPos + k2.Length);
                }
                else
                {
                    break;
                }
            }
            chunks.Insert(0, tempStr);
            return chunks;
        }
        public static bool IsContainSameType(int main, int use)
        {
            return ((int)(main & use)) != 0;
        }
        public static int SetMaskCode(int main,int code,bool isMask)
        {
            if (isMask)
            {
                if (code == 0)
                {
                    main = ~0; 
                }
                else
                {
                    main |= code;
                }
            }
            else
            {
                if (code == 0)
                {
                    main = 0;
                }
                else
                {
                    main &= (~code);
                }
            }
            return main;
        }

        //世界坐标转到ui主表
        public static Vector3 WorldToUIPoint(Vector3 worldPos)
        {
            Vector3 pos = Camera.main.WorldToViewportPoint(worldPos);
            var uiPos = FCMainCanvas.instance.GetMianCamera().ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }
        //世界坐标转到ui主表
        public static Vector3 CamWorldToUIPoint(Vector3 worldPos, Camera cam)
        {
            Vector3 pos = cam.WorldToViewportPoint(worldPos);
            var uiPos = FCMainCanvas.instance.GetMianCamera().ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }
        public static Vector3 UIToWorld(Vector3 worldPos)
        {
            Vector3 pos = FCMainCanvas.instance.GetMianCamera().WorldToViewportPoint(worldPos);
            var uiPos = Camera.main.ViewportToWorldPoint(pos);
            uiPos.z = 0;
            return uiPos;
        }
    }
}
