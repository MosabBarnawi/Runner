using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public class BallPlayer : Character, IAbility, IPlayerTAG, IPlayerSwitchable
    {
        [Header("Ray Casting")]
        [SerializeField] private LayerMask WalkableLayerMask;
        [SerializeField] private LayerMask ForwardDetectionLayerMask;
        [SerializeField] private float GroundRaycastDistance = 1.1f;

        [Space(5)]
        [SerializeField] private float RaycastDistanceForward = 0.8f;

        private Collider currentCollider;
        [SerializeField] private Player playerScript;

        [SerializeField] private GameObject DeathFX;

        RaycastHit[] raycasts = new RaycastHit[1];
        private bool canSmash = false;
        private float ballAttackTimer;
        [SerializeField] private float secondaryAttackTime = 1f;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            if (Anim == null) Debug.LogWarning($"Animator Not Assigned {gameObject.name}");
            if (playerScript == null) Debug.LogError($"Player Script Not Assigned {gameObject.name}");

            currentCollider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            InitilizeControls();
        }

        protected override void Start()
        {
            base.Start();

            OnDeath += PlayerDeath;
            OnAddHealth += AddHealthFX;
            OnTakeHit += TakeHit;
            OnRespawn += RespawnPlayer;
        }

        void Update()
        {
            IsGrounded = CheckIfGrounded();
        }

        private void FixedUpdate()
        {
            CheckForwardRayCast();

            if (canSmash)
            {
                if (ballAttackTimer >= secondaryAttackTime)
                    canSmash = false;
                else
                    ballAttackTimer += Time.fixedDeltaTime;
            }
        }

        private void OnDisable()
        {
            GameManager.SharedInstance.UnRegisterGameState(PlayerInWinState);
        }

        private void OnDestroy()
        {
            GameManager.SharedInstance.UnRegisterGameState(PlayerInWinState);
            PlayerInputControls.MainAbilityAction -= MainAbility;
        }
        #endregion

        private void InitilizeControls()
        {
            PlayerInputControls.Player = this;

            PlayerInputControls.MainAbilityAction = MainAbility;
            GameManager.SharedInstance.RegisterGameState(PlayerInWinState, GameState.WinState);

            if (IsDead)
            {
                imove.EnableMovement();
                QUIC_FIX_IS_ALIVE();
                Debug.Log("Was Dead");
            }
        }

        protected override bool CheckIfGrounded()
        {
            bool hitGround = false;

            bool forwardGrounded = Physics.Raycast(currentCollider.bounds.center + new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down, out RaycastHit hitForward, GroundRaycastDistance, WalkableLayerMask);
            bool middleGrounded = Physics.Raycast(currentCollider.bounds.center, Vector3.down, out RaycastHit hitMiddle, GroundRaycastDistance, WalkableLayerMask);
            bool backwardGrounded = Physics.Raycast(currentCollider.bounds.center - new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down, out RaycastHit hitBackward, GroundRaycastDistance, WalkableLayerMask);

            Transform forwardPositon = hitForward.transform;
            Transform middlePositon = hitMiddle.transform;
            Transform backwardPositon = hitBackward.transform;

            hitGround = forwardGrounded || middleGrounded || backwardGrounded;

            if (forwardPositon != null)
            {
                Enemy forwardEnemy = forwardPositon.GetComponent<Enemy>();

                if (forwardEnemy != null)
                {
                    Debug.Log("F: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                               //TODO:: RESET PLAYER
                }
            }
            else if (middlePositon != null)
            {
                Enemy MiddleEnemy = middlePositon.GetComponent<Enemy>();

                if (MiddleEnemy != null)
                {
                    Debug.Log("M: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                               //TODO:: RESET PLAYER
                }

            }
            else if (backwardPositon != null)
            {
                Enemy backEnemy = backwardPositon.GetComponent<Enemy>();
                if (backEnemy != null)
                {
                    Debug.Log("B: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                               //TODO:: RESET PLAYER
                }
            }

            //TODO THIS EFFECTS HITTING ENEMIES AND IT WILL GO THROGH THE GROUND
            //SlopPlayerAngleAdjustment(in hitGround, in forwardPositon, in middlePositon, in backwardPositon);

#if UNITY_EDITOR
            Color rayColor = Color.green;

            if (hitGround)
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(currentCollider.bounds.center + new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
            Debug.DrawRay(currentCollider.bounds.center, Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
            Debug.DrawRay(currentCollider.bounds.center - new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
#endif

            return hitGround;
        }

        private void CheckForwardRayCast() //TODO:: DESIGN BREAKING WALLS SHOUDL IT BE BUTTON BASED OR NOT
        {
            bool HitWall;

            int hits = Physics.RaycastNonAlloc(RB.position, RB.transform.TransformDirection(Vector3.right), raycasts, RaycastDistanceForward, ForwardDetectionLayerMask);

            HitWall = hits > 0 ? true : false;

            if (raycasts[0].transform != null)
            {
                IDestroyable destroyable = raycasts[0].transform.GetComponent<IDestroyable>();

                if (destroyable != null)
                {
                    if (canSmash)
                    {
                        destroyable.Smached();
                        canSmash = false;
                    }
                    else
                    {
                        raycasts = new RaycastHit[1];
                        InstantDie();
                    }
                }
            }
            Debug.DrawRay(RB.position, new Vector3(RaycastDistanceForward, 0, 0), Color.red);
        }


        #region Animations

        public override void isGroundedAnimation() { }

        public override void MoveAnimation(in float direction) { }

        public override void isJumpAnimation(in bool isJump) { }

        public override void isHardLandAnimation(in bool isHashLand) { }

        #endregion

        #region IPlayerTAG interface
        public void PlayerInWinState()
        {
            playerScript.SwitchToMainPlayer();
            playerScript.WinStateSpecialConditionForSecondCharacter();
        }

        public void EnableReciveInputControls(bool enabled) => CanControl = enabled;

        public void RespawnPlayer(Vector3 positionToSpawn)
        {
            //TODO :: REPSANW FX
            Debug.LogError("This Should Have Not Been Called");
            imove.EnableMovement();
        }

        private IEnumerator DelayStartMoving(Action levelHasStartedCallback)
        {
            yield return new WaitForSecondsRealtime(2f);
            EnableReciveInputControls(true);

            levelHasStartedCallback?.Invoke();

            if (levelHasStartedCallback == null)
                Debug.LogWarning($"No Callback {gameObject.name}");

            imove.EnableMovement();
        }

        #endregion
        #region IPlayerSwitchable

        public void SwitchToMe()
        {
            // SET SETINGS FOR ABILITIES
            gameObject.SetActive(true);
            transform.parent = playerScript.transform;

            transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;
        }

        public void UnSwitchFromMe(Transform parentTransform)
        {
            gameObject.SetActive(false);
            transform.parent = parentTransform;
        }
        #endregion

        public void MainAbility()
        {
            if (!canSmash)
            {
                Debug.Log("ATTACK BALL");
                canSmash = true;
                ballAttackTimer = 0;
                RB.AddForce(Vector3.right * 100, ForceMode.Impulse);
            }
        }

        #region LIVING_ENTITY_OVERRIDES
        private void PlayerDeath()
        {
            Debug.Log("Died");
            gameObject.SetActive(false);
            imove.StopMovement(false);

            Instantiate(DeathFX, RB.transform.position, DeathFX.transform.rotation);
        }

        private void AddHealthFX()
        {
            Debug.Log("Sparks");
        }
        public void TakeHit()
        {

        }

        protected override void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition) => throw new System.NotImplementedException();
        public void RespawnPlayer(Vector3 positionToSpawn, bool OnLevelStart) => throw new NotImplementedException();

        #endregion
    }
}