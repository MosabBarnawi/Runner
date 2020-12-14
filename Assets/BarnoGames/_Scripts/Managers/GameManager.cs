using BarnoGames.Utilities.DeveloperConsole.Behavior;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public enum TEstEnum { Empty, help, good }
namespace BarnoGames.Runner2020
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager SharedInstance { get; private set; }

        public int TEXT_VALUE;

        public Button ConsoleButton;

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

        [Space(10)]
        [Header("Console")]
        [SerializeField] private DeveloperConsoleBehavior developerConsoleBehavior;

        [Space(10)]//TODO:: MOVE TO POST PROCESSING MANAGER
        [Header("Post Processing")]
        [SerializeField] private Volume GlobalVolume;
        [SerializeField] private VolumeProfile Defualtprofile;
        [SerializeField] private VolumeProfile WinStaeprofile;

        [Space(10)] // END LEVEL
        [Header("Inputs")]
        [SerializeField] private Canvas InputCanavas;
        [SerializeField] private EndLevelUI endLevelUI;
        //public Action SlowDown;
        public Action<bool> SlowDown;
        public Action OnWinStateAction;

        [Space(10)]
        [Header("Death UI")]
        [SerializeField] private Canvas DeathCanvas;
        [SerializeField] private Button RestartButton;
        [SerializeField] private Button PlaceHolderAdd;
        public Action OnPlayerRespawn;
        private int deathCounter;
        private int numberOfDeathsToShowAdd = 3;



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
            if (PasueButton != null) PasueButton.onClick.AddListener(TogglePauseMenu);
            if (ConsoleButton != null) ConsoleButton.onClick.AddListener(ToggleConsoleScreen);
            if (RestartButton != null) RestartButton.onClick.AddListener(RestartAfterPlayerDeath);
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.AddListener(RestartAfterPlayerDeath);
        }

        private void Start()
        {
            Init();
            developerConsoleBehavior = FindObjectOfType<DeveloperConsoleBehavior>();
            SceneManager.UnloadSceneAsync("Player"); //TODO:: FIND A BETTER WAY TO UNLAD THIS, IT MIGHT CAUSE ERRORS

            EnableInputCanvas(false);
            SetDefaultGameState(false);
            //PlayerInputControls.Player.PlayerFalling();

            PlayerInputControls.Player.PlayerHasDiedAction = PlayerHasDied;
        }

        private void OnDisable()
        {
            if (PasueButton != null) PasueButton.onClick.RemoveAllListeners();
            if (ConsoleButton != null) ConsoleButton.onClick.RemoveAllListeners();
            if (RestartButton != null) RestartButton.onClick.RemoveAllListeners();
            if (PlaceHolderAdd != null) PlaceHolderAdd.onClick.RemoveAllListeners();
        }

        private void Update()
        {
#if UNITY_EDITOR
            ToggleConsoleScreen();

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetLevel();
            }

            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    //LevelStart();
            //    //PlayerInputControls.Player.LevelStarted();
            //}

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    SendMessageOut();
            //}
