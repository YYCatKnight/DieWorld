//----------------------------------------------
//  F2DEngine: time: 2016.10  by fucong QQ:353204643
//----------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace F2DEngine
{
    [Serializable]
    public class GlobalDataProperty : BaseAssetProperty
    {
        public string Value;
        public string describe;
        public string param1;
        public string param2;
        public string param3;

        public T GetValue<T>() where T : new ()
        {
            return StringSerialize.Deserialize<T>("{" + Value + "}");
        }
    }


    public class GlobalDataAsset : TemplateAsset<GlobalDataAsset, GlobalDataProperty>
    {

    }
}
