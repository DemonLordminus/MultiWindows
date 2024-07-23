using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] Animator animator;
    [SerializeField] float maxSpeed, maxGravity;
    [SerializeField] float speedRate;
    [SerializeField] SpriteRenderer playerSprite;

    public float nowSpeed
    {
        get
        {
            return nowSpeedNet.Value;
        }
        set
        {
            nowSpeedNet.Value = value;
        }
    }
    public float nowGravity
    {
        get
        {
            return nowGravityNet.Value;
        }
        set
        {
            nowGravityNet.Value = value;
        }
    }

    public NetworkVariable<float> nowSpeedNet = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> nowGravityNet = new NetworkVariable<float>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public bool isVisable;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    private void FixedUpdate()
    {
        if (IsOwner)
        {
            rb2d.bodyType = RigidbodyType2D.Dynamic;
            
            nowSpeed = rb2d.velocity.x;
            nowGravity = rb2d.velocity.y;
            nowSpeed += NetdataManager.host.moveControl.x * speedRate;
            nowSpeed = Mathf.Clamp(nowSpeed, -maxSpeed, maxSpeed);
            nowGravity = Mathf.Clamp(nowGravity, -maxGravity, maxGravity);
            rb2d.velocity = new Vector2(nowSpeed, nowGravity);
            animator.SetFloat("Speed", Mathf.Abs(nowSpeed));
            animator.SetFloat("Fall", -nowGravity);
            if (transform.localScale.x * nowSpeed < 0 && Mathf.Abs(nowSpeed) > 0.5f)
            {
                transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
            } 
        }
        isVisable = playerSprite.isVisible;
    }
   
}
