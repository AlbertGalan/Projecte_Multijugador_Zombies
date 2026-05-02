using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Button joinButton;

    private string roomName;
    private NetworkingManager networkingManager;

    void Start()
    {
        if (joinButton != null)
            joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    void OnDestroy()
    {
        if (joinButton != null)
            joinButton.onClick.RemoveListener(OnJoinButtonClicked);
    }

    public void SetRoomInfo(string name, int playerCount, int maxPlayers, NetworkingManager manager)
    {
        roomName = name;
        networkingManager = manager;

        if (roomNameText != null)
            roomNameText.text = name;

        if (playerCountText != null)
            playerCountText.text = $"{playerCount}/{maxPlayers}";
    }

    private void OnJoinButtonClicked()
    {
        if (networkingManager != null)
        {
            Debug.Log($"Joining room: {roomName}");
            networkingManager.JoinRoom(roomName);
        }
    }
}
