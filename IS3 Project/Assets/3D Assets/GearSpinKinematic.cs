using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class Gear : MonoBehaviour
{
    public int teethCount = 8;
    public float speed = 0f;
    [Range(-1, 1)]public int dir = 1; 

    public void Rotate()
    {
        transform.Rotate(new Vector3(0, 0, speed*dir), Space.Self);
    }
}

public class GearSpinKinematic : MonoBehaviour
{
    /* Gear Rotation Formula:
    Gear Ratio (GR) = Driven Teeth Count / Driver Teeth Count
    Driven Gear Speed = 
    */

    public List<Gear> gears;

    void Start()
    {

    }

    void FixedUpdate()
    {
        foreach (Gear gear in gears)
        {
            //ig wait a bit
            gear.Rotate();
        }

    }
}
