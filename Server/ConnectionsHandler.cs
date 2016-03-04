using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using System.Text;
using Protocol;

namespace Server
{
    /// <summary>
    /// This class handles clients connecting and disconnecting, as well as messages.
    /// Messages are relayed to any assigned handlers that implement IConnectionMessageHandler.
    /// </summary>
    public class ConnectionsHandler : ILogger
    {
        public event EventHandler<Player> PlayerConnected;
        public event EventHandler<Player> PlayerDisconnected;

        public string LogName { get { return "Connections Handler"; } }

        List<IConnectionMessageHandler> Handlers { get; set; }
        List<Player> ConnectedPlayers { get; set; }
        SettingsLoader SettingsLoader { get; set; }

        static readonly object _lock = new object();

        public ConnectionsHandler(SettingsLoader settings)
        {
            SettingsLoader = settings;
            Handlers = new List<IConnectionMessageHandler>();
        }

        public void AddMessageListener(IConnectionMessageHandler handler)
        {
            if (!Handlers.Contains(handler))
                Handlers.Add(handler);
        }

        public void RemoveMessageListener(IConnectionMessageHandler handler)
        {
            if (Handlers.Contains(handler))
                Handlers.Remove(handler);
        }

        public void OnOpen(IWebSocketConnection socket)
        {
            Logger.Log(this, "A connection was opened. (Socket ID: {0})", socket.ConnectionInfo.Id);

            // Respond to a join request by assigning a unique ID to the connection and sending it back to the client
            var player = AddPlayer(socket);
            player.RequestClientInfo();

            // Wait for player reponse with their name
            player.Socket.OnBinary += (binary) =>
            {
                // Messages are sent as binary from Unity (the WebGL wrapper only sends binary for some reason)
                var message = Encoding.UTF8.GetString(binary);

                Message.IsType<ClientMessage.GiveClientInfo>(message, (data) =>
                {
                    if (data.ProtocolVersion != ProtocolInfo.Version)
                    {
                        // Notify protocol mismatch. Client and Server must match!
                        player.SendConnectionError(ConnectionError.ProtocolMismatch);
                        return;
                    }

                    // Check if name is within character range
                    bool nameWithinCharLimit = data.Name.Length >= SettingsLoader.Values.Server.NameMinChars && data.Name.Length <= SettingsLoader.Values.Server.NameMaxChars;

                    // Check if an existing connection already has that name
                    bool nameisUnique = !ConnectedPlayers.Any(x => x.Data.Name.ToLower() == data.Name.ToLower());

                    if (nameWithinCharLimit && nameisUnique)
                    {
                        OnConnectionSuccessful(player, data);
                    }
                    else if (!nameWithinCharLimit)
                    {
                        player.SendConnectionError(ConnectionError.InvalidNameLength);
                    }
                    else
                    {
                        player.SendConnectionError(ConnectionError.MatchesExistingName);
                    }
                });
            };
        }

        void OnConnectionSuccessful(Player player, ClientMessage.GiveClientInfo data)
        {
            // Assign the requested name and send the final Server copy of the player data
            player.Data.SetName(data.Name);

            Logger.Log(this, "Player {0} connected.", player.Data.Name);

            // Inform the player that the connection was successful
            player.NotifyConnectionSuccess();

            // Send the player the latest version of their server-side data (they need to know their GUID)
            player.UpdatePlayerInfo(player.Data);

            // Send Player Joined message.
            NotifyPlayerEvent(player, PlayerAction.Connected);

            // Trigger event.
            if (PlayerConnected != null)
                PlayerConnected(this, player);

            // Assign callback
            player.OnMessage += OnPlayerMessage;
        }

        /// <summary>
        /// Passes a player's message to any interested listeners.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="json"></param>
        void OnPlayerMessage(object sender, string json)
        {
            var player = sender as Player;
            Handlers.ToList().ForEach(x => x.HandleMessage(player, json));
        }

        public void OnClose(IWebSocketConnection socket)
        {
            // Remove disconnected player from manager.
            var player = ConnectedPlayers.Find(x => x.Socket == socket);
            if (player != null)
            {
                ConnectedPlayers.Remove(player);

                Logger.Log(this, "Player {0} disconnected", player.Data.Name);

                NotifyPlayerEvent(player, PlayerAction.Disconnected);

                if (PlayerDisconnected != null)
                    PlayerDisconnected(this, player);
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (ConnectedPlayers == null)
                ConnectedPlayers = new List<Player>();

            Player player = GetPlayerFromSocket(socket);

            if (player == null)
            {
                player = new Player("N/A", socket, SettingsLoader.Values);
                player.Data.ID = Guid.NewGuid().ToString();

                ConnectedPlayers.Add(player);
            }
            else
            {
                Logger.Warn(this, "Player {0} already exists...", player.Data.Name);
            }

            return player;
        }

        public void NotifyPlayerEvent(Player player, PlayerAction action)
        {
            ConnectedPlayers.ForEach(x => x.SendAction(player.Data, action, PlayerActionContext.Global));
        }

        Player GetPlayerFromSocket(IWebSocketConnection socket)
        {
            return ConnectedPlayers.FirstOrDefault(x => x.Socket == socket);
        }
    }
}
