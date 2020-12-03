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

        #region Unity Callbacks
        private void OnEnable()
        {
            goToNextLevelButton.onClick.AddListener(IHaveBeenClicked);
        }

        private void OnDisable()
        {
            goToNextLevelButton.onClick.RemoveListener(IHaveBeenClicked);
        }

        #endregion

        public void EnableScreen(bool isActive)
        {
            canvas.gameObject.SetActive(isActive);

            if (isActive) StartCoroutine(SlowMotion());

            GameManager.SharedInstance.SetPostProcessingType(isActive);
        }

        private void IHaveBeenClicked()
        {
            GameManager.SharedInstance.GoToNextLevel();
        }

        // TODO:: WHEN CAMERA STOPS MOVING SHOW BUTTONS AND STATS
        private IEnumerator SlowMotion()
        {
            canvas.gameObject.SetActive(true);
            yield return new WaitForSeconds(activationToImageFadeWait);
            imageFadeout.CrossFadeAlpha(0, imageFadeSpeed, true);
            Time.timeScale = winStateTimeSpeed;
        }

    }
}