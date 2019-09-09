using F2DEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorLoadScene : MonoBehaviour {

    public static GameObject  cc; 
    private void Awake()
    {
        if (cc == null)
        {
            var fengine = FEngineManager.Create(ResConfig.CC_FENGINE, null);
            string curName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            cc = fengine;
            if (curName != LoadSceneManager.instance.GetSceneName(GameProgress.GP_LOG))
            {
                LoadSceneManager.instance.SetScene(GameProgress.GP_Menu, curName);
                LoadSceneManager.instance.LoadDirectScene(GameProgress.GP_NONE);
                LoadSceneManager.instance.LoadScene(GameProgress.GP_Menu);
                return;
            }
        }
    }
}
