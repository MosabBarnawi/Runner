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

        public LevelManager CurrentLevel;

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

        [Header("Global Controls")]
        public GlobalJumpSettings GlobalsJumpSettings;

        [Header("Player Global Controls")]
        public GlobalPlayerMovementSettings GlobalPlayerMovementSettings;

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
        }

        private void Start()
        {
            Init();
            developerConsoleBehavior = FindObjectOfType<DeveloperConsoleBehavior>();
            SceneManager.UnloadScene(SceneManager.GetActiveScene()); //TODO:: FIND A BETTER WAY TO UNLAD THIS, IT MIGHT CAUSE ERRORS
        }

        private void OnDisable()
        {
            if (PasueButton != null) PasueButton.onClick.RemoveAllListeners();
            if (ConsoleButton != null) ConsoleButton.onClick.RemoveAllListeners();
        }

        private void Update()
        {
#if UNITY_EDITOR
            ToggleConsoleScreen();

            if (Input.GetKeyDown(KeyCode.R))
            {
                //Scene sceneLoaded = SceneManager.GetActiveScene();
                //SceneManager.LoadScene(sceneLoaded.buildIndex);

                //CurrentLevel.SpawnPlayerHere(PlayerInputControls.Player);
            }

            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    SendMessageOut();
            //}
#endif
        }

        #endregion

        #region Public API

        #region Leveling Managment
        public void CompletedLevelState()
        {
            SlowDown?.Invoke(true);
            EnableInputCanvas(false);

            PlayerInputControls.Player.PlayerInWinState();

            SceneManager.LoadScene(4, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            endLevelUI.EnableScreen(true);
        }


        public void SetPostProcessingType(bool winState) //TODO:: MOVE TO POST PROCESSING MANAGER
        {
            if (winState) GlobalVolume.profile = WinStaeprofile;
            else GlobalVolume.profile = Defualtprofile;
        }

        public void GoToNextLevel()
        {
            CurrentLevel = null;

            int LevelToUnload = 2;
            int LevelToLoad = 3;

            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                LevelToLoad = 3;
                LevelToUnload = 2;
            }
            else if (SceneManager.GetActiveScene().buildIndex == 3)
            {
                LevelToLoad = 2;
                LevelToUnload = 3;
            }


            AsyncOperation operation = SceneManager.LoadSceneAsync(LevelToLoad, UnityEngine.SceneManagement.LoadSceneMode.Additive);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished");
                SceneManager.UnloadScene(LevelToUnload);
                SceneManager.UnloadScene(4);

                CurrentLevel = FindObjectOfType<LevelManager>();

                Time.timeScale = TIME_CONSTANTS.NORMAL_TIME;

                PlayerInputControls.Player.ReSpawnToPosition(CurrentLevel.SpawnPosition.position);
                SlowDown?.Invoke(false);
                endLevelUI.EnableScreen(false);
                EnableInputCanvas(true);
            };

        }

        #endregion


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
