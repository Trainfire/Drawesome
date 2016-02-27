using UnityEngine;
using System.Collections;
using System;

public class AppConsole : MonoBehaviour, IClientHandler
{
    public ConsoleView View;

    Client Client { get; set; }
    ConsoleController Controller { get; set; }

    public void Initialise(Client client)
    {
        Client = client;

        Controller = new ConsoleController();
        View.SetConsole(Controller);

        // General
        Controller.RegisterCommand(new ConsoleCommand("admin", RequestAdmin, "[Password] You must be *this* cool to use this command"));
        Controller.RegisterCommand(new ConsoleCommand("join", Join, "[RoomID] (Joins a room with the specified ID)"));
        Controller.RegisterCommand(new ConsoleCommand("connect", Connect, "[Name]"));
        Controller.RegisterCommand(new ConsoleCommand("requestrooms", RequestRooms, "[RoomID] (Returns a list of rooms on the server)"));
        Controller.RegisterCommand(new ConsoleCommand("create", CreateRoom, "[Password] (Creates a room with an optional password)"));
        Controller.RegisterCommand(new ConsoleCommand("leave", LeaveRoom, "(Disconnects you from the room you are currently in)"));
        Controller.RegisterCommand(new ConsoleCommand("disconnect", Disconnect));
        Controller.RegisterCommand(new ConsoleCommand("say", Say, "[Message] (Sends a message to all players in the same room)"));

        // Game
        Controller.RegisterCommand(new ConsoleCommand("start", StartGame, "(Starts the game if you're the room owner)"));
        Controller.RegisterCommand(new ConsoleCommand("restart", RestartGame, "(Restarts the game if you're the room owner)"));
        Controller.RegisterCommand(new ConsoleCommand("startnewround", StartNewRound, "(Starts a new round of the game if you're the room owner)"));
        Controller.RegisterCommand(new ConsoleCommand("sendimage", SendImage));
        Controller.RegisterCommand(new ConsoleCommand("submitanswer", SubmitAnswer));
        Controller.RegisterCommand(new ConsoleCommand("submitchoice", SubmitChoice));
        Controller.RegisterCommand(new ConsoleCommand("like", SubmitLike));
        Controller.RegisterCommand(new ConsoleCommand("skip", Skip));
    }

    #region Commands

    #region General

    void RequestAdmin(ConsoleCommand command, string[] args)
    {
        if (args.Length == 1)
        {
            Client.Messenger.RequestAdmin(args[0]);
        }
        else
        {
            Controller.PrintError(command);
        }
    }

    void CreateRoom(ConsoleCommand command, string[] args)
    {
        if (args.Length > 1)
        {
            Controller.PrintError(command);
        }
        else
        {
            // Check if password was provided. Kinda shitty? Eh!
            if (args.Length == 0)
            {
                Client.Messenger.CreateRoom();
            }
            else
            {
                Client.Messenger.CreateRoom(args[0]);
            }
        }
    }

    void RequestRooms(ConsoleCommand command, string[] args)
    {
        if (args.Length > 0)
        {
            Controller.PrintError(command);
        }
        else
        {
            Debug.Log("Request rooms");
            Client.Messenger.RequestRooms();
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

        Client.Messenger.Say(str);
    }

    void Disconnect(ConsoleCommand command, string[] args)
    {
        Client.Connection.Disconnect();
    }

    void Connect(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            Controller.PrintError(command);
        }
        else if (args.Length == 1)
        {
            Debug.LogFormat("Connect with name {0}", args[0]);
            Client.Connection.Connect(args[0]);
        }
        else if (args.Length == 2)
        {
            Debug.LogFormat("Connect with name {0} to {1}", args[0], args[1]);
            Client.Connection.Connect(args[0], args[1]);
        }
        else
        {
            Controller.PrintError(command);
        }
    }

    void Join(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            Controller.PrintError(command);
        }
        else if (args.Length == 1)
        {
            Debug.LogFormat("Join {0}", args[0]);
            Client.Messenger.JoinRoom(args[0]);
        }
        else if (args.Length == 2)
        {
            Debug.LogFormat("Join {0} with password '{1}'", args[0], args[1]);
            Client.Messenger.JoinRoom(args[0], args[1]);
        }
    }

    void LeaveRoom(ConsoleCommand command, string[] args)
    {
        Client.Messenger.LeaveRoom();
    }

    #endregion

    #region Game

    void StartGame(ConsoleCommand command, string[] args)
    {
        Client.Messenger.StartGame();
    }

    void RestartGame(ConsoleCommand command, string[] args)
    {
        Client.Messenger.RestartGame();
    }

    void StartNewRound(ConsoleCommand command, string[] args)
    {
        Client.Messenger.StartNewRound();
    }

    void SendImage(ConsoleCommand command, string[] args)
    {
        Client.Messenger.SendImage(new byte[0]);
    }

    void SubmitAnswer(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            Controller.PrintError(command);
        }
        else
        {
            Client.Messenger.SubmitAnswer(args[0]);
        }
    }

    void SubmitChoice(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            Controller.PrintError(command);
        }
        else
        {
            Client.Messenger.SubmitChosenAnswer(args[0]);
        }
    }

    void SubmitLike(ConsoleCommand command, string[] args)
    {
        if (args.Length < 1)
        {
            Controller.PrintError(command);
        }
        else
        {
            Client.Messenger.SkipPhase();
        }
    }

    void Skip(ConsoleCommand command, string[] args)
    {
        Client.Messenger.SkipPhase();
    }

    #endregion

    #endregion	

    bool StringToBool(string str)
    {
        return str == "1";
    }
}
