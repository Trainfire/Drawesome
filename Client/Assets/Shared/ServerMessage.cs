using System;
using System.Collections.Generic;
using Protocol;
using System.Linq;

namespace Protocol
{
    public class ServerMessage
    {
        public class ConnectionSuccess : Message
        {
            public readonly string ID;

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
                    Type = MessageType.GameServerStateChange;
                    GameState = gameState;
                }
            }

            public class SendImage : Message
            {
                public byte[] Image;

                public SendImage(byte[] image)
                {
                    Type = MessageType.GameServerSendImage;
                    Image = image;
                }
            }

            public class SendPrompt : Message
            {
                public string Prompt { get; private set; }

                public SendPrompt(string prompt)
                {
                    Type = MessageType.GameServerSendPrompt;
                    Prompt = prompt;
                }
            }

            public class SendChoices : Message
            {
                public List<string> Choices;

                public SendChoices(List<string> choices)
                {
                    Type = MessageType.GameServerSendChoices;
                    Choices = choices;
                }
            }

            public class SendResult : Message
            {
                public PlayerData Author { get; private set; }
                public List<PlayerData> Players { get; private set; }
                public string Answer { get; private set; }
                public uint Points { get; private set; }

                public SendResult(PlayerData author, List<PlayerData> players, string answer, uint points)
                {
                    Type = MessageType.GameServerSendResult;
                    Author = author;
                    Players = players;
                    Answer = answer;
                    Points = points;
                }

                //public SendResult(KeyValuePair<AnswerData, ResultData> kvp)
                //{
                //    Type = MessageType.GameServerSendResult;
                //    Author = kvp.Key.Author;
                //    Answer = kvp.Key.Answer;
                //    Players = kvp.Value.Players;

                //    // TODO
                //    Points = 0;
                //}
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
