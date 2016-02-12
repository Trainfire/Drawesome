using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;

public class UiRoom : UiBase
{
    public Button Leave;
    public Text RoomId;
    public Text RoomPassword;

    public override void Initialise(Client client)
    {
        base.Initialise(client);
        Leave.onClick.AddListener(() => Client.Messenger.LeaveRoom());
        Client.MessageHandler.OnRoomUpdate += MessageHandler_OnRoomUpdate;
    }

    void MessageHandler_OnRoomUpdate(ServerMessage.RoomUpdate message)
    {
        var roomData = message.RoomData;

        RoomId.text = string.Format("Room: {0}", roomData.ID);
        RoomPassword.text = string.Format("Password: {0}", roomData.Password);
        RoomPassword.enabled = !string.IsNullOrEmpty(roomData.Password);
    }
}
