using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverUI; // Reference to the Panel we just created

    [Header("Game State")]
    public bool isGameOver = false;
    public int score = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;
        score += amount;
        Debug.Log("Score: " + score);
    }

    public void GameOver()
    {
        if (isGameOver) return; // Prevent running this logic twice

        isGameOver = true;

        // 1. Show the Game Over Screen
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. Stop time (pauses physics and movement)
        Time.timeScale = 0f;

        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        // 1. Unpause time (Crucial! Otherwise the next game starts frozen)
        Time.timeScale = 1f;

        // 2. Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        // Check for Restart Input ONLY if the game is over
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }
}