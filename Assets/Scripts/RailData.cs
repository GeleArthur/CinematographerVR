using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]
public class Node
{
    public Node(Vector3 position)
    {
        _position = position;
    }
    public Vector3 _position;
}

[Serializable]
public class ListOfNode
{
    public List<int> _nodes = new();
}

[CreateAssetMenu(fileName = "RailData", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class RailData : ScriptableObject
{
    public List<Node> _nodes = new();
    public SerializedDictionary<int, ListOfNode> _connections = new();
}
