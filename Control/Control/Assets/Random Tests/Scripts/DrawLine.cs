using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    #region Public Variables
    //public Transform from;
   // public Vector3 to;
    #endregion

    #region Private Variables
    private LineRenderer lr;
    #endregion

    #region Unity Methods
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetWidth(0, .016f);
    }
    #endregion

    #region Public Methods
    public void drawConnection(Transform origin, Vector3 dest)
    {
        lr.SetPosition(0, origin.position);
        lr.SetPosition(1, dest); 
    }
    #endregion

}