using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ConsoleController
{
    delegate void CommandDelegate(ConsoleCommand command, string[] args);

    List<ConsoleCommand> Commands = new List<ConsoleCommand>();

    const string Token = "";

    class ConsoleCommand
    {
        public string Command { get; private set; }
        public CommandDelegate Action { get; private set; }
        public string Help { get; private set; }

        public ConsoleCommand(string command, CommandDelegate action, string help = "")
        {
            Command = command;
            Action = action;
            Help = help;
        }
    }

    public ConsoleController()
    {
        RegisterCommand(new ConsoleCommand("help", Help));
        RegisterCommand(new ConsoleCommand("join", Join, "[RoomID] (Joins a room with the specified ID)"));
        RegisterCommand(new ConsoleCommand("connect", Connect, "[Name]"));
        RegisterCommand(new ConsoleCommand("requestrooms", RequestRooms, "[RoomID] (Returns a list of rooms on the server)"));
        RegisterCommand(new ConsoleCommand("create", CreateRoom, "[Password] (Creates a room with an optional password)"));
        RegisterCommand(new ConsoleCommand("leave", LeaveRoom, "(Disconnects you from the room you are currently in)"));
        RegisterCommand(new ConsoleCommand("disconnect", Disconnect));
        RegisterCommand(new ConsoleCommand("say", Say, "[Message] (Sends a message to all players in the same room)"));

        // Game
        RegisterCommand(new ConsoleCommand("start", StartGame, "(Starts the game if you're the room owner)"));
        RegisterCommand(new ConsoleCommand("sendimage", SendImage));
        RegisterCommand(new ConsoleCommand("submitanswer", SubmitAnswer));
        RegisterCommand(new ConsoleCommand("submitchoice", SubmitChoice));
        RegisterCommand(new ConsoleCommand("like", SubmitLike));
        RegisterCommand(new ConsoleCommand("skip", Skip));
    }

    public void SubmitInput(string input)
    {
        ParseInput(input);
    }

    void ParseInput(string input)
    {
        if (input.StartsWith(Token))
            input = input.Remove(0, Token.Length);

        // Remove trailing whitespace
        input = input.Trim(' ');

        // Split input using space as delimiter
        string[] args = input.Split(' ');

        string parsedCommand = "";
        List<string> parsedArgs = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0)
            {
                parsedCommand = args[i];
            }
            else
            {
                parsedArgs.Add(args[i]);
            }
        }

        // find matching command here
        var command = Commands.Find(x => x.Command == parsedCommand);
        if (command != null)
        {
            Execute(command, parsedArgs.ToArray());
        }
        else
        {
           Debug.LogErrorFormat("Invalid command '{0}'", parsedCommand);
        }
    }

    void Execute(ConsoleCommand consoleCommand, string[] args)
    {
        consoleCommand.Action(consoleCommand, args);
    }

    void RegisterCommand(ConsoleCommand action)
    {
        Commands.Add(action);
    }

    void PrintError(ConsoleCommand consoleCommand)
    {
        Debug.LogErrorFormat("Incorrect number of arguments for {0}. Requires {1}", consoleCommand.Command, consoleCommand.Help);
    }

    #region Commands

    #region General

    void CreateRoom(ConsoleCommand command, string[] args)
    {
        if (args.Length > 1)
        {
            PrintError(command);
        }
        else
        {
            // Check if password was provided. Kinda shitty? Eh!
            if (args.Length == 0)
            {
                Client.Instance.Messenger.CreateRoom();
            }
            else
            {
                Client.Instance.Messenger.CreateRoom(args[0]);
            }
        }
    }

    void RequestRooms(ConsoleCommand command, string[] args)
    {
        if (args.Length > 0)
        {
            PrintError(command);
        }
        else
        {
            Debug.Log("Request rooms");
            Client.Instance.Messenger.RequestRooms();
        }
    }

    void Say(ConsoleCommand command, string[] args)
    {
        // lmao hack.
        string str = "";

        foreach (var arg in args)
        {
            str += arg + " ";
        }

        str.TrimEnd(' ');

        Client.Instance.Messenger.Say(str);
    }

    void Disconnect(ConsoleCommand command, string[] args)
    {
        Client.Instance.Connection.Disconnect();
    }

    void Connect(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1) 
        {
            PrintError(command);
        }
        else
        {
            Debug.LogFormat("Connect with name {0}", args[0]);
            Client.Instance.Connection.Connect(args[0]);
        }
    }

    void Join(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            PrintError(command);
        }
        else if (args.Length == 1)
        {
            Debug.LogFormat("Join {0}", args[0]);
            Client.Instance.Messenger.JoinRoom(args[0]);
        }
        else if (args.Length == 2)
        {
            Debug.LogFormat("Join {0} with password '{1}'", args[0], args[1]);
            Client.Instance.Messenger.JoinRoom(args[0], args[1]);
        }
    }

    void LeaveRoom(ConsoleCommand command, string[] args)
    {
        Client.Instance.Messenger.LeaveRoom();
    }

    void Help(ConsoleCommand command, string[] args)
    {
        Debug.LogFormat("Available Commands:");

        foreach (var c in Commands)
        {
            Debug.LogFormat("\t{0} {1}", c.Command, c.Help);
        }
    }

    #endregion

    #region Game

    void StartGame(ConsoleCommand command, string[] args)
    {
        Client.Instance.Messenger.StartGame();
    }

    void SendImage(ConsoleCommand command, string[] args)
    {
        Client.Instance.Messenger.SendImage(new byte[0]);
    }

    void SubmitAnswer(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            PrintError(command);
        }
        else
        {
            Client.Instance.Messenger.SubmitAnswer(args[0]);
        }
    }

    void SubmitChoice(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            PrintError(command);
        }
        else
        {
            Client.Instance.Messenger.SubmitChoice(args[0]);
        }
    }

    void SubmitLike(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            PrintError(command);
        }
        else
        {
            Client.Instance.Messenger.SkipPhase();
        }
    }

    void Skip(ConsoleCommand command, string[] args)
    {
        Client.Instance.Messenger.SkipPhase();
    }

    #endregion

    #endregion
}
