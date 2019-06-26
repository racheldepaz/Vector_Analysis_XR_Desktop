using UnityEngine;
using UnityEngine.UI; 


public class CanvasScript : MonoBehaviour
{
    #region Text Boxes
    [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
    public Text _distanceLabel;

    [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
    public Text _magnitudeLabel;
    public Text _angleLabel;
    #endregion

    #region Essential Variables
    public Vector3 pos;
    public float mag;
    #endregion

    #region Public Methods
    //set the value of referred variables as global var. referred to by VectorMath::Update
    public void setVariables(Vector3 position, float magnitude)
    {
        pos = position;
        mag = magnitude;
    }
    #endregion

    #region Unity Methods
    void Awake()
    {
        _distanceLabel.text = null;
        _magnitudeLabel.text = null;
        _angleLabel.text = null; 
    }
    void Update()
    {
        _distanceLabel.text = "Distance from origin: " + pos.ToString("N2");
        _magnitudeLabel.text = "Magnitude: " + mag.ToString("N2");
    } 
    #endregion
}
