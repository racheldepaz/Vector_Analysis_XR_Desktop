using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasScript : MonoBehaviour
{
    #region Text Boxes
    [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
    public Text _distanceLabel;

    [SerializeField, Tooltip("TMP Distance X")]
    public TextMeshPro distanceX;

    [SerializeField, Tooltip("TMP Distance Y")]
    public TextMeshPro distanceY;

    [SerializeField, Tooltip("TMP Distance Y")]
    public TextMeshPro distanceZ;

    [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
    public Text _magnitudeLabel;
    public Text _angleLabel;
    #endregion

    #region Essential Variables
    public Vector3 pos;
    public float mag;
    public float angleX, angleY, angleZ;
    #endregion

    #region Public Methods
    //set the value of referred variables as global var. referred to by VectorMath::Update
    public void setVariables(Vector3 position, float magnitude)
    {
        pos = position;
        mag = magnitude;
        SetAngleVals();
    }
    #endregion

    #region Private Methods
    private void SetAngleVals()
    {
        angleX = Mathf.Rad2Deg * Mathf.Acos(pos.x / mag);
        angleY = Mathf.Rad2Deg * Mathf.Acos(pos.y / mag);
        angleZ = Mathf.Rad2Deg * Mathf.Acos(pos.z / mag);

        if (angleX > 90)
            angleX -= 90;
        if (angleY > 90)
            angleY -= 90;
        if (angleZ > 90)
            angleZ -= 90;
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
        _distanceLabel.text = "Distance from origin: " + pos.ToString("N2") + "(meters)";
        _magnitudeLabel.text = "Magnitude: " + mag.ToString("N2") + "(meters)";
        _angleLabel.text = "X Angle: " + angleX.ToString("N2") + "°" + " Y Angle: " + angleY.ToString("N2") + "°" +  " Z Angle: " + angleZ.ToString("N2") + "°";
    } 
    #endregion
}
