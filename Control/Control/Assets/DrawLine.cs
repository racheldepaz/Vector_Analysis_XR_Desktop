﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{

    public Transform origin;
    public Transform to;
    private LineRenderer lr; 

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, origin.position);
        lr.SetWidth(.016f, 0);
    }

    void Update()
    {
        lr.SetPosition(1, to.position);
    }
}
