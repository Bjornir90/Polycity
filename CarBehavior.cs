using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarBehavior : MonoBehaviour
{

    public const float MIN_SPEED = 0.5f, MAX_SPEED = 1.5f;
    public const int MIN_WAIT_LENGTH = 5, MAX_WAIT_LENGTH = 60; // minutes
    public float ActualSpeed; // max speed in m.s-1

    private float Speed;

    private Vector3 currentDestination;

    private DateTime returnDate;
    private bool standBy;

    public BuildingBehavior home {get; set;}

    private TimeBehavior TimeManager;

    public Vector3 start {get; set;} 
    public Vector3 end {get; set;}

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
        ActualSpeed = UnityEngine.Random.Range(MIN_SPEED/1000f, MAX_SPEED/1000f);
        TimeManager = GameObject.FindWithTag("TimeManagerTag").GetComponent<TimeBehavior>();
        start = new Vector3(0, 1000, 0);
        end = new Vector3(0, 1000, 0);
        standBy = false;
        currentDestination = new Vector3(0, 1000, 0);
        Speed = ActualSpeed*TimeManager.TimeSpeed;
        //Debug.Log("Vitesse : " + Speed);
    }

    // Update is called once per frame
    void Update()
    {
        // Entering trip manually
        /*
        if(start.y == 1000){
            if(Input.GetMouseButtonDown(1)){
                start = GridBehavior.GetWorldGridPosition(GridBehavior.GetOnPlaneClick());
                Debug.Log("Starting node : "+start.ToString());
            }
        } else if(end.y == 1000 && start.y != 1000){
            if(Input.GetMouseButtonDown(1)){
                end = GridBehavior.GetWorldGridPosition(GridBehavior.GetOnPlaneClick());
                Debug.Log("Ending node : "+end.ToString());
            }
        }
        if(trip == null){
            if(Input.GetKeyDown("j")){
                trip = GameObject.FindWithTag("GridManagerTag").GetComponent<GridBehavior>().grid.GetTrip(start, end);
                Debug.Log("Trip length : "+trip.Length);
            } else {
                return;
            }
        }
        */

        if(trip == null)
            return;

        if(standBy){
            if(TimeManager.Date < returnDate)
                return;
            else
            {
                standBy = false;
                //Debug.Log("Trajet RETOUR");
                gameObject.GetComponent<Renderer>().enabled = true;
                Vector3 _yModify = transform.position;
                _yModify.y = 0;
                transform.position = _yModify;
            }
        }

        /* int nodenb=0;
        foreach(Vector3 posi in trip.Nodes){
            Debug.Log("Node "+ ++nodenb + " : " + posi);
        }
 */
        Vector3 previousPosition = new Vector3(0, 1000, 0);
        foreach (var node in trip.Nodes){
            if(previousPosition == new Vector3(0, 1000, 0)){
                previousPosition = node;
                continue;
            }

            //Debug.DrawLine(previousPosition, node, Color.red, trip.Length);
            previousPosition = node;

        }
        {
            
        }

        if(currentDestination.y == 1000){
            currentDestination = trip.GetNextNode();
            //throw new Exception(currentDestination.ToString());
            //transform.SetPositionAndRotation(start, Quaternion.Euler(0,0,0));
            transform.position = currentDestination;
        }

        Vector3 carPosition = transform.position;

        if(Math.Abs(carPosition.x - currentDestination.x) < Speed && Math.Abs(carPosition.z - currentDestination.z) < Speed){

            transform.position = currentDestination;
            if(trip.isFinal()){
                if(trip.isReturnTrip){
                    home.AddCar(1);
                    Destroy(gameObject);
                    return;
                }
                standBy = true;
                //Debug.Log("Trajet ALLEE (terminé)");
                System.Random rnd = new System.Random();
                int minToWait = rnd.Next(MIN_WAIT_LENGTH, MAX_WAIT_LENGTH);
                returnDate = TimeManager.Date.AddMinutes(minToWait);
                gameObject.GetComponent<Renderer>().enabled = false; // Makes the car disappear
                Vector3 _yModify = transform.position;
                _yModify.y = 1000;
                transform.position = _yModify;
                trip = trip.GetReturnTrip();
                return;
            }

            currentDestination = trip.GetNextNode();
            return;
        }

        RaycastHit hit;
        Vector3 local = new Vector3(0, -10, 0);
        Ray rayFromCar = new Ray(transform.TransformPoint(local), transform.forward); // Direction will be changed anyway
        float speedRate = 1f; // this rate will make the car slow down if another car is in the way
        int layerMask = 1 << 0;

        //X is the same for the car and the destination : we move on Z
        if(Math.Abs(carPosition.x - currentDestination.x) < Speed){
            
            //Destination is towards positive Z 
            if(carPosition.z - currentDestination.z < 0){
                rayFromCar.direction = new Vector3(0, 0, 1);
                //Debug.DrawRay(rayFromCar.origin, rayFromCar.direction * 10, Color.blue);
                if(Physics.Raycast(rayFromCar, out hit, 5f, layerMask)){
                    //Debug.Log("Hit distance : " + hit.distance);
                    if(hit.distance < 5f){
                        speedRate = Math.Max(0f, 0.8f + (hit.distance-5f)/5f);
                    } else if(hit.distance < 0.01f){
                        speedRate = 0f;
                    }
                }
                transform.Translate(0, 0, Speed*speedRate, Space.World);
                transform.localEulerAngles = new Vector3(90, 0, 90);
            } else {
                rayFromCar.direction = new Vector3(0, 0, -1);
                //Debug.DrawRay(rayFromCar.origin, rayFromCar.direction * 10, Color.blue);
                if(Physics.Raycast(rayFromCar, out hit, 5f, layerMask)){
                    //Debug.Log("Hit distance : " + hit.distance);
                    if(hit.distance < 5f){
                        speedRate = Math.Max(0f, 0.8f + (hit.distance-5f)/5f);
                    } else if(hit.distance < 0.01f){
                        speedRate = 0f;
                    }
                }
                transform.Translate(0, 0, -Speed*speedRate, Space.World);
                transform.localEulerAngles = new Vector3(90, 180, 90);
            }

        } else {

            //Destination is towards positive X
            if(carPosition.x - currentDestination.x < 0){
                rayFromCar.direction = new Vector3(1, 0, 0);
                //Debug.DrawRay(rayFromCar.origin, rayFromCar.direction * 10, Color.blue);
                if(Physics.Raycast(rayFromCar, out hit, 5f, layerMask)){
                    //Debug.Log("Hit distance : " + hit.distance);
                    if(hit.distance < 5f){
                        speedRate = Math.Max(0f, 0.8f + (hit.distance-5f)/5f);
                    } else if(hit.distance < 0.01f){
                        speedRate = 0f;
                    }
                }
                transform.Translate(Speed*speedRate, 0, 0, Space.World);
                transform.localEulerAngles = new Vector3(90, 90, 90);
            } else {
                rayFromCar.direction = new Vector3(-1, 0, 0);
                //Debug.DrawRay(rayFromCar.origin, rayFromCar.direction * 10, Color.blue);
                if(Physics.Raycast(rayFromCar, out hit, 5f, layerMask)){
                    //Debug.Log("Hit distance : " + hit.distance);
                    if(hit.distance < 5f){
                        speedRate = Math.Max(0f, 0.8f + (hit.distance-5f)/5f);
                    } else if(hit.distance < 0.01f){
                        speedRate = 0f;
                    }
                }
                transform.Translate(-Speed*speedRate, 0, 0, Space.World);
                transform.localEulerAngles = new Vector3(90, -90, 90);
            }

        }
        

    }


    public void CreateTrip(){
        //Debug.Log("Start : " + start + ", End : " + end);
        if(trip == null)
            trip = GameObject.FindWithTag("GridManagerTag").GetComponent<GridBehavior>().grid.GetTrip(start, end);
    }
}
