using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class Swipe : MonoBehaviour
    {
        [SerializeField] private float daadZone = 100f;
        [SerializeField] private float doubleTapDelta = 0.5f;

        private bool _tap;
        private bool _hold;
        private bool _doubleTap;
        private bool _swipeLeft;
        private bool _swipeRight;
        private bool _swipeUp;
        private bool _swipeDown;

        private float lastTap;
        private float sqrDeadZone;
        private Vector2 startTouch;
        private Vector2 _swipeDelta;

        #region Properties
        public Vector2 SwipeDelta => _swipeDelta;
        public bool Tap => _tap;
        public bool DoubleTap => _doubleTap;
        public bool Hold => _hold;
        public bool SwipeLeft => _swipeLeft;
        public bool SwipeRight => _swipeRight;
        public bool SwipeUp => _swipeUp;
        public bool SwipeDown => _swipeDown;
        #endregion

        #region Unity Callbacks

        private void Start()
        {
            sqrDeadZone = daadZone * daadZone;
        }

        private void Update()
        {
            _tap = _doubleTap = _swipeLeft = _swipeRight = _swipeUp = _swipeDown = false;

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
                _tap = true;
                _hold = true;
                startTouch = Input.mousePosition;
                _doubleTap = Time.time - lastTap < doubleTapDelta;

                if (_doubleTap) Debug.Log("DoubleTap");

                lastTap = Time.time;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _hold = false;
                startTouch = _swipeDelta = Vector2.zero;
            }

            //REST DISTANCE, GET THE NEW SWIPEDELTA
            _swipeDelta = Vector2.zero;

            if (startTouch != Vector2.zero && Input.GetMouseButton(0))
                _swipeDelta = (Vector2)Input.mousePosition - startTouch;

            if (_swipeDelta.sqrMagnitude > sqrDeadZone)
            {
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    //LEFT OR RIGHT
                    if (x < 0) _swipeLeft = true;
                    else _swipeRight = true;
                }
                else
                {
                    // UP OR DOWN
                    if (y < 0) _swipeDown = true;
                    else _swipeUp = true;
                }

                startTouch = _swipeDelta = Vector2.zero;
            }
        }

        private void UpdateMobile()
        {
            //if (Input.touches.Length != 0)
            if (Input.touchCount != 0)
            {
                if (Input.touches[0].phase == TouchPhase.Began)
                {
                    _tap = true;
                    _hold = true;
                    startTouch = Input.mousePosition;
                    //startTouch = Input.touches[0].position;
                    _doubleTap = Time.time - lastTap < doubleTapDelta;
                    lastTap = Time.time;
                }
                else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
                {
                    _hold = false;
                    startTouch = _swipeDelta = Vector2.zero;
                }
            }

            // RESET DISTANCE, CALCULATE NEW ONE
            _swipeDelta = Vector2.zero;

            if (startTouch != Vector2.zero && Input.touches.Length != 0)
                _swipeDelta = Input.touches[0].position - startTouch;

            if (_swipeDelta.sqrMagnitude > sqrDeadZone)
            {
                float x = _swipeDelta.x;
                float y = _swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    //LEFT OR RIGHT
                    if (x < 0) _swipeLeft = true;
                    else _swipeRight = true;
                }
                else
                {
                    // UP OR DOWN
                    if (y < 0) _swipeDown = true;
                    else _swipeUp = true;
                }

                startTouch = _swipeDelta = Vector2.zero;
            }
        }
    }
}