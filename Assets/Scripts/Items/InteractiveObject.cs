using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player == null) return;
            Interact(player);
        }
    }

    public abstract void Interact(PlayerController player);
}