using Protocol;

public class ChoosingState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.Choosing; } }
    public DrawingCanvas Canvas { private get; set; }

    public ChoosingState(Client client, UiGameStateChoosing view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        base.OnBegin();
        GetView<UiGameStateChoosing>((view) =>
        {
            view.InfoBox.Hide();
            view.OnChoiceSelected += OnChoiceSelected;
            view.OnLike += OnLike;
        });
    }

    private void OnLike(AnswerData choice)
    {
        Client.Messenger.SubmitLike(choice.Answer);
    }

    void OnChoiceSelected(AnswerData choice)
    {
        Client.Messenger.SubmitChosenAnswer(choice.Answer);
    }

    protected override void OnMessage(string json)
    {
        Message.IsType<ServerMessage.Game.SendChoices>(json, (data) =>
        {
            GetView<UiGameStateChoosing>((view) =>
            {
                // Randomise order of choices before showing on UI
                data.Choices.Shuffle();

                // Send to UI
                view.ShowChoices(data.Creator, data.Choices);
            });
        });
    }

    protected override void OnEnd()
    {
        GetView<UiGameStateChoosing>((view) =>
        {
            view.OnChoiceSelected -= OnChoiceSelected;
            view.OnLike -= OnLike;
        });
    }
}
