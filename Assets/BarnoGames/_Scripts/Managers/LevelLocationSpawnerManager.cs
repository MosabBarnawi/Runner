using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace BarnoGames.Runner2020
{
    public class LevelLocationSpawnerManager : MonoBehaviour // TODO:: CHANGE TO SCRIPTABLE OBJECT AND LET LEVEL MANAGER HANDLE THE SPAWN POSITIONS INSTEAD
    {
        //[BarnoScene] [SerializeField] private string SceneName;

        [SerializeField] private Transform spawnPoint;
        [SerializeField] private int levelIndex;
        public Transform SpawnPosition => spawnPoint;
        public int LevelIndex => levelIndex;

        private void OnEnable()
        {
            levelIndex = SceneManager.GetActiveScene().buildIndex; //TODO:: THIS MIGHT CAUSE ISSUES
        }
    }
}