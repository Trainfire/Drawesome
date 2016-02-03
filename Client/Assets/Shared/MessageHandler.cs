using System;

namespace Protocol
{
    public class MessageHandler
    {
        // Add an action for every type of message here.
        public Action<Message> OnGeneric;
        public Action<ServerMessage.ConnectionSuccess> OnServerCompleteConnectionRequest;
        public Action<ServerMessage.RoomList> OnRecieveRoomList;
        public Action<ServerUpdate> OnServerUpdate;

        public void HandleMessage(string json)
        {
            var message = Deserialize<Message>(json);

            // Massive messy switch. IDK.
            switch (message.Type)
            {
                case MessageType.Log:
                    if (OnGeneric != null)
                        OnGeneric(message);
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
