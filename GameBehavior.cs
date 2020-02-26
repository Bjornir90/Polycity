using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehavior : MonoBehaviour
{

    public List<Building> buildings;

    // Start is called before the first frame update
    void Start()
    {
        buildings = new List<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddBuilding(Vector3 nearestRoadPosition, BuildingType type, int interest){
        buildings.Add(new Building(nearestRoadPosition, type, interest));
    }

    public Vector3 GetDestination(Vector3 currentNearestRoadPosition){
        if(buildings.Count <= 1)
            throw new System.IndexOutOfRangeException("Aucune destination possible !");
        float currentMax = 0;
        Vector3 destination = new Vector3(0,1000,0);
        foreach(Building building in buildings){
            if(building.nearestRoadPosition != currentNearestRoadPosition){
                //Debug.Log("Destination ? " + building.nearestRoadPosition);
                float current = Random.Range(0f, 1f)*building.interest;
                //Debug.Log("Interest : " + current);
                if(current > currentMax){
                    currentMax = current;
                    destination = building.nearestRoadPosition;
                }
            }
        }
        if(destination == new Vector3(0, 1000, 0))
            throw new System.IndexOutOfRangeException("Aucune destination possible !");
        //Debug.Log("Destination : " + destination);
        return destination;
    }
}

public class Building{

    public Vector3 nearestRoadPosition {get; set;}

    public BuildingType type {get; set;}

    public int interest {get; set;}

    public Building(Vector3 nearestRoadPosition, BuildingType type, int interest){
        this.nearestRoadPosition = nearestRoadPosition;
        this.type = type;
        this.interest = interest;
    }

}

public enum BuildingType{
    Residential,
    Commercial
}
