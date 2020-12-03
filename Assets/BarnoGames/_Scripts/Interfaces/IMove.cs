
namespace BarnoGames.Runner2020
{
    internal interface IMove
    {
        void SetVelocity(float VelocityVector);
        void StopMovement(bool freezeInSpace);
        void EnableMovement();
    }
}
