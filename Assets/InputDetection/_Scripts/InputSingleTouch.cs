using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace InputDetection.Scripts
{
    //Detect TAP, SWIPE, HOLD -- SingleTouch
    [RequireComponent(typeof(LineRenderer))]
    public class InputSingleTouch : MonoBehaviour
    {
        public Camera ViewCamera;

        public Action<Vector2> Tap_Action_WithPosition;
        public Action<Vector2, Vector2> Swipe_Left_Action_WithPosition;
        public Action<Vector2, Vector2> Swipe_Right_Action_WithPosition;
        public Action<Vector2, Vector2> Swipe_Up_Action_WithPosition;
        public Action<Vector2, Vector2> Swipe_Down_Action_WithPosition;
        public Action<Vector2> Hold_Action_WithPosition;

        public Action Tap_Action_WithoutPosition;
        public Action Swipe_Left_Action_WithoutPosition;
        public Action Swipe_Right_Action_WithoutPosition;
        public Action Swipe_Up_Action_WithoutPosition;
        public Action Swipe_Down_Action_WithoutPosition;
        public Action Hold_Action_WithoutPosition;

        public bool touchDebug;

        public List<InputType> ExcludedInput;
        public AllowObliqueInput allowObliqueInput;

        private LineRenderer lineRenderer;
        private GameObject TapVisualize;

        Vector2 mousePositionStart;
        Vector2 mousePositionEnd;
        float mouseHold;
        float deltaMouseX;
        float deltaMouseY;
        bool holding;
        bool swiping;
        bool firstTouch;
        bool hasTapped;

        [SerializeField]
        RectTransform[] CanvasInScene;

        [SerializeField]
        float swipeThresholdX;
        [SerializeField]
        float swipeThresholdY;
        [SerializeField]
        float holdThreshold;

        void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Start()
        {
            ResetMouseData();
            ResetDebugData();
            TapVisualize = (GameObject)Instantiate(Resources.Load("Prefabs/TapPoint", typeof(GameObject)), Vector3.zero, Quaternion.identity);
            if(TapVisualize != null)
            {
                TapVisualize.SetActive(false);
            }
        }

        void Update()
        {
            CanvasInScene = FindObjectsOfType<RectTransform>();

            if (Input.GetMouseButtonDown(0))
            {
                if (firstTouch)
                {
                    ResetDebugData();
                    mousePositionStart = ViewCamera.ScreenToWorldPoint(Input.mousePosition);
                    //Debug.Log("mousePositionStart = "+mousePositionStart);
                    //Debug.Log("Touch down");
                    firstTouch = false;
                }
            }

            if (Input.GetMouseButton(0))
            {
                Debug.ClearDeveloperConsole();
                //Debug.Log("Touch hold");
                mouseHold += Time.deltaTime;
                if (mouseHold > holdThreshold)
                {
                    holding = true;
                    Hold();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (!holding)
                {
                    mousePositionEnd = ViewCamera.ScreenToWorldPoint(Input.mousePosition);
                    //Debug.Log("mousePositionEnd = " + mousePositionEnd);

                    if(touchDebug)
                    {
                        lineRenderer.SetPosition(0, mousePositionStart);
                        lineRenderer.SetPosition(1, mousePositionEnd);
                    }

                    deltaMouseX = mousePositionStart.x - mousePositionEnd.x;
                    deltaMouseY = mousePositionStart.y - mousePositionEnd.y;

                    if(allowObliqueInput.Equals(AllowObliqueInput.Disallow))
                    {
                        float del = Mathf.Abs(deltaMouseX) < Mathf.Abs(deltaMouseY) ? deltaMouseX = 0.0f : deltaMouseY = 0.0f;
                    }


                    if (Mathf.Abs(deltaMouseX) > swipeThresholdX || Mathf.Abs(deltaMouseY) > swipeThresholdY)
                    {
                        swiping = true;

                        if (deltaMouseX < 0)
                        {
                            SwipeRight();
                        }
                        else if(deltaMouseX > 0)
                        {
                            SwipeLeft();
                        }
                        if (deltaMouseY < 0)
                        {
                            SwipeUp();
                        }
                        else if(deltaMouseY > 0)
                        {
                            SwipeDown();
                        }

                    }

                    if (!swiping)
                    {
                        if(touchDebug)
                        {
                            ResetDebugData();
                        }
                        if(ClickedOnEmptySpace())
                        {
                            Tap();
                        }
                    }
                }

                //Debug.Log("Touch Up");
                ResetMouseData();
            }
        }

        void ResetMouseData()
        {
            mousePositionStart = Vector2.zero;
            mousePositionEnd = Vector2.zero;
            mouseHold = 0;
            deltaMouseX = 0;
            deltaMouseY = 0;
            holding = false;
            swiping = false;
            hasTapped = false;

            //swipeThresholdX = 0.5f;
            //swipeThresholdY = 0.5f;
            //holdThreshold = 0.5f;

            firstTouch = true;
        }

        void ResetDebugData()
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
            if (TapVisualize != null)
            {
                TapVisualize.SetActive(false);
            }
        }

        void SwipeUp()
        {
            if(CheckIfInputIsExcluded(InputType.SwipeUp))
            {
                return;
            }

            Debug.Log("SwipeUp");
            if(Swipe_Up_Action_WithPosition != null)
            {
                Swipe_Up_Action_WithPosition.Invoke(mousePositionStart, mousePositionEnd);
            }
            if(Swipe_Up_Action_WithoutPosition != null)
            {
                Swipe_Up_Action_WithoutPosition.Invoke();
            }
        }
        void SwipeDown()
        {
            if (CheckIfInputIsExcluded(InputType.SwipeDown))
            {
                return;
            }

            Debug.Log("SwipeDown");
            if (Swipe_Down_Action_WithPosition != null)
            {
                Swipe_Down_Action_WithPosition.Invoke(mousePositionStart, mousePositionEnd);
            }
            if (Swipe_Down_Action_WithoutPosition != null)
            {
                Swipe_Down_Action_WithoutPosition.Invoke();
            }
        }
        void SwipeLeft()
        {
            if (CheckIfInputIsExcluded(InputType.SwipeLeft))
            {
                return;
            }

            Debug.Log("SwipeLeft");
            if (Swipe_Left_Action_WithPosition != null)
            {
                Swipe_Left_Action_WithPosition.Invoke(mousePositionStart, mousePositionEnd);
            }
            if (Swipe_Left_Action_WithoutPosition != null)
            {
                Swipe_Left_Action_WithoutPosition.Invoke();
            }
        }
        void SwipeRight()
        {
            if (CheckIfInputIsExcluded(InputType.SwipeRight))
            {
                return;
            }

            Debug.Log("SwipeRight");
            if (Swipe_Right_Action_WithPosition != null)
            {
                Swipe_Right_Action_WithPosition.Invoke(mousePositionStart, mousePositionEnd);
            }
            if (Swipe_Right_Action_WithoutPosition != null)
            {
                Swipe_Right_Action_WithoutPosition.Invoke();
            }
        }
        void Tap()
        {
            Vector2 tapPos = Vector2.zero; 
            if(mousePositionEnd.Equals(Vector2.zero))
            {
                tapPos = mousePositionStart;
            }
            else
            {
                tapPos = mousePositionEnd; 
            }

            if(!hasTapped)
            {
                if(touchDebug)
                {
                    TapVisualize.transform.position = tapPos;
                    TapVisualize.SetActive(true);
                }
                
                Debug.Log("Tap");
                if (Tap_Action_WithPosition != null)
                {
                    Tap_Action_WithPosition.Invoke(tapPos);
                }
                if (Tap_Action_WithoutPosition != null)
                {
                    Tap_Action_WithoutPosition.Invoke();
                }
                hasTapped = true;
            }
        }
        void Hold()
        {
            if (CheckIfInputIsExcluded(InputType.Hold))
            {
                return;
            }

            Debug.Log("Hold");
            if (Hold_Action_WithPosition != null)
            {
                Hold_Action_WithPosition.Invoke(mousePositionStart);
            }
            if (Hold_Action_WithoutPosition != null)
            {
                Hold_Action_WithoutPosition.Invoke();
            }
        }

        bool CheckIfInputIsExcluded(InputType type)
        {
            if(ExcludedInput.Contains(type))
            {
                Tap();
                return true;
            }
            else
            {
                return false;
            }
        }

        bool ClickedOnEmptySpace()
        {
            for (int i = 0; i < CanvasInScene.Length; i++)
            {
                if(CanvasInScene[i].GetComponent<Canvas>() != null)
                {
                    //Ignoring Canvas elements
                    continue;
                }
                if(RectTransformUtility.RectangleContainsScreenPoint(CanvasInScene[i], Input.mousePosition, null))
                {
                    Debug.Log("Clicked on UI Element");
                    return false;
                }
            }
            Debug.Log("Clicked on Empty Space");
            return true;
        }
    }

    public enum InputType
    {
        Tap,
        Hold,
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight
    }

    public enum AllowObliqueInput
    {
        Allow,
        Disallow
    }
}