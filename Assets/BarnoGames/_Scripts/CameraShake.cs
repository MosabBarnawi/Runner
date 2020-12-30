using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace BarnoGames.Runner2020
{
    public class CameraShake : MonoBehaviour
    {
        // TODO:: CLEAN UP :: REMOVE STATIC
        public static CameraShake SharedInstance;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        private float shakeTimer;
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

        [SerializeField] private float shakeIntenisity = 3f;
        [SerializeField] private float shakeTime = 0.1f;

        void Awake()
        {
            SharedInstance = this;
        }

        void Update()
        {
            if (shakeTimer > 0)
            {
                shakeTimer -= Time.deltaTime;
                if (shakeTimer <= 0f)
                {
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                }
            }
        }

        public void Shake()
        {
            ShakeCamera(shakeIntenisity, shakeTime);
        }
        private void ShakeCamera(float intensity, float time)
        {
            cinemachineBasicMultiChannelPerlin =
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            shakeTimer = time;
        }
    }
}