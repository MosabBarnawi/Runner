using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using BarnoUtils;

namespace BarnoGames.Runner2020
{
    public class ShardBehavior : MonoBehaviour, IAbility, ILevelState
    {
        private Rigidbody rb;
        private BoxCollider transformCollider;
        [SerializeField] private Transform sharedParent;

        [SerializeField] private Transform[] CurvePoints = new Transform[2];

        //private RaycastHit[] detectionBuffer = new RaycastHit[5];
        //[SerializeField] private bool USE_NON_ALOC = false;
        [Header("States")]
        [SerializeField] private bool isEnmeyTrageted = false;
        [SerializeField] private ShardMotionType _shardMotionType;
        public ShardMotionType ShardMotionType => _shardMotionType;

        [SerializeField] private ShardAttributesScriptable shardSpecification;

        private TargetDetectionCalculations targetDetectionCalculations = new TargetDetectionCalculations();
        private ReturningCalculations returningCalculations = new ReturningCalculations();

        private bool returnAfterTeleport;

        [SerializeField] private Transform Cliff;
        private GameObject CliffGameObject;
        private ShardClifBehavior shardClifBehavior;

        [SerializeField] private Transform VisualCuverLocation;

        public float ShardBoostForwardSpeed => shardSpecification.SharedBoostForwardSpeed;
        public float ShardBoostUpSpeed => shardSpecification.ShardBoostUpSpeed;

        private bool isHitRecallTiming = false;
        private float timeToRepell;

        private Animator animator;
        private bool endLevel = false;
        [SerializeField] private Player playerScript;
        #region Unity Callbacks

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            if (rb.interpolation != RigidbodyInterpolation.None)
            {
                Debug.LogWarning($"RigidbodyInterpolation Should be Set To None.. it was set to {rb.interpolation} , Correcting");
                rb.interpolation = RigidbodyInterpolation.None;
            }

            transformCollider = GetComponent<BoxCollider>();
            CliffGameObject = Cliff.gameObject;

            shardClifBehavior = CliffGameObject.GetComponent<ShardClifBehavior>();
            animator = GetComponent<Animator>();
        }

        private void OnEnable() => PlayerInputControls.MainAbilityAction = MainAbility;

        void Start()
        {
            if (PlayerInputControls.MainAbilityAction == null)
            {
                //PlayerInputControls.AttackAction = Attack;
                Debug.Log($"***Not Assigned . {PlayerInputControls.MainAbilityAction.Method}");
            }

            if (playerScript == null) Debug.Log("Player Script Not Assigned");
            rb.isKinematic = true;

            transformCollider.enabled = false;
            //sharedParent = transform.parent;

            CliffGameObject.SetActive(false);

            //GameManager.SharedInstance.SlowDown += OnLevelEnd;
            GameManager.SharedInstance.RegisterGameState(OnLevelEnd, GameState.WinState);
            //GameManager.SharedInstance.RegisterGameState(OnLevelReady, GameState.LevelReady);
            GameManager.SharedInstance.RegisterGameState(OnLevelReady, GameState.LevelReady);
            GameManager.SharedInstance.RegisterGameState(OnLevelReady, GameState.LevelRestart);
        }

        private void OnDisable()
        {
            //PlayerInputControls.AttackAction -= Attack;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        private void OnDestroy() => PlayerInputControls.MainAbilityAction -= MainAbility;

        void Update()
        {
            DetectEnemies();
        }
        private void FixedUpdate() => ShardAnimation();

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.GetComponent<IPlayerTAG>() != null) return;

