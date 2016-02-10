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

    public event MessageEvent<ServerMessage.Game.SendImage> OnRecieveImage;
    public event MessageEvent<ServerMessage.Game.SendChoices> OnRecieveChoices;
    public event MessageEvent<ServerMessage.Game.SendPrompt> OnRecievePrompt;
    public event MessageEvent<ServerMessage.Game.SendResult> OnRecieveResult;
    public event MessageEvent<ServerMessage.Game.StateChange> OnStateChange;

    public MessageHandler(Connection connection)
    {
        connection.MessageRecieved += HandleMessage;
    }

    void HandleMessage(object sender, MessageEventArgs message)
    {
        if (message.Type == Opcode.Text)
        {
            var obj = JsonHelper.FromJson<Message>(message.Data);
            OnJson(obj.Type, message.Data);
        }
    }

    void OnJson(MessageType messageType, string json)
    {
        // Massive messy switch. IDK.
        switch (messageType)
        {
            case MessageType.ServerNotifyPlayerAction:
                FireEvent(OnServerNotifyPlayerAction, json);
                break;
            case MessageType.ServerConnectionSuccess:
                FireEvent(OnServerCompleteConnectionRequest, json);
                break;
            case MessageType.ServerUpdate:
                FireEvent(OnServerUpdate, json);
                break;
            case MessageType.ServerSendRoomList:
                FireEvent(OnRecieveRoomList, json);
                break;
            case MessageType.ServerNotifyRoomError:
                FireEvent(OnServerNotifyRoomError, json);
                break;
            case MessageType.Chat:
                FireEvent(OnChat, json);
                break;
            case MessageType.ServerRoomUpdate:
                FireEvent(OnRoomUpdate, json);
                break;

            #region Game

            case MessageType.GameServerStateChange:
                FireEvent(OnStateChange, json);
                break;
            case MessageType.GameServerSendImage:
                FireEvent(OnRecieveImage, json);
                break;
            case MessageType.GameServerSendPrompt:
                FireEvent(OnRecievePrompt, json);
                break;
            case MessageType.GameServerSendChoices:
                FireEvent(OnRecieveChoices, json);
                break;
            case MessageType.GameServerSendResult:
                FireEvent(OnRecieveResult, json);
                break;

            #endregion

            default:
                break;
        }
    }

    /// <summary>
    /// Safely fire event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="messageEvent"></param>
    /// <param name="args"></param>
    void FireEvent<T>(MessageEvent<T> messageEvent, string json) where T : Message
    {
        T obj = JsonHelper.FromJson<T>(json);
        if (messageEvent != null)
            messageEvent(obj);

        if (OnAny != null)
            OnAny(obj);
    }
}
