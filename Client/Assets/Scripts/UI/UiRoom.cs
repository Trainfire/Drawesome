using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;

public class UiRoom : UiBase
{
    public Button Leave;
    public Text RoomOwner;
    public Text RoomId;
    public Text RoomPassword;

    public GameObject GameContainer;
    public Game GamePrototype;

    Game GameInstance { get; set; }

    public override void Initialise(Client client)
    {
        base.Initialise(client);
        Leave.onClick.AddListener(() => Client.Messenger.LeaveRoom());
        GamePrototype.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        GameInstance = UiUtility.AddChild(GameContainer, GamePrototype, true);
        var rect = (RectTransform)GameInstance.transform;
        rect.sizeDelta = Vector2.zero;
        GameInstance.Initialise(Client);
    }

    protected override void OnHide()
    {
        if (GameInstance != null)
            Destroy(GameInstance.gameObject);
    }

    void Update()
    {
        var roomData = Client.Connection.Room;

        RoomId.text = string.Format("Room: {0}", roomData.ID);
        RoomOwner.text = string.Format("Owner: {0}", roomData.Owner.Name);
        RoomPassword.text = string.Format("Password: {0}", roomData.Password);
        RoomPassword.enabled = !string.IsNullOrEmpty(roomData.Password);
    }
}
