using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using F2DEngine;

public class WaitPlane : BasePlane
{
    public override void Init(params object[] o)
    {
        mMainPlane.GetFObject("F_Show").SetActive(false);
        StopAllCoroutines();
        StartCoroutine(PlayFun());
    }

    IEnumerator PlayFun()
    {
        yield return new WaitForSeconds(3.5f);
        mMainPlane.GetFObject("F_Show").SetActive(true);
        var rot = mMainPlane.GetFObject("F_Rot");
        float timeDp = 1000;
        while(true)
        {
            rot.transform.Rotate(0, 0, -300 * Time.deltaTime);
            if ((timeDp -= Time.deltaTime) < 0)
                CloseMySelf(true);
            yield return 0;
        }
    }
}
