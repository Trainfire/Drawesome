using Protocol;
using System;

public class MessageHandler
{
    public delegate void MessageEvent<T>(T message);

    public event MessageEvent<string> OnMessage;
    public event MessageEvent<Message> OnAny;

    public event MessageEvent<ServerMessage.SendConnectionError> OnConnectionError;
    public event MessageEvent<ServerMessage.NotifyConnectionSuccess> OnServerConnectionSuccess;
    public event MessageEvent<ServerMessage.UpdatePlayerInfo> OnServerUpdatePlayerInfo;
    public event MessageEvent<ServerMessage.RoomList> OnRecieveRoomList;
    public event MessageEvent<ServerUpdate> OnServerUpdate;
    public event MessageEvent<ServerMessage.NotifyRoomJoin> OnServerNotifyRoomJoin;
    public event MessageEvent<ServerMessage.NotifyRoomLeave> OnServerNotifyRoomLeave;
    public event MessageEvent<ServerMessage.NotifyPlayerAction> OnServerNotifyPlayerAction;
    public event MessageEvent<ServerMessage.NotifyChatMessage> OnChat;
    public event MessageEvent<ServerMessage.RoomUpdate> OnRoomUpdate;
    public event MessageEvent<ServerMessage.AssignRoomId> OnRoomIdAssigned;
    public event MessageEvent<ServerMessage.NotifyRoomCountdown> OnRoomCountdownStart;
    public event MessageEvent<ServerMessage.NotifyRoomCountdownCancel> OnRoomCountdownCancel;

    public event MessageEvent<ServerMessage.Game.SendPrompt> OnReceivePrompt;
    public event MessageEvent<ServerMessage.Game.SendChoices> OnRecieveChoices;
    public event MessageEvent<ServerMessage.Game.SendResult> OnRecieveResult;
    public event MessageEvent<ServerMessage.Game.ChangeState> OnStateChange;
    public event MessageEvent<ServerMessage.Game.SendAnswerValidation> OnAnswerError;

    public event MessageEvent<ServerMessage.Game.SendImage> OnRecieveImage;
    public event MessageEvent<ServerMessage.Game.AddTimer> OnAddTimer;
    public event MessageEvent<ServerMessage.Game.SetTimer> OnSetTimer;

    public MessageHandler(Connection connection)
    {
        connection.MessageRecieved += HandleMessage;
    }

    void HandleMessage(string json)
    {
        #region General

        Message.IsType<ServerMessage.SendConnectionError>(json, (data) => FireEvent(OnConnectionError, data));
        Message.IsType<ServerMessage.NotifyConnectionSuccess>(json, (data) => FireEvent(OnServerConnectionSuccess, data));
        Message.IsType<ServerMessage.NotifyPlayerAction>(json, (data) => FireEvent(OnServerNotifyPlayerAction, data));
        Message.IsType<ServerMessage.UpdatePlayerInfo>(json, (data) => FireEvent(OnServerUpdatePlayerInfo, data));
        Message.IsType<ServerMessage.RoomUpdate>(json, (data) => FireEvent(OnRoomUpdate, data));
        Message.IsType<ServerMessage.RoomList>(json, (data) => FireEvent(OnRecieveRoomList, data));
        Message.IsType<ServerMessage.NotifyRoomJoin>(json, (data) => FireEvent(OnServerNotifyRoomJoin, data));
        Message.IsType<ServerMessage.NotifyRoomLeave>(json, (data) => FireEvent(OnServerNotifyRoomLeave, data));
        Message.IsType<ServerMessage.NotifyChatMessage>(json, (data) => FireEvent(OnChat, data));
        Message.IsType<ServerMessage.AssignRoomId>(json, (data) => FireEvent(OnRoomIdAssigned, data));
        Message.IsType<ServerMessage.NotifyRoomCountdown>(json, (data) => FireEvent(OnRoomCountdownStart, data));
        Message.IsType<ServerMessage.NotifyRoomCountdownCancel>(json, (data) => FireEvent(OnRoomCountdownCancel, data));

        #endregion

        #region Game

        Message.IsType<ServerMessage.Game.SendChoices>(json, (data) => FireEvent(OnRecieveChoices, data));
        Message.IsType<ServerMessage.Game.SendImage>(json, (data) => FireEvent(OnRecieveImage, data));
        Message.IsType<ServerMessage.Game.SendPrompt>(json, (data) => FireEvent(OnReceivePrompt, data));
        Message.IsType<ServerMessage.Game.SendResult>(json, (data) => FireEvent(OnRecieveResult, data));
        Message.IsType<ServerMessage.Game.ChangeState>(json, (data) => FireEvent(OnStateChange, data));
        Message.IsType<ServerMessage.Game.SendAnswerValidation>(json, (data) => FireEvent(OnAnswerError, data));
        Message.IsType<ServerMessage.Game.AddTimer>(json, (data) => FireEvent(OnAddTimer, data));
        Message.IsType<ServerMessage.Game.SetTimer>(json, (data) => FireEvent(OnSetTimer, data));
        Message.IsType<ServerMessage.Game.SetTimer>(json, (data) => FireEvent(OnSetTimer, data));

        #endregion

        if (OnAny != null)
            OnAny(JsonHelper.FromJson<Message>(json));

        if (OnMessage != null)
            OnMessage(json);
    }

    void FireEvent<T>(MessageEvent<T> messageEvent, T data) where T : Message
    {
        if (messageEvent != null)
            messageEvent(data);
    }
}
