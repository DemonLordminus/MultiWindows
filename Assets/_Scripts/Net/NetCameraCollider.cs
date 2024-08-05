using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetBoxCollider2D))]
public class NetCameraCollider : NetworkBehaviour
{
    [Required] [SerializeField] private NetBoxCollider2D _netBoxCollider2D;
    private Transform followRoot => SystemManager.Instance.cameraRoot;
    // [Required] [SerializeField] private ChangeCameraCollider _changeCameraCollider;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // if(IsHost)
        // {
        //     // Destroy(gameObject);
        //     _netBoxCollider2D.enabled = false;
        //     return;
        // }
        int titleHeight = WindowsAttributeController.Instance.IChange.GetTitleHeight();
        // _netBoxCollider2D.boxCollider2D.offset = new Vector2(0,  titleHeight * HostDataManager.Instance.rate / 2);
        if (IsOwner)
        {
            ChangeCameraCollider.onColliderUpdate += (value) =>
            {
                value.y += titleHeight * HostDataManager.Instance.rate * 2;
                _netBoxCollider2D.boxColliderOffset.Value = new Vector2(0, value.y / 2);
                _netBoxCollider2D.boxColliderSize.Value = new Vector2(value.x, titleHeight * HostDataManager.Instance.rate);
                // _netBoxCollider2D.boxColliderSize.Value = value * 1.05f;

            };
        }
        if(IsHost) // 只让0号窗口有碰撞
        {
            _netBoxCollider2D.boxCollider2D.enabled = true;
        }
        else
        {
            _netBoxCollider2D.boxCollider2D.enabled = false;
        }
    }


    private void Update()
    {
        try
        {
            if (IsOwner)
            {
                transform.position = followRoot.position;
            }
        }
        catch (MissingReferenceException e)
        {
            Debug.LogWarning("NetCameraCollider启动常规报错");
        }
        
    }
}