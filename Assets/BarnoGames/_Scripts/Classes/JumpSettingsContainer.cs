namespace BarnoGames.Runner2020
{
    [System.Serializable] // TODO:: REMOVE THIS
    struct JumpSettingsContainer
    {
        public float Gravity;

        public int MaxNumberOfJumps;
        public float GravityJumpScale;
        public float DefaultGravityScale;
        public float MaxJumpTimePerJump;
        public float JumpVelocity;

        public bool JumpHangEnabled;
        public bool IsFloatUp;
        public bool IsDecreasePerJump;
        public float HangTime;
        public float HangGravity;
        public float HangJumpVelocity;
        public float HangJumpMaxHeight;
        public float JumpDecreaseAmount;
        public float HangForwardSpeed;
    }
}
