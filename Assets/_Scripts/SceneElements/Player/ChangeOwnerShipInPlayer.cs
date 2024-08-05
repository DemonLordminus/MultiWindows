using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeOwnerShipInPlayer : NetworkBehaviour
{
    [SerializeField] private SortingGroup hideGroup;
    [SerializeField] private SpriteRenderer[] spriteRenderer;
    public NetworkObject networkObject;
    [SerializeField] private bool isHide,isHalfHide;
    public static event Action<ulong> OnOwnerShipChange;
    public bool isLock = false;
    
    //[SerializeField] private Color normal, halfHide;
    public override async void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        if (!IsHost && isHide)
        {
            if (!isHalfHide)
            {
                hideGroup.enabled = true; 
            }
            else
            {
                //normal = spriteRenderer.color;
                // var color = spriteRenderer.color;
                // spriteRenderer.color = new Color(color.r,color.g,color.b, 0.5f);
                await UniTask.Delay(100);
                if (!IsOwner)
                {
                    SetSpriteRenderColor(true);
                }
            }
        }

        if (IsHost)
        {
            foreach (var render in spriteRenderer)
            {
                render.enabled = false;
            }
        }
    }

    public void SetSpriteRenderColor(bool isHalfAlpha)
    {
        float newAlpha = 1f;
        if (isHalfAlpha)
        {
            newAlpha = 0.5f;
        }

        // spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        foreach (var render in spriteRenderer)
        {
            var color = render.color;
            render.color =  new Color(color.r,color.g,color.b, newAlpha);
        }
    }

    public void SetSpriteRenderVisible(bool isHide)
    {
        hideGroup.enabled = isHide;
        hideGroup.gameObject.SetActive(!isHide);
    }
    public void ChangeOwnership(ulong targetID)
    {
        if (!isLock)
        {
            ChangeOwnerShipServerRPC(targetID);
        }
    }
    
    public void ChangeOwnership(int gameID,bool isNoOrderCheck = false)
    {
        if (!isLock)
        {
            ChangeOwnerShipServerRPC(gameID,isNoOrderCheck);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnerShipServerRPC(ulong targetID,int nowCheck = -1)
    {
        var targetIndex = NetdataManager.host.clientOrder.FindIndex(x => x == targetID);
        var nowIndex = NetdataManager.host.clientOrder.FindIndex(x => x == OwnerClientId);
        if(nowIndex > targetIndex)
        {
            OnOwnerShipChange?.Invoke(targetID);
            networkObject.ChangeOwnership(targetID);
            SpriteHideClientRPC();
        }
        else
        {

        }
        //Debug.Log(nowCheck);
        //int p;
        //if(nowCheck == -1)
        //{
        //}
        //else
        //{
        //    p = nowCheck;
        //}
        ////ulong[] tests = new ulong[p];
        ////for (int i = 0; i < p; i++)
        ////{
        ////    Debug.Log(i);
        ////    tests[i] = NetdataManager.host.clientOrder[i];
        ////}
        //if (p == 0)
        //{
        //    if (!spriteRenderer.isVisible)
        //    {
        //        networkObject.ChangeOwnership(targetID);
        //        SpriteHideClientRPC();
        //    }
        //    return;
        //}

        //ClientRpcParams clientRpcParams = new ClientRpcParams
        //{
        //    Send = new ClientRpcSendParams
        //    {
        //        TargetClientIds = new ulong[]
        //        {
        //            NetdataManager.host.clientOrder[p - 1]
        //        }
        //    }
        //};
        //CheckIsCanChangeOwnerShipClientRpc(targetID,p - 1,clientRpcParams);

    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnerShipServerRPC(int gameID, bool isNoOrderCheck = false)
    {
        ulong targetID = NetIdManager.Instance.idRefrenceClientID[gameID];
        var targetIndex = NetdataManager.host.clientOrder.FindIndex(x => x == targetID);
        var nowIndex = NetdataManager.host.clientOrder.FindIndex(x => x == OwnerClientId);
        if (nowIndex > targetIndex || isNoOrderCheck)
        {
            OnOwnerShipChange?.Invoke(targetID);
            networkObject.ChangeOwnership(targetID);
            SpriteHideClientRPC();
        }
        else
        {

        }
    }

    // public void LeaveOwnership(ulong targetID)
    // {
    //     if (IsOwner)
    //     {
    //         LeaveOwnershipServerRPC(targetID); 
    //     }
    // }
    // [ServerRpc(RequireOwnership = false)]
    // public void LeaveOwnershipServerRPC(ulong targetID, int nowCheck = -1)
    // {
    //     int p = 0;
    //     if (nowCheck == -1)
    //     {
    //         p = NetdataManager.host.clientOrder.FindIndex(x => x == targetID);
    //     }
    //     else
    //     {
    //         p = nowCheck;
    //     }
    //     //Debug.Log(p);
    //     //Debug.Log(NetdataManager.host.clientOrder.Count-1);
    //     //ulong[] tests = new ulong[p];
    //     //for (int i = p; i < NetdataManager.host.clientOrder.Count; i++)
    //     //{
    //     //    tests[i] = NetdataManager.host.clientOrder[i];
    //     //}
    //     if (p == NetdataManager.host.clientOrder.Count - 1)
    //     {
    //         return;
    //     }
    //     ClientRpcParams clientRpcParams = new ClientRpcParams
    //     {
    //         Send = new ClientRpcSendParams
    //         {
    //             TargetClientIds = new ulong[]
    //             {
    //                 NetdataManager.host.clientOrder[p + 1]
    //             }
    //         }
    //     };
    //
    //     CheckIsCanChangeOwnerShipClientRpc(targetID, nowCheck + 1, clientRpcParams);
    //
    // }
    [ClientRpc]
    public void SpriteHideClientRPC()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            if (!IsOwner)
            {
                // if (HostDataManager.Instance.NowStageCurtainState == StageCurtainState.hideCurtain)
                // {
                //     hideGroup.enabled = true;
                //     return;
                // }
                foreach (var render in spriteRenderer)
                {
                    render.enabled = false;
                }
            }
            else
            {
                //hideGroup.enabled = false;
                SetSpriteRenderColor(false);
                foreach (var render in spriteRenderer)
                {
                    render.enabled = true;
                }
            }
            return;
        }
        else
        {
            if (OwnerClientId == 0)
            {
                hideGroup.enabled = true;
                return;
            }
        }
        hideGroup.enabled = false;
        
        if (isHide)
        {
            if (!IsOwner)
            {
                if(isHalfHide)
                {
                    SetSpriteRenderColor(true);
                }
                else
                { 
                
                    hideGroup.enabled = true;
                }
            }
            else
            {
                if (isHalfHide)
                {
                    SetSpriteRenderColor(false);
                }
                else
                {
                    hideGroup.enabled = false;
                }
              
            } 
        }
    }

    // [ClientRpc]
    // public void CheckIsCanChangeOwnerShipClientRpc(ulong targetID, int nowCheck, ClientRpcParams clientRpcParams = default)
    // {
    //     //Debug.Log(nowCheck + "p");
    //     if (!spriteRenderer.isVisible)
    //     {
    //         LeaveOwnershipServerRPC(targetID, nowCheck);
    //     }
    //     else
    //     {
    //         JustChangeOwnerShipServerRPC(NetworkManager.Singleton.LocalClientId);
    //     }
    // }
    [ServerRpc(RequireOwnership = false)]
    public void JustChangeOwnerShipServerRPC(ulong targetID, int nowCheck = -1)
    {
        networkObject.ChangeOwnership(targetID);
        SpriteHideClientRPC();
    }
    
    
    //[ClientRpc]
    //public void CheckIsCanChangeOwnerShipClientRpc(ulong targetID,int nowCheck,ClientRpcParams clientRpcParams = default)
    //{
    //    if (!spriteRenderer.isVisible)
    //    {
    //        ChangeOwnerShipServerRPC(targetID, nowCheck); 
    //    }
    //}
}
