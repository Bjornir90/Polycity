using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Grid {

    public string RoadManagerTag = "RoadManagerTag";

    private Dictionary<Coords, Cell> Cells {get; set;}

    public Grid(){
        Cells = new Dictionary<Coords, Cell>();
    }

    public Cell GetCell(int x, int z){
        Cell toReturn;

        //toReturn is null if the key is not present
        Cells.TryGetValue(new Coords(x, z), out toReturn);
        return toReturn;
    }

    public Cell GetCell(Vector3Int position){
        return GetCell(position.x, position.z);
    }

    public void SetCell(int x, int z, Cell cell){
        //Debug.Log("Create cell "+x+" : "+z);
        Cells.Add(new Coords(x, z), cell);
    }

    public void SetCell(Vector3Int vec, Cell cell){
        SetCell(vec.x, vec.z, cell);
    }

    public bool IsRoad(Vector3Int vector){
        Cell currentCell = GetCell(vector);
        if(currentCell != null && currentCell.IsRoad){
            return true;
        }
        return false;
    }

    public void CheckAndCreateNode(Cell cell){
        Debug.Log("Checking Neighbor for node");
        if(cell == null)
            return;
        CheckAndCreateNode(cell.Position, cell.GridPosition, false);
    }

    public void CheckAndCreateNode(Vector3 position, Vector3Int gridPosition, bool checkNeighbors){
        bool neighborOnX = false, neighborOnZ = false;
        Debug.Log("Trying to create node on "+gridPosition);

        if(GetCell(gridPosition) != null && GetCell(gridPosition).IsNode)
            return;

        for(int i = -1; i<2; i+=2){

            Vector3Int vec3X = gridPosition;
            vec3X.x += i;
            if(IsRoad(vec3X)){
                neighborOnX = true;
                if(checkNeighbors)
                    CheckAndCreateNode(GetCell(vec3X));
            }

            Vector3Int vec3Z = gridPosition;
            vec3Z.z += i;
            if(IsRoad(vec3Z)){
                neighborOnZ = true;
                if(checkNeighbors)
                    CheckAndCreateNode(GetCell(vec3Z));
            }

        }

        if(neighborOnZ && neighborOnX){
            CreateNode(position, gridPosition);
        }

    }

    public void CheckAndCreateLinks(Vector3Int gridPosition){

        for(int i=-1; i<2; i+=2){

            Vector3Int currentPos = gridPosition;
            currentPos.x += i;
            Cell currentCell = GetCell(currentPos);

            while(IsRoad(currentPos)){
                if(currentCell.IsNode){
                    CreateLink(GetCell(gridPosition).Node, currentCell.Node);
                    break;
                }
                currentPos.x += i;
                currentCell = GetCell(currentPos);
            }

            currentPos = gridPosition;
            currentPos.z += i;
            currentCell = GetCell(currentPos);

            while(IsRoad(currentPos)){
                if(currentCell.IsNode){
                    CreateLink(GetCell(gridPosition).Node, currentCell.Node);
                    break;
                }
                currentPos.z += i;
                currentCell = GetCell(currentPos);
            }

        }
    }

    private void CreateNode(Vector3 position, Vector3Int gridPosition){
        PathFinder pathFinder = GameObject.FindWithTag(RoadManagerTag).GetComponent<PathFinder>();
        Node newNode = pathFinder.CreateNode(position, gridPosition);
        GetCell(gridPosition).Node = newNode;
        CheckAndCreateLinks(gridPosition);
        Debug.Log("Created Node on cell "+gridPosition);
    }

    private void CreateLink(Node node1, Node node2){
        PathFinder pathFinder = GameObject.FindWithTag(RoadManagerTag).GetComponent<PathFinder>();
        pathFinder.CreateLink(node1, node2);
        Debug.Log("Create link between : (" +node1.GridPosition.x + ", " + node1.GridPosition.z + ") and ("+node2.GridPosition.x + ", " + node2.GridPosition.z + ")");
    }

    public Trip GetTrip(Vector3 start, Vector3 end){
        Vector3Int startGridPosition = GridBehavior.GetWorldGridPosition(start);
        Vector3Int endGridPosition = GridBehavior.GetWorldGridPosition(end);
        Vector3Int startGridIndex = GridBehavior.GetGridIndex(startGridPosition);
        Vector3Int endGridIndex = GridBehavior.GetGridIndex(endGridPosition);
        Node startNode=null, endNode=null;
        List<Node> startNodes=null, endNodes=null;
        //Debug.Log("Grid pos : "+ startGridPosition + ", grid index : "+ startGridIndex+ ", Raw position : "+ start);
        if(GetCell(startGridIndex).IsNode)
            startNode = GetCell(startGridIndex).Node;
        else
            startNodes = GetLinkedNodes(startGridIndex);
        if(GetCell(endGridIndex).IsNode)
            endNode = GetCell(endGridIndex).Node;
        else
            endNodes = GetLinkedNodes(endGridIndex);

        // Find best trip
        PathFinder pathFinder = GameObject.FindWithTag(RoadManagerTag).GetComponent<PathFinder>();
        Trip res = null;
        if(startNode!=null)
            if(endNode!=null){
                return pathFinder.GetTrip(startNode, endNode);
            }
            else{
                CreateNode(end, endGridIndex);
                endNode = GetCell(endGridIndex).Node;
                for(int i =0; i<endNodes.Count; i++){
                    CreateLink(endNodes[i], endNode);
                }
                res = pathFinder.GetTrip(startNode, endNode);
                foreach(Edge link in endNode.Links){
                    for(int i=0; i<endNodes.Count; i++){
                        endNodes[i].Links.Remove(link);
                    }
                }
                pathFinder.nodes.Remove(endNode);
            }
        else{
            CreateNode(start, startGridIndex);
            startNode = GetCell(startGridIndex).Node;
            for(int i =0; i<startNodes.Count; i++){
                CreateLink(startNodes[i], startNode);
            }
            if(endNode!=null){
                res = pathFinder.GetTrip(startNode, endNode);
            }
            else{
                CreateNode(end, endGridIndex);
                endNode = GetCell(endGridIndex).Node;
                for(int i =0; i<endNodes.Count; i++){
                    CreateLink(endNodes[i], endNode);
                }
                res = pathFinder.GetTrip(startNode, endNode);
                foreach(Edge link in endNode.Links){
                    for(int i=0; i<endNodes.Count; i++){
                        endNodes[i].Links.Remove(link);
                    }
                }
                pathFinder.nodes.Remove(endNode);
            }
            foreach(Edge link in startNode.Links){
                    for(int i=0; i<startNodes.Count; i++){
                        startNodes[i].Links.Remove(link);
                    }
                }
            pathFinder.nodes.Remove(startNode);
        }
        return res;
    }

    
    public List<Node> GetLinkedNodes(Vector3Int gridIndex){

        List<Node> nodes = new List<Node>();

        for(int i=-1; i<2; i+=2){
            
            Vector3Int currentPos = gridIndex;
            currentPos.x += i;
            Cell currentCell = GetCell(currentPos);

            while(IsRoad(currentPos)){
                if(currentCell.IsNode){
                    nodes.Add(currentCell.Node);
                    break;
                }
                currentPos.x += i;
                currentCell = GetCell(currentPos);
            }

            currentPos = gridIndex;
            currentPos.z += i;
            currentCell = GetCell(currentPos);

            while(IsRoad(currentPos)){
                if(currentCell.IsNode){
                    nodes.Add(currentCell.Node);
                    break;
                }
                currentPos.z += i;
                currentCell = GetCell(currentPos);
            }
        }
        return nodes;
    }
}


