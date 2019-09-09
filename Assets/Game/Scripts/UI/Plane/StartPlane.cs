using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
using UnityEngine.UI;
using EventStrArray;

public class StartPlane : BasePlane
{
    public override void Init(params object[] o)
    {
        base.Init(o);
        GetFObject<Text>("F_Title").text = "start_title_desc".TranslationString();
        mMsgMesh.RegEvent(KeyCodeEvent.KEYCODE_ENTER_DOWN, (f) =>
        {
            LoadSceneManager.instance.LoadScene(GameProgress.GP_City, LoadMode.SetAction(TestAction, LoadSceneManager.LoadType.Lt_Enter));
        });
    }

    protected void TestAction()
    {
        Debug.Log("Ysccy");
    }
}
