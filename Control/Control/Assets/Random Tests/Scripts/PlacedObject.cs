using System;
using UnityEngine.XR.MagicLeap;
using UnityEngine; 

public class PlacedObject : MonoBehaviour
{
    #region Variables
    [Tooltip("Can an object be rotated?")]
    public bool canRotate;

    private float rotValue;
    #endregion

    public float rotationOffset
    {
        get { return rotValue; }
        set { rotValue = value; }
    }
}
