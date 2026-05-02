using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMatchPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject roomLobbyPanel;
    [SerializeField] private GameObject joinRoomPanel;

    [Header("Create Room Panel")]
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button backFromCreateButton;

    [Header("Room Lobby Panel")]
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Transform playerListContent;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;

    [Header("Join Room Panel")]
    [SerializeField] private Transform roomListContent;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private Button backFromJoinButton;

    [Header("Error Display")]
    [SerializeField] private TMP_Text errorMessageText;

    private NetworkingManager networkingManager;
    private List<RoomInfo> availableRooms = new List<RoomInfo>();

    void Start()
    {
        networkingManager = GetComponent<NetworkingManager>();
        
        // Listeners para botones
        if (createRoomButton != null)
            createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        if (backFromCreateButton != null)
            backFromCreateButton.onClick.AddListener(() => ShowMainPanel());
        if (startGameButton != null)
            startGameButton.onClick.AddListener(OnStartGameClicked);
        if (leaveRoomButton != null)
            leaveRoomButton.onClick.AddListener(OnLeaveRoomClicked);
        if (backFromJoinButton != null)
            backFromJoinButton.onClick.AddListener(() => ShowMainPanel());

        HideAllPanels();
    }

    void OnDestroy()
    {
        if (createRoomButton != null)
            createRoomButton.onClick.RemoveListener(OnCreateRoomClicked);
        if (backFromCreateButton != null)
            backFromCreateButton.onClick.RemoveListener(() => ShowMainPanel());
        if (startGameButton != null)
            startGameButton.onClick.RemoveListener(OnStartGameClicked);
        if (leaveRoomButton != null)
            leaveRoomButton.onClick.RemoveListener(OnLeaveRoomClicked);
        if (backFromJoinButton != null)
            backFromJoinButton.onClick.RemoveListener(() => ShowMainPanel());
    }

    private void HideAllPanels()
    {
        if (mainMatchPanel != null) mainMatchPanel.SetActive(false);
        if (createRoomPanel != null) createRoomPanel.SetActive(false);
        if (roomLobbyPanel != null) roomLobbyPanel.SetActive(false);
        if (joinRoomPanel != null) joinRoomPanel.SetActive(false);
    }

    public void ShowMainPanel()
    {
        HideAllPanels();
        if (mainMatchPanel != null) mainMatchPanel.SetActive(true);
    }

    public void OnCreateRoomButtonPressed()
    {
        HideAllPanels();
        if (createRoomPanel != null) createRoomPanel.SetActive(true);
        if (roomNameInput != null) roomNameInput.text = "";
    }

    public void OnJoinRoomButtonPressed()
    {
        HideAllPanels();
        if (joinRoomPanel != null) joinRoomPanel.SetActive(true);
        networkingManager.RequestRoomList();
    }

    private void OnCreateRoomClicked()
    {
        string roomName = roomNameInput != null ? roomNameInput.text : "";
        if (string.IsNullOrEmpty(roomName.Trim()))
        {
            ShowErrorMessage("Por favor ingresa un nombre para la sala");
            return;
        }

        networkingManager.CreateRoom(roomName);
    }

    public void ShowRoomLobbyPanel()
    {
        HideAllPanels();
        if (roomLobbyPanel != null) roomLobbyPanel.SetActive(true);
        
        UpdateRoomInfo();
        UpdatePlayerList();
        UpdateStartGameButton();
    }

    private void UpdateRoomInfo()
    {
        if (PhotonNetwork.InRoom)
        {
            if (roomNameText != null)
                roomNameText.text = $"Sala: {PhotonNetwork.CurrentRoom.Name}";
        }
    }

    public void UpdatePlayerList()
    {
        if (playerListContent != null)
        {
            // Limpiar lista anterior
            foreach (Transform child in playerListContent)
            {
                Destroy(child.gameObject);
            }

            if (PhotonNetwork.InRoom)
            {
                // Añadir cada jugador
                foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
                {
                    GameObject playerItem = Instantiate(playerItemPrefab, playerListContent);
                    TMP_Text playerNameText = playerItem.GetComponentInChildren<TMP_Text>();
                    if (playerNameText != null)
                    {
                        playerNameText.text = player.NickName;
                    }
                }

                // Actualizar contador
                if (playerCountText != null)
                    playerCountText.text = $"Jugadores ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            }
        }
    }

    private void UpdateStartGameButton()
    {
        if (startGameButton != null)
        {
            // Solo el MasterClient puede iniciar
            bool ismaster = PhotonNetwork.IsMasterClient;
            startGameButton.interactable = ismaster;
            
            if (ismaster)
                startGameButton.GetComponentInChildren<TMP_Text>().text = "Iniciar Partida";
            else
                startGameButton.GetComponentInChildren<TMP_Text>().text = "Esperando al anfitrión...";
        }
    }

    private void OnStartGameClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Master Client iniciando partida...");
            PhotonNetwork.LoadLevel(networkingManager.multiplayerSceneGameName);
        }
    }

    private void OnLeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
        ShowMainPanel();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        availableRooms.Clear();
        
        // Filtrar salas abierta y no llenas
        foreach (RoomInfo room in roomList)
        {
            if (!room.IsOpen || room.IsVisible == false || room.PlayerCount >= room.MaxPlayers)
                continue;
            
            availableRooms.Add(room);
        }

        // Actualizar UI
        if (roomListContent != null)
        {
            foreach (Transform child in roomListContent)
            {
                Destroy(child.gameObject);
            }

            if (availableRooms.Count == 0)
            {
                TMP_Text noRoomsText = Instantiate(new GameObject("NoRooms"), roomListContent).AddComponent<TMP_Text>();
                noRoomsText.text = "No hay salas disponibles";
                return;
            }

            foreach (RoomInfo room in availableRooms)
            {
                GameObject roomItem = Instantiate(roomItemPrefab, roomListContent);
                RoomItemUI roomItemUI = roomItem.GetComponent<RoomItemUI>();
                if (roomItemUI != null)
                {
                    roomItemUI.SetRoomInfo(room.Name, room.PlayerCount, room.MaxPlayers, networkingManager);
                }
            }
        }

        Debug.Log($"Updated room list: {availableRooms.Count} available rooms");
    }

    public void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;
            errorMessageText.gameObject.SetActive(true);
            Invoke(nameof(HideErrorMessage), 3f);
        }
    }

    private void HideErrorMessage()
    {
        if (errorMessageText != null)
            errorMessageText.gameObject.SetActive(false);
    }
}
