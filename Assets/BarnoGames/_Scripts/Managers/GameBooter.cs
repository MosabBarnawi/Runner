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
        //[SerializeField] private SceneReference MainGameScene;
        [SerializeField] private SceneReference PlayerScene;
        [SerializeField] private SceneReference ManagersScene;

        [SerializeField] private SceneReference DefaultTransitionScene;
        //[SerializeField] private SceneReference LoadedScene;

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

        [SerializeField] private Button startButton;

        //public MinFloatParameter DOFValue;

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

            if (startButton != null)
            {
                startButton.gameObject.SetActive(false);
                startButton.onClick.AddListener(StartGame);
            }
        }

        private void OnDestroy()
        {
            GameManager.SharedInstance.UnRegisterGameState(AnimateGameInitilizedMethod);
            startButton.onClick.RemoveAllListeners();
        }
        #endregion

        private void AnimateBootLogo()
        {
            LogoImage.transform.localScale = Vector3.zero;
            //Transform logo = LogoImage.GetComponent<Transform>();

            Sequence seq = DOTween.Sequence();
            Sequence seqFocus = DOTween.Sequence();

            seqFocus.Join(DOTween.To(() => dofComponent.focusDistance.value, x => dofComponent.focusDistance.value = x, 10, FocusSpeed));


            seq
                .Append(logoTranform.DOScale(1, GrowDurarion))
                .Join(logoTranform.DOShakeRotation(GrowDurarion, GrowRorationStregth, vibrato, randomness, true))
                .Append(logoTranform.DOShakePosition(ShakeDuration, strength, vibrato, randomness, false, true))
                .Join(logoTranform.DOShakeRotation(ShakeDuration, rotationStrength, vibrato, randomness, true))
                .Join(logoTranform.DOShakePosition(ShakeDuration, strength, vibrato, randomness, false, true));

            seq.OnComplete(OpenMainApplicationLauncher);
        }

        private IEnumerator C_AnimateGameInitilized()
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerScene.ScenePath, LoadSceneMode.Additive);

            //operation.completed += (playerAsyncOperation) =>
            //{
            //    if (GameManager.SharedInstance == null) Debug.LogError("Game Manager is NULL");

            //    GameManager.SharedInstance.OnBooted();
            //};

            yield return new WaitForSeconds(startLogoDropAfterSeconds);
            logoTranform.DOMove(logoOutOfViewPostion, dropSpeed);
            startButton.gameObject.SetActive(true);
        }

        private void StartGame()
        {
            startButton.interactable = false;
            startButton.enabled = false;

            GameManager.SharedInstance.OnLevelLoading();

            AsyncOperation loadTransitionScene = SceneManager.LoadSceneAsync(DefaultTransitionScene.ScenePath, LoadSceneMode.Single);

            loadTransitionScene.completed += (sceneAsyncOperation) =>
            {
                // TRANSITION LEVEL HAS LOADED
                // LOAD PLAYER IN
                // TODO :: IF MTIPLE PLAYER SELCTION HANDLE
                //AsyncOperation operation = SceneManager.LoadSceneAsync(PlayerScene.ScenePath, LoadSceneMode.Additive);

                //operation.completed += (playerAsyncOperation) =>
                //{
                if (GameManager.SharedInstance == null) Debug.LogError("Game Manager is NULL");

                GameManager.SharedInstance.OnBooted();
                //};
            };
        }

        private void OpenMainApplicationLauncher()
        {
            //TODO:: SCREEN FX AND TRANSITION TO MAIN APPLICATION
            AsyncOperation operation = SceneManager.LoadSceneAsync(ManagersScene.ScenePath, LoadSceneMode.Additive);

            operation.completed += (asyncOperation) =>
            {
                Debug.Log("Finished Logo");

                if (GameManager.SharedInstance == null) Debug.LogError("Game Manager is NULL");
                GameManager.SharedInstance.RegisterGameState(AnimateGameInitilizedMethod, GameState.Init);
            };
        }

        private void AnimateGameInitilizedMethod() => StartCoroutine(C_AnimateGameInitilized());

        private IEnumerator C_LogoAnimation()
        {
            //TODO;: LOAD USER DATA IF NECESSERY
            dofComponent.focusDistance.value = 0.1f;
            yield return new WaitForSeconds(1f);
            LogoImage.SetActive(true);
            AnimateBootLogo();
        }
    }
}