using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Log Command", menuName = "Barno Utils/DeveloperConsole/Commands/Log Command")]
    public class LogCommand : ScriptableCommands
    {
        public override string Process( string [] args )
        {
            string logText = string.Join(" " , args);

            Debug.Log(logText);

            return logText;
        }
    }
}
