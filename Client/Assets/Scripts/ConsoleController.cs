using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConsoleController
{
    public delegate void Command(string[] args);

    Dictionary<string, Command> Commands = new Dictionary<string, Command>();

    public ConsoleController()
    {
        RegisterCommand("/join", Join);
        RegisterCommand("/connect", Connect);
        RegisterCommand("/requestrooms", RequestRooms);
        RegisterCommand("/createroom", CreateRoom);
    }

    public void SubmitInput(string input)
    {
        ParseInput(input);
    }

    void ParseInput(string input)
    {
        string[] args = input.Split(' ');

        string command = "";
        List<string> commandArgs = new List<string>();

        for (int i = 0; i < args.Length; i++)
        {
            if (i == 0)
            {
                command = args[i];
            }
            else
            {
                commandArgs.Add(args[i]);
            }
        }

        Execute(command, commandArgs.ToArray());
    }

    void Execute(string command, string[] args)
    {
        if (Commands.ContainsKey(command))
        {
            Logger.Log("Execute command {0} with following arguments: ", command);
            Commands[command](args);
        }
        else
        {
            Logger.LogErrorFormat("Invalid command '{0}'", command);
        }
    }

    void RegisterCommand(string command, Command handler)
    {
        Commands.Add(command, handler);
    }

    void PrintError()
    {
        Logger.LogErrorFormat("Incorrect number of arguments");
    }

    void CreateRoom(string[] args)
    {
        if (args.Length > 1)
        {
            PrintError();
        }
        else
        {
            // Check if password was provided. Kinda shitty? Eh!
            if (args.Length == 0)
            {
                Client.Instance.CreateRoom();
            }
            else
            {
                Client.Instance.CreateRoom(args[0]);
            }
        }
    }

    void RequestRooms(string[] args)
    {
        if (args.Length > 0)
        {
            PrintError();
        }
        else
        {
            Logger.Log("Request rooms");
            Client.Instance.RequestRooms();
        }
    }

    void Connect(string[] args)
    {
        if (args.Length < 1) 
        {
            PrintError();
        }
        else
        {
            Logger.Log("Connect with name {0}", args[0]);
            Client.Instance.Connect(args[0]);
        }
    }

    void Join(string[] args)
    {
        if (args.Length < 1)
        {
            PrintError();
        }
        else
        {
            Logger.Log("Join {0}", args[0]);
            Client.Instance.JoinRoom(args[0]);
        }
    }
}
