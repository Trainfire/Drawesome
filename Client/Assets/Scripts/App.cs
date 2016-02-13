using UnityEngine;

public class App : MonoBehaviour
{
    public Client Client;
    public UserInterface Interface;
    public Game Game;
    public AppConsole Console;

    void Awake()
    {
        Client.Initialise();

        // Inject dependencies here
        Game.Initialise(Client);
        Interface.Initialise(Client);
        Console.Initialise(Client);
    }
}
