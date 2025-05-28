using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Node
{
    public Node(Vector3 position, int index)
    {
        Index = index;
        Position = position;
    }
    public Vector3 Position;
    public int Index;
}

public class Connection
{
    public Connection(Node node1, Node node2)
    {
        Node1 = node1;
        Node2 = node2;
    }
    
    public readonly Node Node1;
    public readonly Node Node2;
}

[CreateAssetMenu(fileName = "RailData", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class RailData : ScriptableObject
{
    public static List<Node> _nodes = new List<Node>()
    {
        new Node(new Vector3(0,0,0), 0),
        new Node(new Vector3(7,5,0), 1),
        new Node(new Vector3(15,5,5), 2),
        new Node(new Vector3(25,5,0), 3),
        new Node(new Vector3(35,8,2), 4),
        new Node(new Vector3(30,8,20), 5),
    };
    public static List<Connection> _connections = new List<Connection>()
    {
        new Connection(_nodes[0], _nodes[1]),
        new Connection(_nodes[1], _nodes[2]),
        new Connection(_nodes[2], _nodes[3]),
        new Connection(_nodes[3], _nodes[4]),
        new Connection(_nodes[4], _nodes[5]),
        new Connection(_nodes[5], _nodes[2]),
    };

    public Dictionary<int, Connection[]> ConnectionLookUp;
    
    private void OnValidate()
    {
        ConnectionLookUp = new Dictionary<int, Connection[]>();
        for (int i = 0; i < _nodes.Count; i++)
        {
            ConnectionLookUp[i] = _connections.Where(e => e.Node1.Index == i).ToArray();
        }
    }
}
