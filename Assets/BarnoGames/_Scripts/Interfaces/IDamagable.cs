using UnityEngine;

namespace BarnoGames.Runner2020
{
    internal interface IDamagable
    {
        void TakeHit(float damage, RaycastHit hit);
        void TakeDamage(float damage);
    }
}

