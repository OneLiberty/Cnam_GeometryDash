using UnityEngine;

public class CubeController : IPlayerMode
{
    private PlayerController playerController;
    private GameObject characterInstance;
    private Rigidbody2D rb;
    private ParticleSystem particleSystem;
    private SpriteRenderer spriteRenderer;

    private const float baseSpeed = 10.4f; // this is the default speed in GD (10.4 blocks per second)
    private const float baseGravityScale = 12.41067f;
    private bool isGrounded = false;
    public float jumpForce = 26.6581f;
    public float speedModifier = 1f;

    public CubeController(PlayerController playerController, Rigidbody2D rb)
    {
        this.playerController = playerController;
        this.rb = rb;
        rb.gravityScale = baseGravityScale;
    }

    public void Initialize(GameObject characterInstance)
    {
        this.characterInstance = characterInstance;
        particleSystem = characterInstance.GetComponentInChildren<ParticleSystem>();
        spriteRenderer = characterInstance.transform.Find("SpriteRenderer").GetComponent<SpriteRenderer>();
        rb.gravityScale = baseGravityScale;

        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }

    public void FixedUpdate()
    {
        rb.linearVelocityX= baseSpeed * speedModifier;
        // if (rb.linearVelocityY < -13.2f)
        // {
        //     rb.gravityScale = 0f;
        // }
        // else
        // {
        //     rb.gravityScale = baseGravityScale;
        // }

        if (!particleSystem.isPlaying && CheckGrounded())
        {
            particleSystem.Play();
        }

        RotateSprite();
    }

    public void Update()
    {
        isGrounded = CheckGrounded();
    }

    public void OnClick()
    {
        if (isGrounded && playerController.isButtonPressed)
        {
            isGrounded = false;
            // Jump
            rb.linearVelocityY = jumpForce;
            
            if (particleSystem.isPlaying) {
                particleSystem.Stop(true);
            }
        }
    }

    private bool CheckGrounded()
    {
        Vector2 frontRayOrigin = rb.position + new Vector2(0.5f, 0);
        Vector2 backRayOrigin = rb.position - new Vector2(0.47f, 0);

        RaycastHit2D frontHit = Physics2D.Raycast(frontRayOrigin, Vector2.down, 0.52f, LayerMask.GetMask("Ground"));
        RaycastHit2D backHit = Physics2D.Raycast(backRayOrigin, Vector2.down, 0.52f, LayerMask.GetMask("Ground"));

        return frontHit.collider != null || backHit.collider != null;
    }

    private void RotateSprite()
    {
        if (!CheckGrounded())
        {
            float rotationAmount = 360 * Time.deltaTime;
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
    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rb.position + new Vector2(0.5f, 0), rb.position + new Vector2(0.5f, 0) + Vector2.down * 0.55f);
        Gizmos.DrawLine(rb.position - new Vector2(0.47f, 0), rb.position - new Vector2(0.47f, 0) + Vector2.down * 0.55f);
    }

}