using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020.BoostTimerStart
{
    public class BoostTimerStart : MonoBehaviour
    {
        private RigidBodyJumping parentJumpComponent;

        private void Awake()
        {
            parentJumpComponent = GetComponentInParent<RigidBodyJumping>();
        }
        public void StartTimer()
        {
            parentJumpComponent.SetAllowBoost(true);
        }

        public void StopTimer()
        {
            parentJumpComponent.SetAllowBoost(false);
        }
    }
}