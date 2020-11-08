using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Utilities.DeveloperConsole.Commands
{
    public abstract class ScriptableCommands : ScriptableObject, IConsoleCommand
    {
        [Header("Default Comand Word")]
        [SerializeField] private string commandWord = string.Empty;

        [Space(5)]
        [Header("Alt Commands Words")]
        [SerializeField] private string [] altCommands;

        [Space(5)]
        [Header("Command Description")]
        [SerializeField] private string commandDescription;

        public string CommandWord => commandWord;
        public string[] AltCommandsWord => altCommands;
        public string CommandDescription => commandDescription;

        public abstract string Process( string [] args );
    }
}
