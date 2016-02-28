using Protocol;
public class RoundBeginState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.RoundBegin; } }
    public DrawingCanvas Canvas { private get; set; }

    public RoundBeginState(Client client, UiGameStateRoundBegin view) : base(client, view)
    {

    }

    protected override void OnMessage(string json)
    {
        // TODO: Show player's readying up here

    }
}
