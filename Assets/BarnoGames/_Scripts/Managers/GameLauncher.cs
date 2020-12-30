using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BarnoGames.Utilities;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    //TODO:: REMOVE THIS COMPLETLY
    public class GameLauncher : MonoBehaviour
    {
        [SerializeField] SceneReference PlayerScene;
        [SerializeField] private LevelLocationSpawnerManager levelLocationSpawnerManager;
        void Start()
        {
            //SceneManager.LoadSceneAsync(PlayerScene.name, LoadSceneMode.Additive);

            AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerScene.ScenePath, LoadSceneMode.Additive);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished Boot");

            };
        }
        public void TestLoadLevel()
        {
            Debug.Log("nEXT");
            //GameManager gameManager = FindObjectOfType<GameManager>();
            //int levelToLoad = levelLocationSpawnerManager.SceneToLoad.GetType(SceneManager.GetSceneByName())
            //int LevelToLoad = SceneManager.GetSceneByName(levelLocationSpawnerManager.SceneToLoad.ToString()).buildIndex;

            //Debug.Log(levelLocationSpawnerManager.SceneToLoad.ToString());
            GameManager.SharedInstance.LoadSpecificLevel(levelLocationSpawnerManager.SceneToLoad, null);
            //FindObjectOfType<GameManager>().LoadSpecificLevel(levelLocationSpawnerManager.SceneToLoad.ToString());
        }
    }
}