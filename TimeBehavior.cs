using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeBehaviour: MonoBehaviour
{

    public static DateTime Date { get; set;}

    public static float TimeSpeed { get; set;} // seconds per second (60 fps)

    void Start()
    {
        Date = new DateTime(1984, 1, 1);

    }

    void Update()
    {
        Date.AddSeconds(TimeSpeed/60);
    }

}