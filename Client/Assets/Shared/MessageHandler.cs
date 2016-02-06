using System;

namespace Protocol
{
    public class MessageHandler
    {
        public delegate void MessageEvent<T>(T message);

        // Add an action for every type of message here.
        public event MessageEvent<ServerMessage.ConnectionSuccess> OnServerCompleteConnectionRequest;
        public event MessageEvent<ServerMessage.RoomList> OnRecieveRoomList;
        public event MessageEvent<ServerUpdate> OnServerUpdate;
        public event MessageEvent<ServerMessage.NotifyRoomError> OnServerNotifyRoomError;
        public event MessageEvent<ServerMessage.NotifyPlayerAction> OnServerNotifyPlayerAction;
        public event MessageEvent<SharedMessage.Chat> OnChat;
        public event MessageEvent<ServerMessage.RoomUpdate> OnRoomUpdate;

        public void HandleMessage(string json)
        {
            var message = Deserialize<Message>(json);

            // Massive messy switch. IDK.
            switch (message.Type)
            {
                case MessageType.ServerNotifyPlayerAction:
                    if (OnServerNotifyPlayerAction != null)
                        OnServerNotifyPlayerAction(Deserialize<ServerMessage.NotifyPlayerAction>(json));
                    break;
                case MessageType.ServerConnectionSuccess:
                    if (OnServerCompleteConnectionRequest != null)
                        OnServerCompleteConnectionRequest(Deserialize<ServerMessage.ConnectionSuccess>(json));
                    break;
                case MessageType.ServerUpdate:
                    if (OnServerUpdate != null)
                        OnServerUpdate(Deserialize<ServerUpdate>(json));
                    break;
                case MessageType.ServerSendRoomList:
                    if (OnRecieveRoomList != null)
                        OnRecieveRoomList(Deserialize<ServerMessage.RoomList>(json));
                    break;
                case MessageType.ServerNotifyRoomError:
                    if (OnServerNotifyRoomError != null)
                        OnServerNotifyRoomError(Deserialize<ServerMessage.NotifyRoomError>(json));
                    break;
                case MessageType.Chat:
                    if (OnChat != null)
                        OnChat(Deserialize<SharedMessage.Chat>(json));
                    break;
                case MessageType.ServerRoomUpdate:
                    if (OnRoomUpdate != null)
                        OnRoomUpdate(Deserialize<ServerMessage.RoomUpdate>(json));
                    break;
                default:
                    break;
            }
        }

        T Deserialize<T>(string json) where T : Message
        {
            return JsonHelper.FromJson<T>(json);
        }
    }
}
