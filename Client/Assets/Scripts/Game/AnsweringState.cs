using Protocol;

public class AnsweringState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.Answering; } }
    public DrawingCanvas Canvas { private get; set; }

    public AnsweringState(Client client, UiGameStateAnswering view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        GetView<UiGameStateAnswering>((view) =>
        {
            view.InfoBox.Hide();
            view.InputField.text = Strings.PromptEnterGuess;
            view.InputField.gameObject.SetActive(true);
            view.Submit.gameObject.SetActive(true);
            view.Submit.onClick.AddListener(() =>
            {
                Client.Messenger.SubmitAnswer(view.InputField.textComponent.text);
            });
        });
    }

    protected override void OnMessage(string json)
    {
        var view = GetView<UiGameStateAnswering>();

        Message.IsType<ServerMessage.Game.SendImage>(json, (data) =>
        {
            Canvas.SetImage(data.Drawing);
            bool isClientsDrawing = Client.IsPlayer(data.Drawing.Creator);
            view.InputField.gameObject.SetActive(!isClientsDrawing);
            view.Submit.gameObject.SetActive(!isClientsDrawing);

            if (isClientsDrawing)
                view.InfoBox.Show(Strings.PlayersOwnDrawing);
        });

        Message.IsType<ServerMessage.Game.SendAnswerValidation>(json, (data) =>
        {
            view.InfoBox.Show(data.Response);
            if (data.Response == GameAnswerValidationResponse.None)
            {
                OnAnswerValidation();
            }
            else
            {
                view.InputField.text = string.Empty;
            }
        });
    }

    void OnAnswerValidation()
    {
        GetView<UiGameStateAnswering>((view) =>
        {
            view.InputField.gameObject.SetActive(false);
            view.Submit.gameObject.SetActive(false);
            view.InfoBox.Show(Strings.AnswerSubmitted);
        });
    }

    protected override void OnEnd()
    {
        GetView<UiGameStateAnswering>((view) =>
        {
            view.Submit.onClick.RemoveAllListeners();
        });
    }
}
