using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gestures : MonoBehaviour
{
    [HideInInspector]
    public bool swipeLeft, swipeRight, swipeUp, swipeDown = false;

    Vector2 touchStartPos;

    float minSwipePixelsDistance = 100.0f;
    bool touchStarted = false;

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStarted = true;
                    touchStartPos = touch.position;

                    swipeLeft
                       = swipeRight
                       = swipeUp
                       = swipeDown
                       = false;
                       break;
                case TouchPhase.Ended:
                    if (touchStarted)
                    {
                        Swipe(touch);
                        touchStarted = false;
                    }
                    break;
                case TouchPhase.Canceled:
                    touchStarted = false;
                    break;
            }
        }
    }

    void Swipe(Touch touch)
    {
        Vector2 touchLastPos = touch.position;
        float distance = Vector2.Distance(touchLastPos, touchStartPos);

        if(distance > minSwipePixelsDistance)
        {
            float dy = touchLastPos.y - touchStartPos.y;
            float dx = touchLastPos.x - touchStartPos.x;

            float swipeeAngle = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
            swipeeAngle = (swipeeAngle + 360) % 360;

            if(swipeeAngle < 50 || swipeeAngle > 350)
            {
                swipeRight = true;
                //Debug.Log("Right");
            }
            else if (swipeeAngle < 150)
            {
                swipeUp = true;
                //Debug.Log("Up");
            }
            else if (swipeeAngle < 190)
            {
                swipeLeft = true;
                //Debug.Log("Left");
            }
            else if (swipeeAngle > 190)
            {
                swipeDown = true;
               // Debug.Log("Down");
            }
        }
    }
}
