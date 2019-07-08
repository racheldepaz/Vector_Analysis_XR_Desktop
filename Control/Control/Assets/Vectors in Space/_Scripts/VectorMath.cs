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
<<<<<<< Updated upstream
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
=======

    private CanvasScript canvasScript = null;
    private ChangeViewModes changeViewModes = null;
    #endregion

    #region Serialized Variables
    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz axes")]
    private LineRenderer[] axesLR = null;

    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz + resultant vector components")]
    private LineRenderer[] componentsLR = null;

    [SerializeField, Tooltip("The Line Rendere game objects that represnet the xyz + resultant unit vector components")]
    private LineRenderer[] unitsLR = null;
>>>>>>> Stashed changes
    #endregion

    #region Unity Methods
    void Start()
    {
<<<<<<< Updated upstream
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
=======
        //initialize all line renderers to zero
        ZeroLRArray(axesLR);
        ZeroLRArray(componentsLR);
        ZeroLRArray(unitsLR);

        canvasScript    = GetComponent<CanvasScript>();
        changeViewModes = GetComponent<ChangeViewModes>(); 
>>>>>>> Stashed changes
    }
    #endregion



    #region Public Methods
    public void vectorComponents(Vector3 point, Transform origin)
    {
<<<<<<< Updated upstream
        relPos = getRelativePosition(origin, point); //position of the point with the non-zero origin as new reference pt
        relMag = relPos.magnitude; //magnitude of the point with the "

        for (int i = 0; i < 3; i++)
=======
        relPos = GetRelativePosition(origin, point); //position of the point with the non-zero origin as new reference pt
        relMag = relPos.magnitude; //magnitude of the point with the
        canvasScript.setVariables(relPos, relMag);

        if (index == 0)
        {
            for (int i = 0; i <= 3; i++)
            {
                VisualizeComponent(point, i, origin);
            }
        }
        if (index == 1)
        {
            for (int i = 0; i <= 3; i++)
            {
                VisualizeAxes(point, i, origin);
            }
        }
        if (index == 2)
>>>>>>> Stashed changes
        {
            visualizeComponent(point, i, origin, x, y, z, r);
        }
    }

    public void vectorUnitComponents(Vector3 point, Transform origin)
    {
        relPos = getRelativePosition(origin, point);
        relMag = relPos.magnitude;
        for(int i = 0; i < 3; i++)
        {
            visualizeUnitVectorComponent(point, i, origin, xUnit, yUnit, zUnit, rUnit);
        }
    }
    #endregion

    #region Private Methods
