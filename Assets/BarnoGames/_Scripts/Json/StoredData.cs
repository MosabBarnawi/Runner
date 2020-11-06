using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class StoredData
{
    public GameSpecifications gameSpecifications;
    public GameManagerSettings gameManagerSettings;

    public StoredData()
    {
        gameSpecifications = new GameSpecifications()
        {
            IS_ENCRYPTED = false
        };

        gameManagerSettings = new GameManagerSettings()
        {
            VALUE_TEST = 50
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
        public int VALUE_TEST;
    }
}
