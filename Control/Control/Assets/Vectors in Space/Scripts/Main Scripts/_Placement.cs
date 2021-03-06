﻿using System; 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>                    
    /// This class allows the user to place their desired object at a valid location.
    /// </summary>
    [RequireComponent(typeof(VectorMath))]
    public class _Placement : MonoBehaviour
    {
        #region Public Variables
        public GameObject menuPanel;
        public GameObject regularCanvas;
        //public Text debug = null; 
        #endregion

        #region Serialized Variables
        [SerializeField, Tooltip("The controller that is used in the scene to cycle and place objects.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null;

        [SerializeField, Tooltip("The Text element that will display the displayed component")]
        private Text _viewLabel = null; 

        [SerializeField, Tooltip("The placement object used in the scene. Object 0: Origin, Object 1: Point")]
        private GameObject[] placementPoint = null;

        [SerializeField, Tooltip("Where you want to drop all the instantiated elements of the visualization")]
        private Transform root;

        [Tooltip("speed to push/pull objects using trackpad - meters per second")]
        public float pushRate;

        //Remove these commnents if you want to make it so that the objects are rotatable
        //[Tooltip("rotate transform around Y axis if grabbed - angles per second")]
        //public float rotateRate;

        private MLInputControllerFeedbackColorLED _color;

        private LineRenderer beam;

        private int index; //denotes what placement mode you are on. index = 0, placing the origin; index = 1, placing the point; index = 2, point placed
        private int bumperIndex; //denotes what visualization mode you are on. bumperIndex = 0, components, bumperIndex = 2, unit components

        private float lastY, lastX; //last (x, y) pos on touchpad
        private float magTouchX, magTouchY; //multipliers for the length of the raycast on the controller 

        private PlacementObject _placementObject = null;
        private VectorMath _vectorMath = null;
        private ChangeViewModes modes = null;

        //Stuff I need globally
        private GameObject content0, content1, content2; //save location of the origin, placed point

        // flags and controller variables
        private bool placementComplete;
        private bool inPlacementState; 
        private bool bumperFirstPress;
        private bool menuActive;
        #endregion

        #region Unity Methods
        void Start()
        {
            MLInput.Start();
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: _Placement._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }

            index = 0;
            bumperIndex = 0; 


            placementComplete = false;
            inPlacementState = true; 
            bumperFirstPress = true; 
            menuActive = false;

            regularCanvas.SetActive(true);
            menuPanel.SetActive(false);

            _vectorMath = GetComponent<VectorMath>();
            modes = GetComponent<ChangeViewModes>(); 

            if (pushRate == 0)
            {
                pushRate = 1;
            }

            //if (rotateRate == 0)
            //{
            //    rotateRate = 180;
            //}

            magTouchX = 1; magTouchY = 1;

            beam = GetComponent<LineRenderer>();


            MLInput.OnTriggerDown += HandleOnTriggerDown;
            MLInput.OnTriggerUp += HandleOnTriggerUp;
            MLInput.OnControllerButtonDown += HandleOnButtonDown;

            HandlePlacementFree(_controllerConnectionHandler.ConnectedController.Position + transform.forward);
        }

        void Update()
        {
            beam.SetPosition(0, _controllerConnectionHandler.ConnectedController.Position);
            beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + transform.forward);
            HandleTouchpadInput();

            if (inPlacementState)
            {
                if (index == 0)
                {
                    _instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                    beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + (transform.forward * magTouchY));
                    HandlePlacementFree(beam.GetPosition(1));
                }

                if (index == 1)
                {
                    _instructionLabel.text = "Great! Press the trigger again to place another point. Press the home button to toggle the main menu.";
                    beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + (transform.forward * magTouchY));
                    HandlePlacementFree(beam.GetPosition(1));

                    if (bumperFirstPress)
                    {
                        _viewLabel.text = "Now viewing: Components";
                    }
                }

                if (index == 2)
                { placementComplete = true; inPlacementState = false; }
            }

            if (placementComplete)
            {
                _instructionLabel.text = "";
                VectorVisualizer(content1.transform.position);
            }
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnTriggerUp -= HandleOnTriggerUp;
        }
        #endregion

        #region Event Handlers
        

        /// <summary>
        /// If the trigger is pressed:
        ///     -If you're placing the origin, make sure the placement you're targetting with the raycast is a plane. 
        ///     -If you're placing the first free point, move to the next condition. 
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="pressure"></param>
        private void HandleOnTriggerDown(byte controllerId, float pressure)
        {
            _controllerConnectionHandler.ConnectedController.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.High);
            if(inPlacementState)
                index++;
        }

        private void HandleOnTriggerUp(byte controllerId, float pressure)
        {
            _controllerConnectionHandler.ConnectedController.StartFeedbackPatternEffectLED(MLInputControllerFeedbackEffectLED.PaintCW, MLInputControllerFeedbackEffectSpeedLED.Fast, MLInputControllerFeedbackPatternLED.Clock6And12, MLInputControllerFeedbackColorLED.BrightCosmicPurple, 1.5f);
        }

        /// <summary>
        /// If the home button is pressed, reload this scene. If the bumper is pressed, EMPTY. 
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="button"></param>
        private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id
                == controllerId && button == MLInputControllerButton.HomeTap)
            {
                if (menuActive == true)
                {
                    menuActive = false;
                    menuPanel.SetActive(false);
                    inPlacementState = true;
                    regularCanvas.SetActive(true);
                }
                else if (menuActive == false)
                {
                    menuActive = true;
                    menuPanel.SetActive(true);
                    inPlacementState = false;
                    regularCanvas.SetActive(false);
                 }
             }

            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId && button == MLInputControllerButton.Bumper)
            {
                if (bumperFirstPress == true)
                    bumperFirstPress = false;

                switch (bumperIndex)                 //omg. my mind. i love me. 
                {
                    case 0:
                        bumperIndex++;
                        _viewLabel.text = "Now viewing: Axes";
                        break;
                    case 1:
                        bumperIndex++;
                        _viewLabel.text = "Now viewing: Unit Vectors ";
                        break;
                    case 2:
                        bumperIndex = 0;
                        _viewLabel.text = "Now viewing: Components";
                        break;
                    default:
                        Debug.Log("uh. theres a mistake in ur bumper loop");
                        break;
                }

            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Given the vector3 position, show the vector components.
        /// </summary>
        /// <param name="newPlacement">The vector position</param>
        private void VectorVisualizer(Vector3 newPlacement)
        {
            _vectorMath.VectorComponents(newPlacement, content0.transform, bumperIndex);
        }

        private void HandlePlacementFree(Vector3 beamPos)
        {
            HandleTouchpadInput();
            switch(index)
            {
                case 0:
                    Destroy(content0);

                    content0 = Instantiate(placementPoint[index], root);

                    Vector3 sourcePos = _controllerConnectionHandler.ConnectedController.Position;
                    Vector3 targetPos = beamPos;


                    content0.transform.position = targetPos;
                    content0.transform.rotation = Quaternion.Euler(Vector3.up);
                    break;
                case 1:
                    Destroy(content1);

                    content1 = Instantiate(placementPoint[index], root);

                    Vector3 sourcePos1 = _controllerConnectionHandler.ConnectedController.Position;
                    Vector3 targetPos1 = beamPos;

                    content1.transform.position = targetPos1;
                    VectorVisualizer(content1.transform.position);  
                    break;
                default:
                    VectorVisualizer(content1.transform.position);
                    break;
            }
            
        }

        //thx Ryan!!!
        private void HandleTouchpadInput()
        {
            if (!_controllerConnectionHandler.IsControllerValid())
            {
                Debug.LogError("Error: Invalid controller detected. MLA is not currently supported as an input source.");
                return;
            }

            MLInputController controller = _controllerConnectionHandler.ConnectedController;
            if (controller.Touch1Active)
            {
                if (controller.Touch1PosAndForce.y - lastY < -0.001)
                    magTouchY -= pushRate;
                else if (controller.Touch1PosAndForce.y - lastY > 0.001)
                    magTouchY += pushRate;
                lastY = controller.Touch1PosAndForce.y;

                
            }
        }
        #endregion
    }
}