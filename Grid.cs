using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid {
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
        Cells.Add(new Coords(x, z), cell);
    }

    public void SetCell(Vector3Int vec, Cell cell){
        SetCell(vec.x, vec.z, cell);
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

}

public class Cell {
    public GameObject Content {get;}
    public bool IsRoad {get;}

    public Coords Position {get;}

    public Cell(Coords position, GameObject content, bool isRoad){
        Position = position;
        Content = content;
        IsRoad = isRoad;
    }

    public Cell(Vector3Int position, GameObject content, bool isRoad) : this(new Coords(position), content, isRoad){

    }
}