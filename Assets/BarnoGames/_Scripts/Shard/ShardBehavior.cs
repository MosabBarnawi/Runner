using BarnoGames.Runner2020.Calculations;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BarnoGames.Runner2020
{
    public class ShardBehavior : MonoBehaviour
    {
        //public static ShardBehavior SharedInstance;

        private Rigidbody rb;
        private BoxCollider transformCollider;
        private Transform sharedParent;

        [SerializeField] private Transform[] CurvePoints = new Transform[2];

        //private RaycastHit[] detectionBuffer = new RaycastHit[5];
        //[SerializeField] private bool USE_NON_ALOC = false;
        [Header("States")]
        [SerializeField] private bool isEnmeyTrageted = false;
        [SerializeField] private ShardMotionType shardMotionType;

        [SerializeField] private ShardAttributesScriptable shardSpecification;

        private TargetDetectionCalculations targetDetectionCalculations = new TargetDetectionCalculations();
        private ReturningCalculations returningCalculations = new ReturningCalculations();

        public Action Action_ShardBoostPlayer;
        private bool canBoostPlayer;

        [SerializeField] private Transform Cliff;
        private GameObject CliffGameObject;
        private ShardClifBehavior shardClifBehavior;

        [SerializeField] private Transform VisualCuverLocation;

        public float ShardBoostSpeed => shardSpecification.SharedBoostSpeed;

        private bool isHitRecallTiming = false;
        private float timeToRepell;

        private Animator animator;
        private bool endLevel = false;


        #region Unity Callbacks

        private void Awake()
        {
            //SharedInstance = this;

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

        private void OnEnable() => PlayerInputControls.AttackAction = Attack;

        void Start()
        {
            if (PlayerInputControls.AttackAction == null)
            {
                PlayerInputControls.AttackAction = Attack;
                Debug.Log($"***Not Assigned . {PlayerInputControls.AttackAction.Method}");
            }

            rb.isKinematic = true;
            //rb.interpolation = RigidbodyInterpolation.Interpolate;

            transformCollider.enabled = false;
            sharedParent = transform.parent;

            CliffGameObject.SetActive(false);

            GameManager.SharedInstance.SlowDown += OnLevelEnd;
        }

        private void OnDisable()
        {
            PlayerInputControls.AttackAction -= Attack;
            rb.interpolation = RigidbodyInterpolation.None;
        }

        private void OnDestroy() => PlayerInputControls.AttackAction -= Attack;

        void Update() => DetectEnemies();

        private void FixedUpdate() => ShardAnimation();

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.transform.GetComponent<IPlayerTAG>() != null) return;

            if (shardMotionType == ShardMotionType.GoingToEnemy || shardMotionType == ShardMotionType.inMotion)
            {
                if (shardMotionType == ShardMotionType.inMotion) // TODO:: FOR REPELLING STATES
                {
                    if (collision.transform.GetComponent<IClimableTAG>() != null)
                    {
                        ClimableWall cliffHeightScript = collision.transform.gameObject.GetComponent<ClimableWall>();

                        float height = cliffHeightScript.Height;

                        shardClifBehavior.CliffHeight = height;

                        StickToWall(in cliffHeightScript);
                    }
                }

                if (collision.transform.GetComponent<IDamagable>() != null)
                {
                    IDamagable damagable = collision.transform.GetComponent<IDamagable>();
                    damagable.TakeDamage(shardSpecification.HitAmount);
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

            if (shardMotionType == ShardMotionType.Stuck) return;

            if (shardMotionType == ShardMotionType.GoingToEnemy)
            {
                // 0.1 for error
                //if (shardSpecification.TargetDetection.TravelToTargetTime <= 5f)
                if (targetDetectionCalculations.TravelToTargetTime <= 2f)
                {
                    //Vector3 targetOffset = new Vector3(EnemyTarget.position.x, EnemyTarget.position.y, EnemyTarget.position.z - 3);

                    //rb.position = CurvePoint.GetBQCPoint(shardSpecification.ReturnAttributes.CurrentPosition, shardSpecification.RadndomDistanceBetweenShardAndTarget, shardSpecification.TargetDetection.TargetPositon, TravelToTargetTime);
                    rb.position = CurvePoint.GetBQCPoint(returningCalculations.CurrentPosition, targetDetectionCalculations.RadndomDistanceBetweenShardAndTarget, targetDetectionCalculations.TargetPositon, targetDetectionCalculations.TravelToTargetTime);

                    targetDetectionCalculations.TravelToTargetTime += shardSpecification.ThrowAttributes.SpeedToTarget * Time.fixedDeltaTime;
                }
                else
                {
                    Debug.LogError("Something Happend, Didint Hit Target");
                    //shardMotionType = ShardMotionType.Returning;
                }
            }
            else if (shardMotionType == ShardMotionType.Repelling)
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

                    shardMotionType = ShardMotionType.Falling;
                }
            }
            else if (shardMotionType == ShardMotionType.inMotion)
            {
                transform.RotateAround(transform.position, shardSpecification.Rotations[0].MotionRotationAxis, shardSpecification.ThrowAttributes.Standard_RotationSpeed);
            }
            else if (shardMotionType == ShardMotionType.Returning)
            {
                if (returningCalculations.ReturningTime < 1f)
                {
                    rb.position = CurvePoint.GetBQCPoint(returningCalculations.CurrentPosition,
                                                         //shardSpecification.ReturnAttributes.CurvePoint[shardSpecification.ReturnAttributes.RandomCurvePointIndex].position,
                                                         CurvePoints[returningCalculations.RandomCurvePointIndex].position,
                                                         sharedParent.position,
                                                         returningCalculations.ReturningTime);
                    //rb.AddTorque(transform.TransformDirection(Vector3.back) * Standard_RotationSpeed , ForceMode.Impulse);

                    if (canBoostPlayer) // TODO:: IF SLOW RETUNR SPEED MIGHT CAUSE AN ISSUE
                    {
                        returningCalculations.ReturningTime += shardSpecification.ReturnAttributes.ReturnBoostSpeed * Time.fixedDeltaTime;
                    }
                    else
                        returningCalculations.ReturningTime += shardSpecification.ReturnAttributes.ReturnSpeed * Time.fixedDeltaTime;
                }
                else ResetShard();
            }
            else if (shardMotionType == ShardMotionType.Falling)
            {

            }
            else
            {
                if (shardMotionType != ShardMotionType.Stuck)
                {
                    transform.RotateAround(transform.position, shardSpecification.Rotations[0].IdelRotationAxis, shardSpecification.ThrowAttributes.Standard_RotationSpeed);
                    //transform.Rotate(rotations[0].IdelRotationAxis.x, rotations[0].IdelRotationAxis.y, rotations[0].IdelRotationAxis.z, Space.World);
                }
            }
        }

        private void OnLevelEnd(bool stop)
        {
            endLevel = stop;

            if (endLevel) Debug.Log("Swithing to Animation");
            else Debug.Log("Swithing To Normal");

            animator.enabled = endLevel;
        }


        private void StickToWall(in ClimableWall cliffHeightScript)
        {
            //isInMotion = false;
            //isStuck = true;
            shardMotionType = ShardMotionType.Stuck;

            rb.isKinematic = true;
            transformCollider.enabled = false;

            Cliff.position = transform.position + new Vector3(-2, 0, 0);
            //Cliff.position = transform.position + cliffHeightScript.CliffCenter;

            Cliff.rotation = Quaternion.Euler(0, 0, 0);

            CliffGameObject.SetActive(true);
        }

        private void ResetShard()
        {
            shardMotionType = ShardMotionType.Idel;

            transform.parent = sharedParent;

            //rb.isKinematic = true;
            returningCalculations.ReturningTime = 0;

            if (canBoostPlayer)
            {
                Action_ShardBoostPlayer?.Invoke();
                Debug.Log("Coming to boost");
            }

            targetDetectionCalculations.TravelToTargetTime = 0;
            timeToRepell = 0;
        }

        private void DetectEnemies() //TODO :: ADD FREQUENCY TO UPDATE MAYBE
        {
            if (shardMotionType != ShardMotionType.Idel) return;


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
                //shardSpecification.EnemyDetection.CurrentEnemy = hit.transform.GetComponent<Enemy>();
                //shardSpecification.EnemyDetection.CurrentEnemy.DetectedColor(shardSpecification.EnemyDetection.DetectedEmenyColor);
                targetDetectionCalculations.Targetable = hit.transform.GetComponent<ITargetable>();

                //if (shardSpecification.EnemyDetection.targetable != null)
                targetDetectionCalculations.Targetable?.DetectedColor(shardSpecification.TargetDetection.DetectedObjectColor);

                targetDetectionCalculations.CurrentHitDistance = hit.distance;

                isEnmeyTrageted = true;
            }
            else
            {
                isEnmeyTrageted = false;

                targetDetectionCalculations.CurrentHitDistance = shardSpecification.TargetDetection.MaxDistance;

                //if (shardSpecification.EnemyDetection.CurrentEnemy != null)
                //    shardSpecification.EnemyDetection.CurrentEnemy.ResetColor();
                //if (shardSpecification.EnemyDetection.targetable != null)
                targetDetectionCalculations.Targetable?.ResetColor();


                //shardSpecification.EnemyDetection.CurrentEnemy = null;
                targetDetectionCalculations.Targetable = null;
            }
        }

        public void Repell()
        {
            Debug.Log("Repelling");
            rb.AddForce(Vector3.up * shardSpecification.RepellAttributes.RepellSpeed, ForceMode.Impulse);
            //isRepelling = true;
            shardMotionType = ShardMotionType.Repelling;
            //GoingToEnemy = false;
        }


        /// <summary>
        /// CAN DO A CALLBACK
        /// AND QUEUE FOR MUTIPLE ACTIONS FOR BONUSES
        /// </summary>
        private void ThrowShard()
        {
            if (isEnmeyTrageted)
            {
                shardMotionType = ShardMotionType.GoingToEnemy;

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
                shardMotionType = ShardMotionType.inMotion;

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


            //shardSpecification.ReturnAttributes.RandomCurvePointIndex = UnityEngine.Random.Range(0, shardSpecification.ReturnAttributes.CurvePoint.Length);

            returningCalculations.GetRandomReturnPoint(CurvePoints.Length);

            returningCalculations.CurrentPosition = transform.position;
            transformCollider.enabled = false;


            shardMotionType = ShardMotionType.Returning;

            rb.isKinematic = true;

            rb.velocity = Vector3.zero;

            CliffGameObject.SetActive(false);

            canBoostPlayer = isboost;
        }


        private void Attack()
        {
            if (shardMotionType == ShardMotionType.Idel)
                ThrowShard();

            else if (shardMotionType != ShardMotionType.Returning)
                RecallShard(false);
        }
        #endregion
    }
}