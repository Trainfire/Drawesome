using Server.Game;
using System.Collections.Generic;
using System;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{
    public class DrawesomeGame : Game<DrawesomeGameData>
    {
        public override string LogName { get { return "Drawesome"; } }

        public DrawesomeGame(ConnectionsHandler connections, Settings settings) : base(connections, settings)
        {
            AddState(GameState.PreGame, new StatePreGame(settings));
            AddState(GameState.RoundBegin, new StateRoundBegin(settings));
            AddState(GameState.Drawing, new StateDrawingPhase(settings));
            AddState(GameState.Answering, new StateAnsweringPhase(settings));
            AddState(GameState.Choosing, new StateChoosingPhase(settings));
            AddState(GameState.Results, new StateResultsPhase(settings));
            AddState(GameState.Scores, new StateScores(settings));
            AddState(GameState.FinalScores, new StateFinalScores(settings));
            AddState(GameState.GameOver, new StateGameOver(settings));
        }

        public override void Start(List<Player> players)
        {
            base.Start(players);
            GameData = new DrawesomeGameData(players, Settings);
            SetState(GameState.RoundBegin, 0f);
        }

        public override void StartNewRound()
        {
            base.StartNewRound();
            GameData = new DrawesomeGameData(GameData.Players, Settings);
            SetState(GameState.Drawing, Settings.Drawesome.Transitions.RoundBeginToDrawing);
        }

        public override void Restart()
        {
            base.Restart();
            GameData = new DrawesomeGameData(GameData.Players, Settings);
            SetState(GameState.PreGame, Settings.Drawesome.Transitions.RoundBeginToDrawing);
        }

        /// <summary>
        /// Determine what to do when the current state ends.
        /// </summary>
        /// <param name="gameData"></param>
        protected override void OnEndState(GameState endingState)
        {
            switch (endingState)
            {
                case GameState.PreGame:
                    SetState(GameState.RoundBegin, Settings.Drawesome.Transitions.RoundBeginToDrawing);
                    break;

                case GameState.RoundBegin:
                    SetState(GameState.Drawing, Settings.Drawesome.Transitions.RoundBeginToDrawing);
                    break;

                case GameState.Drawing:
                    SetState(GameState.Answering, Settings.Drawesome.Transitions.DrawingToAnswering);
                    break;

                case GameState.Answering:

                    Console.WriteLine("Answers");
                    foreach (var answer in GameData.Answers)
                    {
                        Console.WriteLine("\t{0}: {1}", answer.Author.Name, answer.Answer);
                    }

                    Console.WriteLine(GameData.Answers.Count);

                    SetState(GameState.Choosing, Settings.Drawesome.Transitions.AnsweringToChoosing);
                    break;

                case GameState.Choosing:

                    Console.WriteLine("Choices");
                    foreach (var choice in GameData.ChosenAnswers)
                    {
                        if (choice.Choosers.Count != 0)
                        {
                            var players = choice.Choosers
                            .Select(x => x.Name)
                            .Aggregate((current, next) => current + ", " + next);

                            Console.WriteLine("\t{0}: {1}", choice.Answer, players);
                        }
                    }

                    SetState(GameState.Results, Settings.Drawesome.Transitions.ChoosingtoResults);
                    break;

                case GameState.Results:
                    Logger.Log(this, "{0} drawings remain", GameData.Drawings.Count);
                    if (GameData.HasDrawings())
                    {
                        // Show the scores as normal
                        SetState(GameState.Scores, Settings.Drawesome.Transitions.ScoresToAnswering);
                    }
                    else
                    {
                        // Show the final scores
                        SetState(GameState.FinalScores, Settings.Drawesome.Transitions.ScoresToAnswering);
                    }
                    break;

                case GameState.Scores:
                    // Clear data for the next round
                    GameData.OnNewRound();

                    // After scores, return to Answering phase for remaining drawings
                    SetState(GameState.Answering, Settings.Drawesome.Transitions.ScoresToAnswering);
                    break;

                case GameState.FinalScores:
                    End();
                    Logger.Log(this, "Game Over!");

                    SetState(GameState.PreGame, Settings.Drawesome.Transitions.ScoresToAnswering);
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
