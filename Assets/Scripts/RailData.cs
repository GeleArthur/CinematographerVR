using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[Serializable]
public class Node
{
    public Node(Vector3 position)
    {
        Position = position;
    }
    public Vector3 Position;
}

[Serializable]
public struct Connection
{
    public int NodeIndex;
    public float Weight;

    public Connection(int nodeIndex, float weight)
    {
        NodeIndex = nodeIndex;
        Weight = weight;
    }
}

[Serializable]
public class ListOfNode
{
    public List<Connection> Nodes = new();
}

[CreateAssetMenu(fileName = "RailData", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class RailData : ScriptableObject
{
    public List<Node> Nodes = new();
    public SerializedDictionary<int, ListOfNode> Connections = new();
}


