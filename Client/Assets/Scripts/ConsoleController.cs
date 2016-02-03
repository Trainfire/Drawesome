using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConsoleController
{
    public Action<string> Log;

    public delegate void Command(string[] args);

    Dictionary<string, Command> Commands = new Dictionary<string, Command>();

    public ConsoleController()
    {
        RegisterCommand("/join", Join);
        RegisterCommand("/connect", Connect);
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
            Debug.LogFormat("Execute command {0} with following arguments: ", command);
            Commands[command](args);
        }
        else
        {
            Debug.LogErrorFormat("Invalid command '{0}'", command);
        }
    }

    void RegisterCommand(string command, Command handler)
    {
        Commands.Add(command, handler);
    }

    void PrintError()
    {
        Debug.LogErrorFormat("Incorrect number of arguments");
    }

    void LogCommand(string message, params object[] args)
    {
        if (Log != null)
        {
            var formattedMessage = string.Format(message, args);
            Log(formattedMessage);
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
            LogCommand("Connect with name {0}", args[0]);
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
            LogCommand("Join {0}", args[0]);
            Client.Instance.JoinRoom(args[0]);
        }
    }
}
