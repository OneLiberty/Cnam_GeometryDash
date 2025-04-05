using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : IPlayerMode
{
    private PlayerController playerController;
    private Rigidbody2D rb;

    private const float baseSpeed = 10.4f; // this is the default speed in GD (10.4 blocks per second)
    private const float baseGravity = 4f;

    public float speedModifier = 1f;

    public ShipController(PlayerController playerController, Rigidbody2D rb)
    {
        this.playerController = playerController;
        this.rb = rb;
    }

    public void Initialize(GameObject characterInstance) { }

    public void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(baseSpeed * speedModifier, rb.linearVelocityY);

        // limiting y velocity to prevent excessive speed
        if (rb.linearVelocityY > 10f)
        {
            rb.linearVelocityY = 10f;
        }
        else if (rb.linearVelocityY < -10f)
        {
            rb.linearVelocityY = -10f;

        }
    }

    public void Update()
    {
    }

    public void OnClick()
    {
        if (playerController.isButtonPressed)
        {
            rb.gravityScale = -baseGravity;
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
       
    }
}

