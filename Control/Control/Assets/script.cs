using UnityEngine;
using UnityEngine.UI;

public class script : MonoBehaviour
{

    [SerializeField, Tooltip("The GameObject whose values are being modified.")]
    private GameObject pt = null;

    [SerializeField, Tooltip("The Transform that is the origin. ")]
    private Transform ori = null; 

    [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
    private Text _distanceLabel = null;

    [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
    private Text _magLabel = null;

    [SerializeField, Tooltip("The Transform element of the target")]
    public Transform target;

    [SerializeField, Tooltip("Line renderer x")]
    private LineRenderer xLR;

    [SerializeField, Tooltip("Line renderer x")]
    private LineRenderer yLR;

    [SerializeField, Tooltip("Line renderer x")]
    private LineRenderer zLR;

    private DebugScript _help; 

    private int count;

    public float angle;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
       // Vector3 newPlacement = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);

        //Declare new vector from relative origin, originObj in the local space

        /*Vector3 newPlacementx = new Vector3(newPlacement.x, originObj.position.y, originObj.position.z);
        Vector3 newPlacementy = new Vector3(originObj.position.x, newPlacement.y, originObj.position.z);
        Vector3 newPlacementz = new Vector3(originObj.position.x, originObj.position.y, newPlacement.z);


        Vector3 relativePos = getRelativePosition(originObj, newPlacement);*/
        xLR.SetPosition(0, ori.transform.position);

        xLR.SetPosition(1, new Vector3(pt.transform.position.x, ori.position.y, ori.position.z));





        yLR.SetPosition(0, ori.position);

        yLR.SetPosition(1, new Vector3(ori.position.x, pt.transform.position.y, ori.position.z));





        zLR.SetPosition(0, ori.position);

        zLR.SetPosition(1, new Vector3(ori.position.x, ori.position.y, pt.transform.position.z));


      /* Debug.Log("Origin position: " + originObj.transform.position.ToString());
        Debug.Log("Vector NOT ADJUSTED: " + content.transform.position.ToString());

        _distanceLabel.text = "Coordinates: " + getRelativePosition(originObj, newPlacement).ToString("N3");
        _magLabel.text = "Magnitude: " + relativePos.magnitude.ToString("N3");

       
        //Get the relative positions
        float relativeXPos = relativePos.x;
        float relativeYPos = relativePos.y;
        float relativeZPos = relativePos.z;   */
    }

    #region Private Methods
    public static Vector3 getRelativePosition(Transform origin, Vector3 position)
    {
        Vector3 distance = position - origin.position;
        Vector3 relativePosition = Vector3.zero;
        relativePosition.x = Vector3.Dot(distance, origin.right.normalized);
        relativePosition.y = Vector3.Dot(distance, origin.up.normalized);
        relativePosition.z = Vector3.Dot(distance, origin.forward.normalized);

        return relativePosition;
    }
    #endregion
}
