using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CreateForGameID : MonoBehaviour
{
     [SerializeField] private List<int> clientsID;

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
    [ShowIf("option", CreateOption.InvokeUnityEvent)]
    [SerializeField] private UnityEvent _unityEvent;

    
    private async void OnEnable()
    {
        if(spriteRenderer==null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        await UniTask.Yield();
        try
        {
            if (clientsID.Contains(NetdataManager.instance.GameId.Value) == isOnlyFor)
            {
                HandleOnSceneLoad();
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogWarning("CreateForGameID启动常规报错");
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
            case CreateOption.InvokeUnityEvent:
                _unityEvent?.Invoke();
                break;
            default:
                Debug.LogError("未设置类型");
                break;
        }
    }

}
