﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class allows the programmer to quickly make vector calculations and create a visual output
/// for the user seamlessly. It is referred in the _Placement script. 
/// </summary>

//for quaternion: length of vector + i + j + k = orientation
[RequireComponent(typeof(CanvasScript))]
public class VectorMath : MonoBehaviour
{
    #region Private Variables
    private float relMag, adjustedPos;
    private Vector3 relPos;

    private GameObject xArrow, yArrow, zArrow, rArrow;

    private CanvasScript canvasScript = null;
    public Text help;
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

   // [SerializeField, Tooltip("The markers for where the arc should begin")]
   // private GameObject[] dots = null;

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
        ZeroLR(arcs);

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
    private void VisualizeAxes(Vector3 point, int index, Transform origin)
    {
        ZeroLR(components);
        ZeroLR(units);
        ZeroLR(arcs);
        // Destroy(xArrow); Destroy(yArrow); Destroy(zArrow);
        AddArrowHead(origin.position + Vector3.one, index, origin);
        switch (index)
        {
            case 0:
                axes[index].SetPosition(0, origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x + 1, origin.position.y, origin.position.z));

                /*arcs[index].SetPosition(0, origin.position + (point-origin.position)/4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, origin.position.z + .25f));*/

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 1:
                axes[index].SetPosition(0, origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y + 1, origin.position.z));

                /*arcs[index].SetPosition(0, origin.position + (point - origin.position) / 4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y + .25f, origin.position.z));*/

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 2:
                axes[index].SetPosition(0, origin.position);
                axes[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, origin.position.z + 1));

                /*arcs[index].SetPosition(0, origin.position + (point - origin.position) / 4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x + .25f, origin.position.y, origin.position.z));*/

                canvasScript.VisualizeText(axes[index].GetPosition(1), 1, index);
                break;
            case 3:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, point);
              //  AddArrowHead(point, index, origin);
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
        //ZeroLR(arcs);
        AddArrowHead(point, index, origin);
        switch (index)
        {
            case 0:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(point.x, origin.position.y, origin.position.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);

                arcs[index].SetPosition(0, origin.position + (point - origin.position) / 4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x+(point.x - origin.position.x)/4f, origin.position.y, origin.position.z));
                break;
            case 1:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(origin.position.x, point.y, origin.position.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);

                arcs[index].SetPosition(0, origin.position + (point - origin.position) / 4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y + (point.y - origin.position.y) / 4f, origin.position.z));
                break;
            case 2:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, point.z));
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);

                arcs[index].SetPosition(0, origin.position + (point - origin.position) / 4f);
                arcs[index].SetPosition(1, new Vector3(origin.position.x, origin.position.y, origin.position.z + (point.z - origin.position.z) / 4f));
                break;
            case 3:
                components[index].SetPosition(0, origin.position);
                components[index].SetPosition(1, point);
                canvasScript.VisualizeText(components[index].GetPosition(1), 0, index);
              //  AddArrowHead(point, index, origin);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorComponents(V3, T)");
                break;
        }
    }

    private float GetUnitVector(Vector3 point, int index, Transform origin)
    {
        switch (index)
        {
            case 0: //get x unit vec
                adjustedPos = (relPos.x / relMag) + origin.position.x; //account for the offset in the origin
                return adjustedPos;
            case 1:
                adjustedPos = (relPos.y / relMag) + origin.position.y;
                return adjustedPos;
            case 2:
                adjustedPos = (relPos.z / relMag) + origin.position.z;
                return adjustedPos;
             default:
                Debug.Log("Error VectorMath:GetUnitVector, please check correct index call. Returning 0.");
                break;
        }
        return 0;
    }

    private void VisualizeUnitVectorComponent(Vector3 point, int index, Transform origin)
    {
        ZeroLR(axes);
        ZeroLR(components);
        ZeroLR(arcs);
        switch (index)
        {
            case 0:
               // adjustedPos = (relPos.x / relMag) + origin.position.x; //account for the offset in the origin
                Vector3 xComp = new Vector3(GetUnitVector(point, index, origin), origin.position.y, origin.position.z);
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, xComp);
                AddArrowHead(xComp, index, origin);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 1:
               // adjustedPos = (relPos.y / relMag) + origin.position.y; //account for the offset in the origin
                Vector3 yComp = new Vector3(origin.position.x, GetUnitVector(point, index, origin), origin.position.z);
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, yComp);
                AddArrowHead(yComp, index, origin);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 2:
                //adjustedPos = (relPos.z / relMag) + origin.position.z; //account for the offset in the origin
                Vector3 zComp = new Vector3(origin.position.x, origin.position.y, GetUnitVector(point, index, origin));
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, zComp);
                AddArrowHead(zComp, index, origin);
                canvasScript.VisualizeText(units[index].GetPosition(1), 2, index);
                break;
            case 3: //resultant unit vector
                Vector3 oneVec = point - Vector3.one; 
                Vector3 ptNormal = (point - origin.position).normalized;
                units[index].SetPosition(0, origin.position);
                units[index].SetPosition(1, ptNormal + origin.position);
                //AddArrowHead(ptNormal + origin.position, index, origin);
                canvasScript.VisualizeText(ptNormal, 2, index);
                break;
            default:
                Debug.Log("Something went wrong in the for loop from VectorMath::vectorUnitComponents(V3, T)");
                break;
        }
    }

    private void AddArrowHead(Vector3 point, int index, Transform origin)
    {
        switch (index)
        {
            case 0:
                Destroy(xArrow);
                xArrow = Instantiate(arrowHead);

                var xArrowRenderer = xArrow.GetComponentInChildren<Renderer>();
                xArrowRenderer.material = Resources.Load("Materials/xArrow", typeof(Material)) as Material;

                xArrow.transform.position = new Vector3(point.x, origin.position.y, origin.position.z);

                if (point.x - origin.position.x > 0) //pos
                    xArrow.transform.Rotate(new Vector3(0, 270, 0), Space.Self);
                else
                    xArrow.transform.Rotate(new Vector3(0, 90, 0), Space.Self);
                break;
            case 1:
                Destroy(yArrow);
                yArrow = Instantiate(arrowHead);
                yArrow.transform.position = new Vector3(origin.position.x, point.y, origin.position.z);

                var yArrowRenderer = yArrow.GetComponentInChildren<Renderer>();
                yArrowRenderer.material = Resources.Load("Materials/yArrow", typeof(Material)) as Material;

                if (point.y - origin.position.y > 0) //pos
                    yArrow.transform.Rotate(new Vector3(90, 0, 0), Space.Self);
                else //neg
                    yArrow.transform.Rotate(new Vector3(270, 0, 0), Space.Self);
                break;
            case 2:
                Destroy(zArrow);
                zArrow = Instantiate(arrowHead);
                zArrow.transform.position = new Vector3(origin.position.x, origin.position.y, point.z);

                var zArrowRenderer = zArrow.GetComponentInChildren<Renderer>();
                zArrowRenderer.material = Resources.Load("Materials/zArrow", typeof(Material)) as Material;

                if (point.z - origin.position.z > 0)
                    zArrow.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
                else
                    zArrow.transform.Rotate(new Vector3(0, 0, 0));
                break;
            case 3:
                Destroy(rArrow);
                rArrow = Instantiate(arrowHead);

                rArrow.transform.LookAt(origin.position);
                break;
            default:
                Debug.Log("Error with VectorMath:AddArrowhead");  //bruh moment
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
 