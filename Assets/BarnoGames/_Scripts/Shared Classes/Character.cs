using System;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    [RequireComponent(typeof(RigidBodyMovement))]
    [RequireComponent(typeof(RigidBodyJumping))]
    public abstract class Character : LivingEntity, IAnimations, ICharacter
    {
        internal IJump iJump { get; private set; }
        internal IMove imove { get; private set; }
        public Rigidbody RB { get; protected set; }
        //public bool CanSwitchPlayers { get; protected set; }

        [SerializeField] private Animator _animatorController;

        public Animator Anim => _animatorController;

        #region Player Specific 
        //public bool IAmPlayer { get; protected set; } = false;

        protected bool CanControl { get; set; } = true;
        #endregion

        public bool CanControlCharacter => !IsDead && CanControl; //TODO :: MAKE THIS CONTROLINH FROM ONE PLACE

        protected override void Awake()
        {
            base.Awake();

            RB = GetComponent<Rigidbody>();
            iJump = GetComponent<IJump>();
            imove = GetComponent<IMove>();
        }

        protected override void Start()
        {
            base.Start();

            string missingComponents = string.Empty;

            if (RB == null) missingComponents += $"{RB.GetType().Name} \n ";
            if (iJump == null) missingComponents += $"IJump \n";

            if (string.IsNullOrEmpty(missingComponents))
                return;

            Debug.Log($"** {gameObject.name} Components Missing :=> \n {missingComponents}**************** ");
        }

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
        #endregion
    }
}