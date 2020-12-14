using BarnoGames.Runner2020;
using UnityEngine;
using TMPro;
using System;
using System.Collections;

[SelectionBase]
public class Player : Character, IEnablePlayerControlHandler, IBoost
{
    [Header("Ray Casting")]
    [SerializeField] private LayerMask WalkableLayerMask;
    [SerializeField] private LayerMask ForwardDetectionLayerMask;
    [SerializeField] private float GroundRaycastDistance = 0.11f;

    [Space(5)]
    [SerializeField] private float RaycastDistanceForward = 1.5f;
    //[SerializeField] private Vector3 adjustWhenStuckOnWall = new Vector3(1, 2, 0);

    [SerializeField] private float speedToTransitionIntoSlopAngle = 0.2f;

    private IMove imove;

    private Collider currentCollider;

    [Header("Debugging")]
    [SerializeField] private TextMeshProUGUI DebugText;
    [SerializeField] private bool DebugDraw = false;

    [SerializeField] private PlayerSwitching playerSwitching;
    private PlayerType playerType;

    [SerializeField] private ShardBehavior ShardBehavior;

    [SerializeField] private bool isNonAlocRayCastForwad = true;
    RaycastHit[] raycasts = new RaycastHit[1];

    [SerializeField] private GameObject DeathFX;
    private bool hasLevelStarted; // TODO:: CHANGE TO ENUM FOR PLAYER STATE
    public Action PlayerHasDiedAction;

    #region Unity Callbacks

    protected override void Awake()
    {
        //Application.targetFrameRate = -1;

        base.Awake();
        IAmPlayer = true;

        InitilizeControls();

        if (Anim == null) Debug.LogError($"Animator Not Assigned {gameObject.name}");

        imove = GetComponent<IMove>();

        playerSwitching.SwitchPlayerAction = SwitchCharacterType;
    }

    protected override void Start()
    {
        base.Start();

        OnDeath += PlayerDeath;
        OnAddHealth += AddHealthFX;
        OnTakeHit += TakeHit;
        OnRespawn += RespawnPlayer;

        GameManager.SharedInstance.OnPlayerRespawn = Respawn;
        GameManager.SharedInstance.OnWinStateAction = PlayerInWinState;

        SwitchCharacterType(PlayerType.MainPlayer);
    }

    private void Update()
    {
        isGroundedAnimation(); // MOVE TO FIXED UPDATED ?

        CanSwitchPlayersInput();

        if (!hasLevelStarted)
        {
            if (IsGrounded)
            {
                Anim.SetBool(ANIMATIONS_CONSTANTS.IS_PLAYER_FALLING, false);
                hasLevelStarted = true;
            }
        }
    }

    private void FixedUpdate()
    {
        //TODO CAHNGE INTORPOLATION TYPE TO REMOVE JITTER
        //if (Application.targetFrameRate <= 20) rb.interpolation = RigidbodyInterpolation.None;

        CheckForwardRayCast();
        IsGrounded = CheckIfGrounded();

        if (canSmash)
        {
            if (ballAttackTimer >= secondaryAttackTime)
                canSmash = false;
            else
                ballAttackTimer += Time.fixedDeltaTime;
        }
        //isGroundedAnimation();
    }

    #endregion
    public void EnableControls(bool enabled) => CanControl = enabled;

    private void CanSwitchPlayersInput()
    {
        if (ShardBehavior.ShardMotionType == ShardMotionType.Idel)
            CanSwitchPlayers = true;
        else CanSwitchPlayers = false;
    }

    private void SwitchCharacterType(PlayerType playerType)
    {
        this.playerType = playerType;

        if (playerType == PlayerType.MainPlayer)
        {
            RB = playerSwitching.SwitchToMainCharacter(this.playerType, ref currentCollider);
            ShardBehavior.HardResetShard();
            canSmash = false;
        }
        else if (playerType == PlayerType.Secondary)
        {
            RB = playerSwitching.SwitchToSecondaryCharacter(this.playerType, ref currentCollider);
            PlayerInputControls.AttackAction = BallAttack;
        }
    }

    private bool canSmash = false;
    private float ballAttackTimer;
    [SerializeField] private float secondaryAttackTime = 1f;
    private void BallAttack()
    {
        if (!canSmash && playerType == PlayerType.Secondary)
        {
            Debug.Log("ATTACK BALL");
            canSmash = true;
            ballAttackTimer = 0;
            RB.AddForce(Vector3.right * 100, ForceMode.Impulse);
        }
    }

    public void ReSpawnToPosition(Vector3 position)
    {
        gameObject.SetActive(true);
        // SwitchBack to MainPlayer
        SwitchCharacterType(PlayerType.MainPlayer);
        //imove.FreezePositionInSpace();

        // TODO :: DISABLE KENEMATIC
        RB.transform.position = position;

        //Anim.SetBool(ANIMATIONS_CONSTANTS.IS_LEVEL_END, false);
        imove.EnableMovement();
    }

    public void ReSpawnToPositionLevelStart_falling(Vector3 position)
    {
        // SwitchBack to MainPlayer
        SwitchCharacterType(PlayerType.MainPlayer);
        RB.transform.position = new Vector3(position.x, 20, position.z);
        imove.EnableMovement();
    }

