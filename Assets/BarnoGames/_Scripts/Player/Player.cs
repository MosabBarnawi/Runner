using BarnoGames.Runner2020;
using UnityEngine;
using TMPro;
using System;

public enum PlayerType { MainPlayer, Secondary }
[SelectionBase]
public class Player : Character, IEnablePlayerControlHandler
{
    [Header("Ray Casting")]
    [SerializeField] private LayerMask WalkableLayerMask;
    [SerializeField] private LayerMask ForwardDetectionLayerMask;
    [SerializeField] private float GroundRaycastDistance = 0.11f;

    [Space(5)]
    [SerializeField] private float RaycastDistanceForward = 1.5f;
    [SerializeField] private Vector3 adjustWhenStuckOnWall = new Vector3(1, 2, 0);

    [SerializeField] private float speedToTransitionIntoSlopAngle = 0.2f;

    private IMove imove;

    [Header("Switch Player")]
    [SerializeField] private PlayerType playerType;
    [SerializeField] private GameObject MainPlayer;
    [SerializeField] private GameObject SecondaryPlayer; //TODO:: ADD FAKE ROLLING

    private Rigidbody _mainPlayerRB;
    private Rigidbody _secondaryRB;

    private CapsuleCollider capsuleCollider;
    private SphereCollider sphereCollider;
    private Collider currentCollider;

    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI DebugText;
    [SerializeField] private bool DebugDraw = false;

    #region Unity Callbacks

    protected override void Awake()
    {
        //Application.targetFrameRate = -1;

        base.Awake();
        IAmPlayer = true;

        InitilizeControls();

        if (MainPlayer == null)
            Debug.LogError("Main Player Not Assigned");
        if (SecondaryPlayer == null)
            Debug.LogError("Secondary Player Not Assigned");

        capsuleCollider = MainPlayer.GetComponent<CapsuleCollider>();
        sphereCollider = SecondaryPlayer.GetComponent<SphereCollider>();

        _mainPlayerRB = MainPlayer.GetComponent<Rigidbody>();
        _secondaryRB = SecondaryPlayer.GetComponent<Rigidbody>();


        if (Anim == null) Debug.LogError($"Animator Not Assigned {gameObject.name}");

        imove = GetComponent<IMove>();

        rb = _mainPlayerRB;
        currentCollider = capsuleCollider;
    }

    private void OnEnable()
    {
        if (IAmPlayer) PlayerInputControls.SpecialAbility = SwitchPlayers;
    }

    protected override void Start()
    {
        base.Start();

        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;

        if (IAmPlayer && PlayerInputControls.SpecialAbility == null)
        {
            PlayerInputControls.SpecialAbility = SwitchPlayers;
            Debug.LogWarning($"***Not Assigned . Trying to Find {PlayerInputControls.SpecialAbility.Method}");
        }
    }

    private void Update() => isGroundedAnimation(); // MOVE TO FIXED UPDATED ?

    private void FixedUpdate()
    {
        //TODO CAHNGE INTORPOLATION TYPE TO REMOVE JITTER
        //if (Application.targetFrameRate <= 20) rb.interpolation = RigidbodyInterpolation.None;

        CheckForwardRayCast();
        IsGrounded = CheckIfGrounded();
        //isGroundedAnimation();
    }


    private void OnDisable()
    {
        if (IAmPlayer)
            PlayerInputControls.SpecialAbility -= SwitchPlayers;
    }

    private void OnDestroy()
    {
        if (IAmPlayer)
            PlayerInputControls.SpecialAbility -= SwitchPlayers;
    }
    #endregion
    public void EnableControls(bool enabled) => CanControl = enabled;

    public void ReSpawnToPosition(Vector3 position)
    {
        // SwitchBack to MainPlayer
        if (playerType == PlayerType.Secondary) SwitchPlayers();
        //imove.StopMovement();
        //imove.FreezePositionInSpace();

        // TODO :: DISABLE KENEMATIC
        Debug.Log(position);
        rb.transform.position = position;

        Anim.SetBool("EndLevel", false);
        imove.EnableMovement();
    }

    public void PlayerInWinState()
    {
        if (playerType != PlayerType.MainPlayer) SwitchPlayers();
        //TODO:: HANDLE IF SHARD HAS BEEN THROWN TO BRING IT BACKBEFORE STARTING NEXT LEVEL
        Anim.SetBool("EndLevel", true);
        imove.StopMovement(true);
    }

