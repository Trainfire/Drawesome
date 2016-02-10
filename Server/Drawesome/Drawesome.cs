using Server.Game;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{

    public class DrawesomeGameData : GameData
    {
        public Queue<DrawingData> Drawings { get; set; }
        public List<AnswerData> SubmittedAnswers { get; set; }
        public Dictionary<AnswerData, ResultData> ChosenAnswers { get; set; }
    }

    /// TODO: Load settings from JSON in Server folder
    public class DrawesomeSettings
    {
        public const float DrawTime = 60f;
        public const float AnsweringTime = 30f;
    }

    public class Drawesome : Game<DrawesomeGameData>
    {
        DrawesomeGameData Data { get; set; }

        protected override string Name { get { return "Drawesome"; } }

        public Drawesome()
        {
            AddState(GameState.Drawing, new StateDrawingPhase());
            AddState(GameState.Answering, new StateAnsweringPhase());
            AddState(GameState.Results, new StateChoosingPhase());
            AddState(GameState.Choosing, new StateResultsPhase());
            AddState(GameState.RoundEnd, new StateRoundEnd());
        }

        public override void Start()
        {
            SetState(GameState.RoundBegin, new DrawesomeGameData());
        }

        protected override void OnEndState(DrawesomeGameData gameData)
        {
            // Loop game if drawings remain
            if (gameData.Drawings.Count != 0)
            {
                gameData.Drawings.Dequeue();
                SetState(GameState.Answering, gameData);
            }
            else
            {
                SetState(GameState.RoundEnd, gameData);
            }
        }
    }

    #region States

    public class StateRoundBegin : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            var timer = new GameTimer(10f);
            timer.Finish += Timer_Finish;
        }

        void Timer_Finish(object sender, EventArgs e)
        {
            EndState();
        }
    }

    public class StateDrawingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public StateDrawingPhase()
        {
            
        }

        protected override void OnBegin()
        {
            // TODO: Use const
            var timer = new GameTimer(DrawesomeSettings.DrawTime);
            timer.Tick += OnTimerTick;
            timer.Finish += OnTimerFinish;
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            // Send new time to client
        }

        void OnTimerFinish(object sender, EventArgs e)
        {
            // End state
            EndState();
        }

        public override void OnPlayerMessage(PlayerData player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSendImage)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SendImage>(json);

                // Update GameData
                GameData.Drawings.Enqueue(new DrawingData(player, data.Image));

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player));

                if (GameData.Drawings.Count == GameData.Players.Count)
                    EndState();
            }
        }
    }

    public class StateAnsweringPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public StateAnsweringPhase()
        {

        }

        protected override void OnBegin()
        {
            var timer = new GameTimer(DrawesomeSettings.AnsweringTime);
            timer.Tick += OnTimerTick;
            timer.Finish += OnTimerFinish;
        }

        void OnTimerFinish(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnPlayerMessage(PlayerData player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitAnswer)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SubmitAnswer>(json);

                // Add answer here
                GameData.SubmittedAnswers.Add(new AnswerData(player, data.Answer));

                // Tell all clients that player has submitted answer
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player));

                if (GameData.SubmittedAnswers.Count == GameData.Players.Count)
                    EndState();
            }
        }
    }

    public class StateChoosingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Choosing; } }

        public StateChoosingPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Send options to each player
            GameData.Players.ForEach(x => x.SendChoices(GameData.SubmittedAnswers));

            // Add answers here
            foreach (var answer in GameData.SubmittedAnswers)
            {
                GameData.ChosenAnswers.Add(answer, new ResultData());
            }
        }

        public override void OnPlayerMessage(PlayerData player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitAnswer)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SubmitChoice>(json);

                // Add answer
                var answer = GameData.SubmittedAnswers.Find(x => x.Answer == data.Choice);
                GameData.ChosenAnswers[answer].Players.Add(player);

                // Tell all clients of choice
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player));

                // End state if all players have chosen
                if (GameData.ChosenAnswers.Count == GameData.Players.Count)
                    EndState();
            }

            if (message.Type == MessageType.GameClientSubmitLike)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.LikeAnswer>(json);

                // Add like
                var answer = GameData.SubmittedAnswers.Find(x => x.Answer == data.Answer);
                GameData.ChosenAnswers[answer].Likes++;
            }
        }
    }

    public class StateResultsPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Results; } }

        Queue<KeyValuePair<AnswerData, ResultData>> Results { get; set; }

        public StateResultsPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Sort chosen answers here...
            Results = new Queue<KeyValuePair<AnswerData, ResultData>>();

            var sortedAnswers = GameData.ChosenAnswers.OrderBy(x => x.Value.Players.Count);
            foreach (var answer in sortedAnswers)
            {
                Results.Enqueue(new KeyValuePair<AnswerData, ResultData>(answer.Key, answer.Value));
            }

            ShowNextResult();
        }

        void ShowNextResult()
        {
            // Send choice to client and remove from queue
            var result = Results.Dequeue();
            GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.SendResult(result)));

            // Start timer
            var timer = new GameTimer(5f);
            timer.Finish += UpdateQueue;
        }

        void UpdateQueue(object sender, EventArgs e)
        {
            if (Results.Count != 0)
            {
                ShowNextResult();
            }
            else
            {
                EndState();
            }
        }
    }

    public class StateRoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundEnd; } }
    }

    #endregion
}
