using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class Enemy : Character, ITargetable
    {
        private Material material;
        private Color _originalColor;

        #region Unity Callbacks

        protected override void Awake()
        {
            base.Awake();
            material = GetComponent<MeshRenderer>().material;
            _originalColor = material.color;
        }

        protected override void Start()
        {
            base.Start();
            OnDeath += EnemyDeath;
            OnTakeHit += TakeHit;
        }

        #endregion

        #region Private API

        private void EnemyDeath()
        {
            Destroy(gameObject);
            Debug.Log("EnemyScript Die");
        }

        private void TakeHit()
        {
            // todo effect
            material.color = Color.white;
            //ShardBehavior.SharedInstance.Repell();
            Debug.Log("Enemy Script Trigger");
        }
        #endregion

        #region Public API

        public void DetectedColor(Color color) => material.color = color;

        public void ResetColor() => material.color = _originalColor;

        public Vector3 GetPosition()
        {
            return transform.position;
        }

        protected override bool CheckIfGrounded() => throw new System.NotImplementedException();
        protected override void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition) => throw new System.NotImplementedException();
        public override void MoveAnimation(in float direction) => throw new System.NotImplementedException();
        public override void isGroundedAnimation() => throw new System.NotImplementedException();
        public override void isJumpAnimation(in bool isJump) => throw new System.NotImplementedException();
        public override void isHardLandAnimation(in bool isHardLand) => throw new System.NotImplementedException();

        #endregion
    }
}