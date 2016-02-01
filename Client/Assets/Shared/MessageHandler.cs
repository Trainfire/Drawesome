using System;

namespace Protocol
{
    public class MessageHandler
    {
        // Add an action for every type of message here.
        public Action<Message> OnGeneric;
        public Action<PlayerFirstConnectMessage> OnPlayerConnected;
        public Action<PlayerJoined> OnPlayerJoined;
        public Action<PlayerLeft> OnPlayerLeft;
        public Action<PlayerReadyMessage> OnPlayerReady;
        public Action<ValidatePlayer> OnValidatePlayer;
        public Action<ServerUpdate> OnServerUpdate;
        public Action<PlayerAction> OnPlayerAction;

        public void HandleMessage(string json)
        {
            var message = Deserialize<Message>(json);

            // Massive messy switch. IDK.
            switch (message.Type)
            {
                case MessageType.Generic:
                    if (OnGeneric != null)
                        OnGeneric(message);
                    break;
                case MessageType.PlayerConnect:
                    if (OnPlayerConnected != null)
                        OnPlayerConnected(Deserialize<PlayerFirstConnectMessage>(json));
                    break;
                case MessageType.PlayerJoined:
                    if (OnPlayerJoined != null)
                        OnPlayerJoined(Deserialize<PlayerJoined>(json));
                    break;
                case MessageType.PlayerLeft:
                    if (OnPlayerLeft != null)
                        OnPlayerLeft(Deserialize<PlayerLeft>(json));
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
                case MessageType.PlayerAction:
                    if (OnPlayerAction != null)
                        OnPlayerAction(Deserialize<PlayerAction>(json));
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
