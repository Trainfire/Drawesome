using Fleck;
using Protocol;
using System.Collections.Generic;
using System;
using System.Text;

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

    public class Player : ServerMessage.Game.ISendScores
    {
        public event EventHandler<PlayerConnectionClosed> OnConnectionClosed;
        public event EventHandler<SharedMessage.Chat> OnChat;
        public event EventHandler<ClientMessage.Game.SendAction> OnGameAction;
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

            socket.OnBinary += (binary) =>
            {
                var decoded = Encoding.UTF8.GetString(binary);
                OnPlayerMessage(decoded);
            };
        }

        void OnPlayerMessage(string message)
        {
            var obj = JsonHelper.FromJson<Message>(message);

            Message.IsType<SharedMessage.Chat>(message, (data) =>
            {
                if (OnChat != null)
                    OnChat(this, data);
            });

            Message.IsType<ClientMessage.Game.SendAction>(message, (data) =>
            {
                if (OnGameAction != null)
                    OnGameAction(this, data);
            });

            if (OnMessage != null)
                OnMessage(this, obj);

            if (OnMessageString != null)
                OnMessageString(this, message);
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

        public void AssignClientId(string id)
        {
            var message = new ServerMessage.AssignClientId(id);
            Socket.Send(message.AsJson());
        }

        public void RequestClientName()
        {
            Logger.Log("RequestClientName");
            var message = new ServerMessage.RequestClientName();
            Socket.Send(message.AsJson());
        }

        public void UpdatePlayerInfo(PlayerData playerData)
        {
            var message = new ServerMessage.UpdatePlayerInfo(2, playerData);
            Socket.Send(message.AsJson());
        }

        public void SendRoomError(RoomError roomError)
        {
            var message = new ServerMessage.NotifyRoomError(roomError);
            Socket.Send(message.AsJson());
        }

        public void SendChat(PlayerData player, string chat)
        {
            var message = new ServerMessage.NotifyChatMessage(player, chat);
            Socket.Send(message.AsJson());
        }

        public void SendChoices(PlayerData creator, List<AnswerData> answers)
        {
            var message = new ServerMessage.Game.SendChoices(creator, answers);
            Socket.Send(message.AsJson());
        }

        public void SendImage(DrawingData drawing)
        {
            var message = new ServerMessage.Game.SendImage(drawing);
            Socket.Send(message.AsJson());
        }

        public void SendPrompt(string prompt)
        {
            var message = new ServerMessage.Game.SendPrompt(prompt);
            Socket.Send(message.AsJson());
        }

        public void NotifyConnectionSuccess()
        {
            var message = new ServerMessage.NotifyConnectionSuccess();
            Socket.Send(message.AsJson());
        }

        public void NotifyPlayerGameAction(PlayerData actor, GamePlayerAction action)
        {
            var message = new ServerMessage.Game.PlayerAction(actor, action);
            Socket.Send(message.AsJson());
        }

        public void AddTimer(float duration)
        {
            var message = new ServerMessage.Game.AddTimer(duration);
            Socket.Send(message.AsJson());
        }

        public void SetTimer(float time)
        {
            var message = new ServerMessage.Game.SetTimer(time);
            Socket.Send(message.AsJson());
        }

        public void AssignRoomId(uint roomId)
        {
            var message = new ServerMessage.AssignRoomId(roomId);
            Data.RoomId = roomId;
            Socket.Send(message.AsJson());
        }

        public void SendAnswerValidation(GameAnswerValidationResponse error)
        {
            var message = new ServerMessage.Game.SendAnswerValidation(error);
            Socket.Send(message.AsJson());
        }

        public void Update(ConnectionsHandler manager)
        {
            Console.WriteLine("Send update to " + Data.Name);
        }

        public void SendScores(Dictionary<PlayerData, uint> playerScores)
        {
            var message = new ServerMessage.Game.SendScores(playerScores);
            Socket.Send(message.AsJson());
        }

        public void SendTransitionPeriod(float time)
        {
            var message = new ServerMessage.Game.SendTransitionPeriod(time);
            Socket.Send(message.AsJson());
        }

        public override string ToString()
        {
            return Data.Name != null ? Data.Name : "Unassigned Name";
        }
    }
}
