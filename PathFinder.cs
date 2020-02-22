using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PathFinder : MonoBehaviour
{

    public List<Node> nodes;

    public GameObject TextObject;

    // Start is called before the first frame update
    void Start()
    {
        
        Debug.Log("Start");
        nodes = new List<Node>();
        /*
        Vector3 pos = new Vector3(0, 0, 0);
        Node node1 = new Node(pos);
        node1.GridPosition = new Vector3(3, 0, 0);
        Node node2 = new Node(pos);
        node2.GridPosition = new Vector3(3, 1, 0);
        Node node3 = new Node(pos);
        node3.GridPosition = new Vector3(3, 2, 0);
        Node node4 = new Node(pos);
        node4.GridPosition = new Vector3(3, 4, 0);
        Node node5 = new Node(pos);
        node5.GridPosition = new Vector3(1, 4, 0);
        Node node6 = new Node(pos);
        node6.GridPosition = new Vector3(1, 2, 0);
        Node node7 = new Node(pos);
        node7.GridPosition = new Vector3(5, 1, 0);
        Node node8 = new Node(pos);
        node8.GridPosition = new Vector3(5, 4, 0);

        nodes.Add(node1);
        nodes.Add(node2);
        nodes.Add(node3);
        nodes.Add(node4);
        nodes.Add(node5);
        nodes.Add(node6);
        nodes.Add(node7);
        nodes.Add(node8);

        CreateLink(node1, node2);
        CreateLink(node2, node3);
        CreateLink(node3, node4);
        CreateLink(node4, node5);
        CreateLink(node5, node6);
        CreateLink(node6, node3);
        CreateLink(node4, node8);
        CreateLink(node7, node2);
        CreateLink(node7, node8);

        Trip result = GetTrip(node1, node8);
        */

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool ContainsAll(List<Node> Containee, List<Node> Container)
    {
        foreach(Node node in Containee)
        {
            if (!Container.Contains(node))
                return false;
        }
        return true;
    }

    public Trip GetTrip(Node start, Node end)
    {
        // Djikstra

        //Debug.Log("Looking for trip between "+start.ToString()+" and "+end.ToString());

        Dictionary<Node, int> weights = new Dictionary<Node, int>();
        List<Node> processed = new List<Node>();

        foreach(Node node in nodes)
        {
            /*
            // Debug : drw line for every link known
            foreach (Edge edge in node.Links)
            {
                Debug.DrawLine(edge.Nodes[0].Position, edge.Nodes[1].Position, Color.red, 10.0f, false);
                DrawSquare(edge.Nodes[1].Position, 1.0f, Color.blue);
                
            }
            */
            weights.Add(node, Int32.MaxValue);
        }
        weights[start] = 0;
        while (!ContainsAll(nodes, processed))
        {
            int min = Int32.MaxValue;
            Node closest = null;
            
            foreach(Node node in nodes)
            {
                if (!processed.Contains(node))
                {
                    if (weights[node] < min)
                    {
                        min = weights[node];
                        closest = node;
                    }
                }
            }

            processed.Add(closest);

            if(closest == null){
                throw new Exception("No node closest available");
            }

            foreach(Edge edge in closest.Links)
            {
                Node neighbor = edge.GetOtherNode(closest);
                if (!processed.Contains(neighbor))
                {
                    Debug.Log("LONGUEUR : " + edge.Length);
                    weights[neighbor] = Math.Min(weights[neighbor], weights[closest] + edge.Length);
                }
            }
        }

        List<Node> path = new List<Node>();
        path.Add(end);

/*
        foreach(Node node in processed){
                Vector3 textPosition = node.Position+new Vector3(0, 1, 0);
                GameObject textInstance = Instantiate(TextObject, textPosition, Quaternion.Euler(0, 0, 0));
                TextMesh text = textInstance.GetComponent<TextMesh>();
                text.text = weights[node].ToString();
            }
*/ 
        while (!path[0].Equals(start))
        {
            int min = Int32.MaxValue;
            Node closest = null;
            foreach (Edge edge in path[0].Links)
            {
                Node neighbor = edge.GetOtherNode(path[0]);
                if (weights[neighbor] < min && !path.Contains(neighbor))
                {
                    closest = neighbor;
                    min = weights[neighbor];
                }
            }
            path.Insert(0, closest);
        }

        Debug.Log("Path contains "+path.Count);

        List<Vector3> positions = new List<Vector3>();
        foreach (Node node in path)
        {
            positions.Add(node.Position);
        }
        return new Trip(positions, weights[end]);

    }

    public void CreateLink(Node node_1, Node node_2)
    {
        int length = (int) (Math.Abs(node_2.GridPosition.x - node_1.GridPosition.x) + Math.Abs(node_2.GridPosition.z - node_1.GridPosition.z));
        Edge _edge = new Edge(length, new List<Node>() { node_1, node_2 });
        Debug.Log(_edge);
        node_1.AddLink(_edge);
        node_2.AddLink(_edge);
    }

    public Node CreateNode(Vector3 position, Vector3 gridPosition)
    {
        Node created = new Node(position, gridPosition);

        nodes.Add(created);

        return created;
    }

    public void DrawSquare(Vector3 center, float radius, Color color, float duration=100.0f){
        Vector3 luCorner = new Vector3(center.x - radius/2, center.y, center.z + radius/2);
        Vector3 ruCorner = new Vector3(center.x + radius/2, center.y, center.z + radius/2);
        Vector3 lbCorner = new Vector3(center.x - radius/2, center.y, center.z - radius/2);
        Vector3 rbCorner = new Vector3(center.x + radius/2, center.y, center.z - radius/2);
        Debug.DrawLine(luCorner, ruCorner, color, duration);
        Debug.DrawLine(ruCorner, rbCorner, color, duration);
        Debug.DrawLine(rbCorner, lbCorner, color, duration);
        Debug.DrawLine(lbCorner, luCorner, color, duration);
    }


}

public class Edge
{
    public int Length { get; set; }
    public List<Node> Nodes { get; set; }

    public Edge(int length, List<Node> nodes)
    {
        Length = length;
        Nodes = nodes;
    }

    public Node GetOtherNode(Node node)
    {
        return (Nodes[0].Equals(node)) ? Nodes[1] : Nodes[0];
    }

    public override string ToString(){
        return Nodes[0].ToSmallString()+ ", "+Nodes[1].ToSmallString();
    }
}

public class Node
{
    public Vector3 Position { get; set; }
    public Vector3 GridPosition { get; set; }
    public List<Edge> Links { get; set; }

    public Node(Vector3 position, Vector3 gridPosition)
    {
        Position = position;
        GridPosition = gridPosition;
        Links = new List<Edge>();
    }

    public void AddLink(Edge edge)
    {
        Debug.Log(ToString()+" Adding link to : "+edge.GetOtherNode(this));

        Links.Add(edge);
    }

    public override string ToString()
    {
        return "Node : " + GridPosition.x + " : " + GridPosition.z;
    }

    public string ToSmallString()
    {
        return "(" + GridPosition.x + ", " + GridPosition.z + ")";
    }

    public string ToDetailedString(){
        string res = this.ToString();
        foreach (Edge edge in Links)
        {
            res += "\n Lien : " + edge.ToString();
        }
        return res;
    }

    public override bool Equals(object obj){
        //Check for null and compare run-time types.
        if ((obj == null) || ! this.GetType().Equals(obj.GetType())) 
        {
            return false;
        }
        Node n = (Node) obj;
        return (Position == n.Position) && (GridPosition == n.GridPosition);
    }

    public override int GetHashCode(){
        return (int) (GridPosition.x*1000+GridPosition.z);
    }

}

public class Trip
{
    public List<Vector3> Nodes;
    public int Length {get; set;}
    private int Index;

    public bool isReturnTrip {get; set;}

    public Trip(List<Vector3> Nodes, int Length)
    {
        this.Nodes = Nodes;
        this.Length = Length;
        Index = 0;
        isReturnTrip = false;
    }

    public Vector3 GetNextNode()
    {
        Vector3 res = Nodes[Index];
        Index++;
        return res;
    }

    public bool isFinal()
    {
        return Index == Nodes.Count;
    }

    public Trip GetReturnTrip()
    {
        List<Vector3> reversed = new List<Vector3>(Nodes);
        reversed.Reverse();
        Trip res = new Trip(reversed, Length);
        res.isReturnTrip = true;
        return res;
    }
}
