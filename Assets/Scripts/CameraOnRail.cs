using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;


[ExecuteInEditMode]
public class CameraOnRail : MonoBehaviour
{
    [SerializeField] private RailData _railData = null;
    [SerializeField] private ReplayTarget _target = null;
    [SerializeField] private float _speed = 0.1f;
    
    private PointOnRail[] _points = Array.Empty<PointOnRail>();
    public ReplayTarget Target => _target;
    
    private int _fromIndex = 25;

    private readonly SerializedDictionary<int, ListOfNode> _copyOfConnections = new();

    private void OnEnable()
    {
        _points = new PointOnRail[_railData.Nodes.Count];
        for (int i = 0; i < _railData.Nodes.Count; i++)
        {
            _points[i] = new PointOnRail(_railData.Nodes[i], this);
        }

        // That moment when C# doesn't have copies. uhhhh
        foreach (KeyValuePair<int, ListOfNode> pair in _railData.Connections)
        {
            _copyOfConnections[pair.Key] = new ListOfNode();
            foreach (Connection connection in pair.Value.Nodes)
            {
                _copyOfConnections[pair.Key].Nodes.Add(new Connection(connection.NodeIndex, connection.Weight));
            }
        }
    }

    private void OnDrawGizmos()
    {
        HashSet<int> nodeChecked = new HashSet<int>();
        
        foreach (var from in _railData.Connections)
        {
            nodeChecked.Add(from.Key);
            foreach (var to in from.Value.Nodes)
            {
                if(nodeChecked.Contains(to.NodeIndex)) break;
                float distance = Vector3.Dot(_target.transform.position - _railData.Nodes[from.Key].Position, 
                    (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized);
                
                if(distance < 0 || distance > (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).magnitude) continue;
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_railData.Nodes[from.Key].Position + (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized * distance, 0.2f);
            }
        }

        Debug.DrawRay(transform.position, (_target.transform.position - transform.position));
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 32;
        
        foreach (PointOnRail pointOnRail in _points)
        {
            Handles.Label(pointOnRail.Node.Position + Vector3.up, pointOnRail.GetScore().ToString(CultureInfo.InvariantCulture), style);
        }
    }

    private void Update()
    {
        foreach (PointOnRail point in _points)
        {
            point.UpdateScore();
        }

        Array.Sort(_points, (point1, point2) => point2.GetScore().CompareTo(point1.GetScore()));
        MoveCamera(_points[0].Node);

        transform.LookAt(_target.transform.position + Vector3.up);
    }

    private void MoveCamera(Node moveHere)
    {
        int endNodeIndex = _railData.Nodes.FindIndex(e => e == moveHere);
        List<int> path = FindPathToNode(_fromIndex, endNodeIndex, _railData.Connections);
        
        if (path.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, _railData.Nodes[path[0]].Position, Time.deltaTime * _speed);

            if (path.Count > 1)
            {
                if (Vector3.Distance(transform.position, _railData.Nodes[path[0]].Position) < 0.5f)
                {
                    _fromIndex = path[1];
                }
            }
        }
    }


    private List<int> FindPathToNode(int startNodeIndex, int endNodeIndex, SerializedDictionary<int, ListOfNode> graph)
    {
        Priority_Queue.SimplePriorityQueue<int> openList = new();
        Dictionary<int, int> cameFrom = new();
        Dictionary<int, float> costSoFar = new();

        costSoFar[startNodeIndex] = 0;
        openList.Enqueue(startNodeIndex, 0);
        
        while (openList.Count > 0)
        {
            int current = openList.Dequeue();

            if (current == endNodeIndex)
            {
                List<int> path = new List<int>();
                int currentIndex = endNodeIndex;
                while (startNodeIndex != currentIndex)
                {
                    path.Add(currentIndex);
                    currentIndex = cameFrom[currentIndex];
                }
                path.Add(startNodeIndex);

                path.Reverse();

                return path;
            }

            foreach (Connection neighbor in graph[current].Nodes)
            {
                float newCost = costSoFar[current] + neighbor.Weight;
                if (!cameFrom.ContainsKey(neighbor.NodeIndex) || newCost < costSoFar[current])
                {
                    openList.Enqueue(neighbor.NodeIndex, newCost + Heuristic(current, neighbor.NodeIndex));
                    cameFrom[neighbor.NodeIndex] = current;
                    costSoFar[neighbor.NodeIndex] = newCost;
                }
            }
        }
        
        return new List<int>();
    }
    
    float Heuristic(int start, int stop)
    {
        return (_railData.Nodes[stop].Position - _railData.Nodes[start].Position).magnitude;
    }
}
