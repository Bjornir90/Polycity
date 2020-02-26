using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightBehavior : MonoBehaviour
{

    private Vector3Int cellGridPosition;
    // Start is called before the first frame update
    void Start()
    {
        cellGridPosition = GridBehavior.GetGridIndex(transform.position);
        //Debug.Log("Position réelle : " + transform.position);
        //Debug.Log("Index de la lampe : " + cellGridPosition);
    }

    // Update is called once per frame
    void Update()
    {
        checkExistence();
        var date = GameObject.FindWithTag("TimeManagerTag").GetComponent<TimeBehavior>().Date;
        var light = gameObject.GetComponent<Light>();
        if(date.Hour >= 20 || date.Hour < 6){
            light.intensity = 1;
        }
        else
            light.intensity = 0;
    }

    void checkExistence(){
        var grid = GameObject.FindWithTag("GridManagerTag").GetComponent<GridBehavior>().grid;
        if(!grid.GetCell(cellGridPosition).hasLamp)
            Destroy(gameObject);
    }
}
