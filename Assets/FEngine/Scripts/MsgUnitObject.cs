using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;
public class MsgUnitObject : UnitObject
{
    protected MsgMesh mMsgMesh = new MsgMesh();
    private void OnDestroy()
    {
        mMsgMesh.Clear();
        Destroy();
    }

    public virtual void Destroy()
    {

    }
}
