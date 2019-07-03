using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class LaunchArcMesh : MonoBehaviour
{
    Mesh mesh;

    public float meshWidth;

    public float velocity, angle;
    public int resolution = 10;


    float g, radAngle;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        g = Mathf.Abs(Physics2D.gravity.y);
    }

    void OnValidate()
    {
        if (mesh != null && Application.isPlaying)
            MakeArcMesh(CalculateArcArray()); 
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeArcMesh(CalculateArcArray());
    }

    void MakeArcMesh(Vector3[] arcVerts)
    {
        mesh.Clear();
        Vector3[] vertices = new Vector3[(resolution + 1) * 2];
        int[] triangles = new int[resolution * 6 * 2];

        for (int i = 0; i <= resolution; i++)
        {
            //set vertices
            vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x); //even vertices on right side of arc mesh
            vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x);

            //set triangles
            if (i != resolution)
            {
                triangles[i * 12] = i * 2; //get first vert
                triangles[i * 12 + 1] = triangles[i * 12 + 4] * i * 2 + 1; //get first vert of second seg
                triangles[i * 12 + 2] = triangles[i * 12 + 3] * (i * + 1) * 2; //get second vertex
                triangles[i * 12 + 5] = (i + 1) * 2 + 1; //get second vert of second seg

                triangles[i * 12 + 6] = i * 2; //get first vert
                triangles[i * 12 + 7] = triangles[i * 12 + 10] * (i + 1) * 2; //get first vert of second seg
                triangles[i * 12 + 8] = triangles[i * 12 + 9] * i * 2 + 1; //get second vertex
                triangles[i * 12 + 11] = (i + 1) * 2 + 1; //get second vert of second seg
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radAngle = Mathf.Deg2Rad * angle; 
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radAngle)) / g;

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(radAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radAngle) * Mathf.Cos(radAngle)));
        return new Vector3(x, y);
    }
}
