using UnityEngine;

public class WaveController : IPlayerMode
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

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
        rb.gravityScale = baseGravity;
        playerController.transform.rotation = Quaternion.Euler(0, 0, 0);
        characterInstance.transform.rotation = Quaternion.Euler(0, 0, 0);
        spriteRenderer = characterInstance.transform.Find("SpriteRenderer").GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        rb.linearVelocityX = baseSpeed * speedModifier;
        rb.linearVelocityY = _movementDirection * baseSpeed * speedModifier;
        AlignToDirection();
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

    private void AlignToDirection() 
    {
        float angle = Mathf.Atan2(rb.linearVelocityY, rb.linearVelocityX) * Mathf.Rad2Deg;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}


