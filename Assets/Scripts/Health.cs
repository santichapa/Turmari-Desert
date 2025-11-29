using UnityEngine;

public class Health : MonoBehaviour
{
    // Fields visible in the Inspector for configuration
    [Header("Health Settings")]
    public float maxHealth = 50f;

    private float currentHealth;

    // Optional: Event for external scripts (like UI or Game Manager) to subscribe to
    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;

    void Start()
    {
        // Initialize current health to max health
        currentHealth = maxHealth;
        // Notify any subscribers that the health is initialized
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Reduces the current health by the specified amount.
    /// </summary>
    /// <param name="damageAmount">The amount of damage to inflict.</param>
    public void TakeDamage(float damageAmount)
    {
        // Don't take damage if already dead or the amount is invalid
        if (currentHealth <= 0 || damageAmount <= 0)
            return;

        // Reduce health
        currentHealth -= damageAmount;

        // Clamp health to ensure it doesn't go below zero
        currentHealth = Mathf.Max(currentHealth, 0f);

        // Notify subscribers (e.g., a health bar UI script)
        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Current Health: " + currentHealth);

        // Check for death condition
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Restores the current health by the specified amount.
    /// </summary>
    /// <param name="healAmount">The amount of health to restore.</param>
    public void Heal(float healAmount)
    {
        // Don't heal if amount is invalid
        if (healAmount <= 0)
            return;

        // Increase health
        currentHealth += healAmount;

        // Clamp health to ensure it doesn't exceed max health
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        // Notify subscribers
        OnHealthChanged?.Invoke(currentHealth);

        Debug.Log(gameObject.name + " healed " + healAmount + ". Current Health: " + currentHealth);
    }

    /// <summary>
    /// Handles the object's death logic (e.g., destroy, animation, game over).
    /// </summary>
    private void Die()
    {
        // Check if this object is the Player
        if (gameObject.CompareTag("Player"))
        {
            // Call the Game Manager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }

            // destroying the player object instantly breaks the camera if it follows the player so we make it dissapear.
            gameObject.SetActive(false);
        }
        else
        {
            // It's an enemy
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(10); // Add score when enemy dies
            }
            Destroy(gameObject);
        }
    }
}