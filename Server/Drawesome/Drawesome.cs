using Server.Game;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Timers;
using Protocol;
using System.Linq;

namespace Server.Drawesome
{
    public class DrawesomeGameData : GameData
    {
        Queue<DrawingData> drawings = new Queue<DrawingData>();
        //List<AnswerData> answers = new List<AnswerData>();
        Dictionary<AnswerData, ChoiceData> answers = new Dictionary<AnswerData, ChoiceData>();
        Dictionary<Player, PromptData> prompts = new Dictionary<Player, PromptData>();
        Dictionary<PlayerData, uint> scores;

        List<PromptData> PromptPool { get; set; }

        public DrawesomeGameData()
        {

        }

        public DrawesomeGameData(List<Player> players, Settings settings) : base(players, settings)
        {

        }

        public ReadOnlyDictionary<AnswerData, ChoiceData> ChosenAnswers
        {
            get
            {
                return new ReadOnlyDictionary<AnswerData, ChoiceData>(answers);
            }
        }

        public ReadOnlyDictionary<Player, PromptData> SentPrompts
        {
            get
            {
                return new ReadOnlyDictionary<Player, PromptData>(prompts);
            }
        }

        public ReadOnlyDictionary<PlayerData, uint> Scores
        {
            get
            {
                return new ReadOnlyDictionary<PlayerData, uint>(scores);
            }
        }

        public ReadOnlyCollection<AnswerData> Answers
        {
            get
            {
                return new ReadOnlyCollection<AnswerData>(answers.Keys.ToList());
            }
        }

        public void AddPoints(PlayerData player, uint points)
        {
            if (scores == null)
            {
                scores = new Dictionary<PlayerData, uint>();
                Players.ForEach(x => scores.Add(x.Data, 0));
            }

            Console.WriteLine("Give {0} {1} points", player.Name, points);
            scores[player] += points;
        }

        public void AddAnswer(AnswerData answer)
        {
            Console.WriteLine("Add answer {0}", answer.Answer);
            answers.Add(answer, new ChoiceData());
        }

        public void AddChosenAnswer(string answer, Player player)
        {
            var findAnswer = GetAnswer(answer);
            Console.WriteLine("Add chosen answer: {0}", findAnswer.Answer);
            answers[findAnswer].Players.Add(player.Data);

            AddPoints(findAnswer.Author, Settings.Drawesome.PointsPerChoice);
            // TODO: Give more points if answer matches prompt
        }

        public void AddDrawing(DrawingData drawing)
        {
            drawings.Enqueue(drawing);
        }

        public void AddLike(string answer)
        {
            GetAnswer(answer).Likes++;
        }

        public void OnNextRound()
        {
            answers.Clear();
            drawings.Dequeue();
        }

        public DrawingData GetDrawing()
        {
            return drawings.Peek();
        }

        public bool HasDrawings()
        {
            return drawings.Count != 0;
        }

        public PromptData GetPrompt(Player player)
        {
            if (PromptPool == null)
                PromptPool = Settings.Prompts.Items;

            var rnd = new Random();
            var index = rnd.Next(0, PromptPool.Count - 1);
            var prompt = PromptPool[index];
            PromptPool.Remove(prompt);

            if (prompts == null)
                prompts = new Dictionary<Player, PromptData>();

            prompts.Add(player, prompt);

            return prompt;
        }

        AnswerData GetAnswer(string answer)
        {
            return answers.Keys.First(x => x.Answer == answer);
        }
    }

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
            AddState(GameState.RoundEnd, new StateRoundEnd());
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

        protected override void OnEndState(DrawesomeGameData gameData)
        {
            // Determine what to do when the current state ends
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
                        if (choice.Value != null && choice.Value.Players.Count != 0)
                        {
                            var players = choice.Value.Players
                            .Select(x => x.Name)
                            .Aggregate((current, next) => current + ", " + next);

                            Console.WriteLine("\t{0}: {1}", choice.Key.Answer, players);
                        }
                    }

                    SetState(GameState.Results, gameData);
                    break;

                case GameState.Results:
                    SetState(GameState.Scores, gameData);
                    break;

                case GameState.Scores:
                    SetState(GameState.RoundEnd, gameData);
                    break;

                case GameState.RoundEnd:
                    if (gameData.HasDrawings())
                    {
                        SetState(GameState.Answering, gameData);
                    }
                    else
                    {
                        Log("Game Over!");
                    }
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

    #region States

