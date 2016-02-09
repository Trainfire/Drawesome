using System;
using System.Collections.Generic;
using Protocol;

namespace Protocol
{
    public class ServerMessage
    {
        public class ConnectionSuccess : Message
        {
            public string ID;

            public ConnectionSuccess(string id)
            {
                Type = MessageType.ServerConnectionSuccess;
                ID = id;
            }
        }

        public class RoomList : Message
        {
            public List<RoomData> Rooms { get; private set; }

            public RoomList(List<RoomData> rooms)
            {
                Type = MessageType.ServerSendRoomList;
                Rooms = rooms;
            }
        }

        public class NotifyPlayerAction : Message
        {
            public PlayerData Player;
            public PlayerAction Action;

            public NotifyPlayerAction(PlayerData player, PlayerAction action)
            {
                Type = MessageType.ServerNotifyPlayerAction;
                Player = player;
                Action = action;
            }
        }

        public class NotifyRoomError : Message
        {
            public RoomError Notice;

            public NotifyRoomError(RoomError notice)
            {
                Type = MessageType.ServerNotifyRoomError;
                Notice = notice;
            }
        }

        public class RoomUpdate : Message
        {
            public RoomData RoomData;

            public RoomUpdate(RoomData roomData)
            {
                Type = MessageType.ServerRoomUpdate;
                RoomData = roomData;
            }
        }

        #region Game

        public class Game
        {
            public class StateChange : Message
            {
                public GameState GameState;

                public StateChange(GameState gameState)
                {
                    Type = MessageType.GameStateChange;
                    GameState = gameState;
                }
            }

            public class ShowImage : Message
            {
                public PlayerData Player;
                public byte[] Image;

                public ShowImage(PlayerData player, byte[] image)
                {
                    Player = player;
                    Image = image;
                }
            }

            public class ShowOptions : Message
            {
                public List<AnswerData> Answers;

                public ShowOptions(List<AnswerData> answers)
                {
                    Answers = answers;
                }
            }

            public class ShowChoice : Message
            {
                public ResultData Choice;

                public ShowChoice(ResultData choice)
                {
                    Choice = choice;
                }
            }

            public class ShowChosenPlayerOptions : Message
            {
                public List<ResultData> Choices;

                public ShowChosenPlayerOptions(List<ResultData> choices)
                {
                    Choices = choices;
                }
            }

            public class ShowLikes : Message
            {
                public List<ResultData> Choices;

                public ShowLikes(List<ResultData> choices)
                {
                    Choices = choices;
                }
            }

            public class PlayerAction : Message
            {
                public PlayerData Actor { get; private set; }

                public PlayerAction(PlayerData actor)
                {
                    Actor = actor;
                }
            }
        }

        #endregion
    }
}
