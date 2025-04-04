using System.Collections;
using UnityEngine;

public class EndPortal : InteractiveObject
{
    [Header("End Portal Settings")]
    [SerializeField] private float attractionRadius = 6f;
    [SerializeField] private float attractionSpeed = 20f;

    private PlayerController player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.GetComponentInParent<PlayerController>();
            Interact(player);
        }
    }

    public override void Interact(PlayerController player)
    {
        player.rb.linearVelocity = Vector2.zero;
        player.rb.simulated = false;

        StartCoroutine(AttractPlayer(player));
    }

    /// This is a coroutine that handles the attraction of the player towards the portal.
    /// circular motion is calculated using sine and cosine functions.
    /// This is completly unecessary, and i wasted too much time on this.
    private IEnumerator AttractPlayer(PlayerController player)
    {
        float initialTime = Time.time;
        Vector2 targetPosition = transform.position;

        SpriteRenderer spriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        ParticleSystem particleSystem = player.GetComponentInChildren<ParticleSystem>();

        particleSystem.Stop(true);

        while (Vector2.Distance(player.transform.position, targetPosition) > 0.1f)
        {

            float xPos = Mathf.Cos((Time.time - initialTime) * 2f) * attractionRadius;
            float yPos = Mathf.Sin((Time.time - initialTime) * 2f) * attractionRadius;

            player.transform.position = Vector2.MoveTowards(
                player.transform.position,
                new Vector2(targetPosition.x + xPos, targetPosition.y + yPos),
                attractionSpeed * Time.deltaTime
            );

            player.transform.localScale = Vector2.Lerp(player.transform.localScale, new Vector2(0.01f, 0.01f), Time.deltaTime * 0.3f);

            attractionRadius -= Time.deltaTime * 1.2f; // Decrease the radius over time
            yield return null;
        }

        spriteRenderer.enabled = false;
    }
}