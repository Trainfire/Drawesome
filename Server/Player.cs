using Fleck;
using Protocol;
using System.Linq;
using System;

namespace Server
{
    public enum PlayerCloseReason
    {
        None,
        Disconnected,
        Left,
        Kicked,
    }

    public struct PlayerConnectionClosed
    {
        public Player Player { get; private set; }
        public PlayerCloseReason CloseReason { get; private set; }

        public PlayerConnectionClosed(Player player, PlayerCloseReason reason)
        {
            Player = player;
            CloseReason = reason;
        }
    }

    public class Player
    {
        public event EventHandler<PlayerConnectionClosed> OnConnectionClosed;
        public event EventHandler<SharedMessage.Chat> OnChat;
        public event EventHandler<Message> OnMessage;
        public event EventHandler<string> OnMessageString;

        public PlayerData Data { get; set; }
        public IWebSocketConnection Socket { get; private set; }

        public Player(string playerName, IWebSocketConnection socket)
        {
            Data = new PlayerData();
            Data.Name = playerName;
            Data.ID = Guid.NewGuid().ToString();
            Socket = socket;
            
            socket.OnClose += () =>
            {
                if (OnConnectionClosed != null)
                    OnConnectionClosed(this, new PlayerConnectionClosed(this, PlayerCloseReason.Disconnected));
            };

            socket.OnMessage += (message) =>
            {
                if (OnChat != null)
                {
                    var obj = JsonHelper.FromJson<Message>(message);
                    if (obj.Type == MessageType.Chat)
                    {
                        var data = JsonHelper.FromJson<SharedMessage.Chat>(message);
                        OnChat(this, data);
                    }
                }

                if (OnMessage != null)
                {
                    var obj = JsonHelper.FromJson<Message>(message);
                    OnMessage(this, obj);
                }

                if (OnMessageString != null)
                {
                    OnMessageString(this, message);
                }
            };
        }

        public void SendMessage(Message message)
        {
            Socket.Send(message.AsJson());
        }

        /// <summary>
        /// Sends a message to the player about a particular action.
        /// </summary>
        /// <param name="actor">The player that committed the action.</param>
        /// <param name="action">The type of action.</param>
        public void SendAction(PlayerData actor, PlayerAction action)
        {
            var message = new ServerMessage.NotifyPlayerAction(actor, action);
            Socket.Send(message.AsJson());
        }

        public void SendRoomError(RoomError roomError)
        {
            var message = new ServerMessage.NotifyRoomError(roomError);
            Socket.Send(message.AsJson());
        }

        public void Update(ClientConnectionsHandler manager)
        {
            Console.WriteLine("Send update to " + Data.Name);
        }
    }
}
