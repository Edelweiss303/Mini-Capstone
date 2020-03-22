using UnityEngine;
using UnityEngine.UI;

public class RoomListing : MonoBehaviour
{
    [SerializeField]
    private Text _roomNameText;
    private Text RoomNameText
    {
        get { return _roomNameText; }
    }

    public string RoomName { get; private set; }

    public bool Updated
    {
        get; set;
    }

    // Start is called before the first frame update
    void Start()
    {
        LobbyCanvas lobbyCanvas = MainCanvasManager.Instance.LobbyCanvas;
        
        if (!lobbyCanvas)
        {
            return;
        }

        GameObject lobbyCanvasObj = lobbyCanvas.gameObject;

        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => lobbyCanvas.OnClick_JoinRoom(RoomNameText.text));
    }

    private void OnDestroy()
    {
        Button button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
    }

    public void SetRoomNameText(string inText)
    {
        RoomName = inText;
        RoomNameText.text = RoomName;
    }
}
