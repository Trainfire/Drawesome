using System;
using System.Linq;
using System.Collections.Generic;
using Fleck;
using Protocol;

namespace Server
{
    class PlayerManager : WebSocketBehaviour
    {
        public List<Player> Players { get; private set; }

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
            var playerConnectMessage = JsonHelper.FromJson<PlayerConnectMessage>(m);
            if (playerConnectMessage != null)
            {
                Logger.WriteLine(playerConnectMessage.ID);
                foreach (var player in Players)
                {
                    if (player.ID == playerConnectMessage.ID)
                    {
                        player.Name = playerConnectMessage.PlayerName;
                        Logger.WriteLine("Player {0} connected.", player.Name);
                        return;
                    }
                }
            }
        }

        public override void OnClose(IWebSocketConnection socket)
        {
            base.OnClose(socket);

            var player = Players.Find(x => x.Socket == socket);
            if (player != null)
            {
                Players.Remove(player);
                Logger.WriteLine("Player {0} disconnected", player.Name);
            }
        }

        Player AddPlayer(IWebSocketConnection socket)
        {
            if (Players == null)
                Players = new List<Player>();

            Player player = GetPlayerFromSocket(socket);

            if (player == null)
            {
                Logger.WriteLine("A new player connected.");

                player = new Player("N/A", socket);
                player.ID = Guid.NewGuid().ToString();

                Players.Add(player);
            }
            else
            {
                Logger.WriteLine("Player {0} already exists...", player.Name);
            }

            return player;
        }

        /// <summary>
        /// Attemps to find a player with the associated socket. Returns null if not found.
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        Player GetPlayerFromSocket(IWebSocketConnection socket)
        {
            return Players.FirstOrDefault(x => x.Socket == socket);
        }
    }
}
