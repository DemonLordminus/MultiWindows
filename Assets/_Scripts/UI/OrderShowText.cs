using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class OrderShowText : NetworkBehaviour
{
    [SerializeField] private Canvas UIFather;
    [SerializeField] bool isUse;
    [SerializeField] TextMeshProUGUI nowOrderText;
    [SerializeField] TextMeshProUGUI OrderListText;
    [SerializeField] TextMeshProUGUI IDText;
    [SerializeField] private TextMeshProUGUI IDName;
    
    [SerializeField] TextMeshProUGUI PlayerIDText;
    [SerializeField] private TextMeshProUGUI PlayerIDTextName;
    private void Awake()
    {
        if (isUse)
        {
            NetdataManager.OnOrderUpdate += UpdateOrderList;
            NetdataManager.OnOrderUpdate += UpdateNowOrder;

    
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        IDName.text = "窗口编号：";
        if(!IsOwner) 
        {
            nowOrderText.gameObject.SetActive(false);
            IDText.gameObject.SetActive(false);
            IDName.gameObject.SetActive(false);
            PlayerIDText.gameObject.SetActive(false);
            PlayerIDTextName.gameObject.SetActive(false);
        }
        else
        {
            NetdataManager.OnGameAnyStateUpdate += DataUpdate;
        }
        if(!IsHost)
        {
            OrderListText.gameObject.SetActive(false);
            NetdataManager.OnOrderUpdate -= UpdateOrderList;
        }
        else
        {
            IDText.gameObject.SetActive(false);
            IDName.gameObject.SetActive(false);
            PlayerIDText.gameObject.SetActive(false);
            PlayerIDTextName.gameObject.SetActive(false);
        }
        // IDText.text = OwnerClientId.ToString();

    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        OrderListText.gameObject.SetActive(true);
        if (isUse)
        {
            NetdataManager.OnOrderUpdate += UpdateOrderList;
        }
    }

    public void DataUpdate()
    {
        IDText.text = NetdataManager.instance.GameId.Value.ToString();
        PlayerIDText.text = NetdataManager.host.NowPlayerOwnerGameID.Value.ToString();
    }
    private void UpdateNowOrder()
    {
        nowOrderText.text = $"{NetdataManager.instance.GetNowOrder()}";
    }
    private void UpdateOrderList()
    {
        var orderList = NetdataManager.instance.clientOrder;
        string listString = string.Empty;
        for (int i = 0; i < orderList.Count; i++)
        {
            listString += orderList[i].ToString();
            listString += "\n";
        }
        OrderListText.text = listString;
    }

    private void Update()
    {
        // if (!IsHost && IsOwner)
        // {
        //     if (GlobalInput.GetKeyDown(GlobalKeyCode.TAB) || Input.GetKeyDown(KeyCode.Tab))
        //     {
        //         // UIFather.gameObject.SetActive(true);
        //         PlayerIDTextName.gameObject.SetActive(true);
        //     }
        //
        //     if (GlobalInput.GetKeyUp(GlobalKeyCode.TAB) || Input.GetKeyUp(KeyCode.Tab))
        //     {
        //         // UIFather.gameObject.SetActive(false);
        //         PlayerIDTextName.gameObject.SetActive(false);
        //     }
        // }
    }
}
