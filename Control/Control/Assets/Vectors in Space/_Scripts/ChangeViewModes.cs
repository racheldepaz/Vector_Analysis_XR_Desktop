using MagicLeap;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(VectorMath))]
public class ChangeViewModes : MonoBehaviour
{
    #region GameObjects
    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz axes")]
    private GameObject[] axes = null;

    [SerializeField, Tooltip("The Line Renderer game objects that represent the xyz + resultant vector components")]
    private GameObject[] components = null;

    [SerializeField, Tooltip("The Line Rendere game objects that represnet the xyz + resultant unit vector components")]
    private GameObject[] units = null;

    [SerializeField, Tooltip("The curve renderers?? idk how this will work yet")]
    private GameObject[] angles = null;

    private _Placement _placement = null;

    private _Placement.ViewMode lastViewMode = _Placement.ViewMode.Axis;
    #endregion


     void Awake()
    {
        if (null == axes)
        {
            Debug.LogError("Error: ChangeViewMode.axes not set.");
            return;
        }
        if (null == units)
        { }

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}