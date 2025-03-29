using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField] private float offsetX = 10f;
    [SerializeField] private float smoothTimeX = 0.3f;
    [SerializeField] private float smoothTimeY = 0.2f;
    [SerializeField] private float startFollowingX = -6f;
    
    private Vector3 velocity = Vector3.zero;
    private float lowestY = 5f;

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = transform.position;

        // follow only if player is beyond startFollowingX
        if (player.position.x > startFollowingX)
        {
            targetPosition.x = player.position.x + offsetX;
        }

        float targetY =  Mathf.Max(player.position.y, lowestY);
        targetPosition.y = targetY;

        // move the camera to the target position, smoothed with the given time 
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            new Vector2(smoothTimeX, smoothTimeY).magnitude
        );

    }
}