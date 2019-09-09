using F2DEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DragBase : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public BasePlane bp;

    public void Start()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            var editorBps = GetComponentsInParent<BasePlane>();
            if (null != editorBps && editorBps.Length > 0)
            {
                bp = editorBps[editorBps.Length - 1];
            }
            else
            {
                bp = null;
            }
            return;
        }
#endif
        if (null == bp || !bp.IsRealyPlane())
        {
            var bps = GetComponentsInParent<BasePlane>();
            if (null != bps && bps.Length > 0)
            {
                for (int i = bps.Length - 1; i >= 0; --i)
                {
                    if (bps[i].IsRealyPlane())
                    {
                        bp = bps[i];
                    }
                }
            }
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (null != bp)
        {
            bp.OnDrag();
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (null != bp)
        {
            bp.OnDrag();
        }
    }
}
