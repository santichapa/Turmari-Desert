using UnityEngine;
using System.Collections.Generic;

public class DamageOnContact : MonoBehaviour
{
    [Header("Attack Settings")]
    public float damageAmount = 10f;
    public bool destroyOnHit = false;

    [Tooltip("Control flag: Damage is only applied when this is TRUE.")]
    public bool isDamageActive = false;

    // List to track objects already hit during one attack swing
    private List<GameObject> damagedTargets = new List<GameObject>();

    public void StartAttack()
    {
        isDamageActive = true;
        damagedTargets.Clear(); // Forgot who we hit last swing
    }

    public void EndAttack()
    {
        isDamageActive = false;
    }

    // Fired when the sword ENTERS the enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    // Shared logic to avoid repetition
    private void TryDamage(Collider2D other)
    {
        // 1. Safety Checks
        if (!isDamageActive) return; // Not attacking

        // Get the top-level parent (the Player object) for both the weapon and the collision object.
        Transform myRoot = transform.root;
        Transform otherRoot = other.transform.root;

        // If the roots are the same, it's the player attacking themselves.
        if (myRoot == otherRoot)
        {
            return; // Don't deal damage to self!
        }

        if (damagedTargets.Contains(other.gameObject)) return; // Already hit this swing

        // 2. Check for Health
        Health targetHealth = other.gameObject.GetComponent<Health>();

        if (targetHealth != null)
        {
            // 3. Deal Damage
            targetHealth.TakeDamage(damageAmount);

            // 4. Mark as hit so OnTriggerStay doesn't hit them 60 times a second
            damagedTargets.Add(other.gameObject);

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }
        }
    }
}