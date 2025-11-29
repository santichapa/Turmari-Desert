using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Tooltip("The transform holding all the player sprites that should rotate.")]
    public Transform visualRoot;

    [Tooltip("Speed at which the player rotates to face the new direction (degrees/sec).")]
    public float rotationSpeed = 720f;
    [Tooltip("Rotation offset to correct sprite orientation.")]
    public float rotationOffset = 90f; // Often 90 degrees is needed for mouse-look

    private Rigidbody2D rb;
    private float moveX;
    private float moveY;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject. Please add one for movement.");
            enabled = false;
        }
    }

    void Update()
    {
        // 1. Get Input (Done in Update for snappier movement input)
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // 1. Apply Movement (Physics)
        Vector2 moveDirection = new Vector2(moveX, moveY);
        rb.velocity = moveDirection.normalized * moveSpeed;

        // 2. Apply Rotation (Mouse Look)
        RotateToMouse();
    }

    private void RotateToMouse()
    {
        if (visualRoot == null) return;

        // Convert the mouse position from screen space to world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0; // Ensure Z is 0 for 2D calculations
        Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        // Calculate the direction vector from the player to the mouse
        Vector2 direction = (worldMousePosition - visualRoot.position).normalized;

        // Calculate the angle (0 degrees = Right)
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the necessary offset and create the target rotation Quaternion
        targetAngle += rotationOffset;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);

        // Smoothly rotate the visual root
        visualRoot.rotation = Quaternion.RotateTowards(
            visualRoot.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }
}