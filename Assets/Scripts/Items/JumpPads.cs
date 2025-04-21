using System.Collections.Generic;
using UnityEngine;

public class JumpPad : InteractiveObject
{
    [SerializeField] private string jumpPadType = "NormalPad"; // Default jump pad type

    public Dictionary<string, float> JumpPads = new Dictionary<string, float> {
        { "LowPad", 24f }, // Lower jump height
        { "NormalPad", 30f }, // Regular jump height
        { "HighPad", 35f } // Higher jump
    };

    public override void Interact(PlayerController player) 
    {
        if (player == null) return;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        rb.linearVelocityY = 0f; 

        rb.AddForce(Vector2.up * JumpPads[jumpPadType], ForceMode2D.Impulse);

    }
}