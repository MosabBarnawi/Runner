namespace BarnoGames.Runner2020
{
    internal interface ICharacter
    {
        bool IsGrounded { get; }
        bool IsOnSlope { get; }
    }
}