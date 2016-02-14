using System.Collections.Generic;

namespace Server.Game
{
    public class GameData
    {
        public Settings Settings { get; set; }
        public List<Player> Players { get; set; }

        public GameData()
        {

        }

        public GameData(List<Player> players, Settings settings)
        {
            Players = players;
            Settings = settings;
        }
    }
}
