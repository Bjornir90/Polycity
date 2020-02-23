using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeBehavior: MonoBehaviour
{

    public DateTime Date { get; set;}

    public float TimeSpeed {get; set;}// seconds per second (60 fps)

    void Start()
    {
        Date = new DateTime(1984, 1, 1, 0, 0, 0);
        TimeSpeed = 60;
    }

    void Update()
    {
        int previousMin = Date.Minute;
        Date = Date.AddMilliseconds(TimeSpeed/0.06);

    }

}