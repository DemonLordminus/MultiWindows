using System;
using Kirurobo;
using UnityEngine;

[RequireComponent(typeof(UniWindowController))]
[DefaultExecutionOrder(-3000)]
public class UniWindowControllerEditorFix : MonoBehaviour
{
#if UNITY_EDITOR
    private void Awake()
    {
        // GetComponent<UniWindowController>().enabled = false;
        gameObject.SetActive(false);
    }
#endif
}