public class Coords {
    public int X {get; set;}
    public int Z {get; set;}

    public Coords(int x, int z){
        X = x;
        Z = z;
    }

    public Coords(Vector3 position){
        X = (int) position.x;
        Z = (int) position.z;
    }


    override public bool Equals(object obj){
        if(!this.GetType().Equals(obj.GetType()))
            return false;

        Coords other = (Coords) obj;

        return other.X == this.X && other.Z == this.Z;
    }

    override public int GetHashCode(){
        return X.GetHashCode()+Z.GetHashCode()*10000;
    }
}

public class Cell {
    public GameObject Content {get;}
    public bool IsRoad {get;}

    public Vector3 Position {get;}
    public Vector3Int GridPosition {get;}

    public Coords Coordinates {get;}

    private Node _node;

    public Node Node {
        get => _node;
        set{
            if(value != null)
                IsNode = true;
            else
                IsNode = false;
            _node = value;
        }
    }

    public bool IsNode {get; set;}

    public Cell(Coords coordinates, GameObject content, bool isRoad, Vector3 position, Vector3Int gridPosition){
        Coordinates = coordinates;
        Position = position;
        GridPosition = gridPosition;
        Content = content;
        IsRoad = isRoad;
    }

    public Cell(Vector3Int coordinates, GameObject content, bool isRoad, Vector3 position, Vector3Int gridPosition) : this(new Coords(coordinates), content, isRoad, position, gridPosition){

    }
}