    public class StatePreGame : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.PreGame; } }
    }

    public class StateRoundBegin : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundBegin; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            SetTimer("Begin Timer", GameData.Settings.Drawesome.RoundBeginTime, true);
        }
    }

    public class StateDrawingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Drawing; } }

        float SubmittedDrawings { get; set; }

        public StateDrawingPhase()
        {
            
        }

        protected override void OnBegin()
        {
            SetTimer("Drawing Timer", GameData.Settings.Drawesome.DrawTime, true);

            SubmittedDrawings = 0;

            // Send random prompts to players
            foreach (var player in GameData.Players)
            {
                player.SendPrompt(GameData.GetPrompt(player).Text);
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SendImage>(json, (data) =>
            {
                SubmittedDrawings++;

                var prompt = GameData.SentPrompts[player];

                // Update GameData
                GameData.AddDrawing(new DrawingData(player.Data, data.Image, prompt));

                Console.WriteLine("Recieve image from {0} with {1} bytes for prompt {2}", player.Data.Name, data.Image.Length, prompt.Text);

                // Tell all clients that player has submitted drawing
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                if (SubmittedDrawings == GameData.Players.Count)
                    EndState();
            });
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
            SetTimer("Answering Timer", GameData.Settings.Drawesome.AnsweringTime, true);

            if (GameData.HasDrawings())
            {
                GameData.Players.ForEach(x => x.SendImage(GameData.GetDrawing().Image));
            }
            else
            {
                EndState();
            }
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            Message.IsType<ClientMessage.Game.SubmitAnswer>(json, (data) =>
            {
                Console.WriteLine("{0} submitted '{1}'", player.Data.Name, data.Answer);
              
                if (IsPrompt(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches prompt!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerError.MatchesPrompt);
                }
                else if (HasPromptBeenSubmitted(data.Answer))
                {
                    Console.WriteLine("Player {0}'s answer matches an existing answer from another player!", player.Data.Name);
                    player.SendAnswerValidation(GameAnswerError.AlreadyExists);
                }        
                else
                {
                    player.SendAnswerValidation(GameAnswerError.None);

                    // Add answer here
                    GameData.AddAnswer(new AnswerData(player.Data, data.Answer));

                    // Tell all clients that player has submitted answer
                    GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                    if (GameData.Answers.Count == GameData.Players.Count)
                        EndState();
                }
            });
        }

        bool IsPrompt(string answer)
        {
            return GameData.SentPrompts.Any(x => x.Value.Text.ToLower() == answer.ToLower());
        }

        bool HasPromptBeenSubmitted(string answer)
        {
            return GameData.Answers.Any(x => x.Answer.ToLower() == answer.ToLower());
        }
    }

    public class StateChoosingPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Choosing; } }

        float ChosenAnswerCount { get; set; }

        public StateChoosingPhase()
        {
            
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            ChosenAnswerCount = 0;

            SetTimer("Choosing Timer", GameData.Settings.Drawesome.ChoosingTime);

            // Send options to each player
            GameData.Players.ForEach(x => x.SendChoices(GameData.Answers.ToList()));
        }

        public override void OnPlayerMessage(Player player, string json)
        {
            var message = JsonHelper.FromJson<Message>(json);

            Message.IsType<ClientMessage.Game.SubmitChoice>(json, (data) =>
            {
                ChosenAnswerCount++;
                GameData.AddChosenAnswer(data.ChosenAnswer, player);
                GameData.Players.ForEach(x => x.NotifyPlayerGameAction(player.Data));

                // End state if all players have chosen
                if (ChosenAnswerCount == GameData.Players.Count)
                    EndState();
            });

            Message.IsType<ClientMessage.Game.LikeAnswer>(json, (data) =>
            {
                // Add like
                GameData.AddLike(data.Answer);
            });
        }

        protected override void OnTimerFinish(object sender, EventArgs e)
        {
            // TODO: Add decoys
            // ...

            base.OnTimerFinish(sender, e);
        }
    }

    public class StateResultsPhase : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Results; } }

        Queue<KeyValuePair<AnswerData, ChoiceData>> ChosenAnswersQueue { get; set; }

        public StateResultsPhase()
        {

        }

        protected override void OnBegin()
        {
            base.OnBegin();

            if (GameData.ChosenAnswers.Count == 0)
            {
                EndState();
            }
            else
            {
                // Sort chosen answers here...
                ChosenAnswersQueue = new Queue<KeyValuePair<AnswerData, ChoiceData>>();

                var sortedAnswers = GameData.ChosenAnswers.OrderBy(x => x.Value.Players.Count).ToList();
                foreach (var answer in sortedAnswers)
                {
                    ChosenAnswersQueue.Enqueue(new KeyValuePair<AnswerData, ChoiceData>(answer.Key, answer.Value));
                }

                ShowNextResult();
            }
        }

        void ShowNextResult()
        {
            // Send choice to client and remove from queue
            var chosenAnswer = ChosenAnswersQueue.Dequeue();
            var result = new ResultData(chosenAnswer.Key.Author, chosenAnswer.Value.Players, chosenAnswer.Key.Answer, chosenAnswer.Value.Points);
            var message = new ServerMessage.Game.SendResult(result);
            GameData.Players.ForEach(x => x.SendMessage(message));

            // Start timer
            var timer = new GameTimer(GameData.Settings.Drawesome.ResultTimeBetween);
            timer.Finish += UpdateQueue;
        }

        void UpdateQueue(object sender, EventArgs e)
        {
            if (ChosenAnswersQueue.Count != 0)
            {
                ShowNextResult();
            }
            else
            {
                EndState();
            }
        }
    }

    public class StateScores : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.Scores; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            var score = GameData.Scores.ToDictionary(x => x.Key, y => y.Value);
            GameData.Players.ForEach(x => x.SendScores(score));
            SetTimer("Show Scores", 10f, false);
        }
    }

    public class StateRoundEnd : State<DrawesomeGameData>
    {
        public override GameState Type { get { return GameState.RoundEnd; } }

        protected override void OnBegin()
        {
            base.OnBegin();
            GameData.OnNextRound();
            EndState(true);
        }
    }

    #endregion
}
