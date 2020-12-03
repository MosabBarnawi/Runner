using System;

namespace BarnoGames.Runner2020
{
    internal static class PlayerInputControls
    {
        public static Player Player { get; set; }
        public static Action AttackAction;
        public static Action<float> MoveAction;
        public static Action SpecialAbility;

        public static void JumpInput(JumpInput isJump)
        {
            if (isJump == Runner2020.JumpInput.Empty) return;

            if (Player.CanAnimateCharacter)
            {
                Player.iJump.SetJump(isJump);
            }
        }
    }
}
