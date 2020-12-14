using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public class SwipeTwo : MonoBehaviour
    {
        public float maxSwipeTime;
        public float minSwipeDistance;

        private float swipeStartTime;
        private float swipeEndTime;
        private float swipeTime;


        private Vector2 startSwipePosition;
        private Vector2 endSwipePosition;
        private float swipeLength;

        void Start()
        {
        
        }

        void Update()
        {
            SwipeTest();        
        }

        void SwipeTest()
        {
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if(touch.phase == TouchPhase.Began)
                {
                    swipeStartTime = Time.time;
                    startSwipePosition = touch.position;
                }
                else if(touch.phase == TouchPhase.Ended /*|| touch.phase == TouchPhase.Canceled*/)
                {
                    swipeEndTime = Time.time;
                    endSwipePosition = touch.position;

                    swipeTime = swipeEndTime - swipeStartTime;

                    swipeLength = (endSwipePosition - startSwipePosition).magnitude;

                    if(swipeTime <maxSwipeTime && swipeLength > minSwipeDistance)
                    {
                        SwipeControl();
                    }
                }
            }
        }

        void SwipeControl()
        {
            Vector2 Distance = endSwipePosition - startSwipePosition;

            float xDistance = Mathf.Abs(Distance.x);
            float yDistance = Mathf.Abs(Distance.y);

            if(xDistance > yDistance)
            {

            }

        }

    }
}