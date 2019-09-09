//----------------------------------------------
//  F2DEngine: time: 2018.10  by fucong QQ:353204643
//----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    //内存泄露监控器
    public class FLeaker:UnitData
    {
        public static readonly string LEAKER_HEAD = "_LEAKER_";
        private static int mIndex = 0;
        public Texture2D mTex { get; protected set; }
        public string name { get; protected set; }
        public string sceneName { get; protected set; }
        public string texName { get; protected set; }
        public static FLeaker Create(MonoBehaviour cls)
        {
            return _Create(cls.gameObject.name+"-"+cls.GetType().ToString());
        }

        private static FLeaker _Create(string clsName)
        {
            mIndex++;
            var lea = new FLeaker();          
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            return lea;
        }
    }
}
