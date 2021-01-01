using System.Collections.Generic;
using BarnoGames.Utilities;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class OptionsManager : MonoSingleton<OptionsManager>
    {
        [SerializeField] private GameObject optionsCanvas;
        [SerializeField] private Button closeOptionScreenButton;

        [Space(10)]
        [Header("Auto Go In toggle")]
        [SerializeField] private Button autoGoInButton;
        [SerializeField] private Toggle autoGoinToggle;

        private bool _isAutoGoInToGame { get; set; }

        #region Properties
        public bool isAutoGoIn
        {
            get { return _isAutoGoInToGame; }
            private set
            {
                _isAutoGoInToGame = value;
                autoGoinToggle.isOn = value;
                JsonSaveAndLoadScript.instance.SetOptionsSettings();
            }
        }
        #endregion

        #region Unity Calls
        private void OnEnable()
        {
            if (autoGoInButton != null) autoGoInButton.onClick.AddListener(ToggleAutoGoIn);
            if (closeOptionScreenButton != null) closeOptionScreenButton.onClick.AddListener(() => EnableOptionsScreen(false));
        }
        #endregion

        public void SetSetting(Dictionary<SettingsMap, object> hasmap)
        {
            object hash_isAutoGoIn;
            hasmap.TryGetValue(SettingsMap.AutoGoIn, out hash_isAutoGoIn);

            isAutoGoIn = (bool)hash_isAutoGoIn;
        }

        public void EnableOptionsScreen(bool enable)
        {
            optionsCanvas.SetActive(enable);

            if (enable)
            {
                autoGoinToggle.isOn = isAutoGoIn;
            }
        }

        private void ToggleAutoGoIn()
        {
            isAutoGoIn = !isAutoGoIn;
        }
    }
}