using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using F2DEngine;

public class ScrollScene : MonoBehaviour
{
    public FCScrollRect mScroll0;
    public FCScrollRect mScroll1;
    public FCScrollRect mScroll2;
    // Use this for initialization
    private void Awake()
    {
        mScroll0.SetGroup(2);
        mScroll0.SetMode(FCScrollRect.ScrollType.Repeat);
        mScroll0.UpdateEvent = ScrollItem;


        mScroll0.SetCount(100);
        

        mScroll1.SetGroup(3);
        mScroll1.SetMode(FCScrollRect.ScrollType.Asy);
        mScroll1.UpdateEvent = ScrollItem;



        mScroll1.SetCount(50);

        mScroll2.SetGroup(2);
        mScroll2.SetMode(FCScrollRect.ScrollType.Circle);
        mScroll2.UpdateEvent = ScrollItem;
        mScroll2.CenterEvent = (f) =>
        {
            Debug.Log("选中了" + f.Index.ToString());
        };
        mScroll2.SetCount(12);
    }
	

    void ScrollItem(FCScrollRect.ScrollItem item)
    {
        item.gameObject.transform.GetChild(0).GetComponent<Text>().text = item.Index.ToString();
    }
    // Update is called once per frame
    void Update ()
    {
		
	}
}
