using UnityEngine;
using System.Collections.Generic;

public class ConsoleCommand
{
    public delegate void CommandDelegate(ConsoleCommand command, string[] args);

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

public class ConsoleController
{
    List<ConsoleCommand> Commands = new List<ConsoleCommand>();

    const string Token = "";

    public ConsoleController()
    {
        RegisterCommand(new ConsoleCommand("help", Help));
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
            Debug.LogFormat("\t{0} {1}", c.Command, c.Help);
        }
    }
}
