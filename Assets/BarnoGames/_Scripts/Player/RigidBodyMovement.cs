using System;
using UnityEngine;


namespace BarnoGames.Runner2020
{
    [DisallowMultipleComponent]
    public partial class RigidBodyMovement : MonoBehaviour, IMove
    {
        [Header("Movement Controls")]

        [Range(-1f, 1.0f)]
        [SerializeField] private float movementDirection = 1f;
        [SerializeField] private bool isConstantMovement = false;

        private float direction;
        private bool canMove = true;

        #region Caching
        [Header("Caching")]
        private GlobalPlayerMovementSettings playerSettings;
        private GlobalJumpSettings globalJumpSettings;
        private Character character;
        #endregion

        #region Unity Callbacks

        private void Awake() => character = GetComponent<Character>();

        private void OnEnable()
        {
            if (character.IAmPlayer)
                PlayerInputControls.MoveAction = SetVelocity;
        }
        void Start()
        {
            if (character.IAmPlayer && PlayerInputControls.MoveAction == null)
            {
                PlayerInputControls.MoveAction = SetVelocity;
                Debug.Log($"***Not Assigned . {PlayerInputControls.MoveAction.Method}");
            }

            globalJumpSettings = GameManager.SharedInstance.GlobalsJumpSettings;
            playerSettings = GameManager.SharedInstance.GlobalPlayerMovementSettings;
        }

        private void OnDisable()
        {
            if (character.IAmPlayer) PlayerInputControls.MoveAction -= SetVelocity;
        }

        private void OnDestroy()
        {
            if (character.IAmPlayer) PlayerInputControls.MoveAction -= SetVelocity;
        }

        void FixedUpdate()
        {
            if (canMove && character.CanAnimateCharacter)
            {
                if (isConstantMovement)
                    MoveCharacter(movementDirection);
                else
                    MoveCharacter(direction);
            }
        }

        #endregion

        #region Private API

        private void MoveCharacter(in float direction)
        {
            character.MoveAnimation(direction);

            //TODO:: ADD DIFF FOR NON PLAYER
            float speed = direction * playerSettings.MovementSpeed * Time.fixedDeltaTime;

            try
            {
                if (character.iJump.GetHangTime().isHangSpeed)
                    speed = direction * globalJumpSettings.HangForwardSpeed * Time.fixedDeltaTime;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            float speedClmap = Mathf.Clamp(speed * 100, 0, speed * 100);

            character.rb.velocity = new Vector3(/*speed * 100*/speedClmap, character.rb.velocity.y, character.rb.velocity.z);
        }

        #endregion

        #region Public API

        #region IMove Interface
        public void SetVelocity(float VelocityVector)
        {
            if (!isConstantMovement)
                direction = VelocityVector;
        }

        public void StopMovement(bool freezeInSpace)
        {
            canMove = false;
            character.rb.velocity = Vector3.zero;
            Debug.Log("STOP");

            if (freezeInSpace) character.rb.isKinematic = true;
        }

        public void EnableMovement()
        {
            Debug.Log("Enable Running");
            canMove = true;

            character.rb.isKinematic = false;
        }
        #endregion

        #endregion

    }
}
