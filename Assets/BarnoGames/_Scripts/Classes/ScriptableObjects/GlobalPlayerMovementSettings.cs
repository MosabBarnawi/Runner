using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [CreateAssetMenu(fileName = "New Player Settings", menuName = "Barno Utils/Global Settings/Player Movement Speed")]
    public class GlobalPlayerMovementSettings : ScriptableObject
    {
        [SerializeField] private float movementSpeed = 10f;

        [SerializeField] private bool overrideGlobalJumpSettings = false;


        [Header("Jump Settings")]
        [SerializeField] private float defaultGravityScale = 0.8f;
        [SerializeField] private float jumpGravityScale = 0.4f;
        [SerializeField] private int maxNumberOfJumps = 2;
        [SerializeField] private float maxJumpTimePerJump = 0.4f;
        [SerializeField] private float jumpVelocity = 15f;
        [SerializeField] private float timeToBoost = 1f;

        [Header("Jump With Hang")]
        [SerializeField] private bool jumpHangEnabled = false;
        [SerializeField] private bool isFloatUp = false;
        [SerializeField] private bool isDecreasePerJump = false;
        [SerializeField] private float hangTime = 1.2f;
        [SerializeField] private float hangGravity = -2f;
        [SerializeField] private float hangJumpVelocity = 15f;
        [SerializeField] private float hangJumpMaxHeight = 5f;
        [SerializeField] private float jumpDecreaseAmount = 0.5f;
        [SerializeField] private float hangForwardSpeed = 5f;


        #region Properties
        public float MovementSpeed { get => movementSpeed; }
        public bool OverrideGlobalJumpSettings { get => overrideGlobalJumpSettings; }
        public int MaxNumberOfJumps { get => maxNumberOfJumps; }
        public float MaxJumpTimePerJump { get => maxJumpTimePerJump; }
        public float JumpVelocity { get => jumpVelocity; }
        public float TimeToBoost { get => timeToBoost; }
        public float DefaultGravityScale { get => defaultGravityScale; }
        public float JumpGravityScale { get => jumpGravityScale; }
        public bool IsFloatUp { get => isFloatUp; }
        public bool IsDecreasePerJump { get => isDecreasePerJump; }
        public float HangTime { get => hangTime; }
        public float HangGravity { get => hangGravity; }
        public float HangJumpVelocity { get => hangJumpVelocity; }
        public float HangJumpMaxHeight { get => hangJumpMaxHeight; }
        public float JumpDecreaseAmount { get => jumpDecreaseAmount; }
        public float HangForwardSpeed { get => hangForwardSpeed; }
        public bool JumpHangEnabled { get => jumpHangEnabled; }
        #endregion
    }
}