#endif
        }

        #endregion
        public void LevelStart()
        {
            EnableInputCanvas(true);
        }

        public void LoadSpecificLevel(int LevelToLoad)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(LevelToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished");
                //SceneManager.UnloadScene(LevelToUnload);
                //SceneManager.UnloadScene(4);

                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();

                //Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;

                PlayerInputControls.Player.ReSpawnToPositionLevelStart_falling(CurrentLevel.SpawnPosition.position);
                //SlowDown?.Invoke(false);
                SetDefaultGameState(false);
                EnableInputCanvas(true);

                ResetLevel();
            };
        }

        private void PlayerHasDied() => EnableDeathUI(true); // CALLED FROM PLAYER SCRIPT TO DO GAME MANAGER THINGS RELATING TO PLAYERS DEATH

        #region Public API

        #region Leveling Managment
        public void CompletedLevelState()
        {
            //SlowDown?.Invoke(true);
            OnWinStateAction?.Invoke();

            EnableInputCanvas(false);
            //endLevelUI.EnableScreen(true);
            //SetPostProcessingType(true);
            SetDefaultGameState(true);

            SceneManager.LoadScene(4, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        private void SetDefaultGameState(bool winState)
        {
            SetPostProcessingType(winState);
            endLevelUI.EnableScreen(winState);
            SlowDown?.Invoke(winState);

            if (!winState) PlayerInputControls.Player.PlayerFalling();
        }

        private void SetPostProcessingType(bool winState) //TODO:: MOVE TO POST PROCESSING MANAGER
        {
            if (winState)
            {
                GlobalVolume.profile = WinStaeprofile;
            }
            else
            {
                GlobalVolume.profile = Defualtprofile;
                Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;
            }
        }

        public void ResetLevel()
        {
            int currentLevel = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentLevel);

            CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
            PlayerInputControls.Player.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);

            SetDefaultGameState(false);

            //endLevelUI.EnableScreen(false);
            EnableDeathUI(false);
            //TODO:: THIS IS FOR TESTING
            if (PauseMenu.activeSelf) TogglePauseMenu();

            PlayerInputControls.Player.LevelStarted();
        }

        private void RestartAfterPlayerDeath()
        {
            //ResetLevel();
            CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
            PlayerInputControls.Player.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);

            EnableDeathUI(false);

            EnableInputCanvas(true);
            OnPlayerRespawn?.Invoke();
        }

        private bool ShowAD()
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
                PlaceHolderAdd.gameObject.SetActive(true);
                Debug.Log("Show Ad");
                deathCounter = 0;
                return true;
            }
        }

        public void GoToNextLevel()
        {
            CurrentLevel = null;

            //PlayerInputControls.Player.PlayerFalling();
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                LevelToLoad = 3;
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                LevelToLoad = 2;
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(5, UnityEngine.SceneManagement.LoadSceneMode.Single);

            operation.completed += (asyncOperation) =>
            {
                //endLevelUI.EnableScreen(false);
                SetDefaultGameState(false);

                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();
                PlayerInputControls.Player.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);
                PlayerInputControls.Player.PlayerFalling();
                //Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;

                StartCoroutine(startNextLevelStudd());
            };
        }

        #endregion

        int LevelToLoad = 3;
        private IEnumerator startNextLevelStudd()
        {
            yield return new WaitForSecondsRealtime(3);

            CurrentLevel = null;

            AsyncOperation nextLevelOperation = SceneManager.LoadSceneAsync(LevelToLoad, UnityEngine.SceneManagement.LoadSceneMode.Single);

            nextLevelOperation.completed += (asyncOperation2) =>
            {
                Debug.Log("Finished");
                CurrentLevel = FindObjectOfType<LevelLocationSpawnerManager>();

                PlayerInputControls.Player.ReSpawnToPositionLevelStart_falling(CurrentLevel.SpawnPosition.position);

                //SlowDown?.Invoke(false);
                //SetDefaultGameState(false);
            };
        }


        private void EnableInputCanvas(bool enable)
        {
            InputCanavas.gameObject.SetActive(enable);
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

        private void Init()
        {
            if (PauseMenu != null) PauseMenu.SetActive(false);
            else Debug.LogError("Pause Menu Canvas Not Assigned");

            if (TopUI == null) Debug.LogError("Top UI Has not Been Assigned");
            if (PasueButton == null) Debug.LogError("Pause Button Not Assigned");


            /// SPAWN THEN DO THINGS
            //player = FindObjectOfType<Player>();
        }

        private void EnableTopUI(bool enable)
        {
            TopUI.SetActive(enable);
        }

        private void ToggleConsoleScreen()
        {
            if (developerConsoleBehavior != null)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    developerConsoleBehavior.ToggleOptionsMenu();
                }
            }
        }

        private void EnableDeathUI(bool enable)
        {
            DeathCanvas.gameObject.SetActive(enable);
            EnableInputCanvas(!enable);
            EnableTopUI(!enable);

            if (enable) ShowAD();
        }

        private void TogglePauseMenu()
        {
            bool toggleStatus = PauseMenu.activeSelf;
            PauseMenu.SetActive(!toggleStatus);

            if (toggleStatus)
            {
                // RESUME GAME
                XText.text = "II";
                //Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;
                Time.timeScale = timescale; //TODO :: MAYBE KEEP SLOW MOTION FOR END OF LEVEL
            }
            else
            {
                // PAUSE GAME
                timescale = Time.timeScale;
                XText.text = "X";
                Time.timeScale = TIME_CONSTANTS.PAUSE_TIME;
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
