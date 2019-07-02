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
    private Vector3 zero = new Vector3(0, 0, 0);

    private CanvasScript canvasScript = null;
    #endregion

    #region Serialized Variables
    [SerializeField, Tooltip("Line Renderers")]
    private LineRenderer[] lineRenderers = null;
    #endregion

    #region Unity Methods
    void Start()
    {
        //initialize all line renderers to zero
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            lineRenderers[i].SetPosition(0, zero);
            lineRenderers[i].SetPosition(1, zero);
        }

        canvasScript = GetComponent<CanvasScript>(); 
    }
    #endregion

    #region Public Methods
    public void VectorComponents(Vector3 point, Transform origin)
    {
        relPos = GetRelativePosition(origin, point); //position of the point with the non-zero origin as new reference pt
        relMag = relPos.magnitude; //magnitude of the point with the
        canvasScript.setVariables(relPos, relMag);
        for (int i = 0; i <= 3; i++)
        {
            VisualizeComponent(point, i, origin);
        }

    }

    public void VectorUnitComponents(Vector3 point, Transform origin)
    {
        relPos = GetRelativePosition(origin, point);
        relMag = relPos.magnitude;
        for(int i = 0; i <= 3; i++)
        {
            VisualizeUnitVectorComponent(point, i, origin);
        }

        canvasScript.setVariables(relPos, relMag);
    }
    #endregion

    #region Private Methods
    private void ZeroLR(bool unit)
    {
        if (unit)
        {
            for (int i = 4; i < 9; i++)
            {
                lineRenderers[i].SetPosition(0, zero);
                lineRenderers[i].SetPosition(1, zero);
            }

        }
        else
        {
            for (int j = 0; j < 4; j++)
            {
                lineRenderers[j].SetPosition(0, zero);
                lineRenderers[j].SetPosition(1, zero);
            }
        }
    }
    private void VisualizeComponent(Vector3 point, int index, Transform origin)
    {
        ZeroLR(false);
        switch (index)
        {
            case 0:
                lineRenderers[index+4].SetPosition(0, origin.position);
                lineRenderers[index+4].SetPosition(1, new Vector3(point.x, origin.position.y, origin.position.z));
                break;
            case 1:
                lineRenderers[index+4].SetPosition(0, origin.position);
                lineRenderers[index + 4].SetPosition(1, new Vector3(origin.position.x, point.y, origin.position.z));
                break;
            case 2:
                lineRenderers[index + 4].SetPosition(0, origin.position);
                lineRenderers[index + 4].SetPosition(1, new Vector3(origin.position.x, origin.position.y, point.z));
                break;
            case 3:
                lineRenderers[index + 4].SetPosition(0, origin.position);
                lineRenderers[index + 4].SetPosition(1, point);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, T)");
                break;
        }
    }

    private void VisualizeUnitVectorComponent(Vector3 point, int index, Transform origin)
    {
        ZeroLR(true);
        switch (index)
        {
            case 0:
                adjustedPos = ((point.x - origin.position.x) / relMag) + origin.position.x; //account for the offset in the origin
                Vector3 xComp = new Vector3(adjustedPos, origin.position.y, origin.position.z);
                lineRenderers[index].SetPosition(0, origin.position);
                lineRenderers[index].SetPosition(1, xComp);
                break;
            case 1:
                adjustedPos = ((point.y - origin.position.y) / relMag) + origin.position.y; //account for the offset in the origin
                Vector3 yComp = new Vector3(origin.position.x, adjustedPos, origin.position.z);
                lineRenderers[index].SetPosition(0, origin.position);
                lineRenderers[index].SetPosition(1, yComp);
                break;
            case 2:
                adjustedPos = ((point.z - origin.position.z) / relMag) + origin.position.z; //account for the offset in the origin
                Vector3 zComp = new Vector3(origin.position.x, origin.position.y, adjustedPos);
                lineRenderers[index].SetPosition(0, origin.position);
                lineRenderers[index].SetPosition(1, zComp);
                break;
            case 3:
                point.Normalize();
                lineRenderers[index].SetPosition(0, origin.position);
                lineRenderers[index].SetPosition(1, point);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorUnitComponents(V3, T)");
                break;
        }
    }


    private Vector3 GetRelativePosition(Transform origin, Vector3 position)
    {
        float distanceX = position.x - origin.position.x;
        float distanceY = position.y - origin.position.y;
        float distanceZ = position.z - origin.position.z;

        Vector3 relativePosition = new Vector3(distanceX, distanceY, distanceZ);
        return relativePosition;
    }
    #endregion
}
 