using UnityEngine;

public class Portal : MonoBehaviour
{
    public PlayerController.GameMode targetMode;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponentInParent<PlayerController>().ChangeGameMode(targetMode);
        }
    }

    // I could add visual and audio effects here
}
 