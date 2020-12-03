using UnityEngine.EventSystems;
using UnityEngine;

namespace BarnoGames.Runner2020.ScreenInput
{
    public class Input_Touch : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
    {
        [SerializeField] private InputControlType InputToControl;

        public void OnPointerDown(PointerEventData eventData) => SendInput(true);

        public void OnPointerUp(PointerEventData eventData) => SendInput(false);

        private void SendInput(bool pressed)
        {
            switch (InputToControl)
            {
                case InputControlType.Empty:
                    break;
                case InputControlType.Jump:
                    if (pressed)
                        PlayerInputControls.JumpInput(JumpInput.Jump);
                    else
                        PlayerInputControls.JumpInput(JumpInput.NotJump);
                    break;
                case InputControlType.Attack:
                    if (PlayerInputControls.Player.CanAnimateCharacter)
                        PlayerInputControls.AttackAction?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }
}