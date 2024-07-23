using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class CameraGroup : Singleton<CameraGroup>
{
    public Camera nowCamera;
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
    
    // private float initialOrthographicSize;
    // private float initialAspect;
    private IWindowsAttributeChange IChange;
    
    
    private async void Start()
    {
        // initialOrthographicSize = nowCamera.orthographicSize;
        // initialAspect = (float)800 / 600;
        IChange = WindowsAttributeController.Instance.IChange;
        if (NetworkManager.Singleton.IsHost)
        {
            // AutoSetCameraOrthographic();
            HostSetCameraOrthographic();
        }
        else
        {
            await UniTask.Delay(500);
            AutoSetCameraOrthographic();
        }
    }

    public void AutoSetCameraOrthographic()
    {
        if(Application.isEditor) return;
        
        // // nowCamera.orthographicSize = 10 * Screen.height / Screen.width;
        FrameAttribute win = IChange.GetNowFrameAttribute();
        float nowHeight = win.height - IChange.GetTitleHeight(); //测试要不要-2
        //float nowHeight = win.height;
        virtualCamera.m_Lens.OrthographicSize = 10f * (nowHeight / 600);
        // // nowCamera.orthographicSize = initialOrthographicSize * (initialAspect / currentAspect);
        //
        // virtualCamera.m_Lens.OrthographicSize = initialOrthographicSize * (currentAspect / initialAspect);
        // // Debug.Log($"{win.width}，{win.height - 30}，{virtualCamera.m_Lens.OrthographicSize}，{nowCamera.orthographicSize}");
    }

    public void HostSetCameraOrthographic()
    {
        if(Application.isEditor) return;
        
        // // nowCamera.orthographicSize = 10 * Screen.height / Screen.width;
        FrameAttribute win = IChange.GetNowFrameAttribute();
        float nowHeight = win.height + IChange.GetTitleHeight();
        virtualCamera.m_Lens.OrthographicSize = 10f * (nowHeight / 600);
        // // nowCamera.orthographicSize = initialOrthographicSize * (initialAspect / currentAspect);
        //
        // virtualCamera.m_Lens.OrthographicSize = initialOrthographicSize * (currentAspect / initialAspect);
        // // Debug.Log($"{win.width}，{win.height - 30}，{virtualCamera.m_Lens.OrthographicSize}，{nowCamera.orthographicSize}");

    }
    // private void Update()
    // {
    //     if (GlobalInput.GetKeyDown(GlobalKeyCode.E))
    //     {
    //         AutoSetCameraOrthographic();
    //     }
    // }
}



