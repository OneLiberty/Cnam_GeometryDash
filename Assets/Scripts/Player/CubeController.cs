using UnityEngine;
using UnityEngine.InputSystem;

public class CubeController : IPlayerMode 
{
    private Rigidbody2D rb;

    private const float baseSpeed = 10.4f; // this is the default speed in GD (10.4 blocks per second)
    private float jumpForce = 14.0f;

    public float speedModifier = 1f;

    public CubeController(PlayerController playerController)
    {
        rb = playerController.GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate() 
    {
        rb.linearVelocity = new Vector2(baseSpeed * speedModifier, rb.linearVelocityY);

        if (rb.linearVelocityY < -6.6f) {
            rb.gravityScale = 0f;
        } else {
            rb.gravityScale = 4f;
        }

    }

    public void Update()
    {
        if (InputSystem.GetDevice<Keyboard>().spaceKey.wasPressedThisFrame && CheckGrounded())
        {
            OnClick(new InputValue()); // Simulate click input
        }
    }

    public void OnClick(InputValue value)
    {
        if (CheckGrounded())
        {   
            // Jump
            rb.linearVelocityY = 0;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    private bool CheckGrounded()
    {
        Vector2 frontRayOrigin = rb.position + new Vector2(0.5f, 0);
        Vector2 backRayOrigin = rb.position - new Vector2(0.47f, 0);

        RaycastHit2D frontHit = Physics2D.Raycast(frontRayOrigin, Vector2.down, 0.55f, LayerMask.GetMask("Ground"));
        RaycastHit2D backHit = Physics2D.Raycast(backRayOrigin, Vector2.down, 0.55f, LayerMask.GetMask("Ground"));

        return frontHit.collider != null || backHit.collider != null;
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position + new Vector2(0.5f, 0), rb.position + new Vector2(0.5f, 0) + Vector2.down * 0.55f);
        Gizmos.DrawLine(rb.position - new Vector2(0.47f, 0), rb.position - new Vector2(0.47f, 0) + Vector2.down * 0.55f);
    }
        
}