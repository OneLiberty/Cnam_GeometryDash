using UnityEngine;
using UnityEngine.InputSystem;

public class CubeController : IPlayerMode 
{
    private PlayerController playerController;
    private Rigidbody2D rb;

    private const float baseSpeed = 10.4f; // this is the default speed in GD (10.4 blocks per second)
    private const float baseGravity = 4f;
    private float jumpForce = 14.0f;
    public float speedModifier = 1f;

    public CubeController(PlayerController playerController, Rigidbody2D rb)
    {
        this.playerController = playerController;
        this.rb = rb;
    }


    public void FixedUpdate() 
    {
        rb.linearVelocity = new Vector2(baseSpeed * speedModifier, rb.linearVelocityY);

        if (rb.linearVelocityY < -6.6f) {
            rb.gravityScale = 0f;
        } else {
            rb.gravityScale = baseGravity;
        }
        
        RotateSprite();
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

    private void RotateSprite()
    {
        SpriteRenderer spriteRenderer = rb.GetComponentInChildren<SpriteRenderer>();

        if (!CheckGrounded())
        {
            float rotationAmount = 250f * Time.deltaTime;
            spriteRenderer.transform.Rotate(0, 0, -rotationAmount);
        }
        else // il faudrait un petit timer qu'on reset si on detect un saut pour pas snap tout de suite.
        {
            float currentRotation = spriteRenderer.transform.eulerAngles.z;
            float snappedRotation = Mathf.Round(currentRotation / 90) * 90;
            float smoothedRotation = Mathf.LerpAngle(currentRotation, snappedRotation, 0.25f);

            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, smoothedRotation);
        }
    }

    //Debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position + new Vector2(0.5f, 0), rb.position + new Vector2(0.5f, 0) + Vector2.down * 0.55f);
        Gizmos.DrawLine(rb.position - new Vector2(0.47f, 0), rb.position - new Vector2(0.47f, 0) + Vector2.down * 0.55f); 
    }
        
}