using System;

namespace Protocol
{
    public class MessageHandler
    {
        // Add an action for every type of message here.
        public Action<Message> OnGeneric;
        public Action<PlayerConnectMessage> OnPlayerConnected;
        public Action<PlayerReadyMessage> OnPlayerReady;
        public Action<ValidatePlayer> OnValidatePlayer;
        public Action<ServerUpdate> OnServerUpdate;

        public void HandleMessage(string json)
        {
            var message = Deserialize<Message>(json);

            // Massive messy switch. IDK.
            switch (message.Type)
            {
                case MessageType.None:
                    if (OnGeneric != null)
                        OnGeneric(message);
                    break;
                case MessageType.PlayerConnect:
                    if (OnPlayerConnected != null)
                        OnPlayerConnected(Deserialize<PlayerConnectMessage>(json));
                    break;
                case MessageType.PlayerReady:
                    if (OnPlayerReady != null)
                        OnPlayerReady(Deserialize<PlayerReadyMessage>(json));
                    break;
                case MessageType.ValidatePlayer:
                    if (OnValidatePlayer != null)
                        OnValidatePlayer(Deserialize<ValidatePlayer>(json));
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
