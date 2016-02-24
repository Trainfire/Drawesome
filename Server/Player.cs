using Fleck;
using Protocol;
using System.Collections.Generic;
using System;
using System.Linq;

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
        public event EventHandler<ClientMessage.Game.SendAction> OnGameAction;
        public event EventHandler<Message> OnMessage;
        public event EventHandler<string> OnMessageString;

        public PlayerData Data { get; private set; }
        public IWebSocketConnection Socket { get; private set; }
        public bool IsAdmin { get; private set; }

        Settings Settings { get; set; }

        public Player(string playerName, IWebSocketConnection socket, Settings settings)
        {
            Settings = settings;
            Socket = socket;

            Data = new PlayerData();
            Data.Name = playerName;
            Data.ID = Guid.NewGuid().ToString();

            socket.OnClose += () =>
            {
                if (OnConnectionClosed != null)
                    OnConnectionClosed(this, new PlayerConnectionClosed(this, PlayerCloseReason.Disconnected));
            };

            socket.OnMessage += (json) => OnPlayerMessage(json);
        }

        void OnPlayerMessage(string json)
        {
            Message.IsType<ClientMessage.RequestAdmin>(json, (data) =>
            {
                if (data.Password == Settings.Server.AdminPassword)
                {
                    Logger.Log("Admin granted to {0}", Data.Name);
                    IsAdmin = true;
                }
            });

            Message.IsType<SharedMessage.Chat>(json, (data) =>
            {
                if (OnChat != null)
                    OnChat(this, data);
            });

            Message.IsType<ClientMessage.Game.SendAction>(json, (data) =>
            {
                if (OnGameAction != null)
                    OnGameAction(this, data);
            });

            var obj = JsonHelper.FromJson<Message>(json);

            if (OnMessage != null)
                OnMessage(this, obj);

            if (OnMessageString != null)
                OnMessageString(this, json);
        }

        public override string ToString()
        {
            return Data.Name != null ? Data.Name : "Unassigned Name";
        }

        #region Messaging

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

        public void RequestClientName()
        {
            var message = new ServerMessage.RequestClientName();
            Socket.Send(message.AsJson());
        }

        public void UpdatePlayerInfo(PlayerData playerData)
        {
            var message = new ServerMessage.UpdatePlayerInfo(playerData);
            Socket.Send(message.AsJson());
        }

        public void SendRoomJoinNotice(RoomNotice roomNotice)
        {
            var message = new ServerMessage.NotifyRoomJoin(roomNotice);
            Socket.Send(message.AsJson());
        }

        public void SendRoomLeaveReason(RoomLeaveReason reason)
        {
            var message = new ServerMessage.NotifyRoomLeave(reason);
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

        public void SendConnectionError(ConnectionError connectionError)
        {
            var message = new ServerMessage.SendConnectionError(connectionError);
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

        public void NotifyRoomCountdownStart(float duration)
        {
            var message = new ServerMessage.NotifyRoomCountdown(duration);
            Socket.Send(message.AsJson());
        }

        public void NotifyRoomCountdownCancel()
        {
            var message = new ServerMessage.NotifyRoomCountdownCancel();
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

        public void SendScores(Dictionary<PlayerData, ScoreData> playerScores, bool orderByDescending = false)
        {
            var message = new ServerMessage.Game.SendScores(playerScores, orderByDescending);
            Socket.Send(message.AsJson());
        }

        public void SendTransitionPeriod(float time)
        {
            var message = new ServerMessage.Game.SendTransitionPeriod(time);
            Socket.Send(message.AsJson());
        }

        #endregion
    }
}
