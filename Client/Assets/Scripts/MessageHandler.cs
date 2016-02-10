using WebSocketSharp;
using Protocol;

public class MessageHandler
{
    public delegate void MessageEvent<T>(T message);

    public event MessageEvent<ServerMessage.ConnectionSuccess> OnServerCompleteConnectionRequest;
    public event MessageEvent<ServerMessage.RoomList> OnRecieveRoomList;
    public event MessageEvent<ServerUpdate> OnServerUpdate;
    public event MessageEvent<ServerMessage.NotifyRoomError> OnServerNotifyRoomError;
    public event MessageEvent<ServerMessage.NotifyPlayerAction> OnServerNotifyPlayerAction;
    public event MessageEvent<SharedMessage.Chat> OnChat;
    public event MessageEvent<ServerMessage.RoomUpdate> OnRoomUpdate;

    public MessageHandler(Connection connection)
    {
        connection.MessageRecieved += HandleMessage;
    }

    public void HandleMessage(object sender, MessageEventArgs message)
    {
        if (message.Type == Opcode.Text)
        {
            var obj = Deserialize<Message>(message.Data);

            // Massive messy switch. IDK.
            switch (obj.Type)
            {
                case MessageType.ServerNotifyPlayerAction:
                    if (OnServerNotifyPlayerAction != null)
                        OnServerNotifyPlayerAction(Deserialize<ServerMessage.NotifyPlayerAction>(message.Data));
                    break;
                case MessageType.ServerConnectionSuccess:
                    if (OnServerCompleteConnectionRequest != null)
                        OnServerCompleteConnectionRequest(Deserialize<ServerMessage.ConnectionSuccess>(message.Data));
                    break;
                case MessageType.ServerUpdate:
                    if (OnServerUpdate != null)
                        OnServerUpdate(Deserialize<ServerUpdate>(message.Data));
                    break;
                case MessageType.ServerSendRoomList:
                    if (OnRecieveRoomList != null)
                        OnRecieveRoomList(Deserialize<ServerMessage.RoomList>(message.Data));
                    break;
                case MessageType.ServerNotifyRoomError:
                    if (OnServerNotifyRoomError != null)
                        OnServerNotifyRoomError(Deserialize<ServerMessage.NotifyRoomError>(message.Data));
                    break;
                case MessageType.Chat:
                    if (OnChat != null)
                        OnChat(Deserialize<SharedMessage.Chat>(message.Data));
                    break;
                case MessageType.ServerRoomUpdate:
                    if (OnRoomUpdate != null)
                        OnRoomUpdate(Deserialize<ServerMessage.RoomUpdate>(message.Data));
                    break;
                default:
                    break;
            }
        }
    }

    T Deserialize<T>(string json) where T : Message
    {
        return JsonHelper.FromJson<T>(json);
    }
}
