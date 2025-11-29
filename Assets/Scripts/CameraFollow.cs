using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    // Drag your player GameObject (the Circle) into this slot in the Inspector
    public Transform target;

    [Header("Settings")]
    // How fast the camera moves toward the target (smaller value = more lag/delay)
    public float smoothSpeed = 5f;
    // The desired distance between the camera and the target
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    // Using LateUpdate ensures the camera moves AFTER the player has moved in Update/FixedUpdate
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Camera target (player) is not set in the Inspector.");
            return;
        }

        // 1. Calculate the desired position
        Vector3 desiredPosition = target.position + offset;

        // 2. Use Linear Interpolation (Lerp) for smooth movement
        // Lerp moves from 'a' (current position) to 'b' (desired position) by 't' (smoothSpeed * Time.deltaTime)
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,     // Current position of the camera
            desiredPosition,        // The player's position + offset
            smoothSpeed * Time.deltaTime // The smoothing factor
        );

        // 3. Apply the smoothed position to the camera's transform
        transform.position = smoothedPosition;
    }
}