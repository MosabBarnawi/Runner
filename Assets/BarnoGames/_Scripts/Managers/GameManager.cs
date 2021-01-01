using BarnoGames.Utilities.DeveloperConsole.Behavior;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Rendering;
using BarnoGames.Utilities;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;
using BarnoUtils;

namespace BarnoGames.Runner2020
{
    public class GameManager : GameStateManager
    {
        public static GameManager SharedInstance { get; private set; }

        public int TEXT_VALUE;
        [SerializeField] private SceneReference PlayerScene;

        [SerializeField] private SceneReference fallBackScene;
        private string _sceneToLoad;

        public LevelLocationSpawnerManager CurrentLevel;
        [SerializeField] private Button pauseRestartButton;

        public CameraControllerManager CameraControllerManager { get; set; }

        [Space(10)]
        [Header("Top UI")] //TODO SPERATE TO OTHER CLASSES
        [SerializeField] private GameObject TopUI;
        [SerializeField] private Button PasueButton;
        [SerializeField] private Text XText;
        [SerializeField] private TextMeshProUGUI pointText;

        [Space(10)]
        [Header("Pause Menu")]
        [SerializeField] private GameObject PauseMenu;
        [SerializeField] private Button OptionsButton;
        [SerializeField] private Button exitButton;
        private float timescale = 1f;

        [SerializeField] private PostProcessingManager postProcessingManager;

        [Space(10)] // END LEVEL
        [Header("Inputs")]
        [SerializeField] private Canvas InputCanavas;
        [SerializeField] private EndLevelUI endLevelUI;

        [Space(10)]
        [Header("Death UI")]
        [SerializeField] private Canvas DeathCanvas;
        [SerializeField] private Button DeathRestartButton;
        [SerializeField] private Button PlaceHolderAdd;
        private int deathCounter;
        private int numberOfDeathsToShowAdd = 3;

        [Space(10)]
        [Header("FX")]
        public GameObject TeleportFX;

        private int PointsCount;

        #region Unity Callbacks
        void Awake()
        {
            if (SharedInstance == null)
            {
                SharedInstance = this;
            }
            else if (SharedInstance != this)
            {
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            ErrorChecking();

            if (PasueButton != null) PasueButton.onClick.AddListener(() => EnablePauseMenu(!PauseMenu.activeSelf));
            if (DeathRestartButton != null) DeathRestartButton.onClick.AddListener(OnPlayerRestarting);
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.AddListener(OnPlayerRestarting);
            if (pauseRestartButton != null) pauseRestartButton.onClick.AddListener(OnPlayerRestarting);
            if (OptionsButton != null) OptionsButton.onClick.AddListener(() => OptionsManager.instance.EnableOptionsScreen(true));
            if (exitButton != null) exitButton.onClick.AddListener(QuitGame);
        }

        private void OnDisable()
        {
            if (PasueButton != null) PasueButton.onClick.RemoveAllListeners();
            if (DeathRestartButton != null) DeathRestartButton.onClick.RemoveAllListeners();
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.RemoveAllListeners();
            if (pauseRestartButton != null) pauseRestartButton.onClick.RemoveAllListeners();
            if (OptionsButton != null) OptionsButton.onClick.RemoveAllListeners();
            if (exitButton != null) exitButton.onClick.RemoveAllListeners();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                EnablePauseMenu(pause);
            }
        }
        #endregion

        private void ErrorChecking()
        {
            if (TopUI == null) Debug.LogError("Top UI Has not Been Assigned");
            if (PasueButton == null) Debug.LogError("Pause Button Not Assigned");
            if (PauseMenu == null) Debug.LogError("Pause Menu Canvas Not Assigned");

            if (postProcessingManager == null) Debug.LogError("Post Processing Manager Not Assigned");

            if (PasueButton == null) Debug.LogError("Pause Button not Assigned");
            if (DeathRestartButton == null) Debug.LogError("Restart Button not Assigned");
            if (PlaceHolderAdd == null) Debug.LogError("Place Holder Ad Button not Assigned");
            if (pauseRestartButton == null) Debug.LogError("Restart Button Button not Assigned");
            if (OptionsButton == null) Debug.LogError("Options Button not Assigned");
            if (exitButton == null) Debug.LogError("Exit Button is Not Assigned");

            if (endLevelUI == null) Debug.LogError("End Level Ui Not Assigned");
            if (PlayerScene == null) Debug.LogError("Player Scene Not Assigned");
        }

        #region Game State Manager Overrides
        public override void OnInitilization()
        {
            base.OnInitilization();

            EnableTopUI(false);
            EnableButtonInputCanvas(false);

            RegisterGameState(() => EnableTopUI(false), GameState.WinState);

            //TODO SPAWN PLAYER
            AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerScene.ScenePath, LoadSceneMode.Additive);
            operation.completed += (sceneAsyncOperation) =>
            {

            };
        }

