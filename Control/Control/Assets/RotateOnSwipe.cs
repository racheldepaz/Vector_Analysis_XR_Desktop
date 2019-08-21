using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class RotateOnSwipe : MonoBehaviour
{
    [SerializeField]
    private MLInputController controller;

    private float rotateRate = 180;
    private float magTouchX;
    private float lastX; 

    // Start is called before the first frame update
    void Start()
    {
        magTouchX = 1;
        lastX = 0; 
    }

    // Update is called once per frame
    void Update()
    {
        CheckStatus(); 
        transform.Rotate(0, magTouchX * rotateRate, 0);
    }

    void CheckStatus()
    {
        if (controller.Touch1Active)
        {
            if (controller.Touch1PosAndForce.x - lastX < -0.001)
                magTouchX -= rotateRate;
            if (controller.Touch1PosAndForce.x - lastX > 0.001)
                magTouchX += rotateRate;
            lastX = controller.Touch1PosAndForce.x;
        }
    }
}
