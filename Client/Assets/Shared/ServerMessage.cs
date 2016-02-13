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
                ID = id;
            }
        }

        public class RoomList : Message
        {
            public List<RoomData> Rooms { get; private set; }

            public RoomList(List<RoomData> rooms)
            {
                Rooms = rooms;
            }
        }

        public class NotifyPlayerAction : Message
        {
            public PlayerData Player;
            public PlayerAction Action;

            public NotifyPlayerAction(PlayerData player, PlayerAction action)
            {
                Player = player;
                Action = action;
            }
        }

        public class NotifyRoomError : Message
        {
            public RoomError Notice;

            public NotifyRoomError(RoomError notice)
            {
                Notice = notice;
            }
        }

        public class RoomUpdate : Message
        {
            public RoomData RoomData;

            public RoomUpdate(RoomData roomData)
            {
                RoomData = roomData;
            }
        }

        public class AssignRoomId : Message
        {
            public uint RoomId;

            public AssignRoomId(uint roomId)
            {
                RoomId = roomId;
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
                    GameState = gameState;
                }
            }

            public class SendImage : Message
            {
                public byte[] Image;

                public SendImage(byte[] image)
                {
                    Image = image;
                }
            }

            public class SendPrompt : Message
            {
                public string Prompt { get; private set; }

                public SendPrompt(string prompt)
                {
                    Prompt = prompt;
                }
            }

            public class SendChoices : Message
            {
                public List<string> Choices;

                public SendChoices(List<string> choices)
                {
                    Choices = choices;
                }
            }

            public class SendResult : Message
            {
                public ResultData Result;

                public SendResult(ResultData result)
                {
                    Result = result;
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

            public class AddTimer : Message
            {
                public float Duration { get; private set; }

                public AddTimer(float duration)
                {
                    Duration = duration;
                }
            }

            public class SetTimer : Message
            {
                public float CurrentTime { get; private set; }

                public SetTimer(float currentTime)
                {
                    CurrentTime = currentTime;
                }
            }

            public class SendAnswerValidation : Message
            {
                public GameAnswerError Error { get; private set; }

                public SendAnswerValidation(GameAnswerError error)
                {
                    Error = error;
                }
            }
        }

        #endregion
    }
}
