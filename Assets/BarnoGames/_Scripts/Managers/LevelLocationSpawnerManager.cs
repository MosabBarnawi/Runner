using UnityEngine.SceneManagement;
using System.Collections.Generic;
using BarnoGames.Utilities;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class LevelLocationSpawnerManager : MonoBehaviour
    {
        [Tooltip("If True Will Call OnLevelReady()")]
        [SerializeField] private bool autoInitilized = false;
        [SerializeField] private SceneReference _currentScene;
        [SerializeField] private SceneReference _sceneToLoad;

        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _playerRespanPositionAfterDeath;

        #region Properties
        public Transform SpawnPosition => _spawnPoint;
        public Transform PlayerRespawnPosition => _playerRespanPositionAfterDeath;
        public SceneReference CurrentScene => _currentScene;
        public SceneReference SceneToLoad => _sceneToLoad;
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (autoInitilized)
            {
                if (_currentScene == null) Debug.LogError("Current Scene Must be Assigned");
                if (_sceneToLoad == null) Debug.LogError("Scene To Load Must be Assigned");
            }
        }
#endif

        private void OnEnable()
        {
            GameManager.SharedInstance.CurrentLevel = this;

            if (autoInitilized)
            {
                GameManager.SharedInstance.OnLevelReady();
            }
        }
    }
}