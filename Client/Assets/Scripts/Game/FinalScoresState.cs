using UnityEngine;
using Protocol;
using System.Linq;

public class FinalScoresState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.FinalScores; } }
    public DrawingCanvas Canvas { private get; set; }

    public FinalScoresState(Client client, UiGameStateFinalScores view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        GetView<UiGameStateFinalScores>(view => view.StartNewGame.onClick.AddListener(() => Client.Messenger.StartNewGame()));
    }

    protected override void OnMessage(string json)
    {
        Message.IsType<ServerMessage.Game.SendScores>(json, (data) =>
        {
            // Make dictionary mapping players to scores.
            var scores = Enumerable.Range(0, data.Players.Count)
            .ToDictionary(i => data.Players[i], i => data.Scores[i]);

            GetView<UiGameStateFinalScores>().Show(scores);
        });
    }

    public override void Update()
    {
        base.Update();

        // Too lazy to do this properly...
        GetView<UiGameStateFinalScores>(view =>
        {
            if (view.gameObject.activeInHierarchy)
                view.StartNewGame.gameObject.SetActive(Client.Connection.IsRoomOwner());
        });
    }

    protected override void OnEnd()
    {
        base.OnEnd();
        GetView<UiGameStateFinalScores>(view => view.StartNewGame.onClick.RemoveAllListeners());
    }
}
