using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetButton : MonoBehaviour
{
    private float nowTime = 10f;
    public static event Action OnNetDone,OnClientDone,OnHostDone;
    [SceneName] public string firstScene;
    private async void Start()
    {
        DontDestroyOnLoad(this);
        if(NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost)
        {
            return;
        }
        await Task.Yield();
        TestServer();

    }
    [EditorButton]
    private async void TestServer()
    {
        NetworkManager.Singleton.StartClient();
        // if(Application.isEditor)
        // {
        //     await Task.Delay(1000);
        // }
        // else
        // {
        //     await Task.Delay(2000);
        // }
        await UniTask.Delay(500);
        // 判断当前是否有主机
        if (NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkManager.Singleton.OnClientStarted += SceneChange;

            Debug.Log("HasServer");
            OnClientDone?.Invoke();
            OnNetDone?.Invoke();
            // Destroy(gameObject);
        }
        else
        {
            Debug.Log("NoServer");
            NetworkManager.Singleton.OnServerStarted += SceneChange;
            
            await TestHost();
            OnHostDone?.Invoke();
            
            //NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
            //// 自动连接到主机
            //Debug.Log("Running as Client");
        }
      
    }
    // public override void OnNetworkSpawn()
    // {
    //     base.OnNetworkSpawn();
    //     //SpawnDataManager();
    // }
    //private void SpawnDataManager()
    //{
    //    SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
    //}
    //[ServerRpc(RequireOwnership = false)]
    //private void SpawnPlayerServerRpc(ulong playerId)
    //{
    //    var spawn = Instantiate(netdataManager);
    //    spawn.NetworkObject.SpawnAsPlayerObject(playerId);
    //}
    private void SceneChange()
    {
        //if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsServer)
        //{
        
            NetworkManager.Singleton.SceneManager.LoadScene(firstScene, LoadSceneMode.Single);
            Destroy(gameObject);
        
            //}
    }



    private async UniTask TestHost()
    {
        nowTime = 10f;
        NetworkManager.Singleton.Shutdown();
        while (nowTime>0)
        {
            nowTime-= Time.deltaTime;
            if(!NetworkManager.Singleton.ShutdownInProgress)
            {
                break;
            }
            await Task.Yield();
        }

   
        OnNetDone?.Invoke();
        NetworkManager.Singleton.StartHost();
        
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.OnServerStarted -= SceneChange;
        }
        else
        {
            NetworkManager.Singleton.OnClientStarted -= SceneChange;
        }
    }
    // void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(10, 10, 300, 300));
    //     if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    //     {
    //         StartButtons();
    //     }
    //     else
    //     {
    //         StatusLabels();
    //     }
    //
    //     GUILayout.EndArea();
    // }

    static void StartButtons()
    {
        //if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
        //if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
        //if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
    }

    static void StatusLabels()
    {
        //var mode = NetworkManager.Singleton.IsHost ?
        //    "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        //GUILayout.Label("Transport: " +
        //    NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        //GUILayout.Label("Mode: " + mode);
    }

}



