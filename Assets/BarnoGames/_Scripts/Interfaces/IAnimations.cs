
namespace BarnoGames.Runner2020
{
    internal interface IAnimations // TODO:: REMOVE THIS ONLY REFRENCED ON CHARACTER CLASS
    {
        void MoveAnimation(in float direction);
        void isGroundedAnimation();
        void isJumpAnimation(in bool isJump);
        void isHardLandAnimation(in bool isHashLand);
    }
}
