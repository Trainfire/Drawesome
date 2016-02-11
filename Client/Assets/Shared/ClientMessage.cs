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
                Type = MessageType.ClientConnectionRequest;
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
                Type = MessageType.ClientCreateRoom;
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
                Type = MessageType.ClientJoinRoom;
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
                Type = MessageType.ClientLeaveRoom;
                Player = player;
            }
        }

        public class StartGame : Message
        {
            public PlayerData Player { get; private set; }

            public StartGame(PlayerData player)
            {
                Type = MessageType.ClientStartGame;
                Player = player;
            }
        }

        public class RequestRoomList : Message
        {
            public PlayerData Player;

            public RequestRoomList(PlayerData player)
            {
                Type = MessageType.ClientRequestRoomList;
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
                    Type = MessageType.GameClientSendImage;
                    Image = image;
                }
            }

            public class SubmitAnswer : Message
            {
                public string Answer { get; private set; }

                public SubmitAnswer(string answer)
                {
                    Type = MessageType.GameClientSubmitAnswer;
                    Answer = answer;
                }
            }

            public class SubmitChoice : Message
            {
                public string Choice { get; set; }

                public SubmitChoice(string choice)
                {
                    Type = MessageType.GameClientSubmitChoice;
                    Choice = choice;
                }
            }

            public class LikeAnswer : Message
            {
                public string Answer { get; set; }

                public LikeAnswer(string answer)
                {
                    Type = MessageType.GameClientSubmitLike;
                    Answer = answer;
                }
            }

            public class SkipPhase : Message
            {
                public SkipPhase()
                {
                    Type = MessageType.GameClientSkipPhase;
                }
            }
        }

        #endregion
    }
}
