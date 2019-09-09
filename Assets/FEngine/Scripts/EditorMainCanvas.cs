using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorMainCanvas : MonoBehaviour
{
    private void Awake()
    {
        this.gameObject.SetActive(false);
        GameObject.Destroy(this);
    }
}
