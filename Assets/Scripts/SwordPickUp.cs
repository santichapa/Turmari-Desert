using UnityEngine;

public class SwordPickup : MonoBehaviour
{
    // A reference to the player's interaction script when they are in range
    private PlayerInteraction playerInteraction;

    // Called when the player enters the sword's trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the colliding object is the player
        // We assume the player has the "Player" tag for this check
        if (other.CompareTag("Player"))
        {
            // Try to get the PlayerInteraction component
            playerInteraction = other.GetComponent<PlayerInteraction>();

            if (playerInteraction != null)
            {
                // Inform the player script that this sword is available for pickup
                playerInteraction.SetInteractable(this.gameObject);

                // Optional: Display a prompt like "Press E to Pick Up"
                Debug.Log("Press E to pick up.");
            }
        }
    }

    // Called when the player leaves the sword's trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInteraction != null)
            {
                // Clear the reference in the player script
                playerInteraction.ClearInteractable();
                playerInteraction = null;
            }
        }
    }
}