using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using BarnoGames.Utilities;
using UnityEngine.UI;

namespace BarnoGames.Runner2020
{
    public class GameBooter : MonoBehaviour
    {
        [SerializeField] private GameObject LogoImage;
        [SerializeField] private VolumeProfile LogoPPprofile;

        [Space(10)]
        [Header("Logo Start")]
        [Tooltip("DOF Focus speed \n The Higer The value the slower the speed")]
        [SerializeField] private float FocusSpeed = 5f;
        [SerializeField] private float GrowDurarion = 3f;
        [SerializeField] private Vector3 GrowRorationStregth = new Vector3(6, 7, 15);
        [SerializeField] private float ShakeDuration = 3f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private Vector3 rotationStrength = new Vector3(2, 3, 1);
        [SerializeField] private int vibrato = 3;
        private float randomness = 90f;

        [Space(10)]
        [Header("Logo Drop")]
        [Tooltip("logo drop out of frame \n The Higer The value the slower the speed")]
        [SerializeField] private float dropSpeed = 5f;
        [SerializeField] private float startLogoDropAfterSeconds = 3f;
        [SerializeField] private Vector3 logoOutOfViewPostion = new Vector3(0, -50, 0);

        [Space(10)]
        [Header("Main Menu Options")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button optionsButton; //TODO:: MOVE OUT TO MAIN MENU
        [SerializeField] private GameObject buttonsPanel;
        [SerializeField] private Button exitButton;

        private Transform logoTranform;
        private DepthOfField dofComponent;

        #region Unity Calls
        void Start()
        {
            DepthOfField tmp;
            if (LogoPPprofile.TryGet<DepthOfField>(out tmp))
            {
                dofComponent = tmp;
            }

            logoTranform = LogoImage.GetComponent<Transform>();
            StartCoroutine(C_LogoAnimation());

            ErrorChecking();

            if (startButton != null) startButton.onClick.AddListener(StartGame);
            if (optionsButton != null) optionsButton.onClick.AddListener(() => OptionsManager.instance.EnableOptionsScreen(true));
            if (exitButton != null) exitButton.onClick.AddListener(() => { GameManager.SharedInstance.QuitGame(); });
        }

        private void OnDestroy()
        {
            startButton.onClick.RemoveAllListeners();
            optionsButton.onClick.RemoveAllListeners();
            exitButton.onClick.RemoveAllListeners();
        }
        #endregion

        private void ErrorChecking()
        {
            if (startButton == null) Debug.LogError("start Button not Assigned");

            if (optionsButton == null) Debug.LogError("Options Button not Assigned");

            if (buttonsPanel == null) Debug.LogError("Buttons Panel Not Assigned");

            if (exitButton == null) Debug.LogError("Exit Button Not Assigned");

            if (LogoImage == null) Debug.LogError("Log Image Not Assigned");
            if (LogoPPprofile == null) Debug.LogError("Logo Post Processing Not Assigned");
        }

        private void AnimateBootLogo()
        {
            LogoImage.transform.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();
            Sequence seqFocus = DOTween.Sequence();

            seqFocus.Join(DOTween.To(() => dofComponent.focusDistance.value, x => dofComponent.focusDistance.value = x, 10, FocusSpeed));

            seq
                .Append(logoTranform.DOScale(1, GrowDurarion))
                .Join(logoTranform.DOShakeRotation(GrowDurarion, GrowRorationStregth, vibrato, randomness, true))
                .Append(logoTranform.DOShakePosition(ShakeDuration, strength, vibrato, randomness, false, true))
                .Join(logoTranform.DOShakeRotation(ShakeDuration, rotationStrength, vibrato, randomness, true))
                .Join(logoTranform.DOShakePosition(ShakeDuration, strength, vibrato, randomness, false, true));

            seq.OnComplete(AnimateGameInitilizedMethod);
        }

        private IEnumerator C_AnimateGameInitilized()
        {
            yield return new WaitForSeconds(startLogoDropAfterSeconds);

            if (OptionsManager.instance.isAutoGoIn) StartGame();
            else
            {
                logoTranform.DOMove(logoOutOfViewPostion, dropSpeed);

                Transform buttonsTransfrom = buttonsPanel.GetComponent<Transform>();
                buttonsTransfrom.DOScale(1, GrowDurarion);

                EnableButtons(true, true);
            }
        }

        private void EnableButtons(bool isActive, bool isInteractable)
        {
            buttonsPanel.gameObject.SetActive(isActive);

            startButton.interactable = isInteractable;
            startButton.enabled = isInteractable;

            optionsButton.interactable = isInteractable;
            optionsButton.enabled = isInteractable;

            exitButton.interactable = isInteractable;
            exitButton.enabled = isInteractable;
        }

        private void StartGame()
        {
            EnableButtons(true, false);
            GameManager.SharedInstance.OnBooted();
        }

        private void AnimateGameInitilizedMethod() => StartCoroutine(C_AnimateGameInitilized());

        private IEnumerator C_LogoAnimation()
        {
            dofComponent.focusDistance.value = 0.1f;
            yield return new WaitForSeconds(1f);
            LogoImage.SetActive(true);
            AnimateBootLogo();
        }
    }
}