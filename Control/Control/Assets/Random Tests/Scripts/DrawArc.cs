using System.Collections;
using System; 
using UnityEngine;

public class DrawArc : MonoBehaviour
{
    public float velocity;

    [Range(0.01f, 89f)]
    public float angle; 

    public int resolution = 10;

    [SerializeField]
    LineRenderer lr; 

    private float g;

    public float radAngle; 

    void Awake()
    {
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    void OnValidate()
    {
        if (lr != null && Application.isPlaying)
         RenderArc();  
    }

    void Start()
    {
        RenderArc(); 
    }

    void RenderArc()
    {

        lr.positionCount = (resolution + 1);
        lr.SetPositions(CalculateArcArray());
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];
        radAngle = Mathf.Rad2Deg * angle; 
        float maxDistance = ((velocity * velocity) * Mathf.Sin(2 * radAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = ArcPositions(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 ArcPositions(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radAngle) - ((g * (x * x)) / (2 * ((velocity * Mathf.Cos(radAngle))*(velocity*(Mathf.Cos(radAngle))))));
        return new Vector3(x, y);
    }


}