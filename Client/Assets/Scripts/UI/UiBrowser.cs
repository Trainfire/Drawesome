using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System.Collections.Generic;
using System.Linq;

public class UiBrowser : UiMenu
{
    public Button Join;
    public Button Create;
    public Button Refresh;

    public UiBrowserItem BrowserItemPrototype;
    public RectTransform Browser;

    List<UiBrowserItem> browserItems;

    RoomData selectedRoom;
    ToggleGroup toggleGroup;

    const float refreshInterval = 1f;
    float refreshTimeStamp = 0f;

    void Awake()
    {
        BrowserItemPrototype.gameObject.SetActive(false);
        toggleGroup = this.GetOrAddComponent<ToggleGroup>();
        browserItems = new List<UiBrowserItem>();
    }

    void Start()
    {
        Client.Instance.MessageHandler.OnRecieveRoomList += OnRecieveRoomList;

        Create.onClick.AddListener(() => Client.Instance.CreateRoom());
        Refresh.onClick.AddListener(() => OnRefresh());
        Join.onClick.AddListener(() => OnJoin());
    }

    void OnRecieveRoomList(ServerMessage.RoomList message)
    {
        browserItems.ForEach(x => Destroy(x));
        browserItems.Clear();

        foreach (var room in message.Rooms)
        {
            // Make instance of view
            var view = UiUtility.AddChild<UiBrowserItem>(Browser, BrowserItemPrototype, true);
            view.Text.text = string.Format("{0}'s Game", room.ID);
            view.Toggle.group = toggleGroup;
            view.RoomData = room;

            // Add toggle listener
            var temp = room;
            view.Toggle.onValueChanged.AddListener((enabled) =>
            {
                if (enabled)
                {
                    Debug.LogFormat("Select room {0}", view.RoomData.ID);
                    selectedRoom = temp;
                }
            });

            browserItems.Add(view);
        }

        // Select first item
        if (browserItems.Count != 0)
            browserItems[0].Toggle.isOn = true;
    }

    void OnJoin()
    {
        if (selectedRoom != null)
            Client.Instance.JoinRoom(selectedRoom.ID);
    }

    void Update()
    {
        Refresh.interactable = Time.realtimeSinceStartup > (refreshTimeStamp + refreshInterval);
    }

    void OnRefresh()
    {
        refreshTimeStamp = Time.realtimeSinceStartup;
        Client.Instance.RequestRooms();
    }
}
