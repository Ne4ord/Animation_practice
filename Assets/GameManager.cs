using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score = 0;
    public int totalEnemies = 10;

    public GameObject winText;
    public GameObject loseText;
    public GameObject restartButton;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (winText != null)
            winText.SetActive(false);
        if (loseText != null)
            loseText.SetActive(false);
        if (restartButton != null)
            restartButton.SetActive(false);
    }

    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;

        if (score >= totalEnemies)
        {
            isGameOver = true;
            Invoke(nameof(WinGame), 3.5f);
        }
    }

    void WinGame()
    {
        Time.timeScale = 0f;
        ShowCursor();

        if (winText != null)
            winText.SetActive(true);
        if (restartButton != null)
            restartButton.SetActive(true);
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0f;
        ShowCursor();

        if (loseText != null)
            loseText.SetActive(true);
        if (restartButton != null)
            restartButton.SetActive(true);
    }

    void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}