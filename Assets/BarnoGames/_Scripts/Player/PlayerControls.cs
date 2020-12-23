using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public class PlayerControls : MonoBehaviour, IControls
    {
        //[SerializeField] bool usingKeyboard = false;

        #region Unity Calls

        private void Update()
        {
            //MovementDirectionInput();
            //JumpInput(Runner2020.JumpInput.Empty);
            AttackKeyBoardInput();
        }

        #endregion

        #region IControls Interfaces
        //public void MovementDirectionInput()
        //{
        //    if (usingKeyboard)
        //    {
        //        //float direction = Input.GetAxisRaw(CONTROLS.INPUT_HORIONTAL);
        //        ////PlayerInputControls.MoveAction(direction);
        //        //PlayerInputControls.MoveAction?.Invoke(direction);


        //        if (PlayerInputControls.MoveAction == null)
        //            Debug.Log("MOVE ANT");
        //    }
        //}

        public void JumpInput(JumpInput isJump)
        {
            if (isJump == Runner2020.JumpInput.Empty) return;

            if (PlayerInputControls.Player.CanControlCharacter)
            {
                //if (usingKeyboard)
                //{
#if UNITY_EDITOR
                bool jumpped = Input.GetKey(KeyCode.Space);

                isJump = jumpped ? Runner2020.JumpInput.Jump : Runner2020.JumpInput.NotJump;
                //}

                PlayerInputControls.Player.iJump.SetJump(isJump);
#endif
            }
        }

        public void SwitchAbilityInput()
        {
            if (PlayerInputControls.Player.CanControlCharacter)
            {
                PlayerInputControls.SpecialAbility?.Invoke();

                if (PlayerInputControls.SpecialAbility == null)
                    Debug.Log("Jump Action is Null");
            }
        }

        public void AttackInput()
        {
            if (PlayerInputControls.Player.CanControlCharacter)

                PlayerInputControls.MainAbilityAction?.Invoke();

            if (PlayerInputControls.MainAbilityAction == null)
                Debug.Log("Attack Action is Null");
        }
        #endregion

        #region Private API

        private void AttackKeyBoardInput()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftShift)) AttackInput();
#endif
        }

        #endregion
    }
}