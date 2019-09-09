using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class BattleScene : FSceneTemplate<BattleScene>
{
    public override void Begin(params object[] objs)
    {
        UIManager.instance.ShowWindos(UIPlane.BattleUIPanel);
    }
}
