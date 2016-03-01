using Protocol;

public class PreGameState : State, Game.IGameState
{
    public State State { get { return this; } }
    public GameState Type { get { return GameState.PreGame; } }
    public DrawingCanvas Canvas { private get; set; }

    bool IsCountingDown { get; set; }

    public PreGameState(Client client, UiGameStatePreGame view) : base(client, view)
    {

    }

    protected override void OnBegin()
    {
        base.OnBegin();
        Canvas.Clear();

        GetView<UiGameStatePreGame>((view) =>
        {
            view.Start.onClick.AddListener(() => Client.Messenger.StartGame());
            view.Cancel.onClick.AddListener(() => Client.Messenger.CancelGameStart());
            view.Start.gameObject.SetActive(false);
            view.Cancel.gameObject.SetActive(false);
        });
    }

    public override void Update()
    {
        var view = GetView<UiGameStatePreGame>();

        if (!Client.Connection.Room.HasMinimumPlayers())
        {
            // Show players required to start game
            view.InfoLabel.text = string.Format(Strings.WaitingForPlayers, Client.Connection.Room.PlayersNeeded());
        }
        else
        {
            // Show info box "Waiting for Room Owner" if player is NOT the room owner
            view.InfoLabel.text = Client.Connection.IsRoomOwner() ? Strings.StartGame : string.Format(Strings.WaitingForRoomOwner, Client.Connection.Room.Owner.Name);
        }

        // Show Start button if room owner and min players reached
        view.Start.gameObject.SetActive(Client.Connection.IsRoomOwner() && !Client.Connection.Room.GameStarted && Client.Connection.Room.HasMinimumPlayers());

        // Enable info box if game hasn't started, disable it if it has
        view.InfoBox.SetActive(!Client.Connection.Room.GameStarted);
    }

    protected override void OnMessage(string json)
    {
        var view = GetView<UiGameStatePreGame>();

        // Hide Start button, show Cancel button if room owner
        Message.IsType<ServerMessage.NotifyRoomCountdown>(json, (data) =>
        {
            if (Client.Connection.IsRoomOwner())
            {
                view.Start.gameObject.SetActive(false);
                view.Cancel.gameObject.SetActive(true);
            }

            view.SetCountdown(data.Duration);
        });

        // Show Start button, hide Cancel button if room owner
        Message.IsType<ServerMessage.NotifyRoomCountdownCancel>(json, (data) =>
        {
            if (Client.Connection.IsRoomOwner())
            {
                view.Start.gameObject.SetActive(true);
                view.Cancel.gameObject.SetActive(false);
            }

            view.CancelCountdown();
        });
    }

    protected override void OnEnd()
    {
        GetView<UiGameStatePreGame>((view) =>
        {
            view.Start.onClick.RemoveAllListeners();
            view.Cancel.onClick.RemoveAllListeners();
        });
    }
}
