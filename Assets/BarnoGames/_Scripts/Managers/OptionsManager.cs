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
        private bool _isFirstLaunch { get; set; }
        
        #region Properties
        public bool isAutoGoIn
        {
            get { return _isAutoGoInToGame; }
            private set
            {
                _isAutoGoInToGame = value;
                JsonSaveAndLoadScript.instance.SetOptionsSettings();
            }
        }
        public bool isFirstLaunch { get { return _isAutoGoInToGame; } private set {_isAutoGoInToGame = value; } }
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
            object hash_isFirstLaunch;
            hasmap.TryGetValue(SettingsMap.AutoGoIn, out hash_isAutoGoIn);
            hasmap.TryGetValue(SettingsMap.FirstLaunch, out hash_isFirstLaunch);

            isAutoGoIn = (bool)hash_isAutoGoIn;
            autoGoinToggle.isOn = isAutoGoIn;

            isFirstLaunch = (bool)hash_isFirstLaunch;
        }

        public void EnableOptionsScreen(bool enable)
        {
            optionsCanvas.SetActive(enable);
        }

        private void ToggleAutoGoIn()
        {
            isAutoGoIn = !isAutoGoIn;
            autoGoinToggle.isOn = isAutoGoIn;
        }
    }
}