        public override void OnBooted()
        {
            base.OnBooted();

            if (_sceneToLoad != Level.EMPTY_LEVEL)
            {
                Debug.Log(_sceneToLoad + "....................");
                LoadSpecificLevel(_sceneToLoad);
            }
            else
            {
                LoadSpecificLevel(fallBackScene);
            }
        }

        public override void OnLevelRestart()
        {
            base.OnLevelRestart();
            AsyncOperation operation = SceneManager.LoadSceneAsync(CurrentLevel.CurrentScene, LoadSceneMode.Single);
        }
        public override void OnLevelReady()
        {
            base.OnLevelReady();

            postProcessingManager.SetPostProcessingType(PostProcesingType.Default);
            PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.SpawnPosition.position, true);
        }

        public override void OnPlayerRestarting()
        {
            base.OnPlayerRestarting();

            PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.PlayerRespawnPosition.position, false);
            OnInGame();
        }
        public override void OnPlayerDeath()
        {
            base.OnPlayerDeath();

            EnableButtonInputCanvas(false);
            EnableDeathUI(true);
        }

        public override void OnInGame()
        {
            if (CurrentGameState != GameState.PlayerDeath && CurrentGameState != GameState.WinState)
            {
                base.OnInGame();

                EnableDeathUI(false);
                EnableButtonInputCanvas(true);
                EnableTopUI(true);
            }

            Time.timeScale = timescale;
            PauseMenu.SetActive(false);
            XText.text = "II";

        }
        public override void OnPaused()
        {
            if (CurrentGameState != GameState.PlayerDeath && CurrentGameState != GameState.WinState)
            {
                base.OnPaused();
            }

            PauseMenu.SetActive(true);
            XText.text = "X";
            timescale = Time.timeScale;
            Time.timeScale = TIME_CONSTANTS.PAUSE_TIME;
        }
        public override void OnShowingAD() { base.OnShowingAD(); }
        public override void OnWinState()
        {
            base.OnWinState();

            EnableButtonInputCanvas(false);
            postProcessingManager.SetPostProcessingType(PostProcesingType.WinState);
        }

        public void QuitGame()
        {
            // save any game data here
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }

        #endregion

        private bool SavableCondition()
        {
            if (CurrentGameState == GameState.LevelReady
                || CurrentGameState == GameState.InGame
                || CurrentGameState == GameState.Paused)
            {
                return true;
            }

            return false;
        }

        public string GetCurrentScene()
        {
            if (SavableCondition())
            {
                return CurrentLevel.CurrentScene;
            }
            return Level.EMPTY_LEVEL;
        }

        public void SceneToLoad(string sceneReference)
        {
            _sceneToLoad = sceneReference;
        }

        private void LoadSpecificLevel(string LevelToLoad)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(LevelToLoad, LoadSceneMode.Single);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished");
                //OnLevelReady();
            };
        }

        #region On Player Deadth 
        private void EnableDeathUI(bool enable)
        {
            DeathCanvas.gameObject.SetActive(enable);

            if (enable) ShouldShowAd();
        }

        private void EnableButtonInputCanvas(bool enable)
        {
            InputCanavas.gameObject.SetActive(enable);
        }

        private void EnableTopUI(bool enable)
        {
            TopUI.SetActive(enable);
        }

        private bool ShouldShowAd()
        {
            deathCounter++;
            DeathRestartButton.gameObject.SetActive(false);
            PlaceHolderAdd.gameObject.SetActive(false);

            if (deathCounter < numberOfDeathsToShowAdd)
            {
                DeathRestartButton.gameObject.SetActive(true);
                return false;
            }
            else
            {
                AdsManager.SharedInstance.ShowAD(); // TODO:: IMPLEMENT ADS
                PlaceHolderAdd.gameObject.SetActive(true);
                deathCounter = 0;
                return true;
            }
        }

        #endregion

        #region Public API

        #region Leveling Managment
        public void GoToNextLevel()
        {
            AsyncOperation transiionLoadOperation = SceneManager.LoadSceneAsync(CurrentLevel.SceneToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);

            transiionLoadOperation.completed += (asyncOperation) =>
            {

            };
        }

        #endregion

        public void AddPoints(int value, Action Collect)
        {
            //Collect?.Invoke();
            PointsCount += value;
            pointText.text = ($"{PointsCount} Points");

            Collect();
        }

        #endregion
        #region Private API
        private void EnablePauseMenu(bool pause)
        {
            //TODO:: CHECK STATES
            if (CurrentGameState == GameState.WinState) return;

            if (pause)
            {
                // PAUSE GAME
                OnPaused();
            }
            else
            {
                // RESUME GAME
                OnInGame();
            }
        }
        #endregion
    }

}
