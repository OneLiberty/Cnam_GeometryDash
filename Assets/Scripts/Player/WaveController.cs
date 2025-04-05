using UnityEngine;
using UnityEngine.InputSystem;

public class WaveController : IPlayerMode
{
    private PlayerController playerController;
    private Rigidbody2D rb;

    private const float baseSpeed = 10.4f;
    private const float baseGravity = 0f; // Wave mode has no gravity

    private float _movementDirection = -1f; // 1 for up, -1 for down 
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
        Vector2 movement = baseSpeed * speedModifier * Time.deltaTime * new Vector2(1, 1);
        movement.y *= _movementDirection;

        playerController.transform.Translate(movement);
    }

    public void Update()
    {
    }

    public void OnClick()
    { 
        if (playerController.isButtonPressed)
        {
            _movementDirection = -1;
        } 
        else
        {
            _movementDirection = 1;
        }
    }
}


