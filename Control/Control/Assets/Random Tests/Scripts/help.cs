using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.XR.MagicLeap;

public class help : MonoBehaviour
{
    [SerializeField]
    Text text = null;

    [SerializeField]
    Text indexval = null;

    [SerializeField]
    ControllerConnectionHandler handler; 

    private int index; 
    // Start is called before the first frame update
    void Start()
    {
        MLInput.OnTriggerDown += HandleOnTriggerDown;
        MLInput.OnTriggerUp   += HandleOnTriggerUp;

        MLInputController controller = handler.ConnectedController;

        index = 0; 
    }

    void OnDestroy()
    {
        MLInput.OnTriggerDown -= HandleOnTriggerDown;
        MLInput.OnTriggerUp -= HandleOnTriggerUp; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleOnTriggerDown(byte controllerId, float pressure)
    {
        text.text = "Trigger is down";
        index++;
        indexval.text = "Index: " + index; 
    }

    void HandleOnTriggerUp(byte controllerId, float pressure)
    {
        text.text = "Trigger is up";
    }
}
