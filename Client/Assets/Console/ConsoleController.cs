using UnityEngine;
using System.Collections.Generic;

public class ConsoleCommand
{
    public delegate void CommandDelegate(ConsoleCommand command, string[] args);

    public string Command { get; private set; }
    public CommandDelegate Action { get; private set; }
    public string Help { get; private set; }
    public bool Elevated { get; private set; }

    public ConsoleCommand(string command, CommandDelegate action, string help = "")
    {
        Command = command;
        Action = action;
        Help = help;
    }

    public ConsoleCommand(string command, CommandDelegate action, bool elevated, string help = "") : this(command, action, help)
    {
        Elevated = elevated;
    }

    public void MakeElevated()
    {
        Elevated = true;
    }
}

public class ConsoleController
{
    System.Func<bool> HasPermission { get; set; }
    List<ConsoleCommand> Commands = new List<ConsoleCommand>();

    const string Token = "";

    public ConsoleController(System.Func<bool> permissionEvaluator)
    {
        RegisterCommand(new ConsoleCommand("help", Help));
        HasPermission = permissionEvaluator;
    }

    public void SubmitInput(string input)
    {
        if (input != string.Empty)
            ParseInput(input);
    }

    public void RegisterCommand(ConsoleCommand action)
    {
        Commands.Add(action);
    }

    public void RegisterElevatedCommand(ConsoleCommand action)
    {
        action.MakeElevated();
        Commands.Add(action);
    }

    public void PrintError(ConsoleCommand consoleCommand)
    {
        Debug.LogErrorFormat("Incorrect number of arguments for {0}. Requires {1}", consoleCommand.Command, consoleCommand.Help);
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
            bool execute = false;

            if (!command.Elevated)
            {
                execute = true;
            }
            else if (command.Elevated)
            {
                execute = HasPermission();

                if (!execute)
                    Debug.LogError("You do not have permission to do that.");
            }

            if (execute)
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

    void Help(ConsoleCommand command, string[] args)
    {
        Debug.LogFormat("Available Commands:");

        foreach (var c in Commands)
        {
            if (c.Elevated)
            {
                Debug.LogFormat("\t*{0} {1}", c.Command, c.Help);
            }
            else
            {
                Debug.LogFormat("\t*{0} {1}", c.Command, c.Help);
            }
        }
    }
}
