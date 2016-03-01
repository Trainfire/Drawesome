using UnityEngine;
using UnityEngine.UI;
using Protocol;
using System.Collections.Generic;
using System.Linq;

public class UiBrowser : UiBase
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
        toggleGroup = gameObject.GetOrAddComponent<ToggleGroup>();
        browserItems = new List<UiBrowserItem>();
    }

    public override void Initialise(Client client)
    {
        base.Initialise(client);

        Client.MessageHandler.OnRecieveRoomList += OnRecieveRoomList;
        Client.MessageHandler.OnServerNotifyRoomJoin += OnServerNotifyRoomJoin;

        Create.onClick.AddListener(() => OnCreate());
        Refresh.onClick.AddListener(() => OnRefresh());
        Join.onClick.AddListener(() => OnJoin());
    }

    void OnRecieveRoomList(ServerMessage.RoomList message)
    {
        browserItems.ForEach(x => Destroy(x.gameObject));
        browserItems.Clear();

        foreach (var room in message.Rooms)
        {
            // Make instance of view
            var view = UiUtility.AddChild<UiBrowserItem>(Browser.gameObject, BrowserItemPrototype, true);
            view.Text.text = string.Format("{0}'s Game / Players: {1} / Started?: {2}", room.Owner.Name, room.Players.Count, room.GameStarted);
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

            view.gameObject.GetOrAddComponent<DoubleClickHandler>().OnDoubleClick += () =>
            {
                OnJoin();
            };

            browserItems.Add(view);
        }

        // Select first item
        if (browserItems.Count != 0)
            browserItems[0].Toggle.isOn = true;
    }

    void OnCreate()
    {
        Client.UserInterface.Popups.MakeInputPopup("Enter an optional password", (password) => Client.Messenger.CreateRoom(password)).Show();
    }

    void OnJoin()
    {
        if (selectedRoom != null)
        {
            if (!string.IsNullOrEmpty(selectedRoom.Password))
            {
                Client.UserInterface.Popups.MakeInputPopup("Enter the password", (password) => Client.Messenger.JoinRoom(selectedRoom.ID, password)).Show();
            }
            else
            {
                Client.Messenger.JoinRoom(selectedRoom.ID);
            }
        }
    }

    void OnServerNotifyRoomJoin(ServerMessage.NotifyRoomJoin data)
    {
        string message = "";

        switch (data.Notice)
        {
            case RoomNotice.None:
                break;
            case RoomNotice.InvalidPassword:
                message = Strings.Popups.InvalidPassword;
                break;
            case RoomNotice.RoomFull:
                message = Strings.Popups.RoomFull;
                break;
            case RoomNotice.RoomDoesNotExist:
                message = Strings.Popups.RoomDoesNotExist;
                break;
            case RoomNotice.GameAlreadyStarted:
                message = Strings.Popups.GameAlreadyStarted;
                break;
            case RoomNotice.MaxRoomsLimitReached:
                message = Strings.Popups.RoomLimitReached;
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(message))
            Client.UserInterface.Popups.MakeMessagePopup(message).Show();
    }

    void Update()
    {
        Refresh.interactable = Time.realtimeSinceStartup > (refreshTimeStamp + refreshInterval);
    }

    void OnRefresh()
    {
        refreshTimeStamp = Time.realtimeSinceStartup;
        Client.Messenger.RequestRooms();
    }
}
