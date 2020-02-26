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
        TimeSpeed = 600;
    }

    void Update()
    {
        int previousMin = Date.Minute;
        Date = Date.AddMilliseconds(TimeSpeed/0.06);
        updateSunlight();
        updateSunDirection();
    }

    void updateSunlight(){
        Light sunLight = GameObject.FindWithTag("MainLightTag").GetComponent<Light>();
        float dayRate = 1;
        float nightRate = 0.05f;
        float rate;
        if(9 <= Date.Hour && Date.Hour < 17){
            sunLight.intensity = dayRate;
            return;
        }
        if((0 <= Date.Hour && Date.Hour < 5) || (21 <= Date.Hour && Date.Hour <= 23)){
            sunLight.intensity = nightRate;
            return;
        }
        if(5 <= Date.Hour && Date.Hour < 9){
            int totalMinutes = (Date.Hour - 5) * 60 + Date.Minute;
            rate = nightRate + totalMinutes/240f * (dayRate - nightRate);
            sunLight.intensity = rate;
            return;
        } else {
            int totalMinutes = (Date.Hour - 17) * 60 + Date.Minute;
            rate = dayRate - totalMinutes/240f * (dayRate - nightRate);
            sunLight.intensity = rate;
        }
    }

    void updateSunDirection(){
        var sun = GameObject.FindWithTag("MainLightTag");
        float totalMinutes = (Date.Hour - 12) * 60 + Date.Minute;
        float angle = totalMinutes/1440f * 170f + 90f;
        Vector3 rotation = new Vector3(angle, -30, 0);
        sun.transform.localEulerAngles = rotation;
    }
}