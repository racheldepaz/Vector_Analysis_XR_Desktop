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

        private bool placed = false; //move this eventually

        [SerializeField, Tooltip("The placement object used in the scene.")]
        private GameObject[] _placementPrefab = null;
        private int index; 

        public LineRenderer lr;

        private Placement _placement = null;
        private PlacementObject _placementObject = null;
        private Vector3 placedObject = new Vector3(0, 0, 0);
        //private Quaternion origin_q = new Quaternion(0, 0, 0, 0);
        private GameObject xArrow, yArrow, zArrow;
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
            lr = GetComponent<LineRenderer>();

            lr.SetWidth(0, 0.016f);
            lr.SetPosition(0, placedObject);
            lr.SetPosition(1, placedObject);

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

                //Debug stuff
                Debug.Log("Placement Object position: " + _placementObject.transform.position.ToString("N3"));
                Vector3 placedObj = new Vector3(_placementObject.transform.position.x, _placementObject.transform.position.y, _placementObject.transform.position.z);
                VectorComponentVisualizer(placedObj);
            }
        }

        void OnDestroy()
        {
            MLInput.OnTriggerDown -= HandleOnTriggerDown;
            MLInput.OnControllerButtonDown -= HandleOnButtonDown;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the event for trigger down.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="button">The button that is being pressed.</param>

        private void HandleOnTriggerDown(byte controllerId, float pressure)
        {
            _placement.Confirm();
        }


        /// <summary>
        /// Place the origin, snappable point one, free point two in 3d space. 
        /// </summary>
        /// <param name= "position">The adjusted position of the controller in world space.</param>
        /// <param name="rotation">The adjusted rotation value of the controller in world space.</param>
        private void HandlePlacementComplete(Vector3 position, Quaternion rotation)
        {
            if (_placementPrefab != null)
            {
                GameObject content = Instantiate(_placementPrefab[index]);
                content.transform.position = position; //get the position of the placed prefab
                content.transform.rotation = rotation; //get the rotation of the placed prefab


                content.gameObject.SetActive(true);

                //create vector storing the placed object's position
                Vector3 content_vec = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);
                VectorComponentVisualizer(content_vec);
                NextPlacementObject();
            }
        }
        #endregion

        #region Private Methods
        private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
        {
            if (_controllerConnectionHandler.IsControllerValid() && _controllerConnectionHandler.ConnectedController.Id
                == controllerId && button == MLInputControllerButton.HomeTap)
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }

        private void VectorComponentVisualizer(Vector3 newPlacement)
        {
            float xpos = newPlacement.x;
            float ypos = newPlacement.y;
            float zpos = newPlacement.z;

            //Debug Stuff
            Debug.Log("x- " + xpos + " y- " + ypos + " z- " + zpos);

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

            Vector3 newXPos = new Vector3(xpos / 2.0F, 0, 0);
            Vector3 newYPos = new Vector3(0, ypos / 2.0F, 0);
            Vector3 newZPos = new Vector3(0, 0, zpos / 2.0F);

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


            lr.SetPosition(1, newPlacement);
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

                placed = false;

                return placementObject;
            }

            return null;
        }

        private void StartPlacement()
        {
            _placementObject = CreatePlacementObject(index);

            if (_placementObject != null)
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