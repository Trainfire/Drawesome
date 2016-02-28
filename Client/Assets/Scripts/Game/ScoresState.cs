using UnityEngine;
using Protocol;
using System.Collections.Generic;
using System.Linq;

public class ScoresState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.Scores; } }
    public DrawingCanvas Canvas { private get; set; }

    Dictionary<PlayerData, GameScore> scoreCache = new Dictionary<PlayerData, GameScore>();

    public ScoresState(Client client, UiGameStateScores view) : base(client, view)
    {

    }

    protected override void OnMessage(string json)
    {
        Message.IsType<ServerMessage.Game.SendScores>(json, (data) =>
        {
            // Make dictionary mapping players[i] to scores[i]
            var scores = Enumerable.Range(0, data.Players.Count)
            .ToDictionary(i => data.Players[i], i => data.Scores[i]);

            foreach (var score in scores)
            {
                if (!scoreCache.ContainsKey(score.Key))
                    scoreCache.Add(score.Key, new GameScore(score.Value));

                Debug.LogFormat("Player: {0}, Score: {1}", score.Key.Name, score.Value.Score);

                // Set current score
                scoreCache[score.Key].UpdateScore(score.Value);
            }

            GetView<UiGameStateScores>().ShowScores(scoreCache);
        });
    }
}
