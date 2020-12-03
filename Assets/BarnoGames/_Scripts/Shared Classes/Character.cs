using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public abstract class Character : LivingEntity, IAnimations, ICharacter
    {
        internal IJump iJump { get; private set; }
        public Rigidbody rb { get; protected set; }

        [SerializeField] private Animator AnimatorController;

        public Animator Anim => AnimatorController;

        #region Player Specific 
        public bool IAmPlayer { get; protected set; } = false;
        protected bool CanControl { get; set; } = true;
        #endregion

        public bool CanAnimateCharacter => !IsDead && CanControl;

        protected override void Awake()
        {
            base.Awake();
            rb = GetComponent<Rigidbody>();
        }

        protected override void Start()
        {
            base.Start();

            iJump = GetComponent<IJump>();

            string missingComponents = string.Empty;

            if (rb == null) missingComponents += $"{rb.GetType().Name} \n ";
            if (iJump == null) missingComponents += $"IJump \n";

            if (string.IsNullOrEmpty(missingComponents))
                return;

            Debug.Log($"** {gameObject.name} Components Missing :=> \n {missingComponents}**************** ");
        }

        public bool IsGroundSmash { get; set; }

        protected abstract bool CheckIfGrounded();
        protected abstract void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition);

        #region ICharacter Inteface
        public bool IsGrounded { get; protected set; }
        public bool IsOnSlope { get; protected set; }
        #endregion

        #region IAnimations Interface => NEEDS TO BE OVERRIDEN
        public abstract void MoveAnimation(in float direction);
        public abstract void isGroundedAnimation();
        public abstract void isJumpAnimation(in bool isJump);
        public virtual void isHardLandAnimation(in bool isHardLand) { }
        public virtual void isSpeedBoostAnimation(in bool isSpeedUp) { }
        #endregion
    }
}