using BarnoGames.Utilities.DeveloperConsole.Behavior;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager SharedInstance;
    public int TEXT_VALUE;

    [Header("Top UI")]
    public GameObject TopUI;
    public Button PasueButton;
    public Text XText;

    [Header("Pause Menu")]
    public GameObject PauseMenu;

    [Header("Console")]
    [SerializeField] private DeveloperConsoleBehavior developerConsoleBehavior;

    #region Unity Callbacks
    void Awake()
    {
        if (SharedInstance == null)
        {
            DontDestroyOnLoad(this);
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
    }

    private void Start()
    {
        Init();
    }

    private void OnDisable()
    {
        if (PasueButton != null) PasueButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
#if UNITY_EDITOR
        ToggleConsoleScreen();
#endif
    }

#endregion

#region Private API

    private void Init()
    {
        if (PauseMenu != null) PauseMenu.SetActive(false);
        else Debug.LogError("Pause Menu Canvas Not Assigned");

        if (TopUI == null) Debug.LogError("Top UI Has not Been Assigned");
        if (PasueButton == null) Debug.LogError("Pause Button Not Assigned");
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

        if(toggleStatus)
        {
            // RESUME GAME
            XText.text = "II";
            Time.timeScale = Constants.NORMAL_TIME;
        }
        else
        {
            // PAUSE GAME
            XText.text = "X";
            Time.timeScale = Constants.PAUSE_TIME;
        }
    }

#endregion
}
