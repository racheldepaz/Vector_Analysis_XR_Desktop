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

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject _placementPrefab = null;

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

        private float lastY, lastX, magTouchY, magTouchX; //values to compare touchpad swipe x,y pos

        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;
        private VectorMath _vectorMath = null;

        //Stuff I need globally
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject content0, content1, content2, savedGrid; //origin, point 1, point 2

        private GameObject previewFree;

        // flags and controller variables
        private bool triggerIsDown;
        private bool lastTriggerWasUp;
        #endregion

        #region Delegates
        // trigger is held down
        public delegate void TriggerDownCallback();
        public TriggerDownCallback OnTriggerDown;
        // trigger is released
        public delegate void TriggerUpCallback();
        public TriggerUpCallback OnTriggerUp;
        // trigger was clicked (not pressed to pressed)
        public delegate void TriggerClicked();
        public TriggerClicked OnTriggerClicked;
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

            _placement = GetComponent<Placement>();
            _vectorMath = GetComponent<VectorMath>();

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

            StartPlacement();
        }

        void Update()
        {
            beam.SetPosition(0, _controllerConnectionHandler.ConnectedController.Position);
            beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + transform.forward);
            if (index == 0)
            {
                //_instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                _placementObject.transform.position = _placement.AdjustedPosition - _placementObject.LocalBounds.center;
                _placementObject.transform.rotation = _placement.Rotation;
                beam.SetPosition(1, _placementObject.transform.position);
            }
            if (index == 1)
            {
                //_instructionLabel.text = "Great! Press down on the touchpad if you're ready to place your point. Now, point towards another area and press the trigger again. Press the home button to reset.";
                beam.SetPosition(1, _controllerConnectionHandler.ConnectedController.Position + (transform.forward * magTouchY));
                HandlePlacementFree(beam.GetPosition(1));
            }
            _instructionLabel.text = index.ToString();
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnTriggerUp -= HandleOnTriggerUp;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
        }
        #endregion

        #region Event Handlers
        private void HandlePlacementFree(Vector3 beamPos)
        {
            HandleTouchpadInput();  //add this method if you want to include touchpad input
            Destroy(previewFree);

            previewFree = Instantiate(freePoint, root);

            Vector3 sourcePos = _controllerConnectionHandler.ConnectedController.Position;
            Vector3 targetPos = beamPos;

            previewFree.transform.position = targetPos;
            previewFree.transform.rotation = transform.rotation * Quaternion.Euler(Vector3.up);
            VectorVisualizer(targetPos);
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

                if (controller.Touch1PosAndForce.x - lastX < -0.001)
                    magTouchX -= rotateRate;
                if (controller.Touch1PosAndForce.x - lastX > 0.001)
                    magTouchX += rotateRate;
                lastX = controller.Touch1PosAndForce.x;
            }
        }

        /// <summary>
        /// If the trigger is pressed:
        ///     -If you're placing the origin, make sure the placement you're targetting with the raycast is a plane. 
        ///     -If you're placing the first free point, move to the next condition. 
        /// </summary>
        /// <param name="controllerId"></param>
        /// <param name="pressure"></param>
        private void HandleOnTriggerDown(byte controllerId, float pressure)
        {
            if (index == 0)
                _placement.Confirm();
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
                //trigger view mode changes here
                //just a note for future me, you should probs add an enum for allll the view types you want to include (axis, component, unit vec, axis + angle (with resultant vector)). 
                //you can do this! just go back on your statics project and reuse the logic for the enum+view mode. but transport the functions to another visualizer script, because this one is getting full

            }
        }

        /// <summary>
        /// Destroy all previous instances and lock down current position on origin
        /// </summary>
        /// <param name="position">from controller</param>
        /// <param name="rotation">from controller</param>
        private void HandlePlacementComplete(Vector3 position, Quaternion rotation)
        {
            if (_placementPrefab != null)
            {
                Destroy(_placementObject.gameObject);

                _controllerConnectionHandler.ConnectedController.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.High);
                _controllerConnectionHandler.ConnectedController.StopFeedbackPatternLED();

                GameObject content = Instantiate(_placementPrefab, root);
                content.transform.position = position; //get the position of the placed prefab
                content.transform.rotation = rotation; //get the rotation of the placed prefab

                content0 = content; //save in global variable
                index++;
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
            _vectorMath.vectorComponents(newPlacement, content0.transform);
        }

        private PlacementObject CreatePlacementObject()
        {   // Destroy previous preview instance
            if (_placementObject != null)
            {
                Destroy(_placementObject.gameObject);
            }

            // Create the next preview instance.
            if (_placementPrefab != null)
            {
                //Debug.Log("Index is at: " + index);
                GameObject previewObject = Instantiate(_placementPrefab);

                // Detect all children in the preview and set children to ignore raycast.
                Collider[] colliders = previewObject.GetComponents<Collider>();
                for (int i = 0; i < colliders.Length; ++i)
                {
                    colliders[i].enabled = false;
                }

                // Find the placement object.
                PlacementObject placementObject = previewObject.GetComponent<PlacementObject>();

                if (placementObject == null)
                {
                    Destroy(previewObject);
                    Debug.LogError("Error: _Placement.placementObject is not set, disabling script.");

                    enabled = false;
                }

                Debug.Log("in placement object");

                return placementObject;
            }

            return null;
        }

        private void StartPlacement()
        {
            _placementObject = CreatePlacementObject();

            if (_placementObject != null)
            {
                _placement.Cancel();
                _placement.Place(_controllerConnectionHandler.transform, _placementObject.Volume, _placementObject.AllowHorizontal, _placementObject.AllowVertical, HandlePlacementComplete);
            }
        }
        #endregion
    }
}