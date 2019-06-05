using UnityEngine;
using UnityEngine.UI;

public class script : MonoBehaviour
{

    [SerializeField, Tooltip("The GameObject whose values are being modified.")]
    private GameObject content = null;

    [SerializeField, Tooltip("The Text element that will display the vector's distance from origin.")]
    private Text _distanceLabel = null;

    [SerializeField, Tooltip("The Text element that will display the vector's magnitude.")]
    private Text _magLabel = null;

   /* [SerializeField, Tooltip("The Text element that will display the vector's angle to the origin")]
    private Text _angleLabel = null; big sad this does not work */

    private Vector3 placedObject = new Vector3(0,0,0);
    private Vector3 origin = new Vector3(0, 0, 0);
    private GameObject xArrow, yArrow, zArrow; 

    private int count;


    // Start is called before the first frame update
    void Start()
    {
        //initial position of the point
        //placedObject = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);
        count = 0;

        GameObject thing = Instantiate(content);
        Debug.Log("Instance stuff: " + thing.transform.position.x + " " + thing.transform.position.y); 
    }

    // Update is called once per frame
    void Update()
    {
        // content.gameObject.SetActive(true);

//        float xpos, ypos, zpos;
        //Vector3 newPlacement = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);
        Vector3 newPlacement = new Vector3(content.transform.position.x, content.transform.position.y, content.transform.position.z);

        float xpos = content.transform.position.x;
        float ypos = content.transform.position.y;
        float zpos = content.transform.position.z;


        if (placedObject != newPlacement || count == 0)
        {
            Destroy(xArrow);
            Destroy(yArrow);
            Destroy(zArrow);

            Debug.Log("Vector position: " + placedObject.ToString());
            _distanceLabel.text = "Coordinates: " + newPlacement.ToString("N3");
            _magLabel.text = "Magnitude: " + newPlacement.magnitude.ToString("N3");

            /*Angle stuff that doesnt work 
             * float angle = Vector3.Angle(newPlacement, origin) * 100;
            Debug.Log("Angle: " + angle);
            _angleLabel.text = "Angle: " + angle; */

            //Get the absolute value of the position to start creating the arrows
            float x_abs = Mathf.Abs(xpos);
            float y_abs = Mathf.Abs(ypos);
            float z_abs = Mathf.Abs(zpos);

            //create the three lines
            xArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            yArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zArrow = GameObject.CreatePrimitive(PrimitiveType.Cube);

            Vector3 newXPos = new Vector3(xpos / 2.0F, 0, 0);
            Vector3 newYPos = new Vector3(0, ypos / 2.0F, 0);
            Vector3 newZPos = new Vector3(0, 0, zpos/2.0F);
            //xArrow.transform.position.x = new Vector3(x_abs/2.0F, 0, 0);

            xArrow.transform.position = newXPos;
            yArrow.transform.position = newYPos;
            zArrow.transform.position = newZPos;

            //scale down the primitives
            xArrow.transform.localScale = new Vector3(x_abs, 0.01F, 0.01F);
            yArrow.transform.localScale = new Vector3(0.01F, y_abs, 0.01F);
            zArrow.transform.localScale = new Vector3(0.01F, 0.01F, z_abs);

            Debug.Log("Goodbye, objects");
            count++; 
       }
    }
}