    #region Private API

    private void InitilizeControls()
    {
        if (PlayerInputControls.Player == null)
            PlayerInputControls.Player = this;
        else
        {
            if (PlayerInputControls.Player != this)
                Destroy(gameObject);
        }
    }

    #region RAY CASTINGS AND SLOPE DETECTION LOGIC
    private void CheckForwardRayCast()
    {
        bool HitWall = Physics.Raycast(rb.position, rb.transform.TransformDirection(Vector3.right)/*, out RaycastHit hit*/, RaycastDistanceForward, ForwardDetectionLayerMask);

        Debug.DrawRay(rb.position, new Vector3(RaycastDistanceForward, 0, 0), Color.red);

        if (HitWall && !IsGroundSmash)
        {
            Debug.LogWarning("Wall");
            Tool_DebugText("Wall", Color.red);
            // TODO ADJUST PLAYER
            transform.position += adjustWhenStuckOnWall;
        }

        else Tool_DebugText("", Color.green);
    }

    protected override bool CheckIfGrounded()
    {
        bool hitGround = false;

        bool forwardGrounded = Physics.Raycast(currentCollider.bounds.center + new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down, out RaycastHit hitForward, GroundRaycastDistance, WalkableLayerMask);
        bool middleGrounded = Physics.Raycast(currentCollider.bounds.center, Vector3.down, out RaycastHit hitMiddle, GroundRaycastDistance, WalkableLayerMask);
        bool backwardGrounded = Physics.Raycast(currentCollider.bounds.center - new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down, out RaycastHit hitBackward, GroundRaycastDistance, WalkableLayerMask);


        Transform forwardPositon = hitForward.transform;
        Transform middlePositon = hitMiddle.transform;
        Transform backwardPositon = hitBackward.transform;

        if ((IsGroundSmash && forwardPositon != null) || (IsGroundSmash && middlePositon != null) || (IsGroundSmash && backwardPositon != null))
        {
            if (forwardPositon.CompareTag(TAGS.TAG_BREAKABLE) || backwardPositon.CompareTag(TAGS.TAG_BREAKABLE) || middlePositon.CompareTag(TAGS.TAG_BREAKABLE))
            {
                // DO STUFF
                forwardPositon.GetComponent<Destroyable>()?.Smached();
                Debug.Log("Hit Breakable Floor");
                hitGround = false;
            }
            else
            {
                // go back to normal
                imove.EnableMovement();
                IsGroundSmash = false;
                //TODO:: ADD FX
            }
        }
        else
        {
            hitGround = forwardGrounded || middleGrounded || backwardGrounded;
        }


        if (playerType == PlayerType.MainPlayer)
        {
            if (forwardPositon != null)
            {
                Enemy forwardEnemy = forwardPositon.GetComponent<Enemy>();

                if (forwardEnemy != null)
                {
                    Debug.Log("F: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                          //TODO:: RESET PLAYER
                }
            }
            else if (middlePositon != null)
            {
                Enemy MiddleEnemy = middlePositon.GetComponent<Enemy>();

                if (MiddleEnemy != null)
                {
                    Debug.Log("M: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                          //TODO:: RESET PLAYER
                }

            }
            else if (backwardPositon != null)
            {
                Enemy backEnemy = backwardPositon.GetComponent<Enemy>();
                if (backEnemy != null)
                {
                    Debug.Log("B: ENemy");
                    imove.StopMovement(false); // STOP PLAYER MOVEMENT FOR NOT THIS NEEDS TO CHANGE 
                                          //TODO:: RESET PLAYER
                }
            }
        }

        //TODO THIS EFFECTS HITTING ENEMIES AND IT WILL GO THROGH THE GROUND
        //SlopPlayerAngleAdjustment(in hitGround, in forwardPositon, in middlePositon, in backwardPositon);

        if (DebugDraw)
        {
            Color rayColor = Color.green;

            if (hitGround)
            {
                rayColor = Color.red;
            }

            Debug.DrawRay(currentCollider.bounds.center + new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
            Debug.DrawRay(currentCollider.bounds.center, Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
            Debug.DrawRay(currentCollider.bounds.center - new Vector3(currentCollider.bounds.extents.x, 0), Vector3.down * (currentCollider.bounds.extents.y + GroundRaycastDistance), rayColor);
        }

        return hitGround;
    }

    protected override void SlopPlayerAngleAdjustment(in bool hitGround, in Transform forwardPosition, in Transform middlePosition, in Transform backwardPosition)
    {
        //float minSlopAngle = 0.1f;
        float minSlopAngle = 0;
        // TODO FIX SLOPE ISSUE
        Quaternion rotationOnSlope = new Quaternion(0, 0, 0, 0);
        IsOnSlope = false;

        if (hitGround)
        {
            if (forwardPosition != null)
            {
                if (forwardPosition.localRotation.z >= minSlopAngle || forwardPosition.localRotation.z <= -minSlopAngle)
                {
                    IsOnSlope = true;
                    rotationOnSlope = forwardPosition.localRotation;
                }
            }
            else
            {
                if (middlePosition != null)
                {
                    if (middlePosition.localRotation.z >= minSlopAngle || middlePosition.localRotation.z <= -minSlopAngle)
                    {
                        IsOnSlope = true;
                        rotationOnSlope = middlePosition.localRotation;
                    }
                }
                else
                {
                    if (backwardPosition != null)
                    {
                        if (backwardPosition.localRotation.z >= minSlopAngle || backwardPosition.localRotation.z <= -minSlopAngle)
                        {
                            IsOnSlope = true;
                            rotationOnSlope = backwardPosition.localRotation;
                        }
                    }
                }
            }
        }

        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotationOnSlope, speedToTransitionIntoSlopAngle);
    }

    #endregion

    #region Animations

    public override void isGroundedAnimation() => Anim.SetBool(ANIMATIONS_CONSTANTS.IS_GROUNDED_HASH, IsGrounded);

    public override void MoveAnimation(in float direction) => Anim.SetFloat(ANIMATIONS_CONSTANTS.MOVEMENT_SPEED_HASH, direction);

    public override void isJumpAnimation(in bool isJump) => Anim.SetBool(ANIMATIONS_CONSTANTS.IS_JUMP_HASH, isJump);

    public override void isHardLandAnimation(in bool isHashLand) => Anim.SetBool(ANIMATIONS_CONSTANTS.HARD_LAND_HASH, isHashLand);

    public override void isSpeedBoostAnimation(in bool isSpeedUp) => Anim.SetBool(ANIMATIONS_CONSTANTS.SPEED_UP_HASH, isSpeedUp);

    #endregion

    #region LIVING_ENTITY_OVERRIDES
    private void PlayerDeath()
    {
        Debug.Log("Died");
        imove.StopMovement(false);
    }

    private void AddHealthFX()
    {
        Debug.Log("Sparks");
    }
    public void TakeHit()
    {

    }

    #endregion

    private void SwitchPlayers()
    {
        if (playerType == PlayerType.Secondary)
        {
            rb = _mainPlayerRB;
            MainPlayer.transform.parent = transform;

            SecondaryPlayer.transform.parent = MainPlayer.transform;

            MainPlayer.transform.localScale = Vector3.one;
            MainPlayer.transform.localEulerAngles = Vector3.zero;

            SecondaryPlayer.transform.localScale = Vector3.one;
            SecondaryPlayer.transform.localEulerAngles = Vector3.zero;

            MainPlayer.SetActive(true);
            SecondaryPlayer.SetActive(false);

            currentCollider = capsuleCollider;

            playerType = PlayerType.MainPlayer;
        }
        else if (playerType == PlayerType.MainPlayer)
        {
            rb = _secondaryRB;

            SecondaryPlayer.transform.parent = transform;
            MainPlayer.transform.parent = SecondaryPlayer.transform;

            SecondaryPlayer.transform.localScale = Vector3.one;
            SecondaryPlayer.transform.localEulerAngles = Vector3.zero;

            MainPlayer.transform.localScale = Vector3.one;
            MainPlayer.transform.localEulerAngles = Vector3.zero;

            MainPlayer.SetActive(false);
            SecondaryPlayer.SetActive(true);

            currentCollider = sphereCollider;

            playerType = PlayerType.Secondary;
        }
    }

    #endregion

    private void Tool_DebugText(string text, Color color)
    {
        DebugText.text = text;
        DebugText.color = color;
    }
}
