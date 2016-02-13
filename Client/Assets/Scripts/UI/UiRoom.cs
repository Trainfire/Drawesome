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

    public override void Initialise(Client client)
    {
        base.Initialise(client);
        Leave.onClick.AddListener(() => Client.Messenger.LeaveRoom());
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
