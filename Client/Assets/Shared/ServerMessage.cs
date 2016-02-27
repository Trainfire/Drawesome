using System;
using System.Collections.Generic;
using Protocol;
using System.Linq;
using Newtonsoft.Json;

namespace Protocol
{
    public class ServerMessage
    {
        public class AssignClientId : Message
        {
            public string ID;

            public AssignClientId(string id)
            {
                ID = id;
            }
        }

        public class SendConnectionError : Message
        {
            public ConnectionError Error;

            public SendConnectionError(ConnectionError error)
            {
                Error = error;
            }
        }

        public class NotifyConnectionSuccess : Message
        {
            public NotifyConnectionSuccess()
            {

            }
        }

        public class RequestClientName : Message
        {
            public RequestClientName()
            {

            }
        }

        public class UpdatePlayerInfo : Message
        {
            public PlayerData PlayerData;

            public UpdatePlayerInfo(PlayerData playerData)
            {
                PlayerData = playerData;
            }
        }

        public class RoomList : Message
        {
            public List<RoomData> Rooms;

            public RoomList(List<RoomData> rooms)
            {
                Rooms = rooms;
            }
        }

        public class NotifyRoomCountdown : Message
        {
            public float Duration;

            public NotifyRoomCountdown(float duration)
            {
                Duration = duration;
            }
        }

        public class NotifyRoomCountdownCancel : Message
        {
            public NotifyRoomCountdownCancel()
            {

            }
        }

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

        public class NotifyPlayerAction : Message
        {
            public PlayerData Player;
            public PlayerAction Action;
            public PlayerActionContext Context;

            public NotifyPlayerAction(PlayerData player, PlayerAction action, PlayerActionContext context)
            {
                Player = player;
                Action = action;
                Context = context;
            }
        }

        public class NotifyRoomJoin : Message
        {
            public RoomNotice Notice;

            public NotifyRoomJoin(RoomNotice notice)
            {
                Notice = notice;
            }
        }

        public class NotifyRoomLeave : Message
        {
            public RoomLeaveReason Reason;

            public NotifyRoomLeave(RoomLeaveReason reason)
            {
                Reason = reason;
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
            public class ChangeState : Message
            {
                public GameState GameState;

                public ChangeState(GameState gameState)
                {
                    GameState = gameState;
                }
            }

            public class EndState : Message
            {
                public GameStateEndReason Reason;

                public EndState(GameStateEndReason reason)
                {
                    Reason = reason;
                }
            }

            public class SendTransitionPeriod : Message
            {
                public float Duration;

                public SendTransitionPeriod(float duration)
                {
                    Duration = duration;
                }
            }

            public class SendImage : Message
            {
                public DrawingData Drawing;

                public SendImage(DrawingData drawing)
                {
                    Drawing = drawing;
                }
            }

            public class SendPrompt : Message
            {
                public string Prompt;

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
                public List<PlayerData> Players = new List<PlayerData>();
                public List<ScoreData> Scores = new List<ScoreData>();
                public bool IsOrderedByDescending;

                public SendScores()
                {

                }

                public SendScores(Dictionary<PlayerData, ScoreData> playerScores, bool orderByDescending = false)
                {
                    IsOrderedByDescending = orderByDescending;

                    Players = playerScores.Keys.ToList();
                    Scores = playerScores.Values.ToList();

                    if (orderByDescending)
                    {
                        var pairs = new List<KeyValuePair<PlayerData, ScoreData>>();

                        for (int i = 0; i < playerScores.Count; i++)
                        {
                            pairs.Add(new KeyValuePair<PlayerData, ScoreData>(Players[i], Scores[i]));
                        }

                        pairs = pairs.OrderByDescending(x => x.Value.Score).ToList();

                        Players = pairs.Select(x => x.Key).ToList();
                        Scores = pairs.Select(x => x.Value).ToList();

                        IsOrderedByDescending = true;
                    } 
                }
            }

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

            public class AddTimer : Message
            {
                public float Duration;

                public AddTimer(float duration)
                {
                    Duration = duration;
                }
            }

            public class SetTimer : Message
            {
                public float CurrentTime;

                public SetTimer(float currentTime)
                {
                    CurrentTime = currentTime;
                }
            }

            public class SendAnswerValidation : Message
            {
                public GameAnswerValidationResponse Response;

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
