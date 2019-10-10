﻿using UnityEngine;

/// <summary>
/// This class allows the programmer to quickly make vector calculations and create a visual output
/// for the user seamlessly. It is referred in the _Placement script. 
/// </summary>

[RequireComponent(typeof(CanvasScript))]
public class VectorMath : MonoBehaviour
{
    #region Private Variables
    private float relMag, adjustedPos;
    private Vector3 relPos;

    private CanvasScript canvasScript = null;
    #endregion

    #region Serialized Variables
    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz axes")]
    private LineRenderer[] axes = null;

    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz + resultant vector components")]
    private LineRenderer[] components = null;

    [SerializeField, Tooltip("The Line Rendere game objects that represnet the xyz + resultant unit vector components")]
    private LineRenderer[] units = null;

    [SerializeField, Tooltip("Gameobjects that hold the meshes for the arcs")]
    private LineRenderer[] arcs = null;

    [SerializeField, Tooltip("The markers for where the arc should begin")]
    private GameObject[] dots = null;

    [SerializeField]
    private GameObject arrowHead; 
    #endregion

    #region Unity Methods
    void Awake() //debugging stuff to make sure everything is set<3
    {
        if (null == axes)
        {
            Debug.LogError("Error: VectorMath.axes not set.");
            return;
        }
        if (null == components)
        {
            Debug.LogError("Error: VectorMath.components not set.");
            return;
        }
        if (null == units)
        {
            Debug.LogError("Error: VectorMath.units not set.");
            return;
        }
    }

    void Start()
    {
        //initialize all line renderers to zero
        ZeroLR(axes);
        ZeroLR(components);
        ZeroLR(units);

        canvasScript = GetComponent<CanvasScript>();
    }
    #endregion

    #region Public Methods
    public void VectorComponents(Vector3 point, Transform origin, int index)
    {
        //ideally, you would feed the visualizer with info like the point and origin. but thats a lot of work so im just going off indexes<3
        relPos = GetRelativePosition(origin, point); //position of the point with the non-zero origin as new reference pt
        relMag = relPos.magnitude; //magnitude of the point with the
        canvasScript.setVariables(relPos, relMag, origin);
        if (index == 0)
        {
            for (int i = 0; i <= 3; i++)
            {
                VisualizeComponent(point, i, origin);
                VisualizeArcs(point, i, origin);
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
        {
            for (int i = 0; i <= 3; i++)
            {
                VisualizeUnitVectorComponent(point, i, origin);
            }
        }
    }
    #endregion

    #region Private Methods
    private void VisualizeArcs(Vector3 point, int index, Transform origin)
    {
        switch (index)
        {
            case 0:
                arcs[index].SetPosition(0, origin.position); //go from origin to 1/10 of the point on the x axis
                arcs[index].SetPosition(1, new Vector3(point.x - point.x/10f, origin.position.y, origin.position.z));
                break;
            case 1:
                arcs[index].SetPosition(0, origin.position); //go from origin to 1/10 of the point on the x axis
                arcs[index].SetPosition(1, new Vector3(origin.position.x, point.y - point.y/10f, origin.position.z));
                break;
            case 2:
                arcs[index].transform.position = origin.position;
                arcs[index].transform.rotation = origin.rotation;
                LaunchArcMesh arcMesh2 = arcs[index].GetComponentInChildren<LaunchArcMesh>();
                arcMesh2.SetAngle(Mathf.Rad2Deg * Mathf.Acos(relPos.z / relMag));
                break;
            default:
                Debug.Log("Error in VectorMath::VisualizeArcs(V3, i, T)");
                break;
        }
    }

    private void VisualizeAxes(Vector3 point, int index, Transform origin)
    {
        ZeroLR(components);
        ZeroLR(units);
        switch (index)
        {
            case 0:
                axes[index].SetPosition(0, point - origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, origin.position.z + 1));

                Instantiate(arrowHead);
                arrowHead.transform.position = new Vector3(origin.position.x, origin.position.y, origin.position.z + 1);
                arrowHead.transform.Rotate(new Vector3(0, 90, 0), Space.Self);

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 1:
                axes[index].SetPosition(0, origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y + 1, origin.position.z));

                Instantiate(arrowHead);
                arrowHead.transform.position = new Vector3(origin.position.x, origin.position.y + 1, origin.position.z);
                arrowHead.transform.Rotate(new Vector3(0, 0, 90), Space.Self);

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 2:
                axes[index].SetPosition(0, origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x + 1, origin.position.y, origin.position.z));

                Instantiate(arrowHead);
                arrowHead.transform.position = new Vector3(origin.position.x + 1, origin.position.y, origin.position.z);
                arrowHead.transform.Rotate(new Vector3(90, 0, 0), Space.Self);

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 3:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, point);

                Instantiate(arrowHead);
                arrowHead.transform.position = point;
                arrowHead.transform.Rotate(new Vector3(90, 0, 0), Space.Self);

                canvasScript.VisualizeText(components[index].GetPosition(1), 1, index);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, i, T)");
                break;
        }
    }

    private void VisualizeComponent(Vector3 point, int index, Transform origin)
    {
        ZeroLR(axes);
        ZeroLR(units);
        switch (index)
        {
            case 0:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(point.x, origin.position.y, origin.position.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);
                break;
            case 1:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(origin.position.x, point.y, origin.position.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);
                break;
            case 2:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, point.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);
                break;
            case 3:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, point);
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, T)");
                break;
        }
    }

    private void VisualizeUnitVectorComponent(Vector3 point, int index, Transform origin)
    {
        ZeroLR(axes);
        ZeroLR(components);
        switch (index)
        {
            case 0:
                adjustedPos = (relPos.x / relMag) + origin.position.x; //account for the offset in the origin
                Vector3 xComp = new Vector3(adjustedPos, origin.position.y, origin.position.z);
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, xComp);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 1:
                adjustedPos = (relPos.y / relMag) + origin.position.y; //account for the offset in the origin
                Vector3 yComp = new Vector3(origin.position.x, adjustedPos, origin.position.z);
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, yComp);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 2:
                adjustedPos = (relPos.z / relMag) + origin.position.z; //account for the offset in the origin
                Vector3 zComp = new Vector3(origin.position.x, origin.position.y, adjustedPos);
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, zComp);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 3: //resultant unit vector
                Vector3 oneVec = point - Vector3.one; 
                Vector3 ptNormal = (point - origin.position).normalized;
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, ptNormal + origin.position);
                canvasScript.VisualizeText(ptNormal, 2, index);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorUnitComponents(V3, T)");
                break;
        }
    }

    private void ZeroLR(LineRenderer[] lr)
    {
        for (int i = 0; i < lr.Length; i++) //how many linerenderers in the array?
        {
            int posCount = lr[i].positionCount; //how many vertices in that linerenderer?
            for (int j = 0; j < posCount; j++)
            {
                lr[i].SetPosition(j, Vector3.zero); //set this vertex to V3(0,0,0)
            }
        }
    }

    private Vector3 GetRelativePosition(Transform origin, Vector3 position)
    {
        float distanceX = position.x - origin.position.x;
        float distanceY = position.y - origin.position.y;
        float distanceZ = position.z - origin.position.z;

        Vector3 dif = position - origin.position; //uhhhh doesnt this do the same thing? am i dumb stupid?

        Vector3 relativePosition = new Vector3(distanceX, distanceY, distanceZ);
        return relativePosition;
    }
    #endregion
}
 