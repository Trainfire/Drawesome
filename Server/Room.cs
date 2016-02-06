using System;
using System.Linq;
using System.Collections.Generic;
using Protocol;

namespace Server
{
    class Room : WebSocketBehaviour
    {
        public event EventHandler<Room> OnEmpty;

        public RoomData Data { get; set; }
        public List<Player> Players { get; private set; }

        Player Owner { get; set; }

        /// <summary>
        /// We'll limit GUID size to 4 characters for now to make joining games easier for development purposes.
        /// </summary>
        const int GuidSize = 4;

        public Room(Player owner, string password = "")
        {
            // TODO: TESTING!!!
            Data = new RoomData();
            Data.ID = Guid.NewGuid().ToString().Substring(0, GuidSize);
            Data.Password = password;
            Data.Players = new List<PlayerData>();

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
            Console.WriteLine("Player {0} joined room {1}", joiningPlayer.Data.Name, Data.ID);

            if (joiningPlayer.Data.ID != Owner.Data.ID && password != Data.Password)
            {
                Log("Player {0} provided incorrect password {1}. (Is {2})", joiningPlayer.Data.Name, password, Data.Password);
                joiningPlayer.SendRoomError(RoomError.InvalidPassword);
                return;
            }

            if (Players.Contains(joiningPlayer))
            {
                // TODO: Handle player rejoining room after disconnection?
                Log("Already contains player {0}", joiningPlayer.Data.Name);
                joiningPlayer.SendRoomError(RoomError.AlreadyInRoom);
            }
            else
            {
                Players.Add(joiningPlayer);
                Data.Players.Add(joiningPlayer.Data);

                // Add callbacks
                joiningPlayer.OnConnectionClosed += OnPlayerConnectionClosed;
                joiningPlayer.OnChat += OnPlayerChat;

                // Send message to joining player
                EchoActionToAll(joiningPlayer.Data, PlayerAction.Joined);

                SendUpdateToAll();
            }
        }

        public void Leave(PlayerData leavingPlayer)
        {
            var player = Players.Find(x => x.Data.ID == leavingPlayer.ID);
            OnPlayerConnectionClosed(this, new PlayerConnectionClosed(player, PlayerCloseReason.Left));
        }

        public bool HasPlayer(PlayerData player)
        {
            return Players.Exists(x => x.Data.ID == player.ID);
        }

        void OnPlayerChat(object sender, SharedMessage.Chat message)
        {
            var player = sender as Player;
            EchoChatToAll(message);
        }

        void OnPlayerConnectionClosed(object sender, PlayerConnectionClosed e)
        {
            // Remove callbacks
            e.Player.OnConnectionClosed -= OnPlayerConnectionClosed;
            e.Player.OnChat -= OnPlayerChat;

            // Inform players
            switch (e.CloseReason)
            {
                case PlayerCloseReason.Disconnected:
                    EchoActionToAll(e.Player.Data, PlayerAction.Disconnected);
                    break;
                case PlayerCloseReason.Left:
                    EchoActionToAll(e.Player.Data, PlayerAction.Left);
                    break;
                case PlayerCloseReason.Kicked:
                    EchoActionToAll(e.Player.Data, PlayerAction.Kicked);
                    break;
                default:
                    break;
            }

            Players.Remove(e.Player);

            var data = Data.Players.Find(x => x.ID == e.Player.Data.ID);
            Data.Players.Remove(data);

            // Assign a new owner
            if (Players.Count != 0)
            {
                if (Owner != Players[0])
                {
                    Owner = Players[0];
                    EchoActionToAll(Owner.Data, PlayerAction.PromotedToOwner);
                    SendUpdateToAll();
                }
            }
            else
            {
                // Remove room
                if (OnEmpty != null)
                    OnEmpty(this, this);
            }
        }

        #region Messaging

        void EchoActionToAll(PlayerData actor, PlayerAction action)
        {
            Log("{0} ({1})", actor.Name, action);
            Players.ForEach(x => x.SendAction(actor, action));
        }

        void EchoChatToAll(SharedMessage.Chat message)
        {
            Log("{0}: {1}", message.Player.Name, message);
            Players.ForEach(x => x.SendMessage(message));
        }

        void SendUpdateToAll()
        {
            Players.ForEach(x => x.SendMessage(new ServerMessage.RoomUpdate(Data)));
        }

        #endregion

        void Log(string message, params object[] args)
        {
            var str = string.Format(message, args);
            Console.WriteLine("Room {0} : {1}", Data.ID, str);
        }

    }
}
