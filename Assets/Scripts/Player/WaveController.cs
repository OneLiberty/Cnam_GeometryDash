using UnityEngine;

public class WaveController : IPlayerMode
{
    private PlayerController playerController;
    private Rigidbody2D rb;

    private const float baseSpeed = 10.4f;
    private const float baseGravity = 0f; // Wave mode has no gravity

    private float _movementDirection = 1f; // 1 for up, -1 for down 
    public float speedModifier = 1f;

    public WaveController(PlayerController playerController, Rigidbody2D rb)
    {
        this.rb = rb;
        this.playerController = playerController;
        rb.gravityScale = baseGravity;
    }

    public void Initialize(GameObject characterInstance) {
    }

    public void FixedUpdate()
    {
        rb.linearVelocityX = baseSpeed * speedModifier;
        rb.linearVelocityY = _movementDirection * baseSpeed * speedModifier;
    }

    public void Update()
    {
        if (!playerController.isButtonPressed)
        {
            _movementDirection = -1;
        }
    }

    public void OnClick()
    { 
        if (playerController.isButtonPressed)
        {
            _movementDirection = 1;
        } 
    }
}


