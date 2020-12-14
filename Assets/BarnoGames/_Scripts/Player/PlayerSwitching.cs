using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class PlayerSwitching : MonoBehaviour
    {
        public Action<PlayerType> SwitchPlayerAction;

        [Header("Switch Player")]
        private PlayerType playerType;
        [SerializeField] private GameObject MainPlayer;
        [SerializeField] private GameObject SecondaryPlayer; //TODO:: ADD FAKE ROLLING

        private Character character;

        private CapsuleCollider capsuleCollider;
        private SphereCollider sphereCollider;

        private Rigidbody mainPlayerRB;
        private Rigidbody secondaryRB;

        #region Unity Callbacks
        private void OnEnable()
        {
            PlayerInputControls.SpecialAbility = SwitchPlayers;
        }

        private void Awake()
        {
            if (MainPlayer == null)
                Debug.LogError("Main Player Not Assigned");
            if (SecondaryPlayer == null)
                Debug.LogError("Secondary Player Not Assigned");

            mainPlayerRB = MainPlayer.GetComponent<Rigidbody>();
            secondaryRB = SecondaryPlayer.GetComponent<Rigidbody>();

            capsuleCollider = MainPlayer.GetComponent<CapsuleCollider>();
            sphereCollider = SecondaryPlayer.GetComponent<SphereCollider>();

            character = GetComponent<Character>();
        }

        private void Start()
        {
            if (PlayerInputControls.SpecialAbility == null)
            {
                PlayerInputControls.SpecialAbility = SwitchPlayers;
                Debug.LogWarning($"***Not Assigned . Trying to Find {PlayerInputControls.SpecialAbility.Method}");
            }
        }

        private void OnDisable()
        {
            PlayerInputControls.SpecialAbility -= SwitchPlayers;
        }

        private void OnDestroy()
        {
            PlayerInputControls.SpecialAbility -= SwitchPlayers;
        }
        #endregion

        #region Public API
        public Rigidbody SwitchToMainCharacter(PlayerType playerType, ref Collider currentCollider)
        {
            if (playerType != this.playerType) SwitchPlayers();

            currentCollider = capsuleCollider;
            return mainPlayerRB;
        }

        public Rigidbody SwitchToSecondaryCharacter(PlayerType playerType, ref Collider currentCollider)
        {
            if (playerType != this.playerType) SwitchPlayers();

            currentCollider = sphereCollider;
            return secondaryRB;
        }
        #endregion

        private void SwitchPlayers()
        {
            if(character.CanSwitchPlayers)
            {
                if (playerType == PlayerType.Secondary)
                {
                    MainPlayer.transform.parent = transform;

                    SecondaryPlayer.transform.parent = MainPlayer.transform;

                    MainPlayer.SetActive(true);
                    SecondaryPlayer.SetActive(false);

                    playerType = PlayerType.MainPlayer;

                    SwitchPlayerAction?.Invoke(playerType);
                }
                else if (playerType == PlayerType.MainPlayer)
                {
                    SecondaryPlayer.transform.parent = transform;
                    MainPlayer.transform.parent = SecondaryPlayer.transform;

                    MainPlayer.SetActive(false);
                    SecondaryPlayer.SetActive(true);

                    playerType = PlayerType.Secondary;

                    SwitchPlayerAction?.Invoke(playerType);
                }

                ResetTransforms();
            }
        }

        private void ResetTransforms()
        {
            SecondaryPlayer.transform.localScale = Vector3.one;
            SecondaryPlayer.transform.localEulerAngles = Vector3.zero;

            MainPlayer.transform.localScale = Vector3.one;
            MainPlayer.transform.localEulerAngles = Vector3.zero;
        }
    }
}