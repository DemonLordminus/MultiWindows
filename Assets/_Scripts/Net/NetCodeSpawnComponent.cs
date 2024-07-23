using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class NetCodeSpawnComponent : NetworkBehaviour
{
    [Required] [ChildGameObjectsOnly] [SerializeField]
    private NetworkObject _networkObject;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void Start()
    {
        if (_networkObject == null)
        {
            _networkObject = GetComponent<NetworkObject>();
        }
        if (NetworkManager.Singleton.IsHost)
        {
            _networkObject.Spawn();
            Debug.Log($"{gameObject.name}的NetworkObject生成成功");
        }
    }
}
