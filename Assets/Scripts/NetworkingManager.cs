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
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Unir-mos a un lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("A punt per unir-mos a una sala...");
        multiplayerButton.interactable = true;
    }

    public void FindMatch()
    {
        Debug.Log("Cercant partida...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
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
        Debug.Log("Creant una nova sala...");
        PhotonNetwork.CreateRoom($"$RoomName_{randomRoomNumber}", roomOptions);
        Debug.Log($"Sala creada: $RoomName_{randomRoomNumber}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Carregar Escena del joc MP: {PhotonNetwork.CurrentRoom.Name}");
        PhotonNetwork.LoadLevel(multiplayerSceneGameName);
    }
}
