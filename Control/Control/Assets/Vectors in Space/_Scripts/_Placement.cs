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
    //[RequireComponent(typeof(CanvasScript))]
    public class _Placement : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The controller that is used in the scene to cycle and place objects.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;

        private MLInputControllerFeedbackColorLED _color;

        [SerializeField, Tooltip("The Text element that will display the instructions for the student to complete the tasks.")]
        private Text _instructionLabel = null; 

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] _placementPrefab = null;

        [SerializeField, Tooltip("The object used to represent the unit grid.")]
        private GameObject gridPrefab = null; 

        private int index;
        private int bumpcount;
        bool grid; 


        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;
        private VectorMath _vectorMath = null; 


        //Stuff I need globally
        private bool placementModule = true, unitactive = false; 
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject content0, content1, content2, savedGrid; //origin, point 1, point 2
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

            bumpcount = 0; 
            index = 0;

            _placement = GetComponent<Placement>();
            _vectorMath = GetComponent<VectorMath>();

            //zeroLR(lr, xLR, yLR, zLR, xUnitLR, yUnitLR, zUnitLR); 

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

                if (content0 != null)
                {
                    switch (_controllerConnectionHandler.ConnectedController.TouchpadGesture.Type)
                    {
                        case MLInputControllerTouchpadGestureType.SecondForceDown:
                            GameObject grid = Instantiate(gridPrefab);
                            grid.transform.position = content0.transform.position;
                            savedGrid = grid;
                          //  Destroy(grid);
                            break;
                        case MLInputControllerTouchpadGestureType.None:
                           // Destroy(grid);
                            break;
                        default:
                            break;
                    }
                }

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
                        VectorVisualizer(placedObj1);
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

            if(_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id == controllerId && button == MLInputControllerButton.Bumper)
            {
                if (unitactive)
                    unitactive = false;
                if (!unitactive)
                    unitactive = true; 
            }
        }

        private void HandleOnTriggerUp(byte controllerId, float pressure)
        {
            _controllerConnectionHandler.ConnectedController.StopFeedbackPatternLED();
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
                    //content0.transform.position.z in the z for snap
                    Vector3 temp = new Vector3(position.x, position.y, position.z);
                    content.transform.position = temp;
                    VectorVisualizer(temp);
                }
                content.gameObject.SetActive(true);
                NextPlacementObject();
            }
        }
        #endregion

        #region Private Methods

        private void VectorVisualizer(Vector3 newPlacement)
        {
            if (!unitactive)
                _vectorMath.vectorComponents(newPlacement, content0.transform);
            else
                _vectorMath.vectorUnitComponents(newPlacement, content0.transform);
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