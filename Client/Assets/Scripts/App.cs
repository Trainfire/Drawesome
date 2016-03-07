using UnityEngine;

public class App : MonoBehaviour
{
    public Client Client;
    public UserInterface Interface;
    public AppConsole Console;

    void Awake()
    {
        // Make sure player prefs are cleared to prevent old data from being used
        PlayerPrefs.DeleteAll();

        Client.Initialise(Interface);

        // Inject dependencies here
        Interface.Initialise(Client);
        Console.Initialise(Client);
    }
}
