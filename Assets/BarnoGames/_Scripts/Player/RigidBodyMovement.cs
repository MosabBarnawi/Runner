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

        [SerializeField] private MovementSettings movementSettings;
        private Character character;

        #region Unity Callbacks

        private void Awake() => character = GetComponent<Character>();

        void Start()
        {
            //if (character.IAmPlayer /*&& PlayerInputControls.MoveAction == null*/)
            //{
            //    PlayerInputControls.MoveAction = SetVelocity;
            //    //Debug.Log($"***Not Assigned . {PlayerInputControls.MoveAction.Method}");
            //}
        }

        //private void OnDisable()
        //{
        //    if (character.IAmPlayer) PlayerInputControls.MoveAction -= SetVelocity;
        //}

        //private void OnDestroy()
        //{
        //    if (character.IAmPlayer) PlayerInputControls.MoveAction -= SetVelocity;
        //}

        void FixedUpdate()
        {
            if (canMove && character.CanControlCharacter)
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
            float speed = direction * movementSettings.MovementSpeed * Time.fixedDeltaTime;

            try
            {
                if (character.iJump.GetHangTime().isHangSpeed) //TODO:: GET PLAYER HANGFORWARD SPEED
                    speed = direction * movementSettings.HangForwardSpeed * Time.fixedDeltaTime;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            float speedClmap = Mathf.Clamp(speed * 100, 0, speed * 100);

            character.RB.velocity = new Vector3(/*speed * 100*/speedClmap, character.RB.velocity.y, character.RB.velocity.z);
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
            character.RB.velocity = Vector3.zero;

            if (freezeInSpace)
                Debug.Log("STOP - frozen in Place");
            else
                Debug.Log("STOP");

            //if (freezeInSpace) character.RB.isKinematic = true; //TODO:: FIX WARRNING
            character.RB.isKinematic = freezeInSpace; //TODO:: FIX WARRNING
        }

        public void EnableMovement()
        {
            Debug.Log("Enable Running");
            canMove = true;

            character.RB.isKinematic = false;
        }
        #endregion

        #endregion

    }
}
