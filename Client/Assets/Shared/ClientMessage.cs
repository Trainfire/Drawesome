using System;
using Protocol;

namespace Protocol
{
    public class ClientMessage : Message
    {
        public class RequestAdmin : Message
        {
            public string Password;

            public RequestAdmin(string password)
            {
                Password = password;
            }
        }

        public class GiveClientInfo : Message
        {
            public string Name;
            public uint ProtocolVersion;

            public GiveClientInfo(string name, uint protocolVersion)
            {
                Name = name;
                ProtocolVersion = protocolVersion;
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

        public class SendChat : Message
        {
            public string Message;

            public SendChat(string message)
            {
                Message = message;
            }
        }

        #region Game

        public class Game
        {
            public class SendImage : Message
            {
                public byte[] Image;

                public SendImage(byte[] image)
                {
                    Image = image;
                }
            }

            public class SubmitAnswer : Message
            {
                public string Answer;

                public SubmitAnswer(string answer)
                {
                    Answer = answer;
                }
            }

            public class SubmitChoice : Message
            {
                public string ChosenAnswer;

                public SubmitChoice(string chosenAnswer)
                {
                    ChosenAnswer = chosenAnswer;
                }
            }

            public class LikeAnswer : Message
            {
                public string Answer;

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
