using Protocol;

public class DrawingState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.Drawing; } }
    public DrawingCanvas Canvas { private get; set; }

    public DrawingState(Client client, UiGameStateDrawing view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        GetView<UiGameStateDrawing>((view) =>
        {
            view.InfoBox.Label.text = Strings.DrawingSubmitted;
            view.InfoBox.Hide();
            view.Submit.gameObject.SetActive(true);

            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SendImage(Canvas.GetEncodedImage());
                Canvas.AllowDrawing = false;
                view.InfoBox.Show();
                view.Submit.gameObject.SetActive(false);
            });
        });
    }

    protected override void OnMessage(string json)
    {
        Message.IsType<ServerMessage.Game.SendPrompt>(json, (data) =>
        {
            GetView<UiGameStateDrawing>().SetPrompt(data.Prompt);
        });
    }

    protected override void OnEnd()
    {
        GetView<UiGameStateDrawing>((view) =>
        {
            view.Submit.onClick.RemoveAllListeners();
        });
    }
}
