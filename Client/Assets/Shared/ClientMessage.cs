using System;
using Protocol;

namespace Protocol
{
    public class ClientMessage
    {
        public class RequestConnection : Message
        {
            public string ID;
            public string PlayerName;

            public RequestConnection(string id, string playerName)
            {
                ID = id;
                PlayerName = playerName;
            }
        }

        public class CreateRoom : Message
        {
            public PlayerData Player;
            public string Password;

            public CreateRoom(PlayerData player, string password)
            {
                Player = player;
                Password = password;
            }
        }

        public class JoinRoom : Message
        {
            public PlayerData Player;
            public string RoomId;
            public string Password;

            public JoinRoom(PlayerData player, string roomId, string password = "")
            {
                Player = player;
                RoomId = roomId;
                Password = password;
            }
        }

        public class LeaveRoom : Message
        {
            public PlayerData Player;

            public LeaveRoom(PlayerData player)
            {
                Player = player;
            }
        }

        public class RequestRoomList : Message
        {
            public PlayerData Player;

            public RequestRoomList(PlayerData player)
            {
                Player = player;
            }
        }

        #region Game

        public class Game
        {
            public class SendImage : Message
            {
                public byte[] Image { get; private set; }

                public SendImage(byte[] image)
                {
                    Image = image;
                }
            }

            public class SubmitAnswer : Message
            {
                public string Answer { get; private set; }

                public SubmitAnswer(string answer)
                {
                    Answer = answer;
                }
            }

            public class SubmitChoice : Message
            {
                public string ChosenAnswer { get; private set; }

                public SubmitChoice(string chosenAnswer)
                {
                    ChosenAnswer = chosenAnswer;
                }
            }

            public class LikeAnswer : Message
            {
                public string Answer { get; set; }

                public LikeAnswer(string answer)
                {
                    Answer = answer;
                }
            }

            public class SkipPhase : Message
            {
                public SkipPhase()
                {

                }
            }

            public class SendAction : Message
            {
                public PlayerData Player { get; private set; }
                public GameAction Action { get; private set; }

                public SendAction(PlayerData player, GameAction action)
                {
                    Player = player;
                    Action = action;
                }
            }
        }

        #endregion
    }
}
