//----------------------------------------------
//  F2DEngine: time: 2017.9  by fucong QQ:353204643
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace F2DEngine
{
    public class F_Examples : FSceneTemplate<F_Examples>
    {
        private Rect windowRect = new Rect(Screen.width/2, 20, 150, 100);
        private List<Example_Scene> mExampleScenes = new List<Example_Scene>();
        public class Example_Scene
        {
            public string scene;
            public string name;
        }

        public override void Begin(params object[] obj)
        {
            RegScene("换装系统", "SwitchClothes");
            RegScene("状态机", "ArpgState");
            RegScene("扰动测试", "HeatDistortion");
            RegScene("遮罩效果", "Mask");
            RegScene("2DARPG", "Demo");
            RegScene("XLua界面", "LuaScene");
            RegScene("ShadowMap", "Shadow");
            //RegScene("监督AI", "LearnScene");
            //RegScene("神经算法", "NeuralAlgorithm");
            //RegScene("无监督AI", "UnLearnNeuralScene");      
            //RegScene("2D换装", "Spine");
            RegScene("文字游戏", "Story");
            RegScene("水效果", "Water");
            RegScene("GodNet服务器", "GodNet");

            MyLog.SetLog();
            UIManager.instance.ShowWindos(ResConfig.FEXAMPLESPLANE,mExampleScenes);
        }

        public void RegScene(string dec,string sceneName)
        {
            Example_Scene es = new Example_Scene();
            es.name = dec;
            es.scene = sceneName;
            mExampleScenes.Add(es);
        }
    }
}
