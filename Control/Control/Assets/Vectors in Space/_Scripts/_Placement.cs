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
    public class _Placement : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The controller that is used in the scene to cycle and place objects.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
        private Text _distanceLabel = null;

        [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
        private Text _magLabel = null;

        [SerializeField, Tooltip("The Text element that will display component angles.")]
        private Text _angleLabel = null;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null; 

        private bool placed = false; //move this eventually

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] _placementPrefab = null;
        private int index;

        [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to point.")]
        private LineRenderer lr;

        [SerializeField, Tooltip("The Line Renderer for the x vector component")]
        private LineRenderer xLine;

        [SerializeField, Tooltip("The Line Renderer for the y vector component")]
        private LineRenderer yLine;

        [SerializeField, Tooltip("The Line Renderer for the z vector component")]
        private LineRenderer zLine; 

        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;


        //Stuff I need globally
        private bool _placementModule = true; 
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject xArrow, yArrow, zArrow;
        private GameObject content0, content1, content2, x, y, z; //origin, point 1, point 2
        #endregion

        #region Unity Methods
        void Start()
        {
            if (_controllerConnectionHandler == null)
            {
                Debug.LogError("Error: PlacementExample._controllerConnectionHandler is not set, disabling script.");
                enabled = false;
                return;
            }
            index = 0;
            _placement = GetComponent<Placement>();

            x = GetComponent<GameObject>();
            y = GetComponent<GameObject>();
            z = GetComponent<GameObject>();

            //line renderer is invisible
            lr = GetComponent<LineRenderer>();
            xLine = GetComponent<LineRenderer>();
            yLine = GetComponent<LineRenderer>();
            zLine = GetComponent<LineRenderer>();

            zeroLR(lr);
            zeroLR(xLine);
            zeroLR(yLine);
            zeroLR(zLine);

            MLInput.OnTriggerDown += HandleOnTriggerDown;
            MLInput.OnControllerButtonDown += HandleOnButtonDown;
           // MLInput.OnControllerTouchpadGestureStart += HandleGesture;

            StartPlacement();
        }

        void Update()
        {
            // Update the preview location, inside of the validation area.
            if (_placementObject != null)
            {
                _placementObject.transform.position = _placement.AdjustedPosition - _placementObject.LocalBounds.center;
                _placementObject.transform.rotation = _placement.Rotation;

                switch (index)
                {
                    case 0:
                        _instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                        //_controllerConnectionHandler.ConnectedController.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Bump, MLInputControllerFeedbackIntensity.Medium); everything is gross. time to meme. 
                       // _controllerConnectionHandler.ConnectedController.StartFeedbackPatternEffectLED(MLInputControllerFeedbackEffectLED.RotateCW, MLInputControllerFeedbackEffectSpeedLED.Slow, MLInputControllerFeedbackPatternLED.None, MLInputControllerFeedbackColorLED.PastelNebulaPink, 4.0f);
                        break;
                    case 1:
                        _instructionLabel.text = "Great! Now, point towards another area and press the trigger again to place your point. Press the home button to reset.";
                        Vector3 placedObj = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                        VectorComponentVisualizer(placedObj);
                        break;
                    case 2:
                        _instructionLabel.text = "Point towards another area and press the trigger to place your new point. Press the home button to reset.";
                        placedObj = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                        VectorComponentVisualizer(placedObj);
                        break;
                    default: 
                        Debug.Log("Hm. Looks like something's up with the index values.");
                        break;
                }
            }
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
           // MLInput.OnControllerTouchpadGesture -= HandleGesture;
        }
        #endregion

        #region Event Handlers
        private void HandleOnTriggerDown(byte controllerId, float pressure)
        {
            _controllerConnectionHandler.ConnectedController.StartFeedbackPatternEffectLED(MLInputControllerFeedbackEffectLED.PaintCW, MLInputControllerFeedbackEffectSpeedLED.Fast, MLInputControllerFeedbackPatternLED.Clock12, MLInputControllerFeedbackColorLED.BrightShaggleGreen, 0);
            _placement.Confirm();
        }

        private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id
                == controllerId && button == MLInputControllerButton.HomeTap)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }

        private void HandleGesture(byte controllerId, MLInputControllerTouchpadGestureType gestureType)
        {
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId && gestureType == MLInputControllerTouchpadGestureType.ForceTapDown)
            {
                if (_placementModule)
                    _placementModule = false;
                else
                    _placementModule = true; 
            }
        }

        private void HandlePlacementComplete(Vector3 position, Quaternion rotation)
        {
            if (_placementPrefab != null)
            {
                _controllerConnectionHandler.ConnectedController.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.ForceUp, MLInputControllerFeedbackIntensity.High);
                _controllerConnectionHandler.ConnectedController.StopFeedbackPatternLED();
                Debug.Log("In handle placement comp");
                GameObject content = Instantiate(_placementPrefab[index]);
                content.transform.position = position; //get the position of the placed prefab
                content.transform.rotation = rotation; //get the rotation of the placed prefab

                if (index == 0)
                {
                    content0 = content;
                    Debug.Log("content0: " + content0.transform.position.ToString());
                }
                else if (index == 1)
                {
                    content1 = content;
                    Vector3 temp = new Vector3(position.x, position.y, 0);
                    content.transform.position = temp;
                    VectorComponentVisualizer(content.transform.position);
                }
                else if (index == 2)
                {
                    content2 = content;
                    Vector3 content_vec = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);
                    VectorComponentVisualizer(content_vec);
                }
                content.gameObject.SetActive(true);
                NextPlacementObject();
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// From the position (x,y or z) of the vector, create the game object for the component, calculate the angle
        /// </summary>
        /// <param name="position">The x,y, or z position of the vector. </param>
        /// <param name="magnitude">The magnitude of the vector</param>
        /// <param name="obj">0 denotes x, 1 denotes y, 2 denotes z</param>
        private void VectorMaths(float position, float magnitude, int obj, Vector3 oVec)
        {
            switch (obj)
            {
                case 0:
                    Vector3 xdir = new Vector3(position, oVec.y, oVec.z);
                    xLine.SetPosition(0, oVec);
                    xLine.SetPosition(1, xdir);
                    float xAngle = Mathf.Rad2Deg * Mathf.Acos(position / magnitude);
                    break;
                case 1:
                    Vector3 ydir = new Vector3(oVec.x, position, oVec.z);
                    yLine.SetPosition(0, oVec);
                    yLine.SetPosition(1, ydir);
                    float yAngle = Mathf.Rad2Deg * Mathf.Acos(position / magnitude);
                    break;
                case 2:
                    Vector3 zdir = new Vector3(oVec.x, oVec.y, position);
                    zLine.SetPosition(0, oVec);
                    zLine.SetPosition(1, zdir);
                    float zAngle = Mathf.Rad2Deg * Mathf.Acos(position / magnitude);
                    break;
                default:
                    Debug.Log("Something ins't right");
                    break;
            }
         
        }

        private void VectorComponentVisualizer(Vector3 newPlacement)
        {
            float deltaPos= Vector3.Distance(content0.transform.position, newPlacement);
            Vector3 _originVector = new Vector3(content0.transform.position.x, content0.transform.position.y, content0.transform.position.z);

            VectorMaths(newPlacement.x, newPlacement.magnitude, 0, _originVector);
            VectorMaths(newPlacement.y, newPlacement.magnitude, 1, _originVector);
            VectorMaths(newPlacement.z, newPlacement.magnitude, 2,_originVector);

            lr.SetPosition(0, content0.transform.position);
            lr.SetPosition(1, newPlacement);
        }

        private void zeroLR(LineRenderer linerenderer)
        {
            linerenderer.SetWidth(0, 0.016f);
            linerenderer.SetPosition(0, zero);
            linerenderer.SetPosition(1, zero); 
          }

        private PlacementObject CreatePlacementObject(int index)
        {   // Destroy previous preview instance
            if (_placementObject != null)
            {
                Destroy(_placementObject.gameObject);
            }

            // Create the next preview instance.
            if (_placementPrefab[index] != null)
            {
                //Debug.Log("Index is at: " + index);
                GameObject previewObject = Instantiate(_placementPrefab[index]);

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
                    Debug.LogError("Error: PlacementExample.placementObject is not set, disabling script.");

                    enabled = false;
                }

                Debug.Log("in placement object");

                return placementObject;
            }

            return null;
        }

        private void StartPlacement()
        {
            _placementObject = CreatePlacementObject(index);

            if (_placementObject != null && _placementModule == true)
            {
                _placement.Cancel();
                _placement.Place(_controllerConnectionHandler.transform, _placementObject.Volume, _placementObject.AllowHorizontal, _placementObject.AllowVertical, HandlePlacementComplete);
            }
        }

        private void NextPlacementObject()
        {
            if (_placementPrefab != null)
                index++;
            StartPlacement();
        }
        #endregion
    }
}