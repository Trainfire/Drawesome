using WebSocketSharp;
using Protocol;
using System;

public class MessageHandler
{
    public delegate void MessageEvent<T>(T message);

    public event MessageEvent<Message> OnAny;

    public event MessageEvent<ServerMessage.ConnectionSuccess> OnServerCompleteConnectionRequest;
    public event MessageEvent<ServerMessage.RoomList> OnRecieveRoomList;
    public event MessageEvent<ServerUpdate> OnServerUpdate;
    public event MessageEvent<ServerMessage.NotifyRoomError> OnServerNotifyRoomError;
    public event MessageEvent<ServerMessage.NotifyPlayerAction> OnServerNotifyPlayerAction;
    public event MessageEvent<SharedMessage.Chat> OnChat;
    public event MessageEvent<ServerMessage.RoomUpdate> OnRoomUpdate;

    public event MessageEvent<ServerMessage.Game.SendPrompt> OnReceivePrompt;
    public event MessageEvent<ServerMessage.Game.SendChoices> OnRecieveChoices;
    public event MessageEvent<ServerMessage.Game.SendResult> OnRecieveResult;
    public event MessageEvent<ServerMessage.Game.StateChange> OnStateChange;

    public event MessageEvent<ServerMessage.Game.SendImage> OnRecieveImage;
    public event MessageEvent<ServerMessage.Game.SetTimer> OnSetTimer;

    public MessageHandler(Connection connection)
    {
        connection.MessageRecieved += HandleMessage;
    }

    void HandleMessage(object sender, MessageEventArgs message)
    {
        if (message.Type == Opcode.Text)
        {
            var json = message.Data;

            #region General

            Message.IsType<ServerMessage.NotifyPlayerAction>(json, (data) => FireEvent(OnServerNotifyPlayerAction, data));
            Message.IsType<ServerMessage.ConnectionSuccess>(json, (data) => FireEvent(OnServerCompleteConnectionRequest, data));
            Message.IsType<ServerMessage.RoomUpdate>(json, (data) => FireEvent(OnRoomUpdate, data));
            Message.IsType<ServerMessage.RoomList>(json, (data) => FireEvent(OnRecieveRoomList, data));
            Message.IsType<ServerMessage.NotifyRoomError>(json, (data) => FireEvent(OnServerNotifyRoomError, data));
            Message.IsType<SharedMessage.Chat>(json, (data) => FireEvent(OnChat, data));

            #endregion

            #region Game

            Message.IsType<ServerMessage.Game.SendChoices>(json, (data) => FireEvent(OnRecieveChoices, data));
            Message.IsType<ServerMessage.Game.SendImage>(json, (data) => FireEvent(OnRecieveImage, data));
            Message.IsType<ServerMessage.Game.SendPrompt>(json, (data) => FireEvent(OnReceivePrompt, data));
            Message.IsType<ServerMessage.Game.SendResult>(json, (data) => FireEvent(OnRecieveResult, data));
            Message.IsType<ServerMessage.Game.StateChange>(json, (data) => FireEvent(OnStateChange, data));

            Message.IsType<ServerMessage.Game.SetTimer>(json, (data) => FireEvent(OnSetTimer, data));

            #endregion
        }
    }

    void FireEvent<T>(MessageEvent<T> messageEvent, T data) where T : Message
    {
        if (messageEvent != null)
            messageEvent(data);

        if (OnAny != null)
            OnAny(data);
    }
}
