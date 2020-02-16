using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    //Attach a cube GameObject in the Inspector before entering Play Mode
    private GameObject Cursor;
    private GameObject Prefab;

    private List<GameObject> ListPrefab;
    private int indexPrefab;
    private bool currentPrefabIsRoad;

    public static int GridSize = 10;

    private static Plane m_Plane;

    public Grid grid;

    public GameBehavior gameManager;

    Vector3Int positionOfLastInstantiation;
    int currentAngle;

    void Start()
    {
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        currentAngle = 0;
        grid = new Grid();
        positionOfLastInstantiation = new Vector3Int(0, 10, 0);
        ListPrefab = new List<GameObject>();
        indexPrefab = 0;
        currentPrefabIsRoad = true;
        LoadPrefabList();
        Cursor = Instantiate(ListPrefab[indexPrefab], new Vector3(0, 1000, 0), Quaternion.Euler(0, 0, 0));
        Prefab = ListPrefab[indexPrefab];
        gameManager = GameObject.FindWithTag("GameManagerTag").GetComponent<GameBehavior>();
    }

    void Update()
    {
       
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;

        if (m_Plane.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3Int newObjectPos = new Vector3Int(((int) Math.Floor(hitPoint.x/GridSize))*GridSize+GridSize/2,
            (int) hitPoint.y,
            ((int) Math.Floor(hitPoint.z/GridSize))*GridSize+GridSize/2);

            //Move your cube GameObject to the point where you clicked
            Cursor.transform.position = newObjectPos;

            //Detect when there is a mouse click
            if (Input.GetMouseButton(0) && !positionOfLastInstantiation.Equals(newObjectPos))
            {
                if(grid.GetCell(newObjectPos) == null){

                    //Debug.Log("Clicked on "+hitPoint+" grid : "+gridPosition);
                    Vector3Int gridPosition = GetGridIndex(newObjectPos);

                    if(!currentPrefabIsRoad){
                        for(int i=-1; i<2; i+=2){
                            Vector3Int neighborX = gridPosition;
                            Vector3Int neighborZ = gridPosition;
                            Vector3 nearestRoadPosition;
                            neighborX.x += i;
                            neighborZ.z += i;
                            if(grid.IsRoad(neighborX) || grid.IsRoad(neighborZ)){
                                GameObject PrefabInstance = Instantiate(Prefab, newObjectPos, Quaternion.Euler(0, currentAngle, 0));
                                BuildingBehavior prefabBehavior = PrefabInstance.GetComponent<BuildingBehavior>();
                                if(grid.IsRoad(neighborX))
                                    nearestRoadPosition = GridPositionToPosition(neighborX);
                                else
                                    nearestRoadPosition = GridPositionToPosition(neighborZ);
                                prefabBehavior.SetNearestRoadPosition(nearestRoadPosition);
                                gameManager.AddBuilding(nearestRoadPosition, prefabBehavior.type, prefabBehavior.interest);
                                positionOfLastInstantiation = newObjectPos;
                                grid.SetCell(gridPosition, new Cell(newObjectPos, Prefab, currentPrefabIsRoad, newObjectPos, gridPosition));
                                break;
                            }
                        }
                    } else {
                        Instantiate(Prefab, newObjectPos, Quaternion.Euler(0, currentAngle, 0));
                        positionOfLastInstantiation = newObjectPos;
                        grid.SetCell(gridPosition, new Cell(newObjectPos, Prefab, currentPrefabIsRoad, newObjectPos, gridPosition));
                        grid.CheckAndCreateNode(newObjectPos, gridPosition, true);
                        grid.CheckAndCreateLinks(gridPosition, true);
                    }

                }
            }
        }

        if (Input.GetKeyDown("r"))
        {
            currentAngle += 90;
            Cursor.transform.Rotate(new Vector3(0, 90, 0), Space.World);
        } else if (Input.GetKeyDown("e"))
        {
            currentAngle -= 90;
            Cursor.transform.Rotate(new Vector3(0, -90, 0), Space.World);
        } else if (Input.GetKeyDown("u")){
            ChangePrefab();
        }

    }

    private void LoadPrefabList(){
        GameObject Road = Resources.Load<GameObject>("Plane");
        ListPrefab.Add(Road);
        GameObject ResidentialBuilding = Resources.Load<GameObject>("ResidentialBuilding");
        ListPrefab.Add(ResidentialBuilding);
    }

    private void ChangePrefab(){
        if(++indexPrefab >= ListPrefab.Count)
            indexPrefab = 0;
        if(indexPrefab == 0)
            currentPrefabIsRoad = true;
        else
            currentPrefabIsRoad = false;
        Prefab = ListPrefab[indexPrefab];
        Destroy(Cursor);
        Cursor = Instantiate(ListPrefab[indexPrefab], new Vector3(0, 1000, 0), Quaternion.Euler(0, 0, 0));
    }

    public static Vector3Int GetGridIndex(Vector3 position){
        Vector3Int gridPosition = new Vector3Int(
                        (int) Math.Floor((double)position.x/GridSize),
                        (int) position.y,
                        (int) Math.Floor((double)position.z/GridSize)
                        );
        return gridPosition;

    }

    public static Vector3Int GetWorldGridPosition(Vector3 position){
        Vector3Int result = new Vector3Int(((int) Math.Floor(position.x/GridSize))*GridSize+GridSize/2,
            (int) position.y,
            ((int) Math.Floor(position.z/GridSize))*GridSize+GridSize/2);
        return result;
    }

    public static Vector3 GridPositionToPosition(Vector3Int gridPosition){
        return new Vector3(
            gridPosition.x*GridSize+GridSize/2,
            gridPosition.y,
            gridPosition.z*GridSize+GridSize/2
            );
    }

    public static Vector3 GetOnPlaneClick() {
        //Create a ray from the Mouse click position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Initialise the enter variable
        float enter = 0.0f;

        if (m_Plane.Raycast(ray, out enter))
        {
            //Get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }
        throw new InvalidOperationException("The user did not click on the plane");
    }

}
