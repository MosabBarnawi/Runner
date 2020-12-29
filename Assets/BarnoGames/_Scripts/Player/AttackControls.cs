using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class AttackControls : MonoBehaviour/*, IAttack*/
    {

        private Transform transformToGoTo;
        private Action callback;

        public void Attack()
        {
            Debug.Log("Attacked");
            FlyToPosition();
        }

        public void GoToPosition(Transform _transform, Action callback)
        {
            transformToGoTo = _transform;
            this.callback = callback;
        }

        private void FlyToPosition()
        {
            if (transformToGoTo != null)
            {
                transform.position = transformToGoTo.position;
                transformToGoTo = null;
                callback();
            }
        }
    }
}
