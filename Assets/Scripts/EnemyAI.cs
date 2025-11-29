using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target & Detection")]
    [Tooltip("The tag of the player object (e.g., 'Player').")]
    public string playerTag = "Player";
    [Tooltip("How far the enemy can see the player before starting to follow.")]
    public float detectionRange = 15f;
    [Tooltip("How close the enemy must be to stop moving and start attacking.")]
    public float attackRange = 1.5f;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;

    [Header("Rotation Settings")]
    [Tooltip("Adjust this if the enemy is facing the wrong way. Try 90 or -90.")]
    public float rotationOffset = -90f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public int damageAmount = 10;

    // --- Private Variables ---
    private Transform playerTransform;
    private Rigidbody2D rb;
    private float timeUntilNextAttack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Find the player object by tag once at start
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player object with tag '" + playerTag + "' not found!");
            enabled = false;
        }

        timeUntilNextAttack = Time.time;
    }

    void FixedUpdate()
    {
        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Player is in range: Engage
            MoveAndAttack(distanceToPlayer);
        }
        else
        {
            // Player is out of range: Stop moving
            rb.velocity = Vector2.zero;
        }
    }

    private void MoveAndAttack(float distanceToPlayer)
    {
        // 1. Calculate direction to the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;

        if (distanceToPlayer > attackRange)
        {
            // 2. Move towards the player
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // 3. Attack the player
            rb.velocity = Vector2.zero; // Stop moving
            TryAttack();
        }

        // Optional: Make the enemy face the player (using the Z-axis rotation)
        RotateTowardsTarget(direction);
    }

    private void RotateTowardsTarget(Vector2 direction)
    {
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // adjust the angle 
        targetAngle += rotationOffset;
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private void TryAttack()
    {
        if (Time.time >= timeUntilNextAttack)
        {
            // Trigger the attack animation/logic here (e.g., a function call)
            // For now, we'll use a direct damage application.
            Debug.Log(gameObject.name + " ATTACKED " + playerTransform.gameObject.name);

            // Assuming the Player has a Health script
            Health playerHealth = playerTransform.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Set the next attack time
            timeUntilNextAttack = Time.time + attackCooldown;
        }
    }
}