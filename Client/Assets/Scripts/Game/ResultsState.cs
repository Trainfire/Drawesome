using Protocol;

public class ResultsState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.Results; } }
    public DrawingCanvas Canvas { private get; set; }

    public ResultsState(Client client, UiGameStateResults view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        base.OnBegin();

        GetView<UiGameStateResults>((view) =>
        {
            view.OnFinishedShowingResult += () =>
            {
                Client.Messenger.FinishShowingResult();
            };
        });
    }

    protected override void OnMessage(string json)
    {
        Message.IsType<ServerMessage.Game.SendResult>(json, (data) =>
        {
            GetView<UiGameStateResults>().ShowAnswer(data.Answer);
        });
    }

    protected override void OnEnd()
    {
        GetView<UiGameStateResults>((view) =>
        {
            view.OnFinishedShowingResult -= Client.Messenger.FinishShowingResult;
        });
    }
}
