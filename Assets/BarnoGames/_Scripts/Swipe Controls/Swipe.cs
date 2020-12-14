using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class Swipe : MonoBehaviour
    {
        [SerializeField] private float daadZone = 100f;
        [SerializeField] private float doubleTapDelta = 0.5f;

        private bool tap;
        private bool hold;
        private bool doubleTap;
        private bool swipeLeft;
        private bool swipeRight;
        private bool swipeUp;
        private bool swipeDown;

        private float lastTap;
        private float sqrDeadZone;
        private Vector2 startTouch;
        private Vector2 swipeDelta;

        #region Properties
        public Vector2 SwipeDelta => swipeDelta;
        public bool Tap => tap;
        public bool DoubleTap => doubleTap;
        public bool Hold => hold;
        public bool SwipeLeft => swipeLeft;
        public bool SwipeRight => swipeRight;
        public bool SwipeUp => swipeUp;
        public bool SwipeDown => swipeDown;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            sqrDeadZone = daadZone * daadZone;
        }

        private void Update()
        {
            tap = doubleTap = swipeLeft = swipeRight = swipeUp = swipeDown = false;

#if UNITY_EDITOR
            UpdateStandAlone();
#endif
#if UNITY_ANDROID
            UpdateMobile();
#endif
        }

        #endregion

        private void UpdateStandAlone()
        {
            if (Input.GetMouseButtonDown(0))
            {
                tap = true;
                hold = true;
                startTouch = Input.mousePosition;
                doubleTap = Time.time - lastTap < doubleTapDelta;

                if (doubleTap) Debug.Log("DoubleTap");

                lastTap = Time.time;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                hold = false;
                startTouch = swipeDelta = Vector2.zero;
            }

            //REST DISTANCE, GET THE NEW SWIPEDELTA
            swipeDelta = Vector2.zero;

            if (startTouch != Vector2.zero && Input.GetMouseButton(0))
                swipeDelta = (Vector2)Input.mousePosition - startTouch;

            if (swipeDelta.sqrMagnitude > sqrDeadZone)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    //LEFT OR RIGHT
                    if (x < 0) swipeLeft = true;
                    else swipeRight = true;
                }
                else
                {
                    // UP OR DOWN
                    if (y < 0) swipeDown = true;
                    else swipeUp = true;
                }

                startTouch = swipeDelta = Vector2.zero;
            }
        }

        private void UpdateMobile()
        {
            //if (Input.touches.Length != 0)
            if (Input.touchCount != 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    tap = true;
                    hold = true;
                    startTouch = Input.mousePosition;
                    //startTouch = Input.touches[0].position;
                    doubleTap = Time.time - lastTap < doubleTapDelta;
                    lastTap = Time.time;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    hold = false;
                    startTouch = swipeDelta = Vector2.zero;
                }
            }

            // RESET DISTANCE, CALCULATE NEW ONE
            swipeDelta = Vector2.zero;

            if (startTouch != Vector2.zero && Input.touches.Length != 0)
                swipeDelta = Input.touches[0].position - startTouch;

            if (swipeDelta.sqrMagnitude > sqrDeadZone)
            {
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    //LEFT OR RIGHT
                    if (x < 0) swipeLeft = true;
                    else swipeRight = true;
                }
                else
                {
                    // UP OR DOWN
                    if (y < 0) swipeDown = true;
                    else swipeUp = true;
                }

                startTouch = swipeDelta = Vector2.zero;
            }
        }
    }
}