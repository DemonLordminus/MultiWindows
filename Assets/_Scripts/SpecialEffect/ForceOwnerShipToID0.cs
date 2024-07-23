using System;
using Unity.Netcode;
using UnityEngine;

public class ForceOwnerShipToID0 : MonoBehaviour
{
    [SerializeField] private ChangeOwnerShipInPlayer _changeOwnerShipInPlayer;
    public static Action<int,bool> OnInteract;
    [SerializeField] private int id = 1;

    
    private void Awake()
    {
        OnInteract += SetEnable;
    }
    private void OnDestroy()
    {
        OnInteract -= SetEnable;
    }

    private void SetEnable(int _id,bool isEnable)
    {
    
        NetdataManager.host.FinalEffectServerRpc();
        
    }

 
}
