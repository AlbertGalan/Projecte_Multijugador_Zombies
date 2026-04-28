using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Buttons")]
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button pauseExitButton;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMainMenuButton;

    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused;
    private bool isGameOver;

    private void Awake()
    {
        if (pauseResumeButton != null)
        {
            pauseResumeButton.onClick.AddListener(OnPauseResumeButton);
        }

        if (pauseExitButton != null)
        {
            pauseExitButton.onClick.AddListener(OnPauseExitButton);
        }

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(OnGameOverRestartButton);
        }

        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.AddListener(OnGameOverMainMenuButton);
        }
    }

    private void OnDestroy()
    {
        if (pauseResumeButton != null)
        {
            pauseResumeButton.onClick.RemoveListener(OnPauseResumeButton);
        }

        if (pauseExitButton != null)
        {
            pauseExitButton.onClick.RemoveListener(OnPauseExitButton);
        }

        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.RemoveListener(OnGameOverRestartButton);
        }

        if (gameOverMainMenuButton != null)
        {
            gameOverMainMenuButton.onClick.RemoveListener(OnGameOverMainMenuButton);
        }
    }

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        SetCursorStateForGameplay();
    }

    private void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetCursorStateForMenu();

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        SetCursorStateForGameplay();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    public void OnPauseResumeButton()
    {
        ResumeGame();
    }

    public void OnPauseExitButton()
    {
        ExitToMainMenu();
    }

    public void ShowGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        isPaused = false;
        Time.timeScale = 0f;
        SetCursorStateForMenu();

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SetCursorStateForGameplay();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnGameOverRestartButton()
    {
        RestartGame();
    }

    public void ExitToMainMenu()
    {
        Time.timeScale = 1f;
        SetCursorStateForMenu();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnGameOverMainMenuButton()
    {
        ExitToMainMenu();
    }

    private static void SetCursorStateForGameplay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void SetCursorStateForMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
