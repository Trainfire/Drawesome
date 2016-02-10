using UnityEngine;

public class Client : Singleton<Client>
{
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
