using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
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

    [ShowIf("option", CreateOption.SetTrue)] [SerializeField]
    private GameObject targetObject;
    [FormerlySerializedAs("targeComponent")] [ShowIf("option", CreateOption.DestroyComponent)] [SerializeField]
    private Component targetComponent;
    [SerializeField] private bool isOnlyFor;
    

    
    private void OnEnable()
    {
        if(spriteRenderer==null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (clientsID.Contains(NetdataManager.instance.GameId.Value) == isOnlyFor)
        {
            HandleOnSceneLoad();
        }
    }
    

    private void DestroyForID()
    {    
        Destroy(gameObject);
    }
    private void SetFalseForNonID()
    {
        
         gameObject.SetActive(false);
        
    }
    private void SetSprite()
    {
        
         spriteRenderer.sprite = newSprite;
        
    }
    private void SetColor()
    {
       
        spriteRenderer.color = newColor;
        
    }

    private void SetTure()
    {
        targetObject.SetActive(true);

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
            case CreateOption.JustSetFalse:
                SetFalseForNonID();
                break;
            case CreateOption.SetColor:
                SetColor();
                break;
            case CreateOption.SetSprite:
                SetSprite();
                break;
            case CreateOption.SetTrue:
                SetTure();
                break;
            case CreateOption.empty:
                break;
            case CreateOption.DestroyComponent:
                DestroyComponent();
                break;

        }
    }

}
