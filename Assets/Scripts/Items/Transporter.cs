using System.Collections.Generic;
using UnityEngine;

public abstract class Transporter : InteractiveObject
{
    protected enum TransporterType
    {
        Pad,
        Orb
    }

    protected enum JumpForce
    {
        Low,
        Normal,
        High
    }

    [SerializeField] protected TransporterType transporterType;
    [SerializeField] protected JumpForce jumpForceType;
    protected float jumpForce;

    protected Dictionary<JumpForce, float> jumpForcesMap = new Dictionary<JumpForce, float>()
    {
        { JumpForce.Low, 24f },
        { JumpForce.Normal, 30f },
        { JumpForce.High, 35f }
    };

    protected virtual void Awake()
    {
        if (jumpForcesMap.ContainsKey(jumpForceType))
        {
            jumpForce = jumpForcesMap[jumpForceType];
        }
    }

    protected void ApplyJumpForce(PlayerController player) {
        
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocityY = 0f; 

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}