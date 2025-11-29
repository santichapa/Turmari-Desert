using UnityEngine;
using TMPro; // Needed for TextMeshPro

public class GameUIManager : MonoBehaviour
{
    // Assign this in the Inspector
    public TextMeshProUGUI finalScoreText;

    // This function will be called by the GameManager when the player dies.
    public void DisplayFinalScore()
    {
        // Check if the Game Manager exists and the UI text object is linked
        if (GameManager.Instance != null && finalScoreText != null)
        {
            // Use the score value stored in the GameManager
            int score = GameManager.Instance.score;

            // Format and display the score
            finalScoreText.text = "Score: " + score.ToString();
        }
    }
}