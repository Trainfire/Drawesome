public class Client : Singleton<Client>
{
    public const string URI = "ws://127.0.0.1:8181/room";

    public Connection Connection { get; set; }
    public MessageHandler MessageHandler { get; private set; }
    public Messenger Messenger { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        Connection = gameObject.AddComponent<Connection>();
        MessageHandler = new MessageHandler(Connection);
        Messenger = new Messenger(Connection);
    }
}
