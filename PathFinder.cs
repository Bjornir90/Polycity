using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinder : MonoBehaviour
{

    public List<Node> nodes;
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

        Dictionary<Node, int> weights = new Dictionary<Node, int>();
        List<Node> processed = new List<Node>();


        foreach(Node node in nodes)
        {
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

            foreach(Edge edge in closest.Links)
            {
                Node neighbor = edge.GetOtherNode(closest);
                if (!processed.Contains(neighbor))
                {
                    weights[neighbor] = Math.Min(weights[neighbor], weights[closest] + edge.Length);
                }
            }
            
        }
        List<Node> path = new List<Node>();
        path.Add(end);

        Debug.Log("Processed : "+processed.Count);

        while (!path[0].Equals(start))
        {
            int min = Int32.MaxValue;
            Node closest = null;
            foreach (Edge edge in path[0].Links)
            {
                Node neighbor = edge.GetOtherNode(path[0]);
                if (weights[neighbor] < min)
                {
                    closest = neighbor;
                    min = edge.Length;
                }
            }
            path.Insert(0, closest);
        }

        Debug.Log("Path contains "+path.Count);

        foreach (var Node in path)
        {
            Debug.Log("Node in path "+Node.ToString());
        }

        throw new Exception("Magic");

        List<Vector3> positions = new List<Vector3>();
        foreach (Node node in path)
        {
            Debug.Log("Node : (" + node.GridPosition.x + ", " + node.GridPosition.y + ")");
            positions.Add(node.Position);
        }
        return new Trip(positions, weights[end]);

    }

    public void CreateLink(Node node_1, Node node_2)
    {
        int length = (int) (Math.Abs(node_2.GridPosition.x - node_1.GridPosition.x) + Math.Abs(node_2.GridPosition.y - node_1.GridPosition.y));
        Edge _edge = new Edge(length, new List<Node>() { node_1, node_2 });
        node_1.AddLink(_edge);
        node_2.AddLink(_edge);
    }

    public Node CreateNode(Vector3 position, Vector3 gridPosition)
    {
        Node created = new Node(position, gridPosition);

        nodes.Add(created);

        return created;
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
        Links.Add(edge);
    }

    public override string ToString()
    {
        return "Node Position : " + GridPosition.x + " : " + GridPosition.y;
    }

}

public class Trip
{
    private List<Vector3> Nodes;
    public int Length {get; set;}
    private int Index;

    public Trip(List<Vector3> Nodes, int Length)
    {
        this.Nodes = Nodes;
        this.Length = Length;
        Index = 0;
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
}
