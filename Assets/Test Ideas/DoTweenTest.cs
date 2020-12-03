using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

using TEST_TEXT = TMPro.TMP_Text;

namespace BarnoGames.Runner2020
{
    public enum TweenTy
    {
        Move, Path,
        Shake,
        Jump
    }
    public class DoTweenTest : MonoBehaviour
    {
        public TweenTy tweenTy;

        [Header("Move")]
        private Transform cubeTransform;
        private Rigidbody rb;
        [SerializeField] private float animationDuration;
        [SerializeField] private Ease animEase;

        [Space(10)]
        [Header("Path")]
        [SerializeField] private float duration;
        [SerializeField] private PathType pathType;
        [SerializeField] private PathMode pathMode;
        [SerializeField] Transform[] path;
        Vector3[] paths;

        [Space(10)]
        [Header("Shake")]
        [SerializeField] private float ShakeDuration = 3f;
        [SerializeField] private float strength = 1f;
        [SerializeField] private Vector3 rotationStrength = new Vector3(2,3,1);
        [SerializeField] private int vibrato = 10;
        [SerializeField] private float randomness = 90f;

        [Space(10)]
        [Header("Jump")]
        [SerializeField] private Vector3 endPoint = new Vector3(1, 4, 2);
        [SerializeField] private float JumpPower = 5f;
        [SerializeField] private int NumberOfjUMPS = 1;
        [SerializeField] private float jumpDURATION = 5f;
        void Start()
        {
            cubeTransform = transform;
            rb = GetComponent<Rigidbody>();

            if (tweenTy == TweenTy.Move)
            {
                cubeTransform.DOMoveY(3f, animationDuration)
                    .SetEase(animEase)
                    .SetLoops(-1, LoopType.Yoyo)
                    .OnStepComplete(() => { Debug.Log("hI"); });
                //.From(5f);
                //.SetDelay(3f);
            }
            else if (tweenTy == TweenTy.Path)
            {
                paths = new Vector3[path.Length];

                for (int i = 0; i < path.Length; i++)
                {
                    paths[i] = path[i].position;
                }

                cubeTransform.DOPath(paths, duration, pathType, pathMode, 10, Color.red);
            }
            else if (tweenTy == TweenTy.Shake)
            {
                Sequence seq = DOTween.Sequence();

                seq
                    .Append(cubeTransform.DOShakePosition(ShakeDuration, strength, vibrato, randomness, false, true))
                    .AppendInterval(2f)
                    .Append(cubeTransform.DOShakeRotation(ShakeDuration, rotationStrength, vibrato, randomness, true))
                    .AppendInterval(2f)
                    .Append(cubeTransform.DOShakeScale(ShakeDuration, strength, vibrato, randomness, true))
                    .Join(cubeTransform.DOShakeRotation(ShakeDuration, rotationStrength, vibrato, randomness, true));

                //seq.Insert(11f, cubeTransform.DOShakeRotation(ShakeDuration, rotationStrength, vibrato, randomness, true));
            }
            //else if (tweenTy == TweenTy.Jump)
            //{
            //    rb.DOJump( endPoint, JumpPower, NumberOfjUMPS, jumpDURATION, false);
            //}
            //DoTweenTest
        }

        private void FixedUpdate()
        {
                endPoint = new Vector3(rb.velocity.x, 0, endPoint.z);
            if(Input.GetKeyDown(KeyCode.Space))
            {
                transform.DOJump(endPoint, JumpPower, NumberOfjUMPS, jumpDURATION, false);
            }

            rb.velocity = new Vector3(1.5f, rb.velocity.y, rb.velocity.z);
        }

        [SerializeField]
        private TMP_Text text;
    }
}