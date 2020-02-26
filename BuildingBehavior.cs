using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBehavior : MonoBehaviour
{
    public Vector3 nearestRoadPosition;

    public const int MIN_CAR_NUMBER = 3, MAX_CAR_NUMBER = 5;

    public const int MAX_RESIDENTIAL_INTEREST = 5, MAX_COMMERCIAL_INTEREST = 40;

    private bool collision;

    public BuildingType type;

    public int interest {get; set;}

    private int numberOfCars;
    //Time in seconds
    private float timeOfLastSpawn;

    public GameBehavior gameManager;
    public const float SPAWN_INTERVAL = 5f, SPAWN_CHANCE = 0.01f;

    private GameObject Prefab;

    // Start is called before the first frame update
    void Awake()
    {
        numberOfCars = Random.Range(MIN_CAR_NUMBER, MAX_CAR_NUMBER);
        //Debug.Log("Initial car number : "+ numberOfCars);
        //nearestRoadPosition = new Vector3(0, 1000, 0);
        timeOfLastSpawn = Time.time;
        Prefab = Resources.Load<GameObject>("CarWithWheels");
        gameManager = GameObject.FindWithTag("GameManagerTag").GetComponent<GameBehavior>();
        switch(type){
            case BuildingType.Residential:
                interest = Random.Range(1,MAX_RESIDENTIAL_INTEREST);
                break;
            case BuildingType.Commercial:
                interest = Random.Range(1,MAX_COMMERCIAL_INTEREST);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time-timeOfLastSpawn < SPAWN_INTERVAL)
            return;

        if(Random.Range(0f, 1f) > SPAWN_CHANCE)
            return;

        if(nearestRoadPosition == new Vector3(0, 0, 0))
            return;
        
        if(type == BuildingType.Commercial)
            return;

        if(numberOfCars == 0)
            return;

        

        if(collision)
            return;
        
        GameObject carInstance = Instantiate(Prefab, nearestRoadPosition, Quaternion.Euler(0, 0, 0));
        CarBehavior carBehavior = carInstance.GetComponent<CarBehavior>();
        carBehavior.home = this;
        carBehavior.start = nearestRoadPosition;
        try{
            carBehavior.end = gameManager.GetDestination(nearestRoadPosition);
            carBehavior.CreateTrip();
        } catch(System.IndexOutOfRangeException e){ 
            Debug.Log(e);
            Destroy(carInstance);
        }
        numberOfCars --;
        
    }

    void onCollisionEnter(Collision collision){
        this.collision = true;
    }

    void onCollisionExit(Collision collision){
        this.collision = false;
    }

    /* void OnDrawGizmos(){
        Gizmos.color = new Color(1, 0, 0, 1f);
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        Debug.Log("GIZMOS | collider center : " + collider.center + ", collider size : " + collider.size);
        Gizmos.DrawCube(collider.center, collider.size);
    } */

    public void SetNearestRoadPosition(Vector3 nearestRoadPosition){
        this.nearestRoadPosition = nearestRoadPosition;
        //Debug.Log("Nearest : " + nearestRoadPosition);

        Vector3 size = new Vector3(20, 10, 20);
        Vector3 position = nearestRoadPosition;
        position.y += 6;
        BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        collider.center = transform.InverseTransformPoint(position);
        collider.size = size;
        collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        Debug.Log("collider center : " + collider.center + ", collider size : " + collider.size);
    }

    public void AddCar(int nb){
        numberOfCars += nb;
    }

}
