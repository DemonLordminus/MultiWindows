using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class CreateForClient : MonoBehaviour
{
    [SerializeField] private List<ulong> clientsID;

    //[SerializeField] private bool isJustSetFalse;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [EnumToggleButtons] [SerializeField] CreateOption option;

    [ShowIf("option", CreateOption.SetSprite)] [SerializeField]
    private Sprite newSprite;

    [ShowIf("option", CreateOption.SetColor)] [SerializeField]
    private Color newColor;

    [ShowIf("option", CreateOption.SetActive)] [SerializeField]
    private GameObject targetObject;

    [ShowIf("option", CreateOption.SetActive)] [SerializeField]
    private bool isActive;
    [FormerlySerializedAs("targeComponent")] [ShowIf("option", CreateOption.DestroyComponent)] [SerializeField]
    private Component targetComponent;
    [SerializeField] private bool isOnlyFor;
    

    
    private void OnEnable()
    {
        try
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("CreateForClient启动常规报错");
        }
    }
    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent; 
        }
    }
    private void OnSceneEvent(SceneEvent sceneEvent)
    {
        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.Load:
                break;
            case SceneEventType.Unload:
                break;
            case SceneEventType.Synchronize:
                break;
            case SceneEventType.ReSynchronize:
                break;
            case SceneEventType.LoadEventCompleted:
                break;
            case SceneEventType.UnloadEventCompleted:
                break;
            case SceneEventType.LoadComplete:
                if(spriteRenderer==null)
                {
                    spriteRenderer = GetComponent<SpriteRenderer>();
                }
                if (clientsID.Contains(NetworkManager.Singleton.LocalClientId) == isOnlyFor)
                {
                    HandleOnSceneLoad();
                }
                break;
            case SceneEventType.UnloadComplete:
                break;
            case SceneEventType.SynchronizeComplete:
                break;
            case SceneEventType.ActiveSceneChanged:
                break;
            case SceneEventType.ObjectSceneChanged:
                break;
        }
    }
    private void DestroyForID()
    {    
        Destroy(gameObject);
    }

    private void SetActive()
    {
        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        targetObject.SetActive(isActive);
        
    }
    private void SetSprite()
    {
        
         spriteRenderer.sprite = newSprite;
        
    }
    private void SetColor()
    {
       
        spriteRenderer.color = newColor;
        
    }
    
    private void DestroyComponent()
    {
        Destroy(targetComponent);

    }
    private void HandleOnSceneLoad()
    {
        switch (option)
        {
            case CreateOption.DestroyThis:
                DestroyForID();
                break;
            case CreateOption.SetActive:
                SetActive();
                break;
            case CreateOption.SetColor:
                SetColor();
                break;
            case CreateOption.SetSprite:
                SetSprite();
                break;
            case CreateOption.Empty:
                Debug.LogError("未设置类型");
                break;
            case CreateOption.DestroyComponent:
                DestroyComponent();
                break;

        }
    }

}
public enum CreateOption
{ 
    Empty,
    DestroyThis,
    SetActive,
    SetColor,
    SetSprite,
    //SetTrue,
    DestroyComponent,
    InvokeUnityEvent,
}