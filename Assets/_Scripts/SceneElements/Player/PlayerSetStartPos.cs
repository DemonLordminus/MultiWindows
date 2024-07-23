using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class PlayerSetStartPos : MonoBehaviour
{
    [SerializeField] private NetworkObject _networkObject;
    // Start is called before the first frame update
    // async void Start()
    // {
    //     await Task.Yield();
    //     // SetToStartPos();
    // }
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
                Debug.LogWarning("PlayerStartPoint缺失");
            }
        }
        
    }
    private void Update()
    {
        if (transform.position.y < -70 && _networkObject.IsOwner && NetworkManager.Singleton.IsHost)
        {
            SetToStartPos();
            //Debug.Log("需要复活");
        }    
    }
}
