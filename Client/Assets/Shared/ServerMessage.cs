using System;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using Newtonsoft.Json;

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

            public class SendTransitionPeriod : Message
            {
                public GameTransition Transition { get; private set; }
                public float Duration { get; private set; }

                public SendTransitionPeriod(float duration, GameTransition transition)
                {
                    Duration = duration;
                    Transition = transition;
                }
            }

            public class SendImage : Message
            {
                public DrawingData Drawing { get; private set; }

                public SendImage(DrawingData drawing)
                {
                    Drawing = drawing;
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
                public PlayerData Creator;
                public List<AnswerData> Choices;

                public SendChoices(PlayerData creator, List<AnswerData> choices)
                {
                    Creator = creator;
                    Choices = choices;
                }
            }

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

            public class PlayerAction : Message
            {
                public PlayerData Actor { get; private set; }
                public GamePlayerAction Action { get; private set; }

                public PlayerAction(PlayerData actor, GamePlayerAction action)
                {
                    Actor = actor;
                    Action = action;
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
                public GameAnswerValidationResponse Response { get; private set; }

                public SendAnswerValidation(GameAnswerValidationResponse response)
                {
                    Response = response;
                }
            }

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
