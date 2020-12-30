using System.Collections.Generic;
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
                //VALUE_TEST = 50
            };

            generalSettings = new GeneralSettings()
            {
                AutoGoIn = true,
                isFirstLaunch = true,
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
            //public int VALUE_TEST;
        }

        [System.Serializable]
        public struct GeneralSettings
        {
            public bool AutoGoIn;
            public bool isFirstLaunch; // TODO:: FOR LATER ON TO GO STARIGHT TO LEVEL TRANSITION IF AUTO GOING IN IS ENABLED
        }
    }
}

