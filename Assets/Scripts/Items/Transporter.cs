using System.Collections.Generic;
using UnityEngine;

public abstract class Transporter : InteractiveObject {

    [SerializeField] protected float jumpForce; 
    [SerializeField] protected string transporterType;

    protected Dictionary<string, float> jumpForcesMap = new Dictionary<string, float>();

    protected void ApplyJumpForce(PlayerController player) {
        
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocityY = 0f; 

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}