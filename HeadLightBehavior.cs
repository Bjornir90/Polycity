using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLightBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var date = GameObject.FindWithTag("TimeManagerTag").GetComponent<TimeBehavior>().Date;
        var light = gameObject.GetComponent<Light>();
        if(date.Hour >= 20 || date.Hour < 6){
            light.intensity = 1;
        }
        else
            light.intensity = 0;
    }
}
