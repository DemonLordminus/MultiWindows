using System;
using UnityEngine;

public class ResizeSwitch : MonoBehaviour
{
    public bool isCanResize;

    private void Start()
    {
        WindowsAttributeController.Instance.IChange.SetWindowResizeMode(isCanResize);
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         WindowsAttributeController.Instance.IChange.SetWindowResizeMode(false);
    //     }
    //     if (Input.GetKeyDown(KeyCode.Y))
    //     {
    //         WindowsAttributeController.Instance.IChange.SetWindowResizeMode(true);
    //     }
    // }
}
