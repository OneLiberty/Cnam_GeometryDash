using UnityEngine;

public class ShipController : IPlayerMode
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private ParticleSystem particleSystem;
    private SpriteRenderer spriteRenderer;

    private const float baseSpeed = 10.4f; // this is the default speed in GD (10.4 blocks per second)
    private const float baseGravity = 4f;

    public float speedModifier = 1f;

    public ShipController(PlayerController playerController, Rigidbody2D rb)
    {
        this.playerController = playerController;
        this.rb = rb;
    }

    public void Initialize(GameObject characterInstance) 
    {
        particleSystem = characterInstance.GetComponentInChildren<ParticleSystem>();
        spriteRenderer = characterInstance.transform.Find("SpriteRenderer").GetComponent<SpriteRenderer>();
    }

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

        AlignToDirection();
    }

    public void Update()
    {
        if(!playerController.isButtonPressed)
        {
            rb.gravityScale = baseGravity;
            particleSystem.Stop(true);
        } 
        else
        {
            particleSystem.Play();
        }
    }

    public void OnClick()
    {
        rb.gravityScale = -baseGravity;
    }

    private void AlignToDirection() 
    {
        float angle = Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

