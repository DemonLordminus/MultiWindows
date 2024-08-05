using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PlayerColliderTrigger : MonoBehaviour
{
    [SerializeField] LayerMask playerLayer;
    [SerializeField] GameObject interactObject;
    public UnityEvent<GameObject> OnInteract;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((playerLayer.value & 1 << collision.gameObject.layer) > 0)
        {
            interactObject = collision.gameObject;
            var playerNetworkObject = interactObject.GetComponent<NetworkObject>();
            if (playerNetworkObject.IsOwner)
            {
                OnInteract?.Invoke(interactObject);
            }
        }
    }
}
