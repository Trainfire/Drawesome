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

        public GameData(Settings settings)
        {
            Settings = settings;
        }
    }
}
