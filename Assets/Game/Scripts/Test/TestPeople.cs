using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPeople : BaseLive
{
    public TestPeople(int only_id, int id) : base(only_id, id)
    {
    }

    protected override DamagePack DealAttack(BaseLive other)
    {
        throw new System.NotImplementedException();
    }

    protected override void DealHurted(DamagePack pack)
    {
        throw new System.NotImplementedException();
    }
}
