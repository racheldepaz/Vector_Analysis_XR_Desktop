using System; 
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>                    
    /// This class allows the user to place their desired object at a valid location.
    /// </summary>
    [RequireComponent(typeof(Placement))]
    [RequireComponent(typeof(VectorMath))]
    public class _Placement : MonoBehaviour
    {
        #region Public Variables
        public GameObject menuPanel;
        public GameObject regularCanvas;
        public float measurementFactor = 1f;
        public Text debug = null; 
        #endregion

        #region Serialized Variables
        [SerializeField, Tooltip("The controller that is used in the scene to cycle and place objects.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null;

        [SerializeField, Tooltip("The Text element that will display the displayed component")]
        private Text _viewLabel = null; 

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] placementPoint = null;

        [SerializeField, Tooltip("Where you want to drop all the instantiated elements of the visualization")]
        private Transform root;

        [Tooltip("speed to push/pull objects using trackpad - meters per second")]
        public float pushRate;

        [Tooltip("rotate transform around Y axis if grabbed - angles per second")]
        public float rotateRate;
        #endregion

        #region Private Variables
        private MLInputControllerFeedbackColorLED _color;

        private LineRenderer beam;

        private int index;
        private int bumperindex; 

        private float lastY, lastX, magTouchY, magTouchX; //values to compare touchpad swipe x,y pos

        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;
        private VectorMath _vectorMath = null;
        private ChangeViewModes modes = null; 

        //Stuff I need globally
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject content0, content1; //save location of the origin, placed point

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
            bumperindex = 0; 


            placementComplete = false;
            inPlacementState = true; 
            bumperFirstPress = true; 
            menuActive = false;

            regularCanvas.SetActive(true);
            menuPanel.SetActive(false);

            _placement = GetComponent<Placement>();
            _vectorMath = GetComponent<VectorMath>();
            modes = GetComponent<ChangeViewModes>(); 

            if (pushRate == 0)
            {
                pushRate = 1;
            }
            if (rotateRate == 0)
            {
                rotateRate = 180;
            }

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
                    placementComplete = true;

                if (placementComplete)
                {
                    _instructionLabel.text = "Placement complete! Press the bumper to go through different view modes, or press the home button to toggle the main menu.";
                    VectorVisualizer(content1.transform.position);
                }
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

                switch (bumperindex)                 //omg. my mind. i love me. 
                {
                    case 0:
                        bumperindex++;
                        _viewLabel.text = "Now viewing: Axes";
                        break;
                    case 1:
                        bumperindex++;
                        _viewLabel.text = "Now viewing: Unit Vectors ";
                        break;
                    case 2:
                        bumperindex = 0;
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
            _vectorMath.VectorComponents(newPlacement, content0.transform, bumperindex);
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

                    //debug.text = "content 0 pos: " + targetPos.ToString(); 

                    content0.transform.position = targetPos;
                    content0.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up);

<<<<<<< HEAD
                    Destroy(content1);
                    Instantiate(placementPoint[index], root);
                    content1.transform.position = content0.transform.position;
                    content1.transform.rotation = content0.transform.rotation;      
=======
                   // debug.text += "content 0 rot: " + content0.transform.rotation.ToString(); 
                    
>>>>>>> parent of 3cbb09e... Additional edits to add the vector arcs
                    break;
               
                case 1:
                    Destroy(content1);

                    content1 = Instantiate(placementPoint[index], root);
   

                    Vector3 sourcePos1 = _controllerConnectionHandler.ConnectedController.Position;
                    Vector3 targetPos1 = beamPos;
                    content1.transform.position = targetPos1;

<<<<<<< HEAD
                    VectorVisualizer(content1.transform.position);

=======
                   //content1.transform.LookAt(content0.transform, Vector3.forward);

                    Quaternion q;
                    Vector3 a = Vector3.Cross(content0.transform.position, content1.transform.position);
                    q.x = a.x; q.y = a.y; q.z = a.z;

                    double d = content0.transform.position.sqrMagnitude * content0.transform.position.sqrMagnitude;
                    double dd = content1.transform.position.sqrMagnitude * content1.transform.position.sqrMagnitude;
                    float f = Vector3.Dot(content0.transform.position, content1.transform.position);
                    float ff = (float)Math.Sqrt(d * dd);

                    q.w = f + ff;



                    // Vector3 v = content1.transform.position - content0.transform.position;
                    // ..v.Normalize();

                    //debug.text = "content1 pos: " + targetPos1 + "content1 rot: " + content1.transform.rotation; 
                    content1.transform.rotation = q; 
                    debug.text = "content1 pos: " + targetPos1 + "content1 rot: " + content1.transform.rotation.ToString("N3");
                    //transform.rotation * Quaternion.Euler(Vector3.up);

                    VectorVisualizer(content1.transform.position);
>>>>>>> parent of 3cbb09e... Additional edits to add the vector arcs
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