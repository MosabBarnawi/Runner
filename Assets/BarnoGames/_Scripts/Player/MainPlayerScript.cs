using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public class MainPlayerScript : Character, IPlayerTAG, IPlayerSwitchable
    {
        [Header("Ray Casting")]
        [SerializeField] private LayerMask WalkableLayerMask; //WALKABLE / ENEMY
        [SerializeField] private LayerMask ForwardDetectionLayerMask; //REMOVE INGORE RAYCAST
        [SerializeField] private float GroundRaycastDistance = 1.1f;

        [Space(5)]
        [SerializeField] private float RaycastDistanceForward = 0.8f;

        [SerializeField] private PlayerType playerType = PlayerType.MainPlayer;

        [SerializeField] private GameObject DeathFX;

        private Collider currentCollider;
        [SerializeField] private Player playerScript;

        [SerializeField] private ShardBehavior ShardBehavior;
        RaycastHit[] raycasts = new RaycastHit[1];

        private bool levelHasLoaded;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();

            if (Anim == null) Debug.LogError($"Animator Not Assigned {gameObject.name}");
            if (playerScript == null) Debug.LogError($"Player Script Not Assigned {gameObject.name}");

            currentCollider = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            InitilizeControls();

            if(GameManager.SharedInstance !=null)  //TODO:: IMPROVE THIS RACE CONDITION
                                                   //GameManager.SharedInstance.OnWinStateAction = PlayerInWinState;
                GameManager.SharedInstance.RegisterGameState(PlayerInWinState, GameState.WinState);

        }

        protected override void Start()
        {
            base.Start();

            OnDeath += PlayerDeath;
            OnAddHealth += AddHealthFX;
            OnTakeHit += TakeHit;
            OnRespawn += RespawnPlayer;
            //GameManager.SharedInstance.OnLevelFinishedLoading = LevelHasLoadedCallback;
            GameManager.SharedInstance.RegisterGameState(LevelHasLoadedCallback, GameState.LevelFinishedLoading);
            //GameManager.SharedInstance.OnWinStateAction = PlayerInWinState;
            GameManager.SharedInstance.RegisterGameState(PlayerInWinState, GameState.WinState);
        }

        void Update()
        {
            IsGrounded = CheckIfGrounded();
            isGroundedAnimation();

            //if (Input.GetKeyDown(KeyCode.L))
            //{
            //    LevelHasLoadedCallback();
            //}

            if (levelHasLoaded)
            {
                if (IsGrounded)
                {
                    Anim.SetBool(ANIMATIONS_CONSTANTS.IS_PLAYER_FALLING, false);
                    levelHasLoaded = false;
                }
            }
        }

        private void FixedUpdate()
        {
            CheckForwardRayCast();
        }

        private void OnDestroy()
        {
            //GameManager.SharedInstance.OnLevelFinishedLoading -= LevelHasLoadedCallback;
            GameManager.SharedInstance.UnRegisterGameState(LevelHasLoadedCallback, GameState.LevelFinishedLoading);
            //GameManager.SharedInstance.OnWinStateAction -= PlayerInWinState;
            GameManager.SharedInstance.UnRegisterGameState(PlayerInWinState, GameState.WinState);
        }

        #endregion

        private void InitilizeControls()
        {
            //if (PlayerInputControls.Player == null)
            PlayerInputControls.Player = this;
            //else
            //{
            //    if (PlayerInputControls.Player != this)
            //        Destroy(gameObject);
            //}
        }

        #region Animations

        public override void isGroundedAnimation() => Anim.SetBool(ANIMATIONS_CONSTANTS.IS_GROUNDED_HASH, IsGrounded);

        public override void MoveAnimation(in float direction) => Anim.SetFloat(ANIMATIONS_CONSTANTS.MOVEMENT_SPEED_HASH, direction);

        public override void isJumpAnimation(in bool isJump) => Anim.SetBool(ANIMATIONS_CONSTANTS.IS_JUMP_HASH, isJump);

        public override void isHardLandAnimation(in bool isHashLand) => Anim.SetBool(ANIMATIONS_CONSTANTS.HARD_LAND_HASH, isHashLand);

        public override void isSpeedBoostAnimation(in bool isSpeedUp) => Anim.SetBool(ANIMATIONS_CONSTANTS.SPEED_UP_HASH, isSpeedUp);

        #endregion

        #region IPlayerSwitchable
        public void SwitchToMe()
        {
            // SET SETINGS FOR ABILITIES
            gameObject.SetActive(true);
            transform.parent = playerScript.transform;

            transform.localScale = Vector3.one;
            transform.localEulerAngles = Vector3.zero;

            ShardBehavior.HardResetShard();
        }
        public void UnSwitchFromMe(Transform parentTransform)
        {
            gameObject.SetActive(false);
            transform.parent = parentTransform;
        }
        #endregion

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

            if (playerType == PlayerType.MainPlayer)
            {
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

        private void CheckForwardRayCast()
        {
            bool HitWall;

            int hits = Physics.RaycastNonAlloc(RB.position, RB.transform.TransformDirection(Vector3.right), raycasts, RaycastDistanceForward, ForwardDetectionLayerMask);

            HitWall = hits > 0 ? true : false;

            if (raycasts[0].transform != null)
            {
                Debug.LogWarning(raycasts[0].transform.name);
                raycasts = new RaycastHit[1];
                InstantDie();
            }

            Debug.DrawRay(RB.position, new Vector3(RaycastDistanceForward, 0, 0), Color.red);
        }

        #region IPlayerTAG interface

        public void PlayerInWinState()
        {
            //TODO:: HANDLE IF SHARD HAS BEEN THROWN TO BRING IT BACKBEFORE STARTING NEXT LEVEL
            Anim.SetBool(ANIMATIONS_CONSTANTS.IS_LEVEL_END, true);
            imove.StopMovement(true);
        }

        public void EnableReciveInputControls(bool enabled) => CanControl = enabled;

        public void RespawnPlayer(Vector3 positionToSpawn)
        {
            RB.transform.position = positionToSpawn;
            //TODO :: REPSANW FX
            Debug.Log("Respawned Player");

            // TODO:: FALLING MATE

            //IF TOUCED GROUND =>
            //if (IsGrounded)
            //{
                imove.EnableMovement();

                // DO FLAT LANFING SETTINGS THEN MOVE
            //}
        }

        public void PlayerIsFalling(Vector3 positionToSpawn, Action levelHasStartedCallback)
        {
            imove.StopMovement(true);

            Respawn(positionToSpawn);
            EnableReciveInputControls(false);

            Debug.Log("FALLING MATE");

            Anim.SetBool(ANIMATIONS_CONSTANTS.IS_PLAYER_FALLING, true);
            Anim.SetBool(ANIMATIONS_CONSTANTS.IS_LEVEL_END, false);

            //if (IsGrounded)
            //{
                // DO FLAT LANFING SETTINGS THEN MOVE
                //StartCoroutine(DelayStartMoving(levelHasStartedCallback));
            //}
        }

        private IEnumerator DelayStartMoving(Action levelHasStartedCallback)
        {
            yield return new WaitForSecondsRealtime(2f);
            EnableReciveInputControls(true);

            Debug.Log("Start Level");

            levelHasStartedCallback?.Invoke();

            if (levelHasStartedCallback == null)
                Debug.LogWarning($"No Callback {gameObject.name}");

            imove.EnableMovement();
        }
        #endregion

        private IEnumerator Test()
        {
            yield return new WaitForSecondsRealtime(1f);
            EnableReciveInputControls(true);

            Debug.Log("Start Level");

            //imove.EnableMovement();
            imove.StopMovement(false);
            levelHasLoaded = true;
        }

        private void LevelHasLoadedCallback()
        {
            StartCoroutine(Test());
        }

        public void TeleportPlayer(Vector3 endPosition)
        {
            RB.position = endPosition;
            Instantiate(GameManager.SharedInstance.TeleportFX, endPosition, GameManager.SharedInstance.TeleportFX.transform.rotation);

            float xBoost = RB.velocity.x + ShardBehavior.ShardBoostForwardSpeed;
            float yBoost = RB.velocity.y + ShardBehavior.ShardBoostUpSpeed;

            Vector3 Boost = new Vector3(xBoost, yBoost, 0);

            RB.AddForce(Boost, ForceMode.VelocityChange);

            ShardBehavior.RecallShard(false);
        }

        #region LIVING_ENTITY_OVERRIDES
        private void PlayerDeath()
        {
            Debug.Log("Died");
            gameObject.SetActive(false);
            imove.StopMovement(false);
            ShardBehavior.HardResetShard();

            Instantiate(DeathFX, RB.transform.position, DeathFX.transform.rotation);

            //PlayerHasDiedAction?.Invoke();
        }

        private void AddHealthFX()
        {
            Debug.Log("Sparks");
        }
        public void TakeHit()
        {

        }

        protected override void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition) => throw new System.NotImplementedException();

        #endregion
    }
}