using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarBehavior : MonoBehaviour
{

    public float Speed;

    private Vector3 currentDestination;

    private bool arrived;

    private Vector3 start, end;

    private Trip trip;
    // Start is called before the first frame update
    void Start()
    {
        /*
        List<Vector3> nodes = new List<Vector3>();

        nodes.Add(new Vector3(0, 0, 0));

        nodes.Add(new Vector3(-30, 0, 0));

        nodes.Add(new Vector3(-30, 0, 40));

        trip = new Trip(nodes, 3);

        currentDestination = trip.GetNextNode();
        */
        start = new Vector3(0, 1000, 0);
        end = new Vector3(0, 1000, 0);
        arrived = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(arrived)
            return;


        if(start.y == 1000){
            if(Input.GetMouseButton(1)){
                start = GridBehavior.GetOnPlaneClick();
            }
        } else if(end.y == 1000){
            if(Input.GetMouseButton(1)){
                end = GridBehavior.GetOnPlaneClick();
            }
        }
        if(trip == null){
            if(Input.GetKeyDown("j")){
                trip = GameObject.FindWithTag("GridManagerTag").GetComponent<GridBehavior>().grid.GetTrip(start, end);
            } else {
                return;
            }
        }

        if(currentDestination == null){
            currentDestination = trip.GetNextNode();
            transform.SetPositionAndRotation(start, Quaternion.Euler(0,0,0));
        }

        Vector3 carPosition = transform.position;

        if(Math.Abs(carPosition.x - currentDestination.x) < Speed && Math.Abs(carPosition.z - currentDestination.z) < Speed){
            
            Debug.Log("Startoing trup");
            transform.position = currentDestination;
            if(trip.isFinal()){
                arrived = true;
                return;
            }

            currentDestination = trip.GetNextNode();
            return;

        }

        //X is the same for the car and the destination : we move on Z
        if(Math.Abs(carPosition.x - currentDestination.x) < Speed){
            
            //Destination is towards positive Z 
            if(carPosition.z - currentDestination.z < 0){
                transform.Translate(0, 0, Speed, Space.World);
            } else {
                transform.Translate(0, 0, -Speed, Space.World);
            }

        } else {

            //Destination is towards positive X
            if(carPosition.x - currentDestination.x < 0){
                transform.Translate(Speed, 0, 0, Space.World);
            } else {
                transform.Translate(-Speed, 0, 0, Space.World);
            }

        }

    }
}
