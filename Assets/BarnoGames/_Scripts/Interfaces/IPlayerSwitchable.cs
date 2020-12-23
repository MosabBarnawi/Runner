using UnityEngine;

namespace BarnoGames.Runner2020
{
    interface IPlayerSwitchable
    {
        void SwitchToMe();
        void UnSwitchFromMe(Transform parentTransform);
    }
}
