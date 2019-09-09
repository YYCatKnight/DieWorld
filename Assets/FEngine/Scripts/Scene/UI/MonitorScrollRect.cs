using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class MonitorScrollRect : MonoBehaviour {

    public bool ForceAddDragUI = false;

#if UNITY_EDITOR
    private ScrollRect _sr;
    private DragUI _dragUI;
    void Update () {
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            if (null == _sr)
            {
                _sr = GetComponent<ScrollRect>();
            }
            if (null == _sr)
            {
                DestroyImmediate(this, true);
                return;
            }
            if (_sr.horizontal == true || ForceAddDragUI)
            {
                if (null == _dragUI)
                {
                    _dragUI = GetComponent<DragUI>();
                    if (null == _dragUI)
                    {
                        _dragUI = gameObject.AddComponent<DragUI>();
                    }
                    if (ForceAddDragUI)
                    {
                        _dragUI.ForceAdd = true;
                    }
                    else
                    {
                        _dragUI.ForceAdd = false;
                    }
                }
            }
        }
	}
#endif
}
