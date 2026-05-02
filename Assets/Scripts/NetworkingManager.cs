using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button multiplayerButton;
    public string multiplayerSceneGameName = "MultiplayerGame";
    void Start()
    {
        // Si ya estamos conectados, únirse al lobby; si no, conectarse.
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected to Photon. Joining lobby...");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            Debug.Log("Connecting to Photon...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    



    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster: joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinedLobby: ready to join or create rooms");
        if (multiplayerButton != null) multiplayerButton.interactable = true;
    }

    public void FindMatch()
    {
        Debug.Log("FindMatch: attempting JoinRandomRoom...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogWarning($"OnJoinRandomFailed: {returnCode} - {message} ; creating room...");
        MakeRoom();
    }

    private void MakeRoom()
    {
        int randomRoomNumber = Random.Range(0, 5000);
        RoomOptions roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = 6,
            PublishUserId = true
        };
        string roomName = $"Room_{randomRoomNumber}";
        Debug.Log($"Creating new room: {roomName}...");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
        Debug.Log($"Requested room creation: {roomName}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"OnJoinedRoom: joined {PhotonNetwork.CurrentRoom.Name}. Loading scene {multiplayerSceneGameName}...");
        PhotonNetwork.LoadLevel(multiplayerSceneGameName);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"OnCreateRoomFailed: {returnCode} - {message}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"OnPlayerEnteredRoom: {newPlayer.NickName} (ID:{newPlayer.UserId})");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"OnDisconnected: {cause}");
    }

    public void LoadMainMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }
}
