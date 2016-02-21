using System;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using Newtonsoft.Json;

namespace Protocol
{
    [Serializable]
    public class ServerMessage
    {
        [Serializable]
        public class AssignClientId : Message
        {
            public string ID;

            public AssignClientId(string id)
            {
                ID = id;
            }
        }

        [Serializable]
        public class NotifyConnectionSuccess : Message
        {
            public NotifyConnectionSuccess()
            {

            }
        }

        [Serializable]
        public class RequestClientName : Message
        {
            public RequestClientName()
            {

            }
        }

        [Serializable]
        public class UpdatePlayerInfo : Message
        {
            public int SomeInt;
            public PlayerData PlayerData;

            public UpdatePlayerInfo(int someInt, PlayerData playerData)
            {
                SomeInt = someInt;
                PlayerData = playerData;
            }
        }

        [Serializable]
        public class RoomList : Message
        {
            public List<RoomData> Rooms;

            public RoomList(List<RoomData> rooms)
            {
                Rooms = rooms;
            }
        }

        [Serializable]
        public class NotifyChatMessage : Message
        {
            public PlayerData Player;
            public string Message;

            public NotifyChatMessage(PlayerData player, string message)
            {
                Player = player;
                Message = message;
            }
        }

        [Serializable]
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

        [Serializable]
        public class NotifyRoomError : Message
        {
            public RoomError Notice;

            public NotifyRoomError(RoomError notice)
            {
                Notice = notice;
            }
        }

        [Serializable]
        public class RoomUpdate : Message
        {
            public RoomData RoomData;

            public RoomUpdate(RoomData roomData)
            {
                RoomData = roomData;
            }
        }

        [Serializable]
        public class AssignRoomId : Message
        {
            public uint RoomId;

            public AssignRoomId(uint roomId)
            {
                RoomId = roomId;
            }
        }

        #region Game

        [Serializable]
        public class Game
        {
            [Serializable]
            public class ChangeState : Message
            {
                public GameState GameState;

                public ChangeState(GameState gameState)
                {
                    GameState = gameState;
                }
            }

            [Serializable]
            public class EndState : Message
            {
                public GameStateEndReason Reason;

                public EndState(GameStateEndReason reason)
                {
                    Reason = reason;
                }
            }

            [Serializable]
            public class SendTransitionPeriod : Message
            {
                public float Duration;

                public SendTransitionPeriod(float duration)
                {
                    Duration = duration;
                }
            }

            [Serializable]
            public class SendImage : Message
            {
                public DrawingData Drawing;

                public SendImage(DrawingData drawing)
                {
                    Drawing = drawing;
                }
            }

            [Serializable]
            public class SendPrompt : Message
            {
                public string Prompt;

                public SendPrompt(string prompt)
                {
                    Prompt = prompt;
                }
            }

            [Serializable]
            public class SendChoices : Message
            {
                public PlayerData Creator;
                public List<AnswerData> Choices;

                public SendChoices(PlayerData creator, List<AnswerData> choices)
                {
                    Creator = creator;
                    Choices = choices;
                }
            }

            [Serializable]
            public class SendResult : Message
            {
                public AnswerData Answer;

                public SendResult(AnswerData answer)
                {
                    Answer = answer;
                }
            }

            public interface ISendScores
            {
                void SendScores(Dictionary<PlayerData, uint> playerScores);
            }

            [Serializable]
            public class SendScores : Message
            {
                public List<PlayerData> Players;
                public List<uint> Scores;

                public SendScores(Dictionary<PlayerData, uint> playerScores)
                {
                    Players = new List<PlayerData>();
                    Scores = new List<uint>();

                    if (playerScores != null)
                    {
                        Players = playerScores.Keys.ToList();
                        Scores = playerScores.Values.ToList();
                    }   
                }
            }

            [Serializable]
            public class PlayerAction : Message
            {
                public PlayerData Actor;
                public GamePlayerAction Action;

                public PlayerAction(PlayerData actor, GamePlayerAction action)
                {
                    Actor = actor;
                    Action = action;
                }
            }

            [Serializable]
            public class AddTimer : Message
            {
                public float Duration;

                public AddTimer(float duration)
                {
                    Duration = duration;
                }
            }

            [Serializable]
            public class SetTimer : Message
            {
                public float CurrentTime;

                public SetTimer(float currentTime)
                {
                    CurrentTime = currentTime;
                }
            }

            [Serializable]
            public class SendAnswerValidation : Message
            {
                public GameAnswerValidationResponse Response;

                public SendAnswerValidation(GameAnswerValidationResponse response)
                {
                    Response = response;
                }
            }

            [Serializable]
            public class SendActualAnswer : Message
            {
                public AnswerData Answer;

                public SendActualAnswer(AnswerData answer)
                {
                    Answer = answer;
                }
            }
        }

        #endregion
    }
}
