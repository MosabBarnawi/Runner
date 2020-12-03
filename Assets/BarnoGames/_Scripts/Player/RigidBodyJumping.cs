using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class RigidBodyJumping : MonoBehaviour, IJump, IBoost
    {
        //private AnimationCurve animationCurve;

        [SerializeField] private float forceDownSpeed = 50f;
        [SerializeField] private float slopeJumpVelocityMultiplier = 3;

        #region Cahching
        [Header("Caching")]
        private GlobalJumpSettings globalJumpSettings;
        private GlobalPlayerMovementSettings playerMovementSettings;

        private JumpSettingsContainer jumpSettingsContainer = new JumpSettingsContainer();
        private JumpChecker jumpChecker;
        private RollBoostChecker rollBoostChecker;
        private ShardBehavior shardBehavior;

        private Character character;
        private bool isGrounded => character.IsGrounded;
        private bool isJumpping;
        private IMove imove;
        #endregion

        private float _slopMultiPlier;

        #region Unity Callbacks
        private void Awake()
        {
            character = GetComponent<Character>();
            imove = character.GetComponent<IMove>();
        }

        void Start()
        {
            globalJumpSettings = GameManager.SharedInstance.GlobalsJumpSettings;
            playerMovementSettings = GameManager.SharedInstance.GlobalPlayerMovementSettings;

            //ShardBehavior.SharedInstance.Action_ShardBoostPlayer += PushPlayerForwardBoost;
            shardBehavior = GetComponentInChildren<ShardBehavior>();
            shardBehavior.Action_ShardBoostPlayer += PushPlayerForwardBoost;


            JumpSettingsInit();
        }

        void FixedUpdate()
        {
            Jump();
            JumpWithHang();
            AddGravityToVelocity();
        }

        #endregion

        #region Private API

        #region JUMPING

        private void JumpSettingsInit()
        {
            if (character.IAmPlayer && playerMovementSettings.OverrideGlobalJumpSettings)
            {
                jumpSettingsContainer.Gravity = playerMovementSettings.DefaultGravityScale;
                jumpSettingsContainer.GravityJumpScale = playerMovementSettings.JumpGravityScale;
                jumpSettingsContainer.DefaultGravityScale = playerMovementSettings.DefaultGravityScale;

                jumpSettingsContainer.MaxNumberOfJumps = playerMovementSettings.MaxNumberOfJumps;
                jumpSettingsContainer.MaxJumpTimePerJump = playerMovementSettings.MaxJumpTimePerJump;
                jumpSettingsContainer.JumpVelocity = playerMovementSettings.JumpVelocity;

                jumpSettingsContainer.JumpHangEnabled = playerMovementSettings.JumpHangEnabled;
                jumpSettingsContainer.IsFloatUp = playerMovementSettings.IsFloatUp;
                jumpSettingsContainer.IsDecreasePerJump = playerMovementSettings.IsDecreasePerJump;
                jumpSettingsContainer.HangTime = playerMovementSettings.HangTime;
                jumpSettingsContainer.HangGravity = playerMovementSettings.HangGravity;
                jumpSettingsContainer.HangJumpVelocity = playerMovementSettings.HangJumpVelocity;
                jumpSettingsContainer.HangJumpMaxHeight = playerMovementSettings.HangJumpMaxHeight;
                jumpSettingsContainer.JumpDecreaseAmount = playerMovementSettings.JumpDecreaseAmount;
                jumpSettingsContainer.HangForwardSpeed = playerMovementSettings.HangForwardSpeed;
            }
            else
            {
                jumpSettingsContainer.Gravity = globalJumpSettings.DefaultGravityScale;
                jumpSettingsContainer.GravityJumpScale = globalJumpSettings.JumpGravityScale;
                jumpSettingsContainer.DefaultGravityScale = globalJumpSettings.DefaultGravityScale;

                jumpSettingsContainer.MaxNumberOfJumps = globalJumpSettings.MaxNumberOfJumps;
                jumpSettingsContainer.MaxJumpTimePerJump = globalJumpSettings.MaxJumpTimePerJump;
                jumpSettingsContainer.JumpVelocity = globalJumpSettings.JumpVelocity;

                jumpSettingsContainer.JumpHangEnabled = globalJumpSettings.GlobalJumpHang;
                jumpSettingsContainer.IsFloatUp = globalJumpSettings.IsFloatUp;
                jumpSettingsContainer.IsDecreasePerJump = globalJumpSettings.IsDecreasePerJump;
                jumpSettingsContainer.HangTime = globalJumpSettings.HangTime;
                jumpSettingsContainer.HangGravity = globalJumpSettings.HangGravity;
                jumpSettingsContainer.HangJumpVelocity = globalJumpSettings.HangJumpVelocity;
                jumpSettingsContainer.HangJumpMaxHeight = globalJumpSettings.HangJumpMaxHeight;
                jumpSettingsContainer.JumpDecreaseAmount = globalJumpSettings.JumpDecreaseAmount;
                jumpSettingsContainer.HangForwardSpeed = globalJumpSettings.HangForwardSpeed;
            }

            JumpRefreshForMultipleJumps();
        }

        private void Jump()
        {
            if (jumpSettingsContainer.JumpHangEnabled || !character.CanAnimateCharacter) return;

            if (isGrounded)
            {
                if (!isJumpping)
                {
                    jumpChecker.JumpCounter = 0;
                    jumpChecker.FallingTimerDelay = 0;
                    jumpChecker.SkipJumpCheck = true;
                }

                character.isJumpAnimation(false);
                character.isHardLandAnimation(false);
            }

            if (jumpChecker.JumpCounter < jumpSettingsContainer.MaxNumberOfJumps)
            {
                character.isHardLandAnimation(true);

                if (isJumpping && jumpChecker.FallingTimerDelay <= jumpSettingsContainer.MaxJumpTimePerJump)
                {
                    JumpRefreshForMultipleJumps();

                    jumpSettingsContainer.Gravity = jumpSettingsContainer.GravityJumpScale;

                    // TODO:: MAYBE ADD A CLAMP LIKE HANG TIME MAX HEIGHT

                    character.rb.velocity = new Vector3(character.rb.velocity.x, jumpSettingsContainer.JumpVelocity * _slopMultiPlier, character.rb.velocity.z);

                    jumpChecker.FallingTimerDelay += Time.deltaTime;
                }
                else jumpSettingsContainer.Gravity = jumpSettingsContainer.DefaultGravityScale;

                if (!isJumpping)
                {
                    character.isJumpAnimation(false);

                    if (!isGrounded && !jumpChecker.SkipJumpCheck)
                    {
                        jumpChecker.JumpCounter++;
                        jumpChecker.FallingTimerDelay = 0;
                        jumpChecker.SkipJumpCheck = true;
                    }
                }
            }

            SpeedUpMidRollLogic();
        }

        private void JumpRefreshForMultipleJumps()
        {
            _slopMultiPlier = character.IsOnSlope ? slopeJumpVelocityMultiplier : 1f;

            rollBoostChecker.SpeedUpTimer = 0;
            rollBoostChecker.isBoosted = false;
            rollBoostChecker.StartTimer = false;
            SetAllowBoost(false);

            jumpChecker.SkipJumpCheck = false;

            character.isSpeedBoostAnimation(false);
            character.isHardLandAnimation(false);
            character.isJumpAnimation(true);
        }

        private void JumpWithHang()
        {
            if (!jumpSettingsContainer.JumpHangEnabled || !character.CanAnimateCharacter) return;

            if (isGrounded)
            {
                jumpChecker.isHangSpeed = false;

                if (!isJumpping)
                {
                    jumpChecker.FallingTimerDelay = 0;
                    jumpChecker.JumpCounter = 0;
                    jumpChecker.SkipJumpCheck = true;
                    jumpChecker.CalculateMaxHeight = true;
                }

                character.isHardLandAnimation(false);
            }

            SpeedUpMidRollLogic();

            if (jumpChecker.JumpCounter < jumpSettingsContainer.MaxNumberOfJumps)
            {
                character.isHardLandAnimation(true);

                if (isJumpping && jumpChecker.FallingTimerDelay <= jumpSettingsContainer.HangTime)
                {
                    JumpRefreshForMultipleJumps();

                    jumpSettingsContainer.Gravity = jumpSettingsContainer.HangGravity;

                    if (jumpChecker.CalculateMaxHeight)
                    {
                        if (jumpSettingsContainer.IsDecreasePerJump)
                        {
                            if (jumpChecker.JumpCounter == 0) jumpChecker.MaxHeight = transform.position.y + jumpSettingsContainer.HangJumpMaxHeight;
                            else
                            {
                                float amount = jumpSettingsContainer.HangJumpMaxHeight - jumpSettingsContainer.JumpDecreaseAmount;

                                if (amount < 0) amount = 0;

                                jumpChecker.MaxHeight = transform.position.y + amount;
                            }
                        }
                        else jumpChecker.MaxHeight = transform.position.y + jumpSettingsContainer.HangJumpMaxHeight;

                        jumpChecker.CalculateMaxHeight = false;
                    }

                    if (transform.position.y < jumpChecker.MaxHeight && !jumpChecker.isHangSpeed)
                    {
                        if (jumpSettingsContainer.IsDecreasePerJump)
                        {
                            if (jumpChecker.JumpCounter == 0)
                            {
                                //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity * _slopMultiPlier;
                                character.rb.velocity = new Vector3(character.rb.velocity.x, jumpSettingsContainer.HangJumpVelocity * _slopMultiPlier, character.rb.velocity.z);
                            }
                            else
                            {
                                //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity / 2 * _slopMultiPlier;
                                character.rb.velocity = new Vector3(character.rb.velocity.x, jumpSettingsContainer.HangJumpVelocity / 2 * _slopMultiPlier, character.rb.velocity.z);
                            }
                        }
                        else
                        {
                            //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity * _slopMultiPlier;
                            character.rb.velocity = new Vector3(character.rb.velocity.x, jumpSettingsContainer.HangJumpVelocity * _slopMultiPlier, character.rb.velocity.z);
                        }
                    }
                    else if (transform.position.y >= jumpChecker.MaxHeight)
                    {
                        jumpChecker.isHangSpeed = true;
                        character.rb.velocity = new Vector3(character.rb.velocity.x, 0f, character.rb.velocity.z);
                        //transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
                    }

                    jumpChecker.FallingTimerDelay += Time.deltaTime;
                }
                else
                {
                    if (!jumpSettingsContainer.IsFloatUp)
                        jumpChecker.isHangSpeed = false;

                    jumpSettingsContainer.Gravity = jumpSettingsContainer.DefaultGravityScale;
                }

                if (!isJumpping)
                {
                    character.isJumpAnimation(false);

                    if (!isGrounded && !jumpChecker.SkipJumpCheck)
                    {
                        jumpChecker.CalculateMaxHeight = true;

                        if (!jumpSettingsContainer.IsFloatUp)
                            jumpChecker.isHangSpeed = false;

                        jumpChecker.JumpCounter++;
                        jumpChecker.FallingTimerDelay = 0;
                        jumpChecker.SkipJumpCheck = true;
                    }
                }
            }
        }

        #endregion

        private bool GetAllowBoost() => rollBoostChecker.CanBoost;
        public void SetAllowBoost(bool value) => rollBoostChecker.CanBoost = value;

        public void AddShardBoost(float CliffBounceAmount)
        {
            imove.StopMovement(false);

            Vector3 Boost = new Vector3(0, CliffBounceAmount, 0);
            character.rb.AddForce(Boost, ForceMode.VelocityChange);

            // TODO:: DIFFERENT ANIMATION 
            //ShardBehavior.SharedInstance.RecallShard(true);
            shardBehavior.RecallShard(true);
        }

        [Obsolete("Implemet This In the Future or Remove it", true)]
        private void JumpAbility() // TODO:: NOT USED ATM
        {
            if (!character.CanAnimateCharacter) return;

            if (!character.IsGrounded && !character.IsGroundSmash)
            {
                imove.StopMovement(false);

                character.IsGroundSmash = true;
                character.rb.velocity = new Vector3(0, -forceDownSpeed, 0);

                //rb.AddForce(Vector3.down * forceDownSpeed, ForceMode.VelocityChange);
            }
        }

        private void PushPlayerForwardBoost()
        {
            imove.EnableMovement();

            Vector3 Boost = new Vector3(character.rb.velocity.x + shardBehavior.ShardBoostSpeed, 0, 0);
            character.rb.AddForce(Boost, ForceMode.VelocityChange);
        }

        #region Boosting After Land
        private void SpeedUpMidRollLogic()
        {
            if (!rollBoostChecker.isBoosted)
            {
                if (GetAllowBoost())
                {
                    if (!isJumpping)
                    {
                        rollBoostChecker.StartTimer = true;
                        rollBoostChecker.isBoosted = true;
                        rollBoostChecker.isBoosted = true;
                    }
                }
            }

            if (rollBoostChecker.StartTimer) AfterRollBoostTimer();
        }

        private void AfterRollBoostTimer()
        {
            if (rollBoostChecker.StartTimer)
            {
                rollBoostChecker.SpeedUpTimer += Time.deltaTime;

                if (rollBoostChecker.SpeedUpTimer <= globalJumpSettings.TimeToBoost)
                {
                    character.isSpeedBoostAnimation(true);
                    //TODO: IMPLEMENT BOOST FUNCITANLITY
                }
                else
                {
                    character.isSpeedBoostAnimation(false);
                    rollBoostChecker.StartTimer = false;
                }
            }
        }
        #endregion

        private void AddGravityToVelocity() => character.rb.velocity = new Vector3(character.rb.velocity.x, character.rb.velocity.y - jumpSettingsContainer.Gravity, 0);

        #endregion

        #region IJump Interface
        public void SetJump(JumpInput jumpingState) => isJumpping = jumpingState == JumpInput.Jump;

        public JumpChecker GetHangTime() => jumpChecker;
        #endregion
    }
}