using System.Collections.Generic;
using BarnoGames.Runner2020;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Utilities
{
    [System.Serializable]
    public class StoredData
    {
        public GameSpecifications gameSpecifications;
        public GameManagerSettings gameManagerSettings;
        public GeneralSettings generalSettings;

        public StoredData()
        {
            gameSpecifications = new GameSpecifications()
            {
                IS_ENCRYPTED = false
            };

            gameManagerSettings = new GameManagerSettings()
            {
                SavedScene = Level.EMPTY_LEVEL,
            };

            generalSettings = new GeneralSettings()
            {
                AutoGoIn = false,
            };
        }

        [System.Serializable]
        public struct GameSpecifications
        {
            public bool IS_ENCRYPTED;
        }

        [System.Serializable]
        public struct GameManagerSettings
        {
            public string SavedScene;
        }

        [System.Serializable]
        public struct GeneralSettings
        {
            public bool AutoGoIn;
        }
    }
}

