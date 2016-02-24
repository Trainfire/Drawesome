using System;
using System.Linq;
using System.Collections.Generic;
using Protocol;
using Server.Drawesome;
using Server.Game;

namespace Server
{
    public class Room : IConnectionMessageHandler, ILogger
    {
        public event EventHandler<Room> OnEmpty;

        public RoomData RoomData { get; private set; }
        public List<Player> Players { get; private set; }

        string ILogger.LogName { get { return string.Format("Room {0}", RoomData.ID); } }

        Player Owner { get; set; }
        DrawesomeGame Game { get; set; }
        IdPool RoomIdPool { get; set; }
        Settings Settings { get; set; }

        GameTimer CountdownTimer { get; set; }

        const int MaxPlayers = 8; // TODO: Place in Server settings

        /// <summary>
        /// We'll limit GUID size to 4 characters for now to make joining games easier for development purposes.
        /// </summary>
        const int GuidSize = 4;

        public Room(ConnectionsHandler connections, Player owner, Settings settings, string password = "")
        {
            connections.AddMessageListener(this);

            Settings = settings;

            Owner = owner;

            RoomIdPool = new IdPool(MaxPlayers);

            RoomData = new RoomData();
            RoomData.ID = Guid.NewGuid().ToString().Substring(0, GuidSize);
            RoomData.Password = password;
            RoomData.Players = new List<PlayerData>();
            RoomData.Owner = Owner.Data;

            Players = new List<Player>();

            if (string.IsNullOrEmpty(password))
            {
                Logger.Log(this, "Creating room for {0}", Owner.Data.Name);
            }
            else
            {
                Logger.Log(this, "Creating room for {0} with password {1}", Owner.Data.Name, password);
            }

            Join(Owner);

            Game = new DrawesomeGame(connections, settings);
        }

        #region Actions

        public void Join(Player joiningPlayer, string password = "")
        {
            Logger.Log(this, "Player {0} joined room {1}", joiningPlayer.Data.Name, RoomData.ID);

            // Prevent join if password is incorrect
            if (joiningPlayer.Data.ID != Owner.Data.ID && password != RoomData.Password)
            {
                Logger.Log(this, "Player {0} provided incorrect password {1}. (Is {2})", joiningPlayer.Data.Name, password, RoomData.Password);
                joiningPlayer.SendRoomError(RoomError.InvalidPassword);
                return;
            }

            // Prevent join if room is full
            if (Players.Count == Settings.Server.MaxPlayers)
            {
                Logger.Log(this, "Player {0} attempt to join the room {1} but that room is full", joiningPlayer.Data.Name, RoomData.ID);
                joiningPlayer.SendRoomError(RoomError.RoomFull);
                return;
            }

            // Prevent join if game has started
            if (RoomData.GameStarted)
            {
                Logger.Log(this, "Player {0} attempt to join the room {1} but the game has already started", joiningPlayer.Data.Name, RoomData.ID);
                joiningPlayer.SendRoomError(RoomError.GameAlreadyStarted);
                return;
            }

            if (Players.Contains(joiningPlayer))
            {
                // TODO: Handle player rejoining room after disconnection?
                Logger.Log(this, "Already contains player {0}", joiningPlayer.Data.Name);
                joiningPlayer.SendRoomError(RoomError.AlreadyInRoom);
            }
            else
            {
                // Assign colour
                var color = RoomIdPool.GetValue();
                joiningPlayer.AssignRoomId(color);

                Players.Add(joiningPlayer);
                RoomData.Players.Add(joiningPlayer.Data);

                // Add callbacks
                joiningPlayer.OnConnectionClosed += OnPlayerConnectionClosed;

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

        /// <summary>
        /// Begins a countdown timer. The game will begin if the timer reaches 0.
        /// </summary>
        void StartCountdown()
        {
            // Notify players
            Players.ForEach(x => x.NotifyRoomCountdownStart(5f));

            // Start game when countdown reaches 0
            CountdownTimer = new GameTimer("Countdown", 5f, () =>
            {
                Logger.Log(this, "{0} has started the game", Owner.Data.Name);
                Game.Start(Players);
                Game.OnEnd += OnGameEnd;
            });
        }

        void OnGameEnd(object sender, EventArgs e)
        {
            Game.OnEnd -= OnGameEnd;
            RoomData.GameStarted = false;
            SendUpdateToAll();
        }

        /// <summary>
        /// Cancels the countdown timer.
        /// </summary>
        void CancelCountdown()
        {
            // Notify players
            Players.ForEach(x => x.NotifyRoomCountdownCancel());

            if (CountdownTimer != null)
                CountdownTimer.Stop();

            CountdownTimer = null;
        }

        #endregion

        void IConnectionMessageHandler.HandleMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendAction>(json, (data) =>
            {
                if (player == Owner)
                {
                    switch (data.Action)
                    {
                        case GameAction.Start:
                            Logger.Log(this, "{0} has started the game", Owner.Data.Name);
                            RoomData.GameStarted = true;
                            StartCountdown();
                            break;

                        case GameAction.CancelStart:
                            Logger.Log(this, "{0} has cancelled the game from starting", Owner.Data.Name);
                            RoomData.GameStarted = false;
                            CancelCountdown();
                            break;

                        case GameAction.Restart:
                            Logger.Log(this, "{0} has restarted the game", Owner.Data.Name);
                            Game.Restart();
                            break;

                        case GameAction.StartNewRound:
                            Logger.Log(this, "{0} has started a new round", Owner.Data.Name);
                            Game.StartNewRound();
                            break;

                        default:
                            break;
                    }

                    SendUpdateToAll();
                }
            });

            Message.IsType<ClientMessage.SendChat>(json, (data) =>
            {
                Logger.Log(this, "{0}: {1}", player.Data.Name, data.Message);
                // Echo chat to all clients
                Players.ForEach(x => x.SendChat(player.Data, data.Message));
            });
        }

        void OnPlayerConnectionClosed(object sender, PlayerConnectionClosed e)
        {
            // Remove callbacks
            e.Player.OnConnectionClosed -= OnPlayerConnectionClosed;

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

            // Remove player
            var data = RoomData.Players.Find(x => x.ID == e.Player.Data.ID);
            RoomData.Players.Remove(data);
            Players.Remove(e.Player);

            // Return color to pool
            RoomIdPool.ReturnValue(e.Player.Data.RoomId);

            if (Players.Count != 0)
            {
                // Assign a new owner
                if (Owner != Players[0])
                {
                    Owner = Players[0];
                    RoomData.Owner = Owner.Data;
                    EchoActionToAll(Owner.Data, PlayerAction.PromotedToOwner);
                    SendUpdateToAll();
                }
            }
            else
            {
                // Remove room if no players remain
                Game.End();

                if (OnEmpty != null)
                    OnEmpty(this, this);
            }
        }

        #region Messaging

        void EchoActionToAll(PlayerData actor, PlayerAction action)
        {
            Logger.Log(this, "{0} ({1})", actor.Name, action);
            Players.ForEach(x => x.SendAction(actor, action));
        }

        void SendUpdateToAll()
        {
            Players.ForEach(x => x.SendMessage(new ServerMessage.RoomUpdate(RoomData)));
        }

        #endregion

        #region Helpers

        bool IsOwner(PlayerData player)
        {
            return Owner.Data.ID == player.ID;
        }

        public bool HasPlayer(PlayerData player)
        {
            return Players.Exists(x => x.Data.ID == player.ID);
        }

        #endregion
    }
}
