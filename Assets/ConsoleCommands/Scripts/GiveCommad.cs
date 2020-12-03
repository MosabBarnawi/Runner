using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Utilities.DeveloperConsole.Commands
{
    [CreateAssetMenu(fileName = "New Give Command" , menuName = "Barno Utils/DeveloperConsole/Commands/Give")]

    public class GiveCommad : ScriptableCommands
    {
        [SerializeField] private GameObject objectToAlter;

        private string [] commands = new string [] { "kill" , "help" };
        private enum commmandEnum { kill, help };

        public override string Process( string [] args )
        {
            if (args[0].Equals(commands[ (int)commmandEnum.kill ] , StringComparison.OrdinalIgnoreCase))
            {
                Destroy(objectToAlter);
                return "Killed " + objectToAlter.name;
            }
            if (args [ 0 ].Equals(commands [ (int) commmandEnum.help ] , StringComparison.OrdinalIgnoreCase))
            {
                string helpList= null;

                foreach (string item in commands)
                {
                    helpList += item + "\n";
                }
                return helpList;
            }

            Debug.Log(null);
            return null;
        }
    }
}