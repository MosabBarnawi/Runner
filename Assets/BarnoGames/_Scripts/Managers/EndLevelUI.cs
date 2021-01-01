using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class EndLevelUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image imageFadeout;

        [SerializeField] private float imageFadeSpeed = 1;
        [SerializeField] private float activationToImageFadeWait = 0.58f;

        [SerializeField] private Button goToNextLevelButton;
        [SerializeField] private float winStateTimeSpeed = 0.05f;

        [SerializeField] private Button restartLevelButton;

        #region Unity Callbacks
        private void OnEnable()
        {
            goToNextLevelButton.onClick.AddListener(NextLevelClicked);
            restartLevelButton.onClick.AddListener(RestartLevelClick);
        }

        private void OnDisable()
        {
            goToNextLevelButton.onClick.RemoveListener(NextLevelClicked);
            restartLevelButton.onClick.RemoveListener(RestartLevelClick);
        }

        private void Start()
        {
            EnableEndOfLevelUIScreen(false);
            GameManager.SharedInstance.RegisterGameState(OnWinState, GameState.WinState);
        }

        #endregion

        private void OnWinState() => EnableEndOfLevelUIScreen(true);

        private void EnableEndOfLevelUIScreen(bool isActive)
        {
            canvas.gameObject.SetActive(isActive);

            if (isActive) StartCoroutine(SlowMotion2());
        }

        private void NextLevelClicked()
        {
            EnableEndOfLevelUIScreen(false);
            GameManager.SharedInstance.GoToNextLevel();
        }

        private void RestartLevelClick()
        {
            EnableEndOfLevelUIScreen(false);
            GameManager.SharedInstance.OnLevelRestart();
        }

        // TODO:: WHEN CAMERA STOPS MOVING SHOW BUTTONS AND STATS
        private IEnumerator SlowMotion2()
        {
            imageFadeout.gameObject.SetActive(false);
            yield return new WaitForSeconds(activationToImageFadeWait);
            Time.timeScale = winStateTimeSpeed;
        }
        private IEnumerator SlowMotion()
        {
            canvas.gameObject.SetActive(true);
            yield return new WaitForSeconds(activationToImageFadeWait);
            imageFadeout.CrossFadeAlpha(0, imageFadeSpeed, true);
            Time.timeScale = winStateTimeSpeed;
        }

    }
}