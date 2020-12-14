using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class RigidBodyJumping : MonoBehaviour, IJump
    {
        //private AnimationCurve animationCurve;

        [SerializeField] private float slopeJumpVelocityMultiplier = 3;

        #region Cahching
        [SerializeField] private MovementSettings movementSettings;
        private float gravity;

        private JumpChecker jumpChecker;
        private RollBoostChecker rollBoostChecker;

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

        void Start() => JumpRefreshForMultipleJumps();

        void FixedUpdate()
        {
            Jump();
            JumpWithHang();
            AddGravityToVelocity();
        }

        #endregion

        #region Private API

        #region JUMPING

        private void Jump()
        {
            if (movementSettings.JumpHangEnabled || !character.CanAnimateCharacter) return;

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

            if (jumpChecker.JumpCounter < movementSettings.MaxNumberOfJumps)
            {
                character.isHardLandAnimation(true);

                if (isJumpping && jumpChecker.FallingTimerDelay <= movementSettings.MaxJumpTimePerJump)
                {
                    JumpRefreshForMultipleJumps();

                    gravity = movementSettings.GravityJumpScale;

                    // TODO:: MAYBE ADD A CLAMP LIKE HANG TIME MAX HEIGHT

                    character.RB.velocity = new Vector3(character.RB.velocity.x, movementSettings.JumpVelocity * _slopMultiPlier, character.RB.velocity.z);

                    jumpChecker.FallingTimerDelay += Time.deltaTime;
                }
                else gravity = movementSettings.DefaultGravityScale;

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

        private void JumpWithHang() // TODO:: WHEN LANFING INFINIATE LOOPING WITH ROLLING
        {
            if (!movementSettings.JumpHangEnabled || !character.CanAnimateCharacter) return;

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

            if (jumpChecker.JumpCounter < movementSettings.MaxNumberOfJumps)
            {
                character.isHardLandAnimation(true);

                if (isJumpping && jumpChecker.FallingTimerDelay <= movementSettings.HangTime)
                {
                    JumpRefreshForMultipleJumps();

                    gravity = movementSettings.HangGravity;

                    if (jumpChecker.CalculateMaxHeight)
                    {
                        if (movementSettings.IsDecreasePerJump)
                        {
                            if (jumpChecker.JumpCounter == 0) jumpChecker.MaxHeight = character.RB.position.y/*transform.position.y */+ movementSettings.HangJumpMaxHeight;
                            else
                            {
                                float amount = movementSettings.HangJumpMaxHeight - movementSettings.JumpDecreaseAmount;

                                if (amount < 0) amount = 0;

                                //jumpChecker.MaxHeight = transform.position.y + amount;
                                jumpChecker.MaxHeight = character.RB.position.y + amount;
                            }
                        }
                        else jumpChecker.MaxHeight = /*transform.position.y */character.RB.position.y + movementSettings.HangJumpMaxHeight;

                        jumpChecker.CalculateMaxHeight = false;
                    }

                    if (/*transform.position.y*/character.RB.position.y < jumpChecker.MaxHeight && !jumpChecker.isHangSpeed)
                    {
                        if (movementSettings.IsDecreasePerJump)
                        {
                            if (jumpChecker.JumpCounter == 0)
                            {
                                //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity * _slopMultiPlier;
                                character.RB.velocity = new Vector3(character.RB.velocity.x, movementSettings.HangJumpVelocity * _slopMultiPlier, character.RB.velocity.z);
                            }
                            else
                            {
                                //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity / 2 * _slopMultiPlier;
                                character.RB.velocity = new Vector3(character.RB.velocity.x, movementSettings.HangJumpVelocity / 2 * _slopMultiPlier, character.RB.velocity.z);
                            }
                        }
                        else
                        {
                            //rb.velocity = Vector3.up * globalJumpSettings.HangJumpVelocity * _slopMultiPlier;
                            character.RB.velocity = new Vector3(character.RB.velocity.x, movementSettings.HangJumpVelocity * _slopMultiPlier, character.RB.velocity.z);
                        }
                    }
                    else if (/*transform.position.y*/character.RB.position.y >= jumpChecker.MaxHeight)
                    {
                        jumpChecker.isHangSpeed = true;
                        character.RB.velocity = new Vector3(character.RB.velocity.x, 0f, character.RB.velocity.z);
                        //character.rb.velocity = new Vector3(character.rb.velocity.x, jumpChecker.MaxHeight, character.rb.velocity.z);
                        //transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
                    }

                    jumpChecker.FallingTimerDelay += Time.deltaTime;
                }
                else
                {
                    if (!movementSettings.IsFloatUp)
                        jumpChecker.isHangSpeed = false;

                    gravity = movementSettings.DefaultGravityScale;
                }

                if (!isJumpping)
                {
                    character.isJumpAnimation(false);

                    if (!isGrounded && !jumpChecker.SkipJumpCheck)
                    {
                        jumpChecker.CalculateMaxHeight = true;

                        //if (!jumpSettingsContainer.IsFloatUp) //TO MAKE SPEED BACK UP FASTER WITHOUT THIS CHECK
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

                if (rollBoostChecker.SpeedUpTimer <= movementSettings.TimeToBoost)
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

        private void AddGravityToVelocity() => character.RB.velocity = new Vector3(character.RB.velocity.x, character.RB.velocity.y - gravity, 0);

        #endregion

        #region IJump Interface
        public void SetJump(JumpInput jumpingState) => isJumpping = jumpingState == JumpInput.Jump;

        public JumpChecker GetHangTime() => jumpChecker;
        #endregion
    }
}