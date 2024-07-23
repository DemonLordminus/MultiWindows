using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem;
using Unity.Netcode;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class InputManager : NetworkBehaviour
{
    //[SerializeField] private float _cheapInterpolationTime = 0.1f;
    [SerializeField] private bool isUsingThis = false;
    [SerializeField] bool isUpdate = false;
    public static event Action OnInteract;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Init();
    }
    private void Init()
    {
        if (IsHost)
        {
            if (Instance != null && Instance!=this)
            {
                Debug.LogWarning("input Manager重复");
                Destroy(gameObject);
                return;
            }   
            Instance = this;
            Debug.Log("设置input Manager");
        }
    }
    //private readonly NetworkVariable<InputNetworkData> _netState = new(writePerm: NetworkVariableWritePermission.Owner);

    public static InputManager Instance;    
    public MainInput mainInput;
    //public Vector2 moveContorl;
    private void Awake()
    {
        mainInput = new MainInput();
        NetButton.OnNetDone += () => { isUpdate = true; };
    }
    private void OnEnable()
    {
        mainInput.MainControl.Interaction.started += OnInteractKeyPress;
        mainInput.MainControl.ReSpawn.started += OnRespawnKeyPress;
        mainInput.Accessibility.SceneSwitchToNext.started += OnSceneSwitchNext;
        mainInput.Accessibility.SceneSwitchToPrevious.started += OnSceneSwitchPrevious;
        mainInput.Enable();
    }
    private void OnDisable()
    {
        mainInput.Disable();
        mainInput.MainControl.Interaction.started -= OnInteractKeyPress;
        mainInput.MainControl.ReSpawn.started -= OnRespawnKeyPress;
        mainInput.Accessibility.SceneSwitchToNext.started -= OnSceneSwitchNext;
        mainInput.Accessibility.SceneSwitchToPrevious.started -= OnSceneSwitchPrevious;
    }

    private void OnInteractKeyPress(InputAction.CallbackContext obj)
    {
        if (obj.action.IsPressed())
        {
            OnInteractKeyPressServerRPC(); 
        }
    }
    private void OnRespawnKeyPress(InputAction.CallbackContext obj)
    {
        if (obj.action.IsPressed())
        {
            OnRespawnKeyPressServerRPC(); 
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnRespawnKeyPressServerRPC()
    {
        //OnInteract?.Invoke();
        OnRespawnKeyPressClientRPC();
    }
    [ClientRpc]
    private void OnRespawnKeyPressClientRPC()
    {
        PlayerSetStartPos.SetPlayerToStartPos?.Invoke();
    }
    [ServerRpc(RequireOwnership = false)]
    private void OnInteractKeyPressServerRPC()
    {
        //OnInteract?.Invoke();
        OnInteractKeyPressClientRPC();
    }
    [ClientRpc]
    private void OnInteractKeyPressClientRPC()
    {
        OnInteract?.Invoke();
    }

    private void OnSceneSwitchNext(InputAction.CallbackContext obj)
    {
        // NetworkManager.Singleton.SceneManager.LoadScene(HostDataManager.Instance.sceneNames[(SceneManager.GetActiveScene().buildIndex + 1)],LoadSceneMode.Single);
        // Debug.Log("切换场景弃用");
        NetdataManager.host.LevelStateChange((LevelState)((int)NetdataManager.host.levelState.Value + 1));
    }
    private void OnSceneSwitchPrevious(InputAction.CallbackContext obj)
    {
        // NetworkManager.Singleton.SceneManager.LoadScene(HostDataManager.Instance.sceneNames[(SceneManager.GetActiveScene().buildIndex - 1)], LoadSceneMode.Single);
        // Debug.Log("切换场景弃用");
        NetdataManager.host.LevelStateChange((LevelState)((int)NetdataManager.host.levelState.Value - 1));
    }

    private void Update()
    {
        if (isUsingThis && isUpdate)
        {
            //Debug.Log("test");
            var nowMoveDir = mainInput.MainControl.Move.ReadValue<Vector2>();
            //if (IsHost)
            //{
            //    moveContorl = nowMoveDir;
            //}
            //else
            //{
            MoveDirUpdateServerRPC(nowMoveDir);
            //}

            var nowCameraOffset = mainInput.Accessibility.CameraOffset.ReadValue<Vector2>() * 0.1f;
            if (nowCameraOffset!=Vector2.zero)
            {
                if (IsHost)
                {
                    HostDataManager.Instance.startPoint.Translate(nowCameraOffset);
                    CameraOffsetClientRPC();
                }
                else
                {
                    CameraOffsetUpdateServerRPC(nowCameraOffset);
                } 
            }


        }
    }

    private void OnApplicationFocus(bool focus)
    {
        isUsingThis = focus;
    }
    [ServerRpc(RequireOwnership = false)]
    private void MoveDirUpdateServerRPC(Vector2 newMoveDir)
    {
        //moveContorl = newMoveDir;
        NetdataManager.host.moveControl = newMoveDir;
        //MoveDirUpdateClientRPC(newMoveDir);
    }
    //[ClientRpc]
    //private void MoveDirUpdateClientRPC(Vector2 newMoveDir)
    //{
    //    NetdataManager.host.moveControl = newMoveDir;
    //}

    [ServerRpc(RequireOwnership = false)]
    private void CameraOffsetUpdateServerRPC(Vector2 newOffset)
    {
        HostDataManager.Instance.startPoint.Translate(newOffset);
        CameraOffsetClientRPC();
    }
    [ClientRpc]
    private void CameraOffsetClientRPC()
    {
        SystemManager.Instance.cameraController.CameraUpdate(true);
    }

}

