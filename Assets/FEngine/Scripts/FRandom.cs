//----------------------------------------------
//  F2DEngine: time: 2017.10  by fucong QQ:353204643
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FRandom
{
    protected static System.Random mRandom = new System.Random((int)(System.DateTime.Now.Ticks % 1000000));
    private System.Random mMyand;
    public FRandom(int seed)
    {
        mMyand = new System.Random(seed);
    }


    public static int RangeTime(int min,int max)
    {
        return min + mRandom.Next()%(max - min);
    }

    private int rand()
    {
        return mMyand.Next();
    }

    public int Range(int min, int max)
    {
        return min + mMyand.Next() % (max - min);
    }
}
