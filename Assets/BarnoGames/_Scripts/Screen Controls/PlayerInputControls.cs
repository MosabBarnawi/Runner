using System;

namespace BarnoGames.Runner2020
{
    internal static class PlayerInputControls
    {
        public static Player PlayerScript { get; set; }
        public static Character Player { get; set; }
        public static Action MainAbilityAction;
        //public static Action<float> MoveAction;
        public static Action SpecialAbility;

        public static void JumpInput(JumpInput isJump)
        {
            if (isJump == Runner2020.JumpInput.Empty) return;

            if (Player.CanControlCharacter)
            {
                Player.iJump.SetJump(isJump);
            }
        }
    }
}
