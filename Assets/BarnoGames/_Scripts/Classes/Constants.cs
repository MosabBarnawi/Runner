using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class CONTROLS
    {
        /// CONTROLS ///
        internal const string INPUT_HORIONTAL = "Horizontal";
        internal const string INPUT_VERTICAL = "Vertical";
        internal const string INPUT_JUMP = "Jump";
        internal const string INPUT_ATTACK = "Attack";
    }

    public class TAGS
    {
        /// TAGS ///
        internal const string TAG_BREAKABLE = "Breakable";
    }

    public class TIME_CONSTANTS
    {
        // TIME
        internal const int PAUSE_TIME = 0;
        internal const int NORMAL_TIME = 1;
    }

    public class ANIMATIONS_CONSTANTS
    {
        /// ANIMATION ///
        private const string ANIM_MOVEMENT_SPEED = "MovementSpeed";
        private const string ANIM_IS_GROUNDED = "isGrounded";
        private const string ANIM_JUMP = "Jump";
        private const string ANIM_HARD_LAND = "HardLand";
        private const string ANIM_SPEED_UP = "SpeedUp";        
        private const string ANIM_LEVEL_END = "EndLevel";
        private const string ANIM_PLAYER_FALLING = "isFalling";


        internal readonly static int MOVEMENT_SPEED_HASH = Animator.StringToHash(ANIM_MOVEMENT_SPEED);
        internal readonly static int HARD_LAND_HASH = Animator.StringToHash(ANIM_HARD_LAND);
        internal readonly static int SPEED_UP_HASH = Animator.StringToHash(ANIM_SPEED_UP);
        internal readonly static int IS_JUMP_HASH = Animator.StringToHash(ANIM_JUMP);
        internal readonly static int IS_GROUNDED_HASH = Animator.StringToHash(ANIM_IS_GROUNDED);
        internal readonly static int IS_LEVEL_END = Animator.StringToHash(ANIM_LEVEL_END);
        internal readonly static int IS_PLAYER_FALLING = Animator.StringToHash(ANIM_PLAYER_FALLING);
    }
}
