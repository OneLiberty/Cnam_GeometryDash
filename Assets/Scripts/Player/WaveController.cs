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

    public void FixedUpdate() 
    {
        rb.linearVelocity = new Vector2(baseSpeed * speedModifier, baseSpeed * speedModifier * _movementDirection);
    }

    public void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().spaceKey.isPressed) { // temporarily using space key for testing
            OnClick(new InputValue()); // Simulate click input
        } else {
            _movementDirection = -1f; // Default to moving down
        }
    }

    public void OnClick(InputValue value)
    {
        _movementDirection = 1f;
    }
}
        
        