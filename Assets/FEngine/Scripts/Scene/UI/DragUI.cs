using F2DEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DragUI : DragBase
{
    [HideInInspector]
    public bool ForceAdd = false;

#if UNITY_EDITOR
    private ScrollRect _sr;
    private Slider _sld;
    void Update()
    {
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            if (null != _sld)
            {
                if (null == _sld.handleRect || !_sld.handleRect.parent.gameObject.activeSelf)
                {
                    DestroyImmediate(this, true);
                }
                return;
            }
            if (null == _sr)
            {
                _sr = GetComponent<ScrollRect>();
            }
            if (null == _sr)
            {
                if (null == _sld)
                {
                    _sld = GetComponent<Slider>();
                }
                if (null == _sld || null == _sld.handleRect || !_sld.handleRect.parent.gameObject.activeSelf)
                {
                    DestroyImmediate(this, true);
                    return;
                }
            }
            if (null != _sr)
            {
                if (_sr.horizontal == false && !ForceAdd)
                {
                    DestroyImmediate(this, true);
                    return;
                }
            }
        }
    }
#endif
}
