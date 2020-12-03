using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class PlayerControls : MonoBehaviour, IControls
    {
        [SerializeField] bool usingKeyboard = false;

        #region Unity Calls

        private void Update()
        {
            MovementDirectionInput();
            JumpInput(Runner2020.JumpInput.Empty);
            AttackKeyBoardInput();
        }

        #endregion

        #region IControls Interfaces
        public void MovementDirectionInput()
        {
            if (usingKeyboard)
            {
                float direction = Input.GetAxisRaw(CONTROLS.INPUT_HORIONTAL);
                //PlayerInputControls.MoveAction(direction);
                PlayerInputControls.MoveAction?.Invoke(direction);


                if (PlayerInputControls.MoveAction == null)
                    Debug.Log("MOVE ANT");
            }
        }

        public void JumpInput(JumpInput isJump)
        {
            if (isJump == Runner2020.JumpInput.Empty) return;

            if (PlayerInputControls.Player.CanAnimateCharacter)
            {
                if (usingKeyboard)
                {
                    bool jumpped = Input.GetKey(KeyCode.Space);

                    isJump = jumpped ? Runner2020.JumpInput.Jump : Runner2020.JumpInput.NotJump;
                }

                PlayerInputControls.Player.iJump.SetJump(isJump);
            }
        }

        public void JumpAbilityInput()
        {
            if (PlayerInputControls.Player.CanAnimateCharacter)
            {
                PlayerInputControls.SpecialAbility?.Invoke();

                if (PlayerInputControls.SpecialAbility == null)
                    Debug.Log("JUMP ANT");
            }
        }

        public void AttackInput()
        {
            if (PlayerInputControls.Player.CanAnimateCharacter)

                PlayerInputControls.AttackAction?.Invoke();

            if (PlayerInputControls.AttackAction == null)
                Debug.Log("HIT ANT");
        }
        #endregion

        #region Private API

        private void AttackKeyBoardInput()
        {
            if (usingKeyboard)
                if (Input.GetKeyDown(KeyCode.L)) AttackInput();
        }

        #endregion
    }
}