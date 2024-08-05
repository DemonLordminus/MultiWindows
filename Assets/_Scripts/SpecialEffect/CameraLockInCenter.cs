using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CameraLockInCenter : MonoBehaviour
{
    private void Start()
    {
        Lock();
    }

    // Update is called once per frame
    async void Lock()
    {
        WindowsAttributeController.Instance.IChange.LockindowsPositionToCenter();
        WindowsAttributeController.Instance.IChange.SetWinodwsOrder(true,true);
        NetdataManager.host.ChangeOrder();
        ChangeCameraCollider.colliderRefresh?.Invoke();
        await UniTask.Delay(100);
        if (gameObject)
        {
            Lock();
        }
        else
        {
            WindowsAttributeController.Instance.IChange.SetWinodwsOrder(false,true);
        }
    }
}
