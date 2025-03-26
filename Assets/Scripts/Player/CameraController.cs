using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float offsetX = 5f;
    [SerializeField] private float smoothTime = 0.3f;

    private float startFollowingX = -6f;
    
    private Vector3 velocity = Vector3.zero;
    
    void FixedUpdate()
    {
        if (player == null) return;

        if (player.position.x > startFollowingX)
        {
            Vector3 targetPosition = new Vector3(player.position.x + offsetX, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
}