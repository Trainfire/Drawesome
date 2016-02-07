using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Protocol;

public class UiRoom : UiMenu
{
    public Button Leave;
    public Text RoomId;
    public Text RoomPassword;

    void Start()
    {
        Leave.onClick.AddListener(() => Client.Instance.LeaveRoom());
    }

    void Update()
    {
        if (Client.Instance.IsConnected() && Client.Instance.RoomData != null)
        {
            var roomData = Client.Instance.RoomData;

            RoomId.text = string.Format("Room: {0}", roomData.ID);
            RoomPassword.text = string.Format("Password: {0}", roomData.Password);
            RoomPassword.enabled = !string.IsNullOrEmpty(roomData.Password);
        }
    }
}