    public void PlayerFalling()
    {
        //hasLevelStarted = false;
        StartCoroutine(delayHasLevlStartedCHECK());
        imove.StopMovement(true);
        Anim.SetBool(ANIMATIONS_CONSTANTS.IS_PLAYER_FALLING, true);
        Anim.SetBool(ANIMATIONS_CONSTANTS.IS_LEVEL_END, false);
        EnableControls(false);
    }

    private IEnumerator delayHasLevlStartedCHECK()
    {
        yield return new WaitForSecondsRealtime(.2f);
        hasLevelStarted = false;
    }

    public void LevelStarted()
    {
        //Anim.SetBool(ANIMATIONS_CONSTANTS.IS_PLAYER_FALLING, false);
        imove.EnableMovement();
        EnableControls(true);
        GameManager.SharedInstance.LevelStart();
    }

    private void PlayerInWinState()
    {
        SwitchCharacterType(PlayerType.MainPlayer);
        //TODO:: HANDLE IF SHARD HAS BEEN THROWN TO BRING IT BACKBEFORE STARTING NEXT LEVEL
        Anim.SetBool(ANIMATIONS_CONSTANTS.IS_LEVEL_END, true);
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

    private void CheckForwardRayCast() //TODO:: DESIGN BREAKING WALLS SHOUDL IT BE BUTTON BASED OR NOT
    {
        bool HitWall;

        //if (isNonAlocRayCastForwad)
        //{
        int hits = Physics.RaycastNonAlloc(RB.position, RB.transform.TransformDirection(Vector3.right), raycasts, RaycastDistanceForward, ForwardDetectionLayerMask);

        HitWall = hits > 0 ? true : false;

        if (playerType == PlayerType.Secondary)
        {
            if (raycasts[0].transform != null)
            {
                IDestroyable destroyable = raycasts[0].transform.GetComponent<IDestroyable>();

                if (destroyable != null)
                {
                    if (canSmash)
                    {
                        destroyable.Smached();
                        canSmash = false;
                    }
                    else
                    {
                        raycasts = new RaycastHit[1];
                        InstantDie();
                    }
                }
            }
        }
        else if (playerType == PlayerType.MainPlayer)
        {
            if (raycasts[0].transform != null)
            {
                Debug.LogWarning(raycasts[0].transform.name);
                raycasts = new RaycastHit[1];
                InstantDie();
            }
        }

        //}
        //else
        //{
        //    HitWall = Physics.Raycast(RB.position, RB.transform.TransformDirection(Vector3.right), out RaycastHit hit, RaycastDistanceForward, ForwardDetectionLayerMask);

        //    if (playerType == PlayerType.Secondary)
        //    {
        //        if (hit.transform != null)
        //        {
        //            IDestroyable destroyable = hit.transform.GetComponent<IDestroyable>();

        //            if (destroyable != null)
        //            {
        //                destroyable.Smached();
        //            }
        //        }
        //    }
        //}


        Debug.DrawRay(RB.position, new Vector3(RaycastDistanceForward, 0, 0), Color.red);

        if (HitWall /*&& !IsGroundSmash*/)
        {
            Debug.LogWarning("Wall");
            Tool_DebugText("Wall", Color.red);
            // TODO ADJUST PLAYER
            //transform.position += adjustWhenStuckOnWall;
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

        hitGround = forwardGrounded || middleGrounded || backwardGrounded;

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

#if UNITY_EDITOR
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
#endif

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

    #region IBoost Interface
    public void TeleportToPosition(Vector3 endPosition)
    {
        // CAN STOP ROLL AND CONTINUE FOR FX
        // WHEN GROUNDED CONTINUE TO RUN
        //imove.StopMovement(false);

        RB.position = endPosition;
        //Instantiate(GameManager.SharedInstance.TeleportFX, endPosition, Quaternion.identity);
        Instantiate(GameManager.SharedInstance.TeleportFX, endPosition, GameManager.SharedInstance.TeleportFX.transform.rotation);

        float xBoost = RB.velocity.x + ShardBehavior.ShardBoostForwardSpeed;
        float yBoost = RB.velocity.y + ShardBehavior.ShardBoostUpSpeed;

        Vector3 Boost = new Vector3(xBoost, yBoost, 0);

        RB.AddForce(Boost, ForceMode.VelocityChange);

        ShardBehavior.RecallShard(false);
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
        gameObject.SetActive(false);
        imove.StopMovement(false);
        ShardBehavior.HardResetShard();

        Instantiate(DeathFX, RB.transform.position, DeathFX.transform.rotation);

        PlayerHasDiedAction?.Invoke();
    }

    private void RespawnPlayer()
    {
        //TODO :: REPSANW FX
        Debug.Log("Respawned Player");
    }

    private void AddHealthFX()
    {
        Debug.Log("Sparks");
    }
    public void TakeHit()
    {

    }

    #endregion

    #endregion

    private void Tool_DebugText(string text, Color color)
    {
        DebugText.text = text;
        DebugText.color = color;
    }
}
