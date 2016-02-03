using System;

namespace Protocol
{
    public class MessageHandler
    {
        // Add an action for every type of message here.
        public Action<Message> OnGeneric;
        public Action<ServerMessage.ApproveClientConnection> OnServerCompleteConnectionRequest;
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
                case MessageType.ServerCompleteConnectionRequest:
                    if (OnServerCompleteConnectionRequest != null)
                        OnServerCompleteConnectionRequest(Deserialize<ServerMessage.ApproveClientConnection>(json));
                    break;
                case MessageType.ServerUpdate:
                    if (OnServerUpdate != null)
                        OnServerUpdate(Deserialize<ServerUpdate>(json));
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
