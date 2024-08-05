using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetStartPos : MonoBehaviour
{
    [SerializeField] private NetworkObject _networkObject;

    [SerializeField] private ChangeOwnerShipInPlayer _changeOwnerShipInPlayer;
    // Start is called before the first frame update
    // async void Start()
    // {
    //     await Task.Yield();
    //     // SetToStartPos();
    // }
    private void Start()
    {
        if (_changeOwnerShipInPlayer == null)
        {
            _changeOwnerShipInPlayer = GetComponent<ChangeOwnerShipInPlayer>();
        }
    }

    public static Action SetPlayerToStartPos;

    private void OnEnable()
    {
        SetPlayerToStartPos += SetToStartPos;
    }

    private void OnDisable()
    {
        SetPlayerToStartPos -= SetToStartPos;
    }

    public void SetToStartPos()
    {
        if (_networkObject.IsOwner)
        {
            // Debug.Log("尝试重生");
            if (PlayerStartPoint.instance != null)
            {
                transform.position = PlayerStartPoint.instance.transform.position;
                Debug.Log("重生成功");
            }
            else
            {
                // Debug.LogWarning("PlayerStartPoint缺失");
                if (NetworkManager.Singleton.IsHost)
                {
                    if (!NetIdManager.Instance.isOnlyHostExist)
                    {
                        var otherId = NetIdManager.Instance.GetOneExistClientID();
                        if (otherId != 0)
                        {
                            _changeOwnerShipInPlayer.ChangeOwnership(otherId);
                        }
                    }
                }
            }
        }
        
    }
    
    private void Update()
    {
        if (transform.position.y < -70 && _networkObject.IsOwner)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                if (!NetIdManager.Instance.isOnlyHostExist)
                {
                    var otherId = NetIdManager.Instance.GetOneExistClientID();
                    if (otherId != 0)
                    {
                        _changeOwnerShipInPlayer.ChangeOwnership(otherId);
                    }
                }
            }
            else
            {
                
                SetToStartPos();
            }
            //Debug.Log("需要复活");
        }

        if (_networkObject.IsOwner && NetworkManager.Singleton.IsHost)
        {
            if (GlobalInput.GetKeyDown(GlobalKeyCode.R))
            {
                SetToStartPos();
            }
        }
    }
}
