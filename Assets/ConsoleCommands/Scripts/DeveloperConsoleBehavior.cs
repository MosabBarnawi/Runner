using BarnoGames.Utilities.DeveloperConsole.Commands;
using System.Collections.Generic;
using BarnoGames.Runner2020;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace BarnoGames.Utilities.DeveloperConsole.Behavior
{
    public class DeveloperConsoleBehavior : MonoBehaviour
    {
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private ScriptableCommands [] commands = new ScriptableCommands [ 0 ];

        [Header("UI")]
        [SerializeField] private GameObject uiCanvas = null;
        [SerializeField] private TMP_InputField inputField = null;
        [SerializeField] private TextMeshProUGUI resultText = null;
        [SerializeField] private ScrollRect scrollRect;

        private float pausedTimeScale;

        private static DeveloperConsoleBehavior instance;

        private DeveloperConsole developerConsole;
        private DeveloperConsole DeveloperConsole
        {
            get
            {
                if (developerConsole != null) { return developerConsole; }
                return developerConsole = new DeveloperConsole(prefix , commands);
            }
        }

        private readonly string clearCommand = "Clear";
        private readonly string altClearCommand = "clr";
        private readonly string helpCommand = "Help";
        private readonly string exitCommand = "Exit";
        private readonly string altExitCommand = "Close";

        #region Unity Callbacks
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Public API
        public void ToggleOptionsMenu()
        {
            if (uiCanvas.activeSelf)
            {
                Time.timeScale = pausedTimeScale;
                uiCanvas.SetActive(false);
            }
            else
            {
                if (Time.timeScale > 0) pausedTimeScale = TIME_CONSTANTS.NORMAL_TIME;
                else pausedTimeScale = TIME_CONSTANTS.PAUSE_TIME;

                Time.timeScale = 0;
                uiCanvas.SetActive(true);
                inputField.ActivateInputField();
            }
        }

        public void ProcessCommand( string inputValue )
        {
            if (commands == null) return;
            if (!inputValue.StartsWith(prefix)) { return; }

            inputField.text = string.Empty;

#if !UNITY_ANDROID && !UNITY_IOS || UNITY_EDITOR
            inputField.ActivateInputField();
#endif

            if (isDefaultCommand(inputValue)) return;

            string result = DeveloperConsole.ProcessCommand(inputValue);

            if (string.IsNullOrWhiteSpace(resultText.text) || string.IsNullOrEmpty(resultText.text)) resultText.text = result;
            else resultText.text += "\n" + result;
        }
        #endregion

        #region Private API

        private bool isDefaultCommand( string inputValue )
        {
            string defaultCommands = inputValue.Remove(0 , prefix.Length);

            #region CLEAR COMMAND
            if (defaultCommands.Equals(clearCommand , StringComparison.OrdinalIgnoreCase)
        || defaultCommands.Equals(altClearCommand , StringComparison.OrdinalIgnoreCase))
            {
                resultText.text = "";
                return true;
            }
            #endregion

            #region HELP COMMAND
            else if (defaultCommands.Equals(helpCommand , StringComparison.OrdinalIgnoreCase))
            {
                string outputText = "\n ======================== List of Commands ========================= \n";

                foreach (ScriptableCommands item in commands)
                {
                    if (!string.IsNullOrWhiteSpace(item.CommandWord) && !string.IsNullOrEmpty(item.CommandWord))
                        outputText += string.Format("{0} Command" + "\n" , item.CommandWord);
                    else outputText += "Missing Title \n";

                    if (!string.IsNullOrWhiteSpace(item.CommandDescription) && !string.IsNullOrEmpty(item.CommandDescription))
                        outputText += item.CommandDescription + "\n";
                    else outputText += "Missing Description \n";

                    outputText += "---- \n";
                }

                if (string.IsNullOrWhiteSpace(resultText.text) || string.IsNullOrEmpty(resultText.text))
                {
                    resultText.text = outputText + "*========================* \n";
                }
                else
                {
                    resultText.text += outputText + "*========================* \n";
                }
                return true;
            }
            #endregion

            #region CLOSE CONSOLE COMMAND
            else if (defaultCommands.Equals(exitCommand , StringComparison.OrdinalIgnoreCase)
                || defaultCommands.Equals(altExitCommand , StringComparison.OrdinalIgnoreCase))
            {
                ToggleOptionsMenu();
                return true;
            }
            #endregion
            return false;
        }
        #endregion
    }
}
