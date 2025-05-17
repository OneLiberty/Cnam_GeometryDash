using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float smoothTimeX = 0.3f;
    [SerializeField] private float smoothTimeY = 0.2f;
    [SerializeField] private float startFollowingX = -6f;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 startingPosition;

    [Header("Parallax Settings")]
    [SerializeField] private float parallaxFactor = 0.9f;
    [SerializeField] private Transform backgroundTransform;

    private const float DefaultCameraZoom = 5.5f;

    private Vector3 velocity = Vector3.zero;
    private float lowestY = 2.5f;
    private Vector3 lastPosition;
    private Vector2 endPosition;

    private bool isRestarting = false;

    private void Start()
    {
        startingPosition = lastPosition = transform.position;

        GameObject endPortal = GameObject.Find("EndPortal(Clone)");
        if (endPortal != null)
        {
            endPosition = endPortal.transform.position;
        }
        else
        {
            endPosition = new Vector2(1000f, 0f); // default value if not found
        }

        Camera.main.orthographicSize = DefaultCameraZoom;
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        PlayerController playerController = player.GetComponent<PlayerController>();
        Vector3 targetPosition = transform.position;

        if (playerController.isDead)
        {
            if (!isRestarting) { 
                // if player is dead, move the camera to the starting position
                isRestarting = true;
                Invoke(nameof(HandlePlayerDeath), 1f);
                return;
            }
            return;
        }
        isRestarting = false;
        
        // follow only if player is beyond startFollowingX and stop when beyond endPosition
        if (player.position.x >= endPosition.x - offsetX)
        {
            targetPosition.x = endPosition.x;
            targetPosition.y = endPosition.y;

            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5f, Time.deltaTime * 0.5f);
        }
        else if (player.position.x > startFollowingX)
        {
            targetPosition.x = player.position.x + offsetX;

            float targetY = Mathf.Max(player.position.y, lowestY);
            targetPosition.y = targetY;
        }

        // move the camera to the target position, smoothed with the given time 
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            new Vector2(smoothTimeX, smoothTimeY).magnitude
        );

        if (backgroundTransform != null)
        {
            Vector3 parallaxMovement = transform.position - lastPosition;
            backgroundTransform.position += new Vector3(parallaxMovement.x * parallaxFactor, parallaxMovement.y * parallaxFactor, 0);
        }

        lastPosition = transform.position;
    }

    private void HandlePlayerDeath()
    {
        transform.position = startingPosition;
        Camera.main.orthographicSize = DefaultCameraZoom;
    }

}