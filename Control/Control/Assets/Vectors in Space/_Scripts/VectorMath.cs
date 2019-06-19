using UnityEngine;
using UnityEngine.UI; 


/// <summary>
/// This class allows the programmer to quickly make vector calculations and create a visual output
/// for the user seamlessly. It is referred in the _Placement script. 
/// </summary>
public class VectorMath : MonoBehaviour
{
   // #region Private Variables
    private float relMag, adjustedPos;
    private Vector3 relPos;
    private Vector3 zero = new Vector3(0, 0, 0);
    //#endregion

    #region Serialized Variables
    [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to point.")]
    private LineRenderer r;
    [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to x component.")]
    private LineRenderer x;
    [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to y component.")]
    private LineRenderer y;
    [SerializeField, Tooltip("The Line Renderer that will draw the resultant vector from origin to z component.")]
    private LineRenderer z;
    [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to point.")]
    private LineRenderer rUnit;
    [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to x component.")]
    private LineRenderer xUnit;
    [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to y component.")]
    private LineRenderer yUnit;
    [SerializeField, Tooltip("The Line Renderer that will draw the unit vector from origin to z component.")]
    private LineRenderer zUnit;
    #endregion

    #region Unity Methods
    void Start()
    {
        //disgusting. there has to be a better way to do this (an array?????). smh
        r.SetPosition(0, zero);
        r.SetPosition(1, zero);

        rUnit.SetPosition(0, zero);
        rUnit.SetPosition(1, zero);

        x.SetPosition(0, zero);
        x.SetPosition(1, zero);

        y.SetPosition(0, zero);
        y.SetPosition(1, zero);

        z.SetPosition(0, zero);
        z.SetPosition(1, zero);

        xUnit.SetPosition(0, zero);
        xUnit.SetPosition(1, zero);

        yUnit.SetPosition(0, zero);
        yUnit.SetPosition(1, zero);

        zUnit.SetPosition(0, zero);
        zUnit.SetPosition(1, zero);
    }
    #endregion

    #region Public Methods
    public void vectorComponents(Vector3 point, Transform origin)
    {
        relPos = getRelativePosition(origin, point); //position of the point with the non-zero origin as new reference pt
        relMag = relPos.magnitude; //magnitude of the point with the "
       
        for (int i = 0; i < 3; i++)
        {
            visualizeComponent(point, i, origin, x, y, z, r);
        }
    }

    public void vectorUnitComponents(Vector3 point, Transform origin)
    {
        for(int i = 0; i < 3; i++)
        {
            visualizeUnitVectorComponent(point, i, origin, xUnit, yUnit, zUnit, rUnit);
        }
    }
    #endregion

    #region Private Methods
    private void visualizeComponent(Vector3 point, int index, Transform origin, LineRenderer x, LineRenderer y, LineRenderer z, LineRenderer r)
    {
        switch (index)
        {
            case 0:
                x.SetPosition(0, origin.position);
                x.SetPosition(1, new Vector3(point.x, origin.position.y, origin.position.z));
                break;
            case 1:
                y.SetPosition(0, origin.position);
                y.SetPosition(1, new Vector3(origin.position.x, point.y, origin.position.z));
                break;
            case 2:
                z.SetPosition(0, origin.position);
                z.SetPosition(1, new Vector3(origin.position.x, origin.position.y, point.z));
                break;
            case 3:
                r.SetPosition(0, origin.position);
                r.SetPosition(1, point);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, T)");
                break;
        }
    }

    private void visualizeUnitVectorComponent(Vector3 point, int index, Transform origin, LineRenderer uX, LineRenderer uY, LineRenderer uZ, LineRenderer uR)
    {
        switch (index)
        {
            case 0:
                adjustedPos = ((point.x - origin.position.x) / relMag) + origin.position.x; //account for the offset in the origin
                Vector3 xComp = new Vector3(adjustedPos, origin.position.y, origin.position.z);
                uX.SetPosition(0, origin.position);
                uX.SetPosition(1, xComp);
                break;
            case 1:
                adjustedPos = ((point.y - origin.position.y) / relMag) + origin.position.y; //account for the offset in the origin
                Vector3 yComp = new Vector3(origin.position.x, adjustedPos, origin.position.z);
                uY.SetPosition(0, origin.position);
                uY.SetPosition(1, yComp);
                break;
            case 2:
                adjustedPos = ((point.z - origin.position.z) / relMag) + origin.position.z; //account for the offset in the origin
                Vector3 zComp = new Vector3(origin.position.x, origin.position.y, adjustedPos);
                uZ.SetPosition(0, origin.position);
                uZ.SetPosition(1, zComp);
                break;
            case 3:
                point.Normalize();
                uR.SetPosition(0, origin.position);
                uR.SetPosition(1, point);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorUnitComponents(V3, T)");
                break;
        }
    }

    private static Vector3 getRelativePosition(Transform origin, Vector3 position)
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
