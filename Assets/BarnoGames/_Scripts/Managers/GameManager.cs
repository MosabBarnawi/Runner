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

public enum TEstEnum { Empty, help, good }


namespace BarnoGames.Runner2020
{
    public class GameManager : GameStateManager
    {
        public static GameManager SharedInstance { get; private set; }

        public int TEXT_VALUE;

        //public Button ConsoleButton;

        public LevelLocationSpawnerManager CurrentLevel;

        [Space(10)]
        [Header("Top UI")]
        [SerializeField] private GameObject TopUI;
        [SerializeField] private Button PasueButton;
        [SerializeField] private Text XText;
        [SerializeField] private TextMeshProUGUI pointText;

        [Space(10)]
        [Header("Pause Menu")]
        [SerializeField] private GameObject PauseMenu;
        private float timescale;

        //[Space(10)]
        //[Header("Console")]
        //[SerializeField] private DeveloperConsoleBehavior developerConsoleBehavior;

        //[Space(10)]//TODO:: MOVE TO POST PROCESSING MANAGER
        //[Header("Post Processing")]
        //[SerializeField] private Volume GlobalVolume;
        //[SerializeField] private VolumeProfile Defualtprofile;
        //[SerializeField] private VolumeProfile WinStaeprofile;
        [SerializeField] private PostProcessingManager postProcessingManager;

        [Space(10)] // END LEVEL
        [Header("Inputs")]
        [SerializeField] private Canvas InputCanavas;
        [SerializeField] private EndLevelUI endLevelUI;
        //public Action SlowDown;
        //public Action<bool> SlowDown;
        //public Action OnWinStateAction;

        [Space(10)]
        [Header("Death UI")]
        [SerializeField] private Canvas DeathCanvas;
        [SerializeField] private Button RestartButton;
        [SerializeField] private Button PlaceHolderAdd;
        private int deathCounter;
        private int numberOfDeathsToShowAdd = 3;

        //public Action OnLevelFinishedLoading;
        //public Action<bool> OnWinState;

        //public GameStateManager gameStateManager = new GameStateManager();

        [SerializeField] private Button RestartButtton;

        [Space(10)]
        [Header("FX")]
        public GameObject TeleportFX;

        private int PointsCount;
        public event EventHandler<TestingM> OnCollectedItem;
        public class TestingM : EventArgs
        {
            public int counter;
            public TEstEnum estEnum;
        }

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
            //if (ConsoleButton != null) ConsoleButton.onClick.AddListener(ToggleConsoleScreen);
            if (RestartButton != null) RestartButton.onClick.AddListener(OnPlayerRestarting);
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.AddListener(OnPlayerRestarting);
            if (RestartButtton != null) RestartButtton.onClick.AddListener(OnPlayerRestarting);
        }

        private void OnDisable()
        {
            if (PasueButton != null) PasueButton.onClick.RemoveAllListeners();
            //if (ConsoleButton != null) ConsoleButton.onClick.RemoveAllListeners();
            if (RestartButton != null) RestartButton.onClick.RemoveAllListeners();
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.RemoveAllListeners();
            if (RestartButtton != null) RestartButtton.onClick.RemoveAllListeners();
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
        }

        #region Game State Manager Overrides
        public override void OnLoadSaveData()
        {
            base.OnLoadSaveData();
            //BarnoDebug.Log($"{name}", "..OnLoadSaveData", BarnoColor.SunYellow);

            // TODO:: IMPLEMET LEVEL TO LOAD
            // BY THIS POING WE SHOULD HAVE ALL OUR DATA
            OnInitilization();
        }
        public override void OnInitilization()
        {
            base.OnInitilization();

            EnableTopUI(false);
            EnablePauseMenu(false);
            EnableButtonInputCanvas(false); //TODO:: CHANGE TO ENUM INSTEAD OF BOOL MAYBE

            //postProcessingManager.SetPostProcessingType(PostProcesingType.Default);
            //BarnoDebug.Log($"{name}", "..OnInitilization", BarnoColor.SunYellow);
        }