<<<<<<< Updated upstream
    private void zeroLR(bool unit)
    {
        if (unit)
        {
            r.SetPosition(0, zero);
            r.SetPosition(1, zero);

            x.SetPosition(0, zero);
            x.SetPosition(1, zero);

            y.SetPosition(0, zero);
            y.SetPosition(1, zero);

            z.SetPosition(0, zero);
            z.SetPosition(1, zero);

        }
        else
        {
            rUnit.SetPosition(0, zero);
            rUnit.SetPosition(1, zero);

            xUnit.SetPosition(0, zero);
            xUnit.SetPosition(1, zero);

            yUnit.SetPosition(0, zero);
            yUnit.SetPosition(1, zero);

            zUnit.SetPosition(0, zero);
            zUnit.SetPosition(1, zero);
=======
    private void VisualizeAxes(Vector3 point, int index, Transform origin)
    {
        ZeroLRArray(componentsLR);
        ZeroLRArray(unitsLR);
        switch (index)
        {
            case 0: //x
                axesLR[index].SetPosition(0, origin.position);
                axesLR[index].SetPosition(1, new Vector3(origin.position.x + 1, origin.position.y, origin.position.z));
                break;
            case 1: //y
                axesLR[index].SetPosition(0, origin.position);
                axesLR[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y + 1, origin.position.z));
                break;
            case 2: //z
                axesLR[index].SetPosition(0, origin.position);
                axesLR[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, origin.position.z + 1));
                break;
            case 3: //the resultant vector
                axesLR[index].SetPosition(0, origin.position);
                axesLR[index].SetPosition(1, point); // new Vector3(point.x, origin.position.y, origin.position.z));
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, i, T)");
                break;
>>>>>>> Stashed changes
        }
    }
    private void visualizeComponent(Vector3 point, int index, Transform origin, LineRenderer x, LineRenderer y, LineRenderer z, LineRenderer r)
    {
<<<<<<< Updated upstream
        zeroLR(false);
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
=======
        ZeroLRArray(unitsLR);
        ZeroLRArray(axesLR);
        switch (index)
        {
            case 0: //x
                componentsLR[index].SetPosition(0, origin.position);
                componentsLR[index].SetPosition(1, new Vector3(point.x, origin.position.y, origin.position.z));
                break;
            case 1: //y 
                componentsLR[index].SetPosition(0, origin.position);
                componentsLR[index].SetPosition(1, new Vector3(origin.position.x, point.y, origin.position.z));
                break;
            case 2: //z
                componentsLR[index].SetPosition(0, origin.position);
                componentsLR[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, point.z));
                break;
            case 3: //resultant vec
                componentsLR[index].SetPosition(0, origin.position);
                componentsLR[index].SetPosition(1, point);
>>>>>>> Stashed changes
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, i, T)");
                break;
        }
    }

    private void visualizeUnitVectorComponent(Vector3 point, int index, Transform origin, LineRenderer uX, LineRenderer uY, LineRenderer uZ, LineRenderer uR)
    {
<<<<<<< Updated upstream
        zeroLR(true);
=======
        ZeroLRArray(componentsLR);
        ZeroLRArray(axesLR);
>>>>>>> Stashed changes
        switch (index)
        {
            case 0:
                adjustedPos = (relPos.x / relMag) + origin.position.x; //account for the offset in the origin
                Vector3 xComp = new Vector3(adjustedPos, origin.position.y, origin.position.z);
<<<<<<< Updated upstream
                uX.SetPosition(0, origin.position);
                uX.SetPosition(1, xComp);
=======
                unitsLR[index].SetPosition(0, origin.position);
                unitsLR[index].SetPosition(1, xComp);
>>>>>>> Stashed changes
                break;
            case 1:
                adjustedPos = (relPos.y / relMag) + origin.position.y; //account for the offset in the origin
                Vector3 yComp = new Vector3(origin.position.x, adjustedPos, origin.position.z);
<<<<<<< Updated upstream
                uY.SetPosition(0, origin.position);
                uY.SetPosition(1, yComp);
=======
                unitsLR[index].SetPosition(0, origin.position);
                unitsLR[index].SetPosition(1, yComp);
>>>>>>> Stashed changes
                break;
            case 2:
                adjustedPos = (relPos.z / relMag) + origin.position.z; //account for the offset in the origin
                Vector3 zComp = new Vector3(origin.position.x, origin.position.y, adjustedPos);
<<<<<<< Updated upstream
                uZ.SetPosition(0, origin.position);
                uZ.SetPosition(1, zComp);
                break;
            case 3:
                point.Normalize();
                uR.SetPosition(0, origin.position);
                uR.SetPosition(1, point);
=======
                unitsLR[index].SetPosition(0, origin.position);
                unitsLR[index].SetPosition(1, zComp);
                break;
            case 3:
                Vector3 adjPos = relPos.normalized;
                unitsLR[index].SetPosition(0, origin.position);
                unitsLR[index].SetPosition(1, adjPos);
>>>>>>> Stashed changes
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorUnitComponents(V3, i, T)");
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

    //iterate through every element of a linerenderer array and set its positions to zero
    private void ZeroLRArray(LineRenderer[] lr)
    {
        for (int i = 0; i <= lr.Length; i++)
        {
            int positions = lr[i].positionCount;
            for (int j = 0; j < positions; j++)
            {
                lr[i].SetPosition(0, zero);
                lr[i].SetPosition(1, zero);
            }
        }
    }
    #endregion
}
 