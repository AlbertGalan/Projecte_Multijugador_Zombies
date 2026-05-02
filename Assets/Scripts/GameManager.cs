using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    public int enemiesAlive;
    public int round;

    public GameObject[] spawnPoints;
    //public GameObject enemyPrefab;

    public PhotonView photonView;

    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Buttons")]
    [SerializeField] private Button pauseResumeButton;
    [SerializeField] private Button pauseExitButton;
    [SerializeField] private Button gameOverRestartButton;
    [SerializeField] private Button gameOverMainMenuButton;

    [Header("Scenes")]
    [SerializeField] private string gameSceneNameOff = "Game";
    [SerializeField] private string gameSceneNameOn = "Game Online";

    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string multMenuSceneName = "MultiplayerMenu";

     [Header("HUD")]
     [SerializeField] private TMP_Text roundText;

    public bool isPaused;
    public bool isGameOver;

    public PlayerManager playerManager;

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

    void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawners");
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
    // Update is called once per frame
    void Update()
    {
        if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)        {
           if (enemiesAlive <= 0)
        {
            round++;
            NextWave(round);
            if(PhotonNetwork.InRoom)
                {
                    Hashtable hash = new Hashtable();
                    hash.Add("currentRound", round);
                    hash.Add("enemiesAlive", enemiesAlive);

                    PhotonNetwork.CurrentRoom.SetCustomProperties(hash);
                }
                else
                {
                    DisplayNextRoundText();
                }
        }
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

    void NextWave(int roundNumber)
    {
        for (int i = 0; i < roundNumber; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject spawnPoint = spawnPoints[randomIndex];

            if (spawnPoint == null)
            {
                continue;
            }
            GameObject enemyInstance;
            EnemyManager enemyManager;

            if(PhotonNetwork.InRoom)
            {
                enemyInstance = PhotonNetwork.Instantiate("Zombie", spawnPoint.transform.position, Quaternion.identity);
                enemyManager = enemyInstance.GetComponent<EnemyManager>();
            }
            else
            {
                enemyInstance = Instantiate(Resources.Load("Zombie"), spawnPoint.transform.position, Quaternion.identity) as GameObject;
                enemyManager = enemyInstance.GetComponent<EnemyManager>();
            }
            if (enemyManager != null)
            {
                enemyManager.gameManager = this;
            }

            enemiesAlive++;
        }
    }

    private void DisplayNextRoundText()
    {
        DisplayNextRoundText(round);
    }

    private void DisplayNextRoundText(int roundNumber)
    {
        if (roundText != null)
        {
            roundText.text = "Ronda: " + roundNumber.ToString();
        } 
    }

        private void PauseGame()
    {
        isPaused = true;
        if(!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0f;
        }   
        SetCursorStateForMenu();

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
     
    }

    public void ResumeGame()
    {
        isPaused = false;
        if(!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1f;
        }
        //Time.timeScale = 1f;
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

    if(!PhotonNetwork.InRoom)
        {
            Time.timeScale = 0f;
        }

        isGameOver = true;
        isPaused = false;
        //Time.timeScale = 0f;
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
        //Time.timeScale = 1f;

        if(!PhotonNetwork.InRoom)
        {
            SetCursorStateForGameplay();
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneNameOn);
        }
        else
        {
            SetCursorStateForGameplay();
            SceneManager.LoadScene(gameSceneNameOff);
        }
    }

    public void OnGameOverRestartButton()
    {
        RestartGame();
    }

    public void ExitToMainMenu()
    {
        if(!PhotonNetwork.InRoom)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(multMenuSceneName);
        }
        else
        {
            SetCursorStateForMenu();
            SceneManager.LoadScene(mainMenuSceneName);
        }
        //Time.timeScale = 1f;
      
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

  // Manejo de propiedades de sala y sincronización al unirse
public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged) 
{
    if (propertiesThatChanged.ContainsKey("currentRound"))
    {
        int networkRound = (int)propertiesThatChanged["currentRound"];
        this.round = networkRound; // Actualiza la variable local para que el cliente sepa en qué ronda está
        DisplayNextRoundText(networkRound);
    }

    if (propertiesThatChanged.ContainsKey("enemiesAlive"))
    {
        this.enemiesAlive = (int)propertiesThatChanged["enemiesAlive"];
    }
}

public override void OnJoinedRoom()
{
    base.OnJoinedRoom();

    // Sincronizar estado de la sala para jugadores que se unen
    if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties != null)
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("currentRound"))
        {
            this.round = (int)PhotonNetwork.CurrentRoom.CustomProperties["currentRound"];
            DisplayNextRoundText(this.round);
        }

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("enemiesAlive"))
        {
            this.enemiesAlive = (int)PhotonNetwork.CurrentRoom.CustomProperties["enemiesAlive"];
        }
    }
}
}