        public override void OnBooted()
        {
            base.OnBooted();

            // GET LEVEL INFO AND SET PLAYER SETTINGS
            PlayerInputControls.PlayerScript.PlaceInPosition(CurrentLevel.SpawnPosition.position);
        }

        public override void OnLevelTransition() { }
        public override void OnLevelRestart()
        {
            StartCoroutine(C_DelayLevelRestart()); // REMOVE THIS AFTER TESTING
        }
        public override void OnLevelLoading() { base.OnLevelLoading(); }
        public override void OnLevelFinishedLoading() { }
        public override void OnLevelReady()
        {
            base.OnLevelReady();
            PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);
        }

        public override void OnPlayerSpawning() { }
        public override void OnPlayerReady()
        {
            base.OnPlayerReady();
            EnableButtonInputCanvas(true);
        }
        public override void OnPlayerRestarting()
        {
            //CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
            PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);


            EnableDeathUI(false);
            EnableButtonInputCanvas(true);
            //OnLevelFinishedLoading?.Invoke();
        }
        public override void OnPlayerDeath()
        {
            base.OnPlayerDeath();

            EnableButtonInputCanvas(false);
            EnableDeathUI(true);
        }

        public override void OnInGame()
        {
            base.OnInGame();
        }
        public override void OnPaused()
        {
            base.OnPaused();
        }
        public override void OnShowingAD() { }
        public override void OnWinState()
        {
            base.OnWinState();

            //SlowDown?.Invoke(true);
            //OnWinStateAction?.Invoke();
            //GameSateChange(GameState.WinState);

            EnableButtonInputCanvas(false);
            //endLevelUI.EnableScreen(true);
            //SetPostProcessingType(true);
            //SetDefaultGameState(true);

            SceneManager.LoadScene(4, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
        #endregion
        //public void LevelStart()
        //{
        //    EnableButtonInputCanvas(true);
        //}

        public void LoadSpecificLevel(SceneReference LevelToLoad, Action callback) //TOOD:: REMOVE THIS
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(LevelToLoad.ScenePath, LoadSceneMode.Single);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished");

                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();

                //EnableButtonInputCanvas(true);
                callback?.Invoke();
                //ResetLevelOnFinish();
            };
        }

        #region On Player Deadth 
        //public void PlayerHasDied() => EnableDeathUI(true); // CALLED FROM PLAYER SCRIPT TO DO GAME MANAGER THINGS RELATING TO PLAYERS DEATH
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
            RestartButton.gameObject.SetActive(false);
            PlaceHolderAdd.gameObject.SetActive(false);

            if (deathCounter < numberOfDeathsToShowAdd)
            {
                RestartButton.gameObject.SetActive(true);
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

        //private void RestartLevelOnDeath() // change to RestartLevel
        //{
        //    //CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
        //    PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);


        //    EnableDeathUI(false);
        //    EnableButtonInputCanvas(true);
        //    //OnLevelFinishedLoading?.Invoke();
        //}

        #endregion

        #region Public API

        #region Leveling Managment

        //private void SetDefaultGameState(bool winState)
        //{
        //    //SetPostProcessingType(winState);
        //    postProcessingManager.SetPostProcessingType(winState);
        //    //endLevelUI.EnableScreen(winState);
        //    //SlowDown?.Invoke(winState);

        //    if (!winState)
        //    {
        //        CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
        //        Vector3 newSpawnPositon = new Vector3(CurrentLevel.SpawnPosition.position.x, 20, CurrentLevel.SpawnPosition.position.z);
        //        PlayerInputControls.PlayerScript.PlayerFalling(newSpawnPositon);
        //    }
        //}

        //private void SetPostProcessingType(bool winState) //TODO:: MOVE TO POST PROCESSING MANAGER
        //{
        //    if (winState)
        //    {
        //        GlobalVolume.profile = WinStaeprofile;
        //    }
        //    else
        //    {
        //        GlobalVolume.profile = Defualtprofile;
        //        Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;
        //    }

        //    //OnWinState?.Invoke(winState);
        //}

        //public void ResetLevelOnFinish()
        //{
        //    StartCoroutine(DelayLevelRestart()); // REMOVE THIS AFTER TESTING
        //}

        public void GoToNextLevel()
        {
            StartCoroutine(C_DelayTransition()); // REMOVE THIS AFTER TESTING
        }

        #endregion

        int LevelToLoad = 4;
        private IEnumerator C_DelayTransition()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            //if (CurrentLevel.LevelIndex == 3)
            //{
            //    LevelToLoad = 4;
            //}
            //else if (CurrentLevel.LevelIndex == 4)
            //{
            //    LevelToLoad = 3;
            //}

            AsyncOperation transiionLoadOperation = SceneManager.LoadSceneAsync(CurrentLevel.SceneToLoad.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);

            transiionLoadOperation.completed += (asyncOperation) =>
            {
                //endLevelUI.EnableScreen(false);
                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
                //SetDefaultGameState(false);

                //PlayerInputControls.PlayerScript.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);
                //PlayerInputControls.Player.PlayerFalling();
                //Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;

                StartCoroutine(C_DelayNextLevelLoad());
            };
        }

        private IEnumerator C_DelayNextLevelLoad()
        {
            yield return new WaitForSecondsRealtime(1);

            AsyncOperation loadNextLevelOperation = SceneManager.LoadSceneAsync(LevelToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);

            loadNextLevelOperation.completed += (asyncOperation) =>
            {
                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
                //SetDefaultGameState(false);

                //GameSateChange(GameState.LevelFinishedLoading);
            };
        }

        private IEnumerator C_DelayLevelRestart()
        {
            yield return new WaitForSecondsRealtime(1);

            AsyncOperation reloadLevelOperation = SceneManager.LoadSceneAsync(CurrentLevel.CurrentScene.ToString(), UnityEngine.SceneManagement.LoadSceneMode.Single);

            reloadLevelOperation.completed += (asyncOperation) =>
            {
                //SetDefaultGameState(false);

                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();

                EnableDeathUI(false); // REMOVE THIS IS BUTTONS SHOWING BEFORE PLAYER LANDS

                //TODO:: THIS IS FOR TESTING
                //if (PauseMenu.activeSelf) TogglePauseMenu();
                //OnLevelFinishedLoading?.Invoke();
                //GameSateChange(GameState.LevelFinishedLoading);
            };

        }

        public void AddPoints(int value, Action Collect)
        {
            //Collect?.Invoke();
            PointsCount += value;
            pointText.text = ($"{PointsCount} Points");

            Collect();
        }

        #endregion
        #region Private API

        //private void ToggleConsoleScreen()
        //{
        //    if (developerConsoleBehavior != null)
        //    {
        //        if (Input.GetKeyDown(KeyCode.BackQuote))
        //        {
        //            developerConsoleBehavior.ToggleOptionsMenu();
        //        }
        //    }
        //}

        private void EnablePauseMenu(bool pause)
        {
            //TODO:: CHECK STATES
            if (CurrentGameState != GameState.None) return;

            PauseMenu.SetActive(pause);

            if (pause)
            {
                // PAUSE GAME
                timescale = Time.timeScale;
                XText.text = "X";
                Time.timeScale = TIME_CONSTANTS.PAUSE_TIME;

                OnPaused();
            }
            else
            {
                // RESUME GAME
                XText.text = "II";
                //Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;
                Time.timeScale = timescale; //TODO :: MAYBE KEEP SLOW MOTION FOR END OF LEVEL

                OnInGame();
            }
        }

        private void SendMessageOut()
        {
            PointsCount++;
            OnCollectedItem?.Invoke(this, new TestingM { counter = PointsCount, estEnum = TEstEnum.help });
        }
        #endregion
    }

}
