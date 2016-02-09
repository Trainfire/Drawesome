using Server.Game;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Timers;
using Protocol;

namespace Server.Drawesome
{

    public class DrawesomeGameData : GameData
    {
        public Queue<DrawingData> Drawings { get; set; }
        public List<GuessData> Guesses { get; set; }
        public List<ChoiceData> Choices { get; set; }
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
            AddState(GameState.ShowChoices, new ShowChoicesState());
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
                // GameData.Drawings.Add(data.DrawingData);

                // Tell all clients that player has submitted drawing
                //GameData.Players.ForEach(x => x.SendMessage());

                // if GameData.Drawings.Count == GameData.Players.Count
                //      EndState();
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
                // Add guess here
                // GameData.Guesses.Add(new GuessData(Player, guess));

                // Tell all clients that player has submitted guess
                // ...

                // if (GameData.Guesses.Count == GameData.Players.Count)
                // ...
            }
        }
    }

    public class MakeChoiceState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        public MakeChoiceState()
        {

        }

        public override void OnMessage(string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            if (message.Type == MessageType.GameClientSubmitChoice)
            {
                // Add choice
                // ...

                // Tell all clients of choice
            }
        }
    }

    public class ShowChoicesState : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Answering; } }

        Queue<ChoiceData> Choices { get; set; }

        public ShowChoicesState()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            // Sort choices by most chosen to least chosen
            // (Always put correct answer last)
            // Add to queue
            var Choices = new Queue<ChoiceData>();

            ShowNextChoice();
        }

        void ShowNextChoice()
        {
            // Send choice to client and remove from queue
            var choice = Choices.Dequeue();
            GameData.Players.ForEach(x => x.SendMessage(new ServerMessage.Game.ShowChoice(choice)));

            // Start timer
            var timer = new GameTimer(5f);
            timer.Finish += UpdateQueue;
        }

        void UpdateQueue(object sender, EventArgs e)
        {
            if (Choices.Count != 0)
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
