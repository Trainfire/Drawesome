using System;
using System.Collections.Generic;
using Json;

namespace Shared
{
    public enum MessageType
    {
        None,
        UserConnected,
        UserDisconnected,
        UserMessage,
    }

#if !UNITY_EDITOR
    public class MessageEventHandler
    {
        public void HandleMessage(MessageEvent messageEvent)
        {
            switch (messageEvent.Name)
            {
                case MessageType.None:
                    break;
                case MessageType.UserConnected:
                    OnUserConnected(DeserializeMessage<User>(messageEvent));
                    break;
                case MessageType.UserDisconnected:
                    OnUserDisconnected(DeserializeMessage<User>(messageEvent));
                    break;
                case MessageType.UserMessage:
                    OnUserSendMessage(DeserializeMessage<UserMessage>(messageEvent));
                    break;
                default:
                    break;
            }
        }

        public T DeserializeMessage<T>(MessageEvent messageEvent)
        {
            T data = JsonParser.Deserialize<T>(messageEvent.Json);
            return data;
        }

        protected virtual void OnUserConnected(User user) { }
        protected virtual void OnUserDisconnected(User user) { }
        protected virtual void OnUserSendMessage(UserMessage userMessage) { }
    }
#endif

    public abstract class MessageEvent
    {
        public abstract MessageType Name { get; }
        public string Json { get; set; }
    }

    //public abstract class MessageEventHandler<T>
    //{
    //    public MessageType Name;

    //    public void HandleData(string jsonData)
    //    {
    //        T data = JsonConvert.DeserializeObject<T>(jsonData);
    //        OnDeserialize(data);
    //    }

    //    protected abstract void OnDeserialize(T data);
    //}

    public class User
    {
        public string Name;
        public int Wins;
    }

    public class UserMessage
    {
        public User User;
        public string Message;
    }
}
