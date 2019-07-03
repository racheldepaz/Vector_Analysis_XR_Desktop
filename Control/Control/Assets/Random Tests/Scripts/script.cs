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

    [SerializeField]
    private LineRenderer LR0;

    [SerializeField]
    private LineRenderer LR1;

    [SerializeField]
    private LineRenderer LR2;

    [SerializeField]
    private LineRenderer LR3;

    [SerializeField]
    private LineRenderer LR4;

    private DebugScript _help;

    /*[SerializeField]
    private DrawArc drawArc;*/

    private int count;

    public float angle;

    private Vector3 normalised;

    Vector3 xComp, xComp1;

    void Awake()
    {
        
    }

    /*This is where I test stuff so it is SUPER messy sorry future me/person who has taken over my project. 
     * this scene isnt really meant to be published <3 */

    // Update is called once per frame
    void Update()
    {
        Vector3 ptA = pt.transform.position;
        Vector3 relptA = getRelativePosition(ori, ptA);
        Vector3 ptB = ori.position;

        //float mag_test = ptA.magnitude; 
        float mag_test = relptA.magnitude;

        xComp1 = new Vector3(ptA.x, ptB.y, ptB.z); //regular x comp

        Debug.Log("Mag: " + ptA.magnitude);

        float adj = ((ptA.x-ori.position.x) / mag_test) + (ori.position.x); //adjusted value for x omponent of unit vector
        Debug.Log("pta.x: " + ptA.x);
        Debug.Log("Adjusted value for ptA.x/mag: " + adj);

        xComp = new Vector3(adj, ptB.y, ptB.z); //x comp of unit vector 
        Debug.Log("x unit vector: " + xComp.ToString());
        Debug.Log("Regular X-Component Position: " + xComp1.ToString());

        LR0.SetPosition(0, ori.position);
        LR0.SetPosition(1, xComp1);

        LR1.SetPosition(0, ori.position);
        LR1.SetPosition(1, xComp);

        angle = Mathf.Acos(ptA.x / mag_test);
        //drawArc.setRadAngle(angle);

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
