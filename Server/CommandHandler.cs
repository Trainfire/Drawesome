using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class CommandHandler
    {
        List<Command> Commands = new List<Command>();

        public class Command
        {
            public string Trigger { get; private set; }
            public Action Action { get; private set; }

            public Command(string trigger, Action action)
            {
                Trigger = trigger;
                Action = action;
            }
        }

        public CommandHandler()
        {
            Commands.Add(new Command("help", () => Help()));
        }

        public void AddCommand(Command command)
        {
            if (!Commands.Contains(command))
                Commands.Add(command);
        }

        public void ParseCommand(string arg)
        {
            var command = Commands.FirstOrDefault(x => x.Trigger == arg.Trim());

            if (command != null)
            {
                command.Action();
            }
            else
            {
                Console.WriteLine("Unrecognised command: '{0}'. Type 'help' for a list of commands.", arg);
            }
        }

        void Help()
        {
            Console.WriteLine("Available Commands:");
            Commands.ForEach(x => Console.WriteLine("\t{0}", x.Trigger));
        }
    }
}
