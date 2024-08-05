using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraLockInTop : MonoBehaviour
{
    private void Start()
    {
        Lock();
        WindowsAttributeController.Instance.IChange.SetWinodwsOrder(true,true);
    }

    // Update is called once per frame
    async void Lock()
    {
        NetdataManager.host.ChangeOrder();
        ChangeCameraCollider.colliderRefresh?.Invoke();
        await UniTask.Delay(100);
        if (gameObject)
        {
            Lock();
        }
        else
        {
            // WindowsAttributeController.Instance.IChange.SetWinodwsOrder(false,true);
        }
    }

    private async void OnDestroy()
    {
        WindowsAttributeController.Instance.IChange.SetWinodwsOrder(false,true);
        await UniTask.Delay(500);
        WindowsAttributeController.Instance.IChange.HighLightWindow();
    }
}
