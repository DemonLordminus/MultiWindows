using System;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class FollowWorldObject : MonoBehaviour
{
    private IWindowsAttributeChange IChange;
    private void Start()
    {
        IChange = WindowsAttributeController.Instance.IChange;
        if (NetworkManager.Singleton.IsHost)
        {
            enabled = false;
        }
        Follow();
    }

    // Update is called once per frame
    async void Follow()
    {
        await UniTask.Delay(100);
        if (gameObject)
        {
            IChange.ChanageWindowsPos(IChange.GetWorldPositionToScreen(transform.position));
            Follow();
        }
    }
}
