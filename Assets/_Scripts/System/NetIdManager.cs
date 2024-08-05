using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

// 只有host的netIdManager算数
public class NetIdManager : Singleton<NetIdManager>
{
    public bool isNextSpecial;
    public NetIdSpecialData nextData;
    public bool[] existId = new bool[100];
    public ulong[] idRefrenceClientID = new ulong[100];
    public bool isEnable = false;
    public bool isOnlyHostExist => NetworkManager.Singleton.ConnectedClientsList.Count == 1;
    private void Start()
    {
        existId = new bool[100];
        idRefrenceClientID = new ulong[100];
        for (int i = 0; i < existId.Length; i++)
        {
            existId[i] = false;
            idRefrenceClientID[i] = 100;
        }
        Debug.Log("id初始化");
        if (NetworkManager.Singleton.IsHost)
        {
            isEnable = true;
        }
    }

    public struct NetIdSpecialData
    {
        
    }
     
    public int GetNextId(ulong ownerClientID)
    {
        if(!isEnable) Debug.LogError("非Host，不应启用");
        if (!isNextSpecial)
        {
            for (int i = 0; i < existId.Length; i++)
            {
                if (existId[i] == false)
                {
                    existId[i] = true;
                    
                    Debug.Log($"分配id:{ownerClientID} to {i}");
                    idRefrenceClientID[i] = ownerClientID;
                    return i;
                }
            }
        }

        
        return -1;
    }

    public ulong GetOneExistClientID()
    {
        if (isOnlyHostExist) return 0;
        
        for (int i = 1; i < existId.Length; i++)
        {
            if (existId[i])
            {
                return idRefrenceClientID[i];
            }
        }

        return 0;
    }
}
