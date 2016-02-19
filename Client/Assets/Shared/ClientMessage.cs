using System;
using Protocol;

namespace Protocol
{
    public class ClientMessage : Message
    {
        public class RequestConnection : Message
        {
            public PlayerData PlayerInfo { get; private set; }
            public string Name { get; private set; }

            public RequestConnection(PlayerData playerInfo, string name)
            {
                PlayerInfo = playerInfo;
                Name = name;
            }
        }

        public class CreateRoom : Message
        {
            public string Password;

            public CreateRoom(PlayerData playerInfo, string password)
            {
                Password = password;
            }
        }

        public class JoinRoom : Message
        {
            public string RoomId;
            public string Password;

            public JoinRoom(PlayerData playerInfo, string roomId, string password = "")
            {
                RoomId = roomId;
                Password = password;
            }
        }

        public class LeaveRoom : Message
        {
            public LeaveRoom(PlayerData playerInfo)
            {

            }
        }

        public class RequestRoomList : Message
        {
            public RequestRoomList(PlayerData playerInfo)
            {

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
