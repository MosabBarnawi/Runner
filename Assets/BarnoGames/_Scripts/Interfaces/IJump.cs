namespace BarnoGames.Runner2020
{
    internal interface IJump 
    {
        void SetJump(JumpInput jumpped);
        JumpChecker GetHangTime();
    }
}