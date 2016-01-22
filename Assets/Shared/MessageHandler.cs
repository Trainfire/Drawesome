using System;

namespace Protocol
{
    public class MessageHandler
    {
        public Action<Message> OnGeneric;
        public Action<PlayerConnectMessage> OnPlayerConnected;
        public Action<PlayerReadyMessage> OnPlayerReady;
        public Action<ValidatePlayer> OnValidatePlayer;

        public void HandleMessage(string json)
        {
            var message = Deserialize<Message>(json);

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
                case MessageType.ForceStartRound:
                    break;
                default:
                    break;
            }
        }

        T Deserialize<T>(string json) where T : Message
        {
            T data = null;
#if UNITY_5
            data = UnityEngine.JsonUtility.FromJson<T>(json);
#else
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
#endif
            return data;
        }
    }
}
