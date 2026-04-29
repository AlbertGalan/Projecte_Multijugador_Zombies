using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHUDManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button playButton;    
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button exitButton;

    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "Game";

    private void Awake()
    {
        if (playButton != null)
        {
            playButton.onClick.AddListener(OnPlayClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitClicked);
        }

        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.AddListener(OnMultiplayerClicked);
        }
    }

    private void OnDestroy()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitClicked);
        }

        if (multiplayerButton != null)
        {
            multiplayerButton.onClick.RemoveListener(OnMultiplayerClicked);
        }
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnMultiplayerClicked()
    {
        SceneManager.LoadScene("MultiplayerMenu");
    }
}