            if (_shardMotionType == ShardMotionType.GoingToEnemy || _shardMotionType == ShardMotionType.inMotion)
            {
                if (_shardMotionType == ShardMotionType.inMotion) // TODO:: FOR REPELLING STATES
                {
                    if (collision.transform.GetComponent<IClimableTAG>() != null)
                    {
                        //ClimableWall cliffHeightScript = collision.transform.gameObject.GetComponent<ClimableWall>();
                        //IClimableTAG cliffHeightScript = collision.transform.gameObject.GetComponent<ClimableWall>();
                        IClimableTAG cliffHeightScript = collision.transform.gameObject.GetComponent<IClimableTAG>();

                        Vector3 exitPoint = new Vector3(cliffHeightScript.ExitPointTransform.position.x, cliffHeightScript.ExitPointTransform.position.y, 0);
                        shardClifBehavior.ExitPosition = exitPoint;

                        StickToWall();
                    }
                }

                if (collision.transform.GetComponent<IDamagable>() != null)
                {
                    IDamagable damagable = collision.transform.GetComponent<IDamagable>();
                    damagable.TakeDamage(shardSpecification.HitAmount);

                    HitFX();

                    Repell();
                }
            }
        }
        //private void OnTriggerEnter(Collider other) // TODO:: SOMETHIMES THIS DOSE NOT WORK // AFTER ADDING RIGIDBODY ON ENEMY IT SEEMS TO WORK FINE
        //{
        //    if (other.GetComponent<IPlayerTAG>() != null) return;

        //    if (shardMotionType == ShardMotionType.GoingToEnemy || shardMotionType == ShardMotionType.inMotion)
        //    {
        //        if (shardMotionType == ShardMotionType.inMotion) // TODO:: FOR REPELLING STATES
        //        {
        //            //if (other.gameObject.CompareTag(TAGS.TAG_CLIMABLE))
        //            if (other.GetComponent<IClimableTAG>() != null)
        //            {
        //                ClimableWall cliffHeightScript = other.gameObject.GetComponent<ClimableWall>();

        //                float height = cliffHeightScript.Height;

        //                shardClifBehavior.CliffHeight = height;

        //                StickToWall(in cliffHeightScript);
        //            }
        //        }

        //        if (other.GetComponent<IDamagable>() != null)
        //        {
        //            IDamagable damagable = other.GetComponent<IDamagable>();
        //            damagable.TakeDamage(shardSpecification.HitAmount);
        //        }
        //    }
        //}

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            //Debug.DrawLine(shardSpecification.TargetDetection.Origin, shardSpecification.TargetDetection.Origin + shardSpecification.TargetDetection.direction * shardSpecification.TargetDetection.CurrentHitDistance);
            Debug.DrawLine(targetDetectionCalculations.Origin, targetDetectionCalculations.Origin + targetDetectionCalculations.Direction * targetDetectionCalculations.CurrentHitDistance);

            Gizmos.DrawWireSphere(targetDetectionCalculations.Origin + targetDetectionCalculations.Direction * targetDetectionCalculations.CurrentHitDistance, shardSpecification.TargetDetection.SphereRadious);
        }

        #endregion

        #region Private API
        private void ShardAnimation()
        {
            if (endLevel) return;

            if (_shardMotionType == ShardMotionType.Stuck) return;

            if (_shardMotionType == ShardMotionType.GoingToEnemy)
            {
                // 0.1 for error
                if (targetDetectionCalculations.TravelToTargetTime <= 2f)
                {
                    rb.position = CurvePoint.GetBQCPoint(returningCalculations.CurrentPosition, targetDetectionCalculations.RadndomDistanceBetweenShardAndTarget, targetDetectionCalculations.TargetPositon, targetDetectionCalculations.TravelToTargetTime);

                    targetDetectionCalculations.TravelToTargetTime += shardSpecification.ThrowAttributes.SpeedToTarget * Time.fixedDeltaTime;
                }
                else
                {
                    Debug.LogError("Something Happend, Didint Hit Target");
                    //shardMotionType = ShardMotionType.Returning;
                }
            }
            else if (_shardMotionType == ShardMotionType.Repelling)
            {
                transformCollider.enabled = false;

                if (timeToRepell >= shardSpecification.RepellAttributes.MinTimeCatch && timeToRepell <= shardSpecification.RepellAttributes.MaxTimeCatch)
                {
                    GetComponent<MeshRenderer>().material.color = Color.red;
                    isHitRecallTiming = true;
                }

                if (timeToRepell <= shardSpecification.RepellAttributes.RepellTime)
                {
                    timeToRepell += Time.fixedDeltaTime;
                }
                else
                {
                    isHitRecallTiming = false;

                    GetComponent<MeshRenderer>().material.color = Color.white;

                    rb.AddForce(new Vector3(rb.velocity.x, -shardSpecification.RepellAttributes.FallSpeedAfterRepellSpeed, rb.velocity.z + 0.5f)
                        , ForceMode.Impulse);

                    _shardMotionType = ShardMotionType.Falling;
                }
            }
            else if (_shardMotionType == ShardMotionType.inMotion)
            {
                transform.RotateAround(transform.position, shardSpecification.Rotations[0].MotionRotationAxis, shardSpecification.ThrowAttributes.Standard_RotationSpeed);
            }
            else if (_shardMotionType == ShardMotionType.Returning)
            {
                if (returningCalculations.ReturningTime < 1f)
                {
                    rb.position = CurvePoint.GetBQCPoint(returningCalculations.CurrentPosition,
                                                         CurvePoints[returningCalculations.RandomCurvePointIndex].position,
                                                         sharedParent.position,
                                                         returningCalculations.ReturningTime);

                    if (returnAfterTeleport) // TODO:: START FROM END POINT TELEPORTATION SPOT
                    {
                        returningCalculations.ReturningTime += shardSpecification.ReturnAttributes.ReturnBoostSpeed * Time.fixedDeltaTime;
                    }
                    else
                        returningCalculations.ReturningTime += shardSpecification.ReturnAttributes.ReturnSpeed * Time.fixedDeltaTime;
                }
                else ResetShard();
            }
            else if (_shardMotionType == ShardMotionType.Falling)
            {

            }
            else
            {
                if (_shardMotionType != ShardMotionType.Stuck)
                {
                    transform.RotateAround(transform.position, shardSpecification.Rotations[0].IdelRotationAxis, shardSpecification.ThrowAttributes.Standard_RotationSpeed);
                    //transform.Rotate(rotations[0].IdelRotationAxis.x, rotations[0].IdelRotationAxis.y, rotations[0].IdelRotationAxis.z, Space.World);
                }
            }
        }

        //private void OnLevelEnd(bool stop)
        //{
        //    endLevel = stop;

        //    if (endLevel) Debug.Log("Swithing to Animation");
        //    else Debug.Log("Swithing To Normal");

        //    animator.enabled = endLevel;
        //}

        #region ILevel State
        public void OnLevelEnd()
        {
            endLevel = true;

            if (endLevel) Debug.Log("Swithing to Animation");
            else Debug.Log("Swithing To Normal");

            animator.enabled = endLevel;
        }

        public void OnLevelReady()
        {
            endLevel = false;

            if (endLevel) BarnoDebug.Log(name,"Swithing to Animation", BarnoColor.RussionGreen);
            else BarnoDebug.Log(name,"Swithing To Normal",BarnoColor.RussionGreen);

            animator.enabled = endLevel;
        }
        #endregion

        private void StickToWall()
        {
            _shardMotionType = ShardMotionType.Stuck;

            rb.isKinematic = true;
            transformCollider.enabled = false;

            Cliff.position = transform.position + new Vector3(-2, 0, 0);

            Cliff.rotation = Quaternion.Euler(0, 0, 0);

            CliffGameObject.SetActive(true);
        }

        private void ResetShard()
        {
            _shardMotionType = ShardMotionType.Idel;

            playerScript.CanSwitchPlayers = true;

            transform.parent = sharedParent;

            transform.position = sharedParent.position;

            returningCalculations.ReturningTime = 0;

            targetDetectionCalculations.TravelToTargetTime = 0;
            timeToRepell = 0;
        }

        private void DetectEnemies() //TODO :: ADD FREQUENCY TO UPDATE MAYBE
        {
            if (_shardMotionType != ShardMotionType.Idel) return;

            targetDetectionCalculations.Origin = transform.position;
            targetDetectionCalculations.Direction = Vector3.right;


            if (Physics.SphereCast(
                 targetDetectionCalculations.Origin,
                 shardSpecification.TargetDetection.SphereRadious,
                 targetDetectionCalculations.Direction,
                 out RaycastHit hit,
                 shardSpecification.TargetDetection.MaxDistance,
                 shardSpecification.TargetDetection.TargetableLayerMask,
                 QueryTriggerInteraction.UseGlobal))
            {
                targetDetectionCalculations.Targetable = hit.transform.GetComponent<ITargetable>();

                targetDetectionCalculations.Targetable?.DetectedColor(shardSpecification.TargetDetection.DetectedObjectColor);

                targetDetectionCalculations.CurrentHitDistance = hit.distance;

                isEnmeyTrageted = true;
            }
            else
            {
                isEnmeyTrageted = false;

                targetDetectionCalculations.CurrentHitDistance = shardSpecification.TargetDetection.MaxDistance;

                targetDetectionCalculations.Targetable?.ResetColor();
                targetDetectionCalculations.Targetable = null;
            }
        }

        public void Repell()
        {
            Debug.Log("Repelling");
            rb.AddForce(Vector3.up * shardSpecification.RepellAttributes.RepellSpeed, ForceMode.Impulse);
            _shardMotionType = ShardMotionType.Repelling;
        }

        private void ThrowShard()
        {
            playerScript.CanSwitchPlayers = false;

            if (isEnmeyTrageted)
            {
                _shardMotionType = ShardMotionType.GoingToEnemy;

                returningCalculations.CurrentPosition = transform.position;
                float distance = Vector3.Distance(transform.position, targetDetectionCalculations.TargetPositon);
                Vector3 pos = new Vector3(transform.position.x + (distance / 2), transform.position.y + (distance / 5), transform.position.z + (distance / 2));


                targetDetectionCalculations.RadndomDistanceBetweenShardAndTarget = pos;
                VisualCuverLocation.position = targetDetectionCalculations.RadndomDistanceBetweenShardAndTarget;

                transform.parent = null;
                transformCollider.enabled = true;
                rb.isKinematic = false;
                isEnmeyTrageted = false;
            }
            else
            {
                _shardMotionType = ShardMotionType.inMotion;

                transform.parent = null;
                transformCollider.enabled = true;
                rb.isKinematic = false;

                rb.AddForce(Vector3.right * shardSpecification.ThrowAttributes.Standard_ShardSpeed, ForceMode.Impulse);
            }

            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void RecallShard(bool isboost)
        {
            if (isHitRecallTiming)
            {
                Debug.Log("Perfect Timing");
                // TODO RESET SHARD FX OR USE FX FOR PURFECT TIMING
                isHitRecallTiming = false;
            }

            returningCalculations.GetRandomReturnPoint(CurvePoints.Length);

            returningCalculations.CurrentPosition = transform.position;
            transformCollider.enabled = false;

            _shardMotionType = ShardMotionType.Returning;

            rb.isKinematic = true;

            rb.velocity = Vector3.zero;

            CliffGameObject.SetActive(false);

            returnAfterTeleport = isboost;
        }

         public void HardResetShard()
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            transformCollider.enabled = false;
            CliffGameObject.SetActive(false);
            ResetShard();
        }

        #region IAbility Interface
        public void MainAbility()
        {
            if (_shardMotionType == ShardMotionType.Idel)
                ThrowShard();

            else if (_shardMotionType != ShardMotionType.Returning)
                RecallShard(false);
        }
        #endregion

        private bool stopping;
        [SerializeField] private float stopTime = 0.2f;
        [SerializeField] private float slowTime = 0.2f;

        private void HitFX()
        {
            if (!stopping)
            {
                Vibration.Vibrate();
                stopping = true;
                Time.timeScale = 0;
                // IF USE COROTINE FOR SHAKE IT SHAKES THE SAME SPEED AS IT WOULD
                // BUT USING UPDATE IT SHAKES RELATED TO TIME SCALES
                StartCoroutine(HitEffectSlow());
                CameraShake.SharedInstance.Shake();
            }
        }

        private IEnumerator HitEffectSlow()
        {
            yield return new WaitForSecondsRealtime(stopTime);

            Time.timeScale = 0.01f;

            yield return new WaitForSecondsRealtime(slowTime);

            Time.timeScale = 1;
            stopping = false;
        }

        #endregion
    }
}