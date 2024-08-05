using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Cysharp.Threading.Tasks;

public class NetdataManager : NetworkBehaviour
{
    public List<ulong> clientOrder;
    public static NetdataManager instance;
    public static event Action OnOrderUpdate;
    public static event Action OnHostManagerInitComplete;
    public static event Action OnGameAnyStateUpdate;
    private readonly NetworkVariable<InputNetworkData> _netState = new(writePerm: NetworkVariableWritePermission.Owner);
    public Vector2 moveControl;
    public static NetdataManager host;
    public bool isThisHost;
    
    public NetworkVariable<int> GameId =  new NetworkVariable<int>(-1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<int> NowPlayerOwnerGameID = new NetworkVariable<int>(default,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    public NetworkVariable<LevelState> levelState = new NetworkVariable<LevelState>(global::LevelState.start_SingleWindow,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public bool isPlayerInHere => host.NowPlayerOwnerGameID == GameId;
    public bool isNoNeedToQuitWhenOtherGameQuit = false;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log(OwnerClientId);
        Init();
        
    }
    public override void OnDestroy()
    {
    
        base.OnDestroy();
#if !UNITY_EDITOR
        if (IsHost)
        {
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 1 && !isNoNeedToQuitWhenOtherGameQuit)
            {
                if (!Application.isEditor)
                {
                    Application.Quit();
                }
                else
                {
                    Debug.Log("程序模拟退出");
                }
            }
        }
#endif
    }

    
    // public void DestroyPlayer(NetworkObject networkObject)
    // {
    //     if (IsHost)
    //     {
    //         networkObject.Despawn(true);
    //         // Destroy(networkObject.gameObject);
    //     }
    // }
    // public void CreatePlayer(NetworkObject newPlayer,ulong ownerID)
    // {
    //     if (IsHost)
    //     {
    //         
    //         // Destroy(networkObject.gameObject);
    //     }
    // }

    private void OnApplicationQuit()
    {
        // Debug.Log("test");
        // if (IsOwner)
        // {
        //     Debug.Log($"netdataManager销毁测试,id:{OwnerClientId}");
        //     ClientDestroyServerRPC(OwnerClientId);
        // }
        
    }
    // [ServerRpc(RequireOwnership = false)]
    // private void ClientDestroyServerRPC(ulong clientID)
    // {
    //     Debug.Log($"netdataManager销毁测试Server,id:{OwnerClientId}");
    //     // instance.
    // }


    

    private async void Init()
    {
        if (IsOwner)
        {
            instance = this;
            AddNewOrder(OwnerClientId);
        }

        if (OwnerClientId == 0)
        {
            host = this;
            isThisHost = true;
            Debug.Log("Host确定", gameObject);

        }

        if (IsHost && IsOwner)
        {
            ChangeOwnerShipInPlayer.OnOwnerShipChange += (value) =>
            {
                for (int i = 0; i < NetIdManager.Instance.idRefrenceClientID.Length; i++)
                {
                    if (NetIdManager.Instance.idRefrenceClientID[i] == value)
                    {
                        NowPlayerOwnerGameID.Value = i;
                        break;
                    }
                }
               
            };
        }

        if (IsOwner)
        {
            ChangeOwnerShipInPlayer.OnOwnerShipChange += async (value) =>
            {
                await UniTask.Delay(100);
                OnGameAnyStateUpdate?.Invoke();
            };
        }
        await UniTask.Delay(500);
        if (IsHost)
        {
            GameId.Value = NetIdManager.Instance.GetNextId(OwnerClientId);
        }

        
        Debug.Log("生成Manager");
        await UniTask.Delay(100);
        OnGameAnyStateUpdate?.Invoke();
        Debug.Log(GameId.Value);
        // if (IsHost)
        // {
        //     Debug.Log("隐藏玩家");
        //     SpecialEffectManager.Instance.MakeHostPlayerSpriteVisible(true);
        //     SpecialEffectManager.Instance.MakeHostPlayerNoMove(true);
        // }
        if (IsHost && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
        {
            // Debug.Log("尝试转移控制权");
            await UniTask.Delay(100);
            // SpecialEffectManager.Instance.ChangePlayerOwnerShip(1);
        }

        if (IsOwner)
        {
            OneSceneLevelManager.Instance.LevelSwitch(NetdataManager.host.levelState.Value);
            await UniTask.Delay(10);
            PlayerSetStartPos.SetPlayerToStartPos?.Invoke(); // BUG:目前新打开窗口时直接让角色切换到新窗口会触发重生 暂时先不处理
            
        }

        await UniTask.Delay(300);
        if (IsHost && IsOwner)
        {
            OnHostManagerInitComplete?.Invoke();
        }
}
    
    // [ServerRpc(RequireOwnership = false)]
    // private ulong GetNowGameIDServerRPC()
    // {
    //     return NetIdManager.Instance.GetNextId();
    // }
    //
    
    public override void OnNetworkDespawn()
    {
        if (IsHost)
        {
            Debug.Log($"netdataManager销毁测试,id:{OwnerClientId},gameID:{GameId.Value}");
            NetIdManager.Instance.existId[GameId.Value] = false;
            NetIdManager.Instance.idRefrenceClientID[GameId.Value] = 100;
            if (NetworkManager.Singleton.ConnectedClientsList.Count == 1 && !isNoNeedToQuitWhenOtherGameQuit)
            {
                if (!Application.isEditor)
                {
                    NetworkManager.Singleton.Shutdown();
                    Application.Quit();
                }
                else
                {
                    Debug.Log("程序模拟退出");
                }
            }
        }
        base.OnNetworkDespawn();
        if (IsOwner)
        {
            RemoveOrder(); 
        }
    }
    private void OnApplicationFocus(bool focus)
    {
        if(focus && IsOwner)
        {
            ChangeOrder();
        }
    }
    private void AddNewOrder(ulong clientID)
    {

        AddNewOrderServerRPC(clientID);

    }
    [ServerRpc(RequireOwnership = false)]
    private void AddNewOrderServerRPC(ulong clientID)
    {
        //Debug.Log(clientID);
        AddNewOrderClientRPC(clientID);
        //clientOrder.Insert(0, clientID);
    }
    [ClientRpc]
    private void AddNewOrderClientRPC(ulong clientID)
    {
        instance.clientOrder.Insert(0, clientID);
        OnOrderUpdate?.Invoke();
    }

    public void ChangeOrder()
    {
        ChangeOrderServerRPC(OwnerClientId);

    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeOrderServerRPC(ulong clientID)
    {
        //clientOrder.Remove(clientID);
        //clientOrder.Insert(0, clientID);
        //OnOrderUpdate?.Invoke();
        ChangeOderClientRPC(clientID);
    }
    [ClientRpc]
    private void ChangeOderClientRPC(ulong clientID)
    {
        instance.clientOrder.Remove(clientID);
        instance.clientOrder.Insert(0, clientID);
        OnOrderUpdate?.Invoke();
    }
    public int GetNowOrder()
    {
        return clientOrder.FindIndex(u => u == OwnerClientId);
    }


    private void RemoveOrder()
    {
        RemoveOrderServerRPC(OwnerClientId);

    }

    public void LevelStateChange(LevelState newState)
    {
        if (IsHost)
        {
            LevelStateChangeClientRPC(newState);
        }
        else
        {
            LevelStateChangeServerRPC(newState);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void LevelStateChangeServerRPC(LevelState newState)
    {
        Debug.Log("LevelStateChangeServerRPC");
        levelState.Value = newState;
        LevelStateChangeClientRPC(newState);
    }
    [ClientRpc]
    private void LevelStateChangeClientRPC(LevelState newState)
    {
        Debug.Log("LevelStateChangeClientRPC");
        OneSceneLevelManager.Instance.LevelSwitch(newState);
    }
    [ServerRpc(RequireOwnership = false)]
    private void RemoveOrderServerRPC(ulong clientID)
    {
        //clientOrder.Remove(clientID);
        //clientOrder.Insert(0, clientID);
        //OnOrderUpdate?.Invoke();
        RemoveOrderClientRPC(clientID);
    }
    [ClientRpc]
    private void RemoveOrderClientRPC(ulong clientID)
    {
        instance.clientOrder.Remove(clientID);
        OnOrderUpdate?.Invoke();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public void FinalEffectServerRpc()
    {
        FinalEffectClientRpc();
        ChangeOwnerShipInPlayer _changeOwnerShipInPlayer = (ChangeOwnerShipInPlayer)GameObject.FindObjectOfType(typeof(ChangeOwnerShipInPlayer));
        _changeOwnerShipInPlayer.networkObject.ChangeOwnership(0);
        _changeOwnerShipInPlayer.isLock = true;
    }
    [ClientRpc]
    public void FinalEffectClientRpc()
    {
        EnableForInteract.OnInteract?.Invoke(1,false);
        ChangeOwnerShipInPlayer _changeOwnerShipInPlayer = (ChangeOwnerShipInPlayer)GameObject.FindObjectOfType(typeof(ChangeOwnerShipInPlayer));
        _changeOwnerShipInPlayer.isLock = true;
        _changeOwnerShipInPlayer.networkObject.ChangeOwnership(0);
        // ChangeOwnerShipInPlayer _changeOwnerShipInPlayer = (ChangeOwnerShipInPlayer)GameObject.FindObjectOfType(typeof(ChangeOwnerShipInPlayer));
        // _changeOwnerShipInPlayer.networkObject.ChangeOwnership(0);
        // _changeOwnerShipInPlayer.isLock = true;
    }
    
    private void Update()
    {
        if(isThisHost)
        {
            if (IsOwner)
            {
                _netState.Value = new InputNetworkData() 
                {
                    moveDir = moveControl
                };
            }
            else
            {
                moveControl = _netState.Value.moveDir;
            }
        }
    }
}
struct InputNetworkData : INetworkSerializable
{
    private float _x, _y;
    internal Vector2 moveDir
    {
        get => new Vector2(_x, _y);
        set
        {
            _x = value.x;
            _y = value.y;
        }
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _x);
        serializer.SerializeValue(ref _y);
    }
}