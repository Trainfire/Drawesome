using System;
using System.Linq;
using System.Collections.Generic;
using Protocol;

namespace Server
{
    class Room : WebSocketBehaviour
    {
        public RoomData Data { get; set; }

        List<Player> Players { get; set; }
        Player Owner { get; set; }

        public Room(Player owner, string password = "")
        {
            // TODO: TESTING!!!
            Data = new RoomData();
            Data.ID = Guid.Empty.ToString();
            Data.Password = password;

            Players = new List<Player>();
            Owner = owner;

            if (string.IsNullOrEmpty(password))
            {
                Console.WriteLine("Creating room for {0}", Owner.Data.Name);
            }
            else
            {
                Console.WriteLine("Creating room for {0} with password {1}", Owner.Data.Name, password);
            }

            Join(Owner);
        }

        public void Join(Player joiningPlayer, string password = "")
        {
            // Check validity of password and notify player if incorrect
            if (password != Data.Password)
            {
                joiningPlayer.SendMessage(new ServerMessage.RoomJoinError(RoomError.InvalidPassword));
                return;
            }

            if (Players.Contains(joiningPlayer))
            {
                // TODO: Handle player rejoining room after disconnection?
            }
            else
            {
                Players.Add(joiningPlayer);
            }

            Console.WriteLine("Player {0} joined room {1}", joiningPlayer.Data.Name, Data.ID);

            // Add callbacks
            joiningPlayer.ConnectionClosed += OnPlayerConnectionClosed;

            // Remove player if they leave
            joiningPlayer.Socket.OnMessage += (message) =>
            {
                // TODO: Trash
                var obj = JsonHelper.FromJson<Message>(message);
                if (obj.Type == MessageType.ClientLeaveRoom)
                {
                    OnPlayerConnectionClosed(this, new PlayerConnectionClosed(joiningPlayer, PlayerCloseReason.Left));
                }
            };

            // Send message to joining player
            SendMessage(joiningPlayer, "You joined the room {0}", Data.ID);
            SendMessageToAll("{0} joined the room", joiningPlayer.Data.Name);
        }

        void OnPlayerConnectionClosed(object sender, PlayerConnectionClosed e)
        {
            Players.Remove(e.Player);
            e.Player.ConnectionClosed -= OnPlayerConnectionClosed;

            // Inform players
            switch (e.CloseReason)
            {
                case PlayerCloseReason.Disconnected:
                    SendMessageToAll("{0} disconnected.", e.Player);
                    break;
                case PlayerCloseReason.Left:
                    SendMessageToAll("{0} left.", e.Player);
                    break;
                case PlayerCloseReason.Kicked:
                    SendMessageToAll("{0} was kicked by {1}.", e.Player, Owner.Data.Name);
                    break;
                default:
                    break;
            }

            // Assign a new owner
            if (Players.Count != 0)
            {
                Owner = Players[0];
                SendMessage(Owner, "You are now the room owner");
                SendMessageToAll("{0} is now the room owner", Owner.Data.Name);
            }

            Console.WriteLine("{0} left the room. ({1})", e.Player.Data.Name, e.CloseReason);
        }

        #region Messaging

        void SendMessage(Player player, string message, params object[] args)
        {
            Console.WriteLine("Sending message: {0}", message);
            player.SendMessage(new Log(message, args));
        }

        void SendMessage(Message message, Player player)
        {
            player.SendMessage(message);
        }

        void SendMessageToAll(string message, params object[] args)
        {
            Console.WriteLine("Sending message: {0}", message);
            Players.ForEach(x => x.SendMessage(new Log(message, args)));
        }

        void SendMessageToAll(Message message)
        {
            Players.ForEach(x => x.SendMessage(message));
        }

        void SendMessageToAll(Message message, Player exception)
        {
            var allowedPlayers = Players.Where(x => x != exception).ToList();
            allowedPlayers.ForEach(x => x.SendMessage(message));
        }

        #endregion

    }
}
