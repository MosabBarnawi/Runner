using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BarnoGames.Utilities;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class LevelLocationSpawnerManager : MonoBehaviour // TODO:: CHANGE TO SCRIPTABLE OBJECT AND LET LEVEL MANAGER HANDLE THE SPAWN POSITIONS INSTEAD
    {
        [SerializeField] private SceneReference currentScene;
        [SerializeField] private SceneReference sceneToLoad;

        [SerializeField] private Transform spawnPoint;

        //[SerializeField] private int levelIndex;
        public Transform SpawnPosition => spawnPoint;
        public SceneReference CurrentScene => currentScene;
        public SceneReference SceneToLoad => sceneToLoad;

        //public int LevelIndex => levelIndex;

        private void OnEnable()
        {
            //TODO:: SHOULD BE DONE WHEN A CONDITION IS MET
            GameManager.SharedInstance.CurrentLevel = this;
            GameManager.SharedInstance.OnLevelReady();
            //levelIndex = SceneManager.GetActiveScene().buildIndex; //TODO:: THIS MIGHT CAUSE ISSUES
            //levelIndex = SceneManager.GetActiveScene().buildIndex; 
        }
    }
}