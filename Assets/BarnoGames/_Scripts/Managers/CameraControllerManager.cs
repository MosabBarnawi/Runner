using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace BarnoGames.Runner2020
{
    public class CameraControllerManager : MonoBehaviour // TODO:: WINSTATE CAMERA CONTROLLER MANAGER
    {
        private Swipe swipe;
        private float directionH;
        private float directionV;

        private bool TAP, DOUBLE_TAP, RIGHT, LEFT, UP, DOWN;
        private bool isWinState = false;

        private Transform _target;

        [SerializeField] private CinemachineVirtualCamera CameraH;
        [SerializeField] private CinemachineVirtualCamera CameraV;
        [SerializeField] private CinemachineBrain cinemachineBrain;

        CinemachineTrackedDolly horizontalDolly;
        CinemachineTrackedDolly verticalDolly;

        [SerializeField] private CinemachineStateDrivenCamera stateDrivenCamera;
        //[SerializeField] private CinemachineMixingCamera winStateCamera;
        //[SerializeField] private CinemachineVirtualCamera defaultCamera;
        //[SerializeField] private CinemachineVirtualCamera fallingCamera;

        #region Unity Calls
        private void Awake()
        {
            GameManager.SharedInstance.CameraControllerManager = this;
            GameManager.SharedInstance.RegisterGameState(EnableCameraManager, GameState.Booted);
            gameObject.SetActive(false);
        }
        void Start()
        {
            swipe = GetComponent<Swipe>();
            horizontalDolly = CameraH.GetCinemachineComponent<CinemachineTrackedDolly>();
            verticalDolly = CameraV.GetCinemachineComponent<CinemachineTrackedDolly>();

            GameManager.SharedInstance.RegisterGameState(OnWinState, GameState.WinState);
            GameManager.SharedInstance.RegisterGameState(FallingCamera, GameState.LevelReady);
            GameManager.SharedInstance.RegisterGameState(DefaultCamera, GameState.InGame);
        }

        void Update()
        {
            MoveCameraInWinstate();
        }

        #endregion

        public void SetTarget(Character target)
        {
            _target = target.transform;
            stateDrivenCamera.m_AnimatedTarget = target.Anim;
        }

        private void MoveCameraInWinstate()
        {
            if (isWinState)
            {
                if (swipe.SwipeRight)
                {
                    RIGHT = true;
                    LEFT = false;
                }
                else if (swipe.SwipeLeft)
                {
                    RIGHT = false;
                    LEFT = true;
                }

                if (swipe.SwipeUp)
                {
                    UP = true;
                    DOWN = false;
                }
                else if (swipe.SwipeDown)
                {
                    UP = false;
                    DOWN = true;
                }

                if (!swipe.Hold)
                {
                    cinemachineBrain.m_IgnoreTimeScale = false;
                    RIGHT = LEFT = UP = DOWN = false;
                    directionH = 1;
                    directionV = 1;
                }
                else
                {
                    cinemachineBrain.m_IgnoreTimeScale = true;

                    if (RIGHT) directionH = 2;
                    else if (LEFT) directionH = 0;

                    if (UP) directionV = 0;
                    else if (DOWN) directionV = 2;
                }


                horizontalDolly.m_PathPosition = directionH;
                verticalDolly.m_PathPosition = directionV;
            }
        }

        private void OnWinState()
        {
            EnableSwipeControls();
        }

        private void DefaultCamera()
        {
            DisableSwipeControls();
        }

        private void FallingCamera()
        {
            DisableSwipeControls();
        }

        private void EnableCameraManager() => gameObject.SetActive(true);

        private void EnableSwipeControls() => isWinState = true;

        private void DisableSwipeControls() => isWinState = false;
    }
}