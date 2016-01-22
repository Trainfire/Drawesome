using System;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    class PlayerManager : WebSocketBehaviour
    {
        public Dictionary<IWebSocketConnection, Player> Players { get; private set; }

        public override void OnOpen(IWebSocketConnection socket)
        {
            base.OnOpen(socket);

            // Assign a unique ID to the player and send it back to the client.
            var player = AddPlayer(socket);
            ProtocolHelper.Send(socket, new ValidatePlayer(player.ID));
        }

        public override void OnMessage(string m)
        {
            // Validates a player's request to change name. Usually sent on a first-time connection to the server after server-side validation.
            var message = Newtonsoft.Json.JsonConvert.DeserializeObject<PlayerConnectMessage>(m);
            if (message != null)
            {
                Logger.WriteLine(message.ID);
                foreach (var player in Players.Values)
                {
                    if (player.ID == message.ID)
                    {
                        player.Name = message.PlayerName;
                        Logger.WriteLine("Player {0} connected.", player.Name);
                        return;
                    }
                }
            }
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);

            if (Players.ContainsKey(socket))
            {
                var player = Players[socket];
                Players.Remove(socket);
                Logger.WriteLine("Player {0} disconnected", player.Name);
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (Players == null)
                Players = new Dictionary<IWebSocketConnection, Player>();

            Player player = null;

            if (!Players.ContainsKey(socket))
            {
                Logger.WriteLine("A new player connected.");

                player = new Player("N/A", socket);
                player.ID = Guid.NewGuid().ToString();

                Players.Add(socket, player);
            }
            else
            {
                Logger.WriteLine("Player already exists...");
                player = Players[socket];
            }

            return player;
        }
    }
}
