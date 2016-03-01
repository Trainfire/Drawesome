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
        Leave.onClick.AddListener(() => OnLeave());
        GamePrototype.gameObject.SetActive(false);
    }

    protected override void OnShow()
    {
        MakeInstance();
    }

    protected override void OnHide()
    {
        RemoveInstance();
    }

    void MakeInstance()
    {
        GameInstance = UiUtility.AddChild(GameContainer, GamePrototype, true);
        var rect = (RectTransform)GameInstance.transform;
        rect.sizeDelta = Vector2.zero;

        GameInstance.Initialise(Client);
        GameInstance.OnGameEnd += OnGameEnd;
    }

    void OnGameEnd(object sender, System.EventArgs e)
    {
        RemoveInstance();
        MakeInstance();
    }

    void RemoveInstance()
    {
        if (GameInstance != null)
        {
            GameInstance.OnGameEnd -= OnGameEnd;
            Destroy(GameInstance.gameObject);
        }
    }

    void Update()
    {
        var roomData = Client.Connection.Room;

        RoomId.text = string.Format("Room: {0}", roomData.ID);
        RoomOwner.text = string.Format("Owner: {0}", roomData.Owner.Name);
        RoomPassword.text = string.Format("Password: {0}", roomData.Password);
        RoomPassword.enabled = !string.IsNullOrEmpty(roomData.Password);
    }

    void OnLeave()
    {
        Client.UserInterface.Popups.MakeConfirmationPopup(Strings.Popups.ConfirmLeave, () => Client.Messenger.LeaveRoom()).Show();
    }
}
