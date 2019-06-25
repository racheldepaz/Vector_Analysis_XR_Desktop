using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

/// <summary>
/// Control6DOF 
/// Pointer system used from Les Bird. 
/// </summary>
public class Control6DOF : MonoBehaviour
{
    #region Public Variables
    [Tooltip("1.0 = exact, 0.99 = near - this is a dot product result")]
    public float pointerAccuracy;
    [Tooltip("speed to push/pull objects using trackpad - meters per second")]
    public float pushRate;
    [Tooltip("rotate object around Y axis if grabbed - angles per second")]
    public float rotateRate;
    [Tooltip("scale rate - meters per second")]
    public float scaleRate;

    public GameObject grabbedObject;
    #endregion

    #region Private Variables
    [SerializeField, Tooltip("Controller asset")]
    private ControllerConnectionHandler _controller;
    [SerializeField]
    private Text gestureStateText = null;
    [SerializeField]
    private LineRenderer beam;

    //distance from pointer source to the grabbed object
    private float grabbedDist;

    //flags
    private bool triggerIsDown;
    private bool lastTriggerWasUp;
    private float trackPadHor;
    private float trackPadVer;

    //delegates

    // trigger is held down
    public delegate void TriggerDownCallback();
    public TriggerDownCallback OnTriggerDown;
    // trigger is released
    public delegate void TriggerUpCallback();
    public TriggerUpCallback OnTriggerUp;
    //trigger is clicked
    public delegate void TriggerClickedCallback();
    public TriggerClickedCallback OnTriggerClicked;
    #endregion

    #region Event Handlers
    void HandleOnTriggerDown(byte idx, float v)
    {
        triggerIsDown = true;
        if (OnTriggerDown != null)
        {
            OnTriggerDown();
        }
    }

    void HandleOnTriggerUp(byte idx, float v)
    {
        triggerIsDown = false;
        if (OnTriggerUp != null)
        {
            OnTriggerUp();
        }
    }


    #endregion

    #region Unity Methods
    void Start()
    {
        //Start receiving input by the Control
        MLInput.Start();
        if (_controller == null)
        {
            Debug.LogError("Error: PlacementExample._controllerConnectionHandler is not set, disabling script.");
            enabled = false;
            return;
        }

        if (MagicLeapDevice.IsReady())
        {
            MLInput.OnTriggerDown += HandleOnTriggerDown;
            MLInput.OnTriggerUp += HandleOnTriggerUp;
        }

        //how forgiving it is
        if (pointerAccuracy == 0)
        {
            pointerAccuracy = 0.995f;
        }
        if (pushRate == 0)
        {
            pushRate = 1;
        }
        if (rotateRate == 0)
        {
            rotateRate = 180;
        }
        if (scaleRate == 0)
        {
            scaleRate = 1;
        }
    }

    void OnDestroy()
    {
        //Stop receiving input by the Control
        MLInput.Stop();

        //handle the trigger stuff
        MLInput.OnTriggerDown -= HandleOnTriggerDown;
        MLInput.OnTriggerUp -= HandleOnTriggerUp;
    }

    void Update()
    {
        //Attach the Beam GameObject to the Control
        transform.position = _controller.ConnectedController.Position;
        transform.rotation = _controller.ConnectedController.Orientation;

        Vector3 sourcePos = transform.position;
        Vector3 targetPos = sourcePos + transform.forward;

        bool triggerClick = (lastTriggerWasUp & triggerIsDown) ? true : false;

        bool triggerClicked = (lastTriggerWasUp && triggerIsDown) ? true : false;

        if (triggerClicked)
        {
            if (OnTriggerClicked != null)
            {
                OnTriggerClicked();
            }
        }

        if (grabbedObject != null)
        {
            if (trackPadVer != 0)
            {
                // push grabbed object in/out
                grabbedDist += trackPadVer * Time.deltaTime;
            }

            //move the object attached
            targetPos = sourcePos + (transform.forward * grabbedDist);

            grabbedObject.transform.position = targetPos;

            beam.SetPosition(0, transform.position);
            beam.SetPosition(1, targetPos);

            lastTriggerWasUp = (triggerIsDown ? false : true);
        }
    }
    #endregion

    #region Private Methods
    void HandleMLControllers()
    {
        trackPadVer = 0;
        if (_controller.ConnectedController != null)
        {
            if (_controller.IsControllerValid())
            {
                if (_controller.ConnectedController.Touch1Active && _controller.ConnectedController.Touch1PosAndForce.z < -.2f)
                {
                    if (_controller.ConnectedController.Touch1PosAndForce.x > 0.2f || _controller.ConnectedController.Touch1PosAndForce.y < -0.2f)
                    {
                        trackPadHor = _controller.ConnectedController.Touch1PosAndForce.x;
                    }

                    if (_controller.ConnectedController.Touch1PosAndForce.y > 0.2f || _controller.ConnectedController.Touch1PosAndForce.y < -0.2f)
                    {
                        trackPadVer = _controller.ConnectedController.Touch1PosAndForce.y;
                    }
                }
            }
        }
    }

        void updateTouchpadGesture()
        {
            gestureStateText.text = "State: " + _controller.ConnectedController.TouchpadGesture.Type.ToString() + " " + _controller.ConnectedController.TouchpadGestureState.ToString() + " " + _controller.ConnectedController.TouchpadGesture.Direction;
            //if swiping up, touchpad is purple
            if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
                && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.Start
                && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
            {
                _controller.ConnectedController.StartFeedbackPatternLED(MLInputControllerFeedbackPatternLED.Clock12, MLInputControllerFeedbackColorLED.PastelCosmicPurple, 1f);
                beam.transform.localScale += new Vector3(0, 0, .003f); //grow the beam
                Debug.Log("swiping up start");
            }

            //if swiping up and the gesture has ended, stop the LED
            else if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
                    && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.Continue
                    && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
            {
                _controller.ConnectedController.StartFeedbackPatternLED(MLInputControllerFeedbackPatternLED.Clock12, MLInputControllerFeedbackColorLED.PastelCosmicPurple, 1f);
                beam.transform.localScale += new Vector3(0, 0, .003f); //grow the beam
                Debug.Log("swiping up cont");
            }

            else if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
            && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.End
            && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Up)
            {
                _controller.ConnectedController.StopFeedbackPatternLED();
                Debug.Log("swiping up end");
            }

            //if swiping down, touchpad is yellow
            else if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
                    && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.Start
                    && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
            {
                _controller.ConnectedController.StartFeedbackPatternLED(MLInputControllerFeedbackPatternLED.Clock6, MLInputControllerFeedbackColorLED.BrightLunaYellow, 1f);
                beam.transform.localScale -= new Vector3(0, 0, 0.003f);
                Debug.Log("swiping down start");
            }

            else if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
                    && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.Start
                    && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
            {
                _controller.ConnectedController.StartFeedbackPatternLED(MLInputControllerFeedbackPatternLED.Clock6, MLInputControllerFeedbackColorLED.BrightLunaYellow, 1f);
                beam.transform.localScale -= new Vector3(0, 0, 0.003f);
                Debug.Log("swiping down continue");
            }

            else if (_controller.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe
                    && _controller.ConnectedController.TouchpadGestureState == MLInputControllerTouchpadGestureState.Start
                    && _controller.ConnectedController.TouchpadGesture.Direction == MLInputControllerTouchpadGestureDirection.Down)
            {
                _controller.ConnectedController.StopFeedbackPatternLED();
                Debug.Log("swiping up end");
            }
        }
    #endregion
}