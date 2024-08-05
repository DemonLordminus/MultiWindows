using System;
using Unity.Netcode;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class NetBoxCollider2D : NetworkBehaviour
{
    public BoxCollider2D boxCollider2D;

    public NetworkVariable<Vector2> boxColliderSize = new NetworkVariable<Vector2>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector2> boxColliderOffset = new NetworkVariable<Vector2>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    // public NetworkVariable<bool> isEnableCollider = new NetworkVariable<bool>(false,
    // NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void OnEnable()
    {
        boxColliderSize.OnValueChanged += OnColliderSizeChange;
        boxColliderOffset.OnValueChanged += OnColliderOffsetChange;
        // isEnableCollider.OnValueChanged += OnColliderEnableChange;
    }

    private void OnDisable()
    {
        boxColliderSize.OnValueChanged -= OnColliderSizeChange;
        boxColliderOffset.OnValueChanged -= OnColliderOffsetChange;
        // isEnableCollider.OnValueChanged -= OnColliderEnableChange;
    }

    public void OnColliderSizeChange(Vector2 previous,Vector2 current)
    {
        boxCollider2D.size = current;
    }
    public void OnColliderOffsetChange(Vector2 previous,Vector2 current)
    {
        boxCollider2D.offset = current;
    }
    public void OnColliderEnableChange(bool previous,bool current)
    {
        boxCollider2D.enabled = current;
    }
}
