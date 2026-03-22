using UnityEngine;
using UnityEngine.SceneManagement; 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); }
        else { Instance = this; }

        Time.timeScale = 1f;
    }

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
            BossController boss = FindFirstObjectByType<BossController>();
            if (boss != null && AnalyticsManager.Instance != null)
            {
                float timePlayed = boss.GetFightTimer();
                int currentPhase = boss.GetCurrentPhase();

                AnalyticsManager.Instance.LogBalancingData(timePlayed, currentPhase, false);
            }
        }
    }

    public void RetryButton()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenuButton()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}
