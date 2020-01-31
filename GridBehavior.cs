using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    //Attach a cube GameObject in the Inspector before entering Play Mode
    public GameObject Cursor;
    public GameObject Prefab;

    public static int GridSize = 10;

    private static Plane m_Plane;

    public Grid grid;

    Vector3Int positionOfLastInstantiation;
    int currentAngle;

    void Start()
    {
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(Vector3.up, new Vector3(0, 0, 0));
        currentAngle = 0;
        grid = new Grid();
        positionOfLastInstantiation = new Vector3Int(0, 10, 0);
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

                    Instantiate(Prefab, newObjectPos, Quaternion.Euler(0, currentAngle, 0));
                    positionOfLastInstantiation = newObjectPos;


                    Vector3Int gridPosition = GetGridIndex(newObjectPos);

                    //Debug.Log("Clicked on "+hitPoint+" grid : "+gridPosition);
                    grid.SetCell(gridPosition, new Cell(newObjectPos, Prefab, true, newObjectPos, gridPosition));
                    Debug.Log("Cell created on : "+ gridPosition);

                    grid.CheckAndCreateNode(newObjectPos, gridPosition, true);

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
        }

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
