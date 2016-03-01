using UnityEngine;

public class App : MonoBehaviour
{
    public Client Client;
    public UserInterface Interface;
    public AppConsole Console;

    void Awake()
    {
        Client.Initialise(Interface);

        // Inject dependencies here
        Interface.Initialise(Client);
        Console.Initialise(Client);
    }
}
