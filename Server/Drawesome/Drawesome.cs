using Server.Game;
using System.Collections.Generic;
using System;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{
    public class DrawesomeGame : Game<DrawesomeGameData>
    {
        DrawesomeGameData Data { get; set; }

        protected override string Name { get { return "Drawesome"; } }

        public DrawesomeGame(Settings settings) : base(settings)
        {
            AddState(GameState.PreGame, new StatePreGame());
            AddState(GameState.RoundBegin, new StateRoundBegin());
            AddState(GameState.Drawing, new StateDrawingPhase());
            AddState(GameState.Answering, new StateAnsweringPhase());
            AddState(GameState.Choosing, new StateChoosingPhase());
            AddState(GameState.Results, new StateResultsPhase());
            AddState(GameState.Scores, new StateScores());
            AddState(GameState.GameOver, new StateRoundEnd());
        }

        public override void Start(List<Player> players)
        {
            base.Start(players);
            Data = new DrawesomeGameData(players, Settings);
            SetState(GameState.RoundBegin, Data);
        }

        public override void StartNewRound()
        {
            base.StartNewRound();
            SetState(GameState.Drawing, GameData);
        }

        public override void Restart()
        {
            base.Restart();
            SetState(GameState.PreGame, Data);
        }

        /// <summary>
        /// Determine what to do when the current state ends.
        /// </summary>
        /// <param name="gameData"></param>
        protected override void OnEndState(DrawesomeGameData gameData)
        {
            switch (CurrentState.Type)
            {
                case GameState.PreGame:
                    SetState(GameState.PreGame, gameData);
                    break;

                case GameState.RoundBegin:
                    SetState(GameState.Drawing, gameData);
                    break;

                case GameState.Drawing:
                    SetState(GameState.Answering, gameData);
                    break;

                case GameState.Answering:

                    Console.WriteLine("Answers");
                    foreach (var answer in gameData.Answers)
                    {
                        Console.WriteLine("\t{0}: {1}", answer.Author.Name, answer.Answer);
                    }

                    SetState(GameState.Choosing, gameData);
                    break;

                case GameState.Choosing:

                    Console.WriteLine("Choices");
                    foreach (var choice in gameData.ChosenAnswers)
                    {
                        if (choice.Choosers.Count != 0)
                        {
                            var players = choice.Choosers
                            .Select(x => x.Name)
                            .Aggregate((current, next) => current + ", " + next);

                            Console.WriteLine("\t{0}: {1}", choice.Answer, players);
                        }
                    }

                    SetState(GameState.Results, gameData);
                    break;

                case GameState.Results:
                    gameData.OnNewRound();
                    if (GameData.HasDrawings())
                    {
                        SetState(GameState.Scores, gameData);
                    }
                    else
                    {
                        SetState(GameState.GameOver, gameData);
                    }
                    break;

                case GameState.Scores:
                    // After scores, return to Answering phase for remaining drawings
                    SetState(GameState.Answering, gameData);
                    break;

                case GameState.GameOver:
                    Log("Game Over!");
                    break;

                default:
                    break;
            }
        }

        protected override bool IsGameOver()
        {
            return false;
        }
    }
}
