using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    public GameObject origin;
    public GameObject to; 

    void Start()
    {
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, origin.transform.localPosition);
        lr.SetPosition(1, to.transform.localPosition);
    }
}
