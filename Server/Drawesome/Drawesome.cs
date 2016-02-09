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

    public class DrawesomeSettings
    {
        public const float DrawTime = 60f;
        public const float AnsweringTime = 30f;
    }

    public class Drawesome : Game<DrawesomeGameData>
    {
        DrawesomeGameData Data { get; set; }

        public Drawesome()
        {
            AddState(GameState.Drawing, new DrawingState());
            AddState(GameState.Answering, new AnsweringState());
            AddState(GameState.MakeChoices, new MakeChoiceState());
            AddState(GameState.ShowChoices, new ShowResultsState());
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

            }
        }
    }

    #region States

    public class DrawingState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        public DrawingState()
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

        public override void OnMessage(string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitDrawing)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SendImage>(json);

                // Update GameData
                GameData.Drawings.Enqueue(data.DrawingData);

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(data.DrawingData.Creator));

                if (GameData.Drawings.Count == GameData.Players.Count)
                    EndState();
            }
        }
    }

    public class AnsweringState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public AnsweringState()
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

        public override void OnMessage(string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitAnswer)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SubmitAnswer>(json);

                // Add answer here
                GameData.SubmittedAnswers.Add(data.Answer);

                // Tell all clients that player has submitted answer
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(data.Answer.Author));

                if (GameData.SubmittedAnswers.Count == GameData.Players.Count)
                    EndState();
            }
        }
    }

    public class MakeChoiceState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public MakeChoiceState()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Send options to each player
            GameData.Players.ForEach(x => x.SendOptions(GameData.SubmittedAnswers));

            // Add answers here
            foreach (var answer in GameData.SubmittedAnswers)
            {
                GameData.ChosenAnswers.Add(answer, new ResultData());
            }
        }

        public override void OnMessage(string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitAnswer)
            {
                var data = JsonHelper.FromJson<ClientMessage.Game.SubmitChoice>(json);

                // Add answer
                GameData.ChosenAnswers[data.Choice].Players.Add(data.Player);

                // Tell all clients of choice
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(data.Player));

                if (GameData.ChosenAnswers.Count == GameData.Players.Count)
                    EndState();
            }
        }
    }

    public class ShowResultsState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        Queue<ResultData> Results { get; set; }

        public ShowResultsState()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Sort chosen answers here...
            Results = new Queue<ResultData>();

            var sortedAnswers = GameData.ChosenAnswers.OrderBy(x => x.Value.Players.Count);
            foreach (var answer in sortedAnswers)
            {
                Results.Enqueue(answer.Value);
            }

            ShowNextChoice();
        }

        void ShowNextChoice()
        {
            // Send choice to client and remove from queue
            var choice = Results.Dequeue();
            GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.ShowChoice(choice)));

            // Start timer
            var timer = new GameTimer(5f);
            timer.Finish += UpdateQueue;
        }

        void UpdateQueue(object sender, EventArgs e)
        {
            if (Results.Count != 0)
            {
                ShowNextChoice();
            }
            else
            {
                EndState();
            }
        }
    }

    public class RoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundEnd; } }
    }

    #endregion
}
