using BarnoGames.Utilities.DeveloperConsole.Commands;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace BarnoGames.Utilities.DeveloperConsole
{
    public class DeveloperConsole
    {
        private readonly string prefix;
        private readonly IEnumerable<IConsoleCommand> commands;

        public DeveloperConsole( string prefix , IEnumerable<IConsoleCommand> commands )
        {
            this.prefix = prefix;
            this.commands = commands;
        }

        public string ProcessCommand( string inputValue )
        {
            if (!inputValue.StartsWith(prefix)) { return null; }

            inputValue = inputValue.Remove(0 , prefix.Length);

            string [] inputSplit = inputValue.Split(' ');

            string commandInput = inputSplit [ 0 ];
            string [] args = inputSplit.Skip(1).ToArray();

            return ProcessCommand(commandInput , args);
        }

        public string ProcessCommand( string commandInput , string [] args )
        {
            foreach (var command in commands)
            {
                bool defaultWord = commandInput.Equals(command.CommandWord , StringComparison.OrdinalIgnoreCase);

                if (defaultWord)
                {
                    return command.Process(args);
                }
                else
                {
                    if (command.AltCommandsWord.Length > 0)
                    {
                        for (int i = 0 ; i < command.AltCommandsWord.Length ; i++)
                        {
                            if (command.AltCommandsWord == null) continue;

                            bool altWord = commandInput.Equals(command.AltCommandsWord [ i ] , StringComparison.OrdinalIgnoreCase);

                            if (!altWord) continue;

                            return command.Process(args);
                        }
                    }
                }
            }
            return string.Format("{0} command Not Found" , commandInput);
        }
    }
}

