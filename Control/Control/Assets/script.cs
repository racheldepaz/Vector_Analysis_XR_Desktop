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

    /*This is where I test stuff so it is SUPER messy sorry future me/person who has taken over my project. 
     * this scene isnt really meant to be published <3 */

    // Update is called once per frame
    void Update()
    {

        
        xLR.SetPosition(0, ori.transform.position);

        xLR.SetPosition(1, new Vector3(pt.transform.position.x/pt.transform.position.magnitude, ori.position.y, ori.position.z));

        yLR.SetPosition(0, ori.position);

        yLR.SetPosition(1, new Vector3(pt.transform.position.x / getRelativePosition(ori, pt.transform.position).magnitude, ori.position.y, ori.position.z));

        zLR.SetPosition(0, ori.position);

        zLR.SetPosition(1, new Vector3(pt.transform.position.x, ori.position.y, ori.position.z));
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
