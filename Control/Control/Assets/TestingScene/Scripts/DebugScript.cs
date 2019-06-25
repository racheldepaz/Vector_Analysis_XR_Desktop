using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void updatePosition(Vector3 originalPos, Vector3 newPos)
    {
        lineRenderer.SetPosition(0, originalPos);
        lineRenderer.SetPosition(1, newPos);
    }
}
