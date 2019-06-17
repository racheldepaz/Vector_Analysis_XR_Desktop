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

        //References to Placement and PlacementObject scripts
        private Placement _placement = null;
        private PlacementObject _placementObject = null;
       // private DrawLine _drawLine = null; 


        //Stuff I need globally
        private bool placementModule = true; 
        private Vector3 zero = new Vector3(0, 0, 0);
        private GameObject xArrow, yArrow, zArrow;
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
            //_drawLine = GetComponent<DrawLine>(); 

            //line renderer initialize
            lr = GetComponent<LineRenderer>();
            zeroLR(lr);

            MLInput.OnTriggerDown += HandleOnTriggerDown;
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

                switch (index)
                {
                    case 0:
                        _instructionLabel.text = "Welcome! Time to place your origin. Point your controller towards a level surface and use the trigger to place your point.";
                        break;
                    case 1:
                        _instructionLabel.text = "Great! Now, point towards another area and press the trigger again to place your point. Press the home button to reset.";
                        Vector3 placedObj = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                        VectorComponentVisualizer(placedObj);
                        break;
                    case 2:
                        placementModule = false;
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
                if (placementModule)
                    placementModule = false;
                else
                    placementModule = true; 
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
                    //Destroy previous instance
                    Destroy(xArrow);

                    //Create visual representation 
                    xArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    xArrow.GetComponent<Renderer>().material.color = new Color(1F, 0, 0, 1F); //x is red
                    xArrow.transform.position = new Vector3(oVec.x - (position / 2.0f), oVec.y, oVec.z);

           
                    //scale accordingly
                    xArrow.transform.localScale = new Vector3(oVec.x - position, .01f, .01f);

                    break;
                case 1:
                    //Destroy previous instance
                    Destroy(yArrow);

                    //Create visual representation 
                    yArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    yArrow.GetComponent<Renderer>().material.color = new Color(1F, 0, 0, 1F); //x is red
                    yArrow.transform.position = new Vector3(oVec.x, oVec.y - (position / 2.0f), oVec.z);


                    //scale accordingly
                    yArrow.transform.localScale = new Vector3(.01f, Mathf.Abs(oVec.y - position), .01f);
                    break;
                case 2:
                    //Destroy prev instnace
                    Destroy(zArrow);

                    //create the obj im tired o no i want to go home
                    zArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    zArrow.GetComponent<Renderer>().material.color = new Color(0, 0.1F, 1F, 1F);
                    zArrow.transform.position = new Vector3(oVec.x, oVec.y, oVec.z - (position / 2.0f));


                    //scale accordingly 
                    zArrow.transform.localScale = new Vector3(.01f, .01f, Mathf.Abs(oVec.z - position));
                    break;
                default:
                    Debug.Log("Something isn't right"); //i wanna aaaaaah
                    break;
            }
         
        }

        private void oldvector(Vector3 newPlacement)
        {
            float xpos = newPlacement.x;
            float ypos = newPlacement.y;
            float zpos = newPlacement.z;

            //Delete previous instance
            Destroy(xArrow);
            Destroy(yArrow);
            Destroy(zArrow);

            float mag = newPlacement.magnitude;

            //Debug.Log("Vector position: " + newPlacement.ToString());
            _distanceLabel.text = "Distance from origin: " + newPlacement.ToString("N3");
            _magLabel.text = "Magnitude: " + newPlacement.magnitude.ToString("N3");

            //Get the absolute value of the position to start creating the arrows
            float x_abs = Mathf.Abs(xpos);
            float y_abs = Mathf.Abs(ypos);
            float z_abs = Mathf.Abs(zpos);

            //create the three lines
            xArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            xArrow.GetComponent<Renderer>().material.color = new Color(1F, 0, 0, 1F); //x is red

            yArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yArrow.GetComponent<Renderer>().material.color = new Color(0, 1F, 0.1F, 1F);

            zArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zArrow.GetComponent<Renderer>().material.color = new Color(0, 0.1F, 1F, 1F);

            Vector3 newXPos = new Vector3(Vector3.Distance(content0.transform.position, newPlacement) + (xpos / 2.0F), content0.transform.position.y, content0.transform.position.z);
            Vector3 newYPos = new Vector3(content0.transform.position.x, Vector3.Distance(content0.transform.position, newPlacement) + (ypos / 2.0F), content0.transform.position.z);
            Vector3 newZPos = new Vector3(content0.transform.position.x, content0.transform.position.y, Vector3.Distance(content0.transform.position, newPlacement) + (zpos / 2.0F));

            xArrow.transform.position = newXPos;
            yArrow.transform.position = newYPos;
            zArrow.transform.position = newZPos;

            //scale down the primitives
            xArrow.transform.localScale = new Vector3(x_abs, 0.01F, 0.01F);
            yArrow.transform.localScale = new Vector3(0.01F, y_abs, 0.01F);
            zArrow.transform.localScale = new Vector3(0.01F, 0.01F, z_abs);

            //angle stuff
            float xAngle = Mathf.Rad2Deg * Mathf.Acos(xpos / mag);
            float yAngle = Mathf.Rad2Deg * Mathf.Acos(ypos / mag);
            float zAngle = Mathf.Rad2Deg * Mathf.Acos(zpos / mag);

            _angleLabel.text = "Angles: " + xAngle + "(x) " + yAngle + "(y) " + zAngle + "(z)";
        }

        private void VectorComponentVisualizer(Vector3 newPlacement)
        {
            float deltaPos= Vector3.Distance(content0.transform.position, newPlacement);
            Vector3 _originVector = new Vector3(content0.transform.position.x, content0.transform.position.y, content0.transform.position.z);

            /* VectorMaths(newPlacement.x, newPlacement.magnitude, 0, _originVector);
             VectorMaths(newPlacement.y, newPlacement.magnitude, 1, _originVector);
             VectorMaths(newPlacement.z, newPlacement.magnitude, 2,_originVector);

             Vector3 xdir = new Vector3(newPlacement.x, _originVector.y, _originVector.z);
             float xDist = Vector3.Distance(_originVector, xdir);
             float xAngle = Mathf.Rad2Deg * Mathf.Acos(xDist / newPlacement.magnitude);

             Vector3 ydir = new Vector3(_originVector.x, newPlacement.y, _originVector.z);
             float yDist = Vector3.Distance(_originVector, ydir);
             float yAngle = Mathf.Rad2Deg * Mathf.Acos(yDist / newPlacement.magnitude);

             Vector3 zdir = new Vector3(_originVector.x, _originVector.y , newPlacement.z);
             float zDist = Vector3.Distance(_originVector, zdir);
             float zAngle = Mathf.Rad2Deg * Mathf.Acos(zDist / newPlacement.magnitude);*/

            oldvector(newPlacement);


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