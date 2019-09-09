using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.XR.MagicLeap;
using System.Collections.Generic;

public class LookAndTrack : MonoBehaviour
{

    public List<string> rowData;
    private int count = 0;

    [SerializeField]
    private Text eyeConf = null;

    [SerializeField]
    private Text conf = null; 

    public ControllerConnectionHandler handler;

    // Start is called before the first frame update
    void Start()
    {
        MLEyes.Start();
        MLInput.OnControllerButtonDown += HandleOnButtonDown;
 
        conf.text = "";

        rowData.Add("This spreadsheet displays the fixation point vector and time when the eye position was recorded.");

        rowData.Add("Date and Time, x-position, y-position, z-position");
      
    }

    // Update is called once per frame
    void Update()
    {
        if (MLEyes.FixationConfidence > 0.75)
        {
            eyeConf.text = "Current eye position: " + MLEyes.FixationPoint.ToString();
            Vector3 fixationPoint = MLEyes.FixationPoint;
            rowData.Add(System.DateTime.Now.ToString("MM_dd_yyyy__HH_mm_ss") + "," + fixationPoint.ToString());
        }
    }

    void OnDestroy()
    {
        MLEyes.Stop();
        MLInput.OnControllerButtonDown -= HandleOnButtonDown;
    }

    #region Event Handlers
    private void HandleOnButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (handler.ConnectedController.Id == controllerId && button == MLInputControllerButton.Bumper)
        {
            //conf.text = "button pressed";
            string filename = "sampledata" + ".csv";
            string extension = System.IO.Path.GetExtension(filename);
            string pathName = System.IO.Path.Combine(Application.persistentDataPath, filename);
            StreamWriter outstream = File.CreateText(pathName);
            foreach (string s in rowData)
            { 
                outstream.Write(s);
                outstream.WriteLine();
            }
            conf.text = "Data sucessfully outputted.";
        }
    }

    #endregion

}
