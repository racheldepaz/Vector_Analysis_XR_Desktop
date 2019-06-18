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

        private MLInputControllerFeedbackColorLED _color;

        [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
        private Text _distanceLabel = null;

        [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
        private Text _magLabel = null;

        [SerializeField, Tooltip("The Text element that will display component angles.")]
        private Text _angleLabel = null;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null; 

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] _placementPrefab = null;
        private int index;

        [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to point.")]
        private LineRenderer lr;

        [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to x component.")]
        private LineRenderer xLR;

        [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to y component.")]
        private LineRenderer yLR;

        [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to z component.")]
        private LineRenderer zLR;

        [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to x component.")]
        private LineRenderer xUnitLR;

        [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to y component.")]
        private LineRenderer yUnitLR;

        [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to z component.")]
        private LineRenderer zUnitLR;

        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;


        //Stuff I need globally
        private bool placementModule = true; 
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject content0, content1, content2; //origin, point 1, point 2
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

            zeroLR(lr, xLR, yLR, zLR, xUnitLR, yUnitLR, zUnitLR); 

            MLInput.OnTriggerDown += HandleOnTriggerDown;
            MLInput.OnTriggerUp += HandleOnTriggerUp;
            MLInput.OnControllerButtonDown += HandleOnButtonDown; 

            StartPlacement();
        }

        void Update()
        {
            // Update the preview location, inside of the validation area.
            if (_placementObject != null)
            {
                _placementObject.transform.position = _placement.AdjustedPosition - _placementObject.LocalBounds.center;
                _placementObject.transform.rotation = _placement.Rotation;

                if (_controllerConnectionHandler.ConnectedController.Touch1Active && _controllerConnectionHandler.ConnectedController.TouchpadGesture.Type == MLInputControllerTouchpadGestureType.ForceTapDown)
                    if (placementModule)
                        placementModule = false;
                    else
                        placementModule = true;

                switch (index)
                {
                    case 0:
                        _instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                        placementModule = true;
                        break;
                    case 1:
                        _instructionLabel.text = "Great! Press down on the touchpad if you're ready to place your point. Now, point towards another area and press the trigger again. Press the home button to reset.";
                        _color = MLInputControllerFeedbackColorLED.BrightCosmicPurple;
                        Debug.Log("You have met the update case 1 condition");
                        Vector3 placedObj1 = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                        VectorComponentVisualizer(placedObj1);
                        break;
                    case 2:
                        _instructionLabel.text = "Point towards another area and press the trigger to place your new point. Press the home button to reset.";
                        Vector3 placedObj = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                        VectorComponentVisualizer(placedObj);
                        break;
                    default: 
                        Debug.Log("Hm. Looks like our current index value indicates we shouldn't be displaying any further information.");
                        break;
                }
            }
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
            MLInput.OnTriggerUp -= HandleOnTriggerUp; 
           // MLInput.OnControllerTouchpadGesture -= HandleGesture;
        }
        #endregion

        #region Event Handlers
        private void HandleOnTriggerDown(byte controllerId, float pressure)
        {
            _controllerConnectionHandler.ConnectedController.StartFeedbackPatternEffectLED(MLInputControllerFeedbackEffectLED.Pulse, MLInputControllerFeedbackEffectSpeedLED.Fast, MLInputControllerFeedbackPatternLED.Clock12, MLInputControllerFeedbackColorLED.BrightShaggleGreen, 0);
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

        private void HandleOnTriggerUp(byte controllerId, float pressure)
        { _controllerConnectionHandler.ConnectedController.StopFeedbackPatternLED();  }

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
                    //content0.transform.position.z in the z for snap
                    Vector3 temp = new Vector3(position.x, position.y, position.z);
                    content.transform.position = temp;
                    VectorComponentVisualizer(temp);
                }
                else if (index == 2)
                {
                    placementModule = false;

                    content2 = content;
                    Vector3 content_vec = new Vector3(position.x, position.y, position.z);
                    VectorComponentVisualizer(content_vec);
                }
                content.gameObject.SetActive(true);
                NextPlacementObject();
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the width to .016 units and sets each line renderer's position to zero
        /// </summary>
        /// <param name="linerenderer">The resultant vector</param>
        /// <param name="lrx">The x component</param>
        /// <param name="lry">The y component</param>
        /// <param name="lrz">The z component</param>
        /// <param name="uvx">The unit vector in the x direction</param>
        /// <param name="uvy">The unit vector in the y direction</param>
        /// <param name="uvz">The unit vector in the z direction</param>
        private void zeroLR(LineRenderer linerenderer, LineRenderer lrx, LineRenderer lry, LineRenderer lrz, LineRenderer uvx, LineRenderer uvy, LineRenderer uvz)
        {
            linerenderer.SetWidth(0.016f, 0.016f);
            linerenderer.SetPosition(0, zero);
            linerenderer.SetPosition(1, zero);

            lrx.SetWidth(0.016f, 0.016f);
            lrx.SetPosition(0, zero);
            lrx.SetPosition(1, zero);

            lry.SetWidth(0.016f, 0.016f);
            lry.SetPosition(0, zero);
            lry.SetPosition(1, zero);

            lrz.SetWidth(0.016f, 0.016f);
            lrz.SetPosition(0, zero);
            lrz.SetPosition(1, zero);

            uvx.SetWidth(0.016f, 0.016f);
            uvx.SetPosition(0, zero);
            uvx.SetPosition(1, zero);

            uvy.SetWidth(0.016f, 0.016f);
            uvy.SetPosition(0, zero);
            uvy.SetPosition(1, zero);

            uvz.SetWidth(0.016f, 0.016f);
            uvz.SetPosition(0, zero);
            uvz.SetPosition(1, zero);
        }

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
                    xLR.SetPosition(0, oVec);
                    xLR.SetPosition(1, new Vector3(position, oVec.y, oVec.z));
                    xUnitLR.SetPosition(0, oVec);
                    xUnitLR.SetPosition(1, new Vector3(position / magnitude, oVec.y, oVec.z));
                    break;
                case 1:
                    yLR.SetPosition(0, oVec);
                    yLR.SetPosition(1, new Vector3(oVec.x, position, oVec.z));
                    yUnitLR.SetPosition(0, oVec);
                    yUnitLR.SetPosition(1, new Vector3(oVec.x, position/magnitude, oVec.z));
                    break;
                case 2:
                    zLR.SetPosition(0, oVec);
                    zLR.SetPosition(1, new Vector3(oVec.x, oVec.y, position));
                    zUnitLR.SetPosition(0, oVec);
                    zUnitLR.SetPosition(1, new Vector3(oVec.x, oVec.y, position / magnitude)); 
                    break;
                default:
                    Debug.Log("Something isn't right"); //i wanna aaaaaah
                    break;
            }
         
        }

        private void VectorComponentVisualizer(Vector3 newPlacement)
        {
            float deltaPos= Vector3.Distance(content0.transform.position, newPlacement);
            Vector3 _originVector = new Vector3(content0.transform.position.x, content0.transform.position.y, content0.transform.position.z);

            VectorMaths(newPlacement.x, newPlacement.magnitude, 0, _originVector);
            VectorMaths(newPlacement.y, newPlacement.magnitude, 1, _originVector);
            VectorMaths(newPlacement.z, newPlacement.magnitude, 2, _originVector);

            Vector3 xdir = new Vector3(newPlacement.x, _originVector.y, _originVector.z);
            Vector3 ydir = new Vector3(_originVector.x, newPlacement.y, _originVector.z);
   
            Vector3 zdir = new Vector3(_originVector.x, _originVector.y, newPlacement.z);

            Vector3 relPos = getRelativePosition(content0.transform, newPlacement);
            float relMag = relPos.magnitude;

            _distanceLabel.text = "Distance from origin: " + relPos.ToString("N1");
            _magLabel.text = "Magnitude: " + relMag.ToString("N1");

            lr.SetPosition(0, content0.transform.position);
            lr.SetPosition(1, newPlacement);
        }

        public static Vector3 getRelativePosition(Transform origin, Vector3 position)
        {
            Vector3 distance = position - origin.position;
            Vector3 relativePosition = Vector3.zero;
            relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
            relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
            relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);
            return relativePosition;
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

            if (_placementObject != null && placementModule == true)
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