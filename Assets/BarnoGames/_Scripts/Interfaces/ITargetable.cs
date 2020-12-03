using UnityEngine;


namespace BarnoGames.Runner2020
{
    public interface ITargetable
    {
        void DetectedColor(Color color);
        void ResetColor();
        Vector3 GetPosition();
    }
}

