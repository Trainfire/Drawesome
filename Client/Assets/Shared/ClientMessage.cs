using System;
using Protocol;

namespace Protocol
{
    [Serializable]
    public class ClientMessage : Message
    {
        [Serializable]
        public class GiveName : Message
        {
            public string Name;

            public GiveName(string name)
            {
                Name = name;
            }
        }

        [Serializable]
        public class CreateRoom : Message
        {
            public string Password;

            public CreateRoom(PlayerData playerInfo, string password)
            {
                Password = password;
            }
        }

        [Serializable]
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

        [Serializable]
        public class LeaveRoom : Message
        {
            public LeaveRoom(PlayerData playerInfo)
            {

            }
        }

        [Serializable]
        public class RequestRoomList : Message
        {
            public RequestRoomList(PlayerData playerInfo)
            {

            }
        }

        [Serializable]
        public class SendChat : Message
        {
            public string Message;

            public SendChat(string message)
            {
                Message = message;
            }
        }

        #region Game

        [Serializable]
        public class Game
        {
            [Serializable]
            public class SendImage : Message
            {
                public byte[] Image;

                public SendImage(byte[] image)
                {
                    Image = image;
                }
            }

            [Serializable]
            public class SubmitAnswer : Message
            {
                public string Answer;

                public SubmitAnswer(string answer)
                {
                    Answer = answer;
                }
            }

            [Serializable]
            public class SubmitChoice : Message
            {
                public string ChosenAnswer;

                public SubmitChoice(string chosenAnswer)
                {
                    ChosenAnswer = chosenAnswer;
                }
            }

            [Serializable]
            public class LikeAnswer : Message
            {
                public string Answer;

                public LikeAnswer(string answer)
                {
                    Answer = answer;
                }
            }

            [Serializable]
            public class SkipPhase : Message
            {
                public SkipPhase()
                {

                }
            }

            [Serializable]
            public class SendAction : Message
            {
                public PlayerData Player;
                public GameAction Action;

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
