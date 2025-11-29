using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Weapon Attachment")]
    public Transform handPosition;

    // assign item to the inspector
    [Header("Item Prefabs")]
    public GameObject swordPrefab;

    private GameObject equippedWeapon;
    private GameObject currentInteractable;
    private PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        if (playerAttack == null)
        {
            Debug.LogError("PlayerAttack script is missing on the player object! Pick-up/Drop will fail.");
        }
    }

    void Update()
    {
        // Pickup Input Logic (Press E)
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null && equippedWeapon == null)
        {
            PickUpWeapon(currentInteractable);
        }

        // Drop Input Logic (Press Q)
        if (Input.GetKeyDown(KeyCode.Q) && equippedWeapon != null)
        {
            DropWeapon();
        }
    }

    /// <summary>
    /// Sets the GameObject the player can interact with. Called by the item's script.
    /// </summary>
    public void SetInteractable(GameObject interactable)
    {
        currentInteractable = interactable;
    }

    /// <summary>
    /// Clears the interaction target when the player moves away.
    /// </summary>
    public void ClearInteractable()
    {
        currentInteractable = null;
    }

    private void PickUpWeapon(GameObject weapon)
    {
        // The main weapon object is now the PIVOT parent (not the sprite object).

        // Parent the entire weapon (which is now the pivot) to the hand position.
        // The pivot point (0, 0, 0 of the weapon GameObject) will now be at the hand position.
        weapon.transform.SetParent(handPosition);

        // 1. Reset Position (The pivot should always be at the hand's origin)
        weapon.transform.localPosition = Vector3.zero;

        // 2. Set Rotation
        // This rotation should be the IDLE rotation offset needed for the sprite
        weapon.transform.localRotation = Quaternion.Euler(0f, 0f, -135f);

        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true; // Essential for triggers to work!
            rb.velocity = Vector2.zero;
        }
        // 3. --- FINALIZE PICKUP ---
        equippedWeapon = weapon;

        // CHECK: Ensure playerAttack is available before calling SetWeapon
        if (playerAttack != null)
        {
            playerAttack.SetWeapon(weapon);
        }

        Debug.Log(weapon.name + " picked up with 'E'!");
    }

    private void DropWeapon()
    {
        if (equippedWeapon == null || swordPrefab == null)
        {
            Debug.LogWarning("Cannot drop: No weapon equipped or swordPrefab not assigned in Inspector.");
            return;
        }

        // Save the current world position to drop the new prefab instance there
        Vector3 dropPosition = equippedWeapon.transform.position;

        // 1. --- PLAYER STATE CLEANUP ---
        equippedWeapon.transform.SetParent(null); // Unparent the weapon

        // FIX: Check for playerAttack before calling ClearWeapon()
        if (playerAttack != null)
        {
            playerAttack.ClearWeapon(); // Crucial: Tell the attack script the weapon is gone
        }

        // 2. --- INSTANTIATE NEW PREFAB ---
        GameObject newSword = Instantiate(swordPrefab, dropPosition, Quaternion.identity);

        // Apply a small force for a "toss" effect (Optional)
        Rigidbody2D rb = newSword.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(Random.Range(-2f, 2f), 1f);
        }

        // 3. --- DESTROY THE OLD INSTANCE ---
        Destroy(equippedWeapon);

        // 4. --- FINALIZE DROP ---
        equippedWeapon = null; // Reset the weapon reference

        Debug.Log("Weapon dropped and replaced with fresh prefab. Ready for re-pickup!");
    }

    private void CleanupWeaponComponents(GameObject weapon)
    {
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // this change prevents the weapon from falling
            rb.simulated = true; // IT MUST BE SIMULATED to detect collisions!
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        // ... (Keep the rest of your cleanup, like disabling the pickup script)
        SwordPickup pickupScript = weapon.GetComponent<SwordPickup>();
        if (pickupScript != null)
        {
            Destroy(pickupScript);
        }
    }
}