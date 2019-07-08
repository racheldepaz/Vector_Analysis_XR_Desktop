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
        #region View Mode Enum + declaration
        public enum ViewMode : int
        {
            Axis = 0,
            Components,
            Units,
            AxisAngle,
        }

        private ViewMode _viewMode = ViewMode.Axis;
        #endregion

        #region Serialized Variables
        [SerializeField, Tooltip("The controller that is used in the scene to cycle and place objects.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null;

        [SerializeField]
        private Text debugText = null; 

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] placementPoint = null;

        [SerializeField, Tooltip("The placement object not bound to surfaces in the scene.")]
        private GameObject freePoint;

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
            if (index == 0)
            {
                _instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + (transform.forward * magTouchY));
                HandlePlacementFree(beam.GetPosition(1));
            }
            if (index == 1)
            {
                _instructionLabel.text = "Great! Press down on the touchpad if you're ready to place your point. Now, point towards another area and press the trigger again. Press the home button to reset.";
                beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + (transform.forward * magTouchY));
                HandlePlacementFree(beam.GetPosition(1));
            }
            if (debugText.text == null)
                debugText.text = "Now viewing: Components";
            if (index == 2)
                placementComplete = true; 
            if (placementComplete)
            {
                _instructionLabel.text = "Placement complete! Press the bumper to go through different view modes, or hover towards the menu icon to view more information about your vector.";
                VectorVisualizer(content1.transform.position);
            }
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnTriggerUp -= HandleOnTriggerUp;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
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
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }

            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId && button == MLInputControllerButton.Bumper)
            {
                switch (bumperindex)
                {
                    case 0:
                        bumperindex++;
                        debugText.text = "Now viewing: Axes";
                        break; 
                    case 1:
                        bumperindex++;
                        debugText.text = "Now viewing: Unit Vector";
                        break;
                    case 2:
                        bumperindex = 0;
                        debugText.text = "Now viewing: Components";
                        break;
                    default:
                        break;
                }
                //omg. my mind. i love me. 
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
            HandleTouchpadInput();  //add this method if you want to include touchpad input yo gotteeeem 
            switch(index)
            {
                case 0:
                    Destroy(content0);

                    content0 = Instantiate(placementPoint[index], root);

                    Vector3 sourcePos = _controllerConnectionHandler.ConnectedController.Position;
                    Vector3 targetPos = beamPos;

                    content0.transform.position = targetPos;
                    content0.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up);
                    break;
                case 1:
                    Destroy(content1);

                    content1 = Instantiate(placementPoint[index], root);

                    Vector3 sourcePos1 = _controllerConnectionHandler.ConnectedController.Position;
                    Vector3 targetPos1 = beamPos;

                    content1.transform.position = targetPos1;
                    content1.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up);
                    VectorVisualizer(targetPos1);
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
                if (magTouchY <= 0)
                    magTouchY = 0; 
                else if (controller.Touch1PosAndForce.y - lastY > 0.001)
                    magTouchY += pushRate;
                lastY = controller.Touch1PosAndForce.y;

                if (controller.Touch1PosAndForce.x - lastX < -0.001)
                    magTouchX -= rotateRate;
                if (controller.Touch1PosAndForce.x - lastX > 0.001)
                    magTouchX += rotateRate;
                lastX = controller.Touch1PosAndForce.x;
            }
        }
        #endregion
    }
}