using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BarnoGames.Runner2020
{
    public enum SwipeDirection
    {
        NONE = 0,
        LEFT = 1,
        RIGHT = 2,
        UP = 4,
        DOWN = 8
    }

    public class SwipeThree : MonoBehaviour
    {
        private Vector3 touchPosition;
        private float swipeResistanceX = 50f;
        private float swipeResistanceY = 100f;
        public SwipeDirection Direction { get; set; }

        void Update()
        {
            Direction = SwipeDirection.NONE;

            if (Input.GetMouseButtonDown(0))
            {
                touchPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                Vector2 deltaSwipe = touchPosition - Input.mousePosition;

                if(Mathf.Abs(deltaSwipe.x)  > swipeResistanceX)
                {
                    Direction |= (deltaSwipe.x < 0) ? SwipeDirection.RIGHT : SwipeDirection.LEFT;
                }

                if(Mathf.Abs(deltaSwipe.y) > swipeResistanceY)
                {
                    Direction |= (deltaSwipe.y < 0) ? SwipeDirection.UP: SwipeDirection.DOWN;

                }
            }
        }


    }
}