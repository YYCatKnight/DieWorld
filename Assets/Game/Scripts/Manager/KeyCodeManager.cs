using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class KeyCodeManager : FBaseController<KeyCodeManager>
{
    protected override void Init()
    {
        base.Init();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            EventListenManager.Send(EventStrArray.KeyCodeEvent.KEYCODE_ENTER_DOWN);
        }
    }
}
