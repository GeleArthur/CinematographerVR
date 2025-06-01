using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Object = System.Object;


[ExecuteInEditMode]
public class CameraOnRail : MonoBehaviour, CameraScore
{
    [SerializeField] private RailData _railData = null;
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 0.1f;

    private float _score = 0;
    private bool _controllingCamera = false;

    private int _fromIndex = 0;
    private int _toIndex = 1;
    private float _distance = 0.0f;

    private readonly SerializedDictionary<int, ListOfNode> _copyOfConnections = new();

    private void OnEnable()
    {
        _fromIndex = 0;
        _toIndex = 0;
        _distance = 0.0f;

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
                float distance = Vector3.Dot(_target.position - _railData.Nodes[from.Key].Position, 
                    (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized);
                
                if(distance < 0 || distance > (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).magnitude) continue;
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_railData.Nodes[from.Key].Position + (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized * distance, 0.2f);
            }
        }

        Debug.DrawRay(transform.position, (_target.position - transform.position));


    }

    private void Update()
    {
        Vector3 left = _target.position - transform.right;
        if (Physics.Raycast(_target.position, -transform.right, out RaycastHit hit))
        {
            left = hit.point;
        }
        
        Vector3 right = _target.position + transform.right;
        if (Physics.Raycast(_target.position, transform.right, out hit))
        {
            right = hit.point;
        }
        
        if (Physics.Raycast(transform.position, _target.position - transform.position) || 
            Physics.Raycast(transform.position, left - transform.position) ||
            Physics.Raycast(transform.position, right - transform.position)
           )
        {
            MoveCamera();
        }
        
    
        transform.LookAt(_target);
    }

    private void MoveCamera()
    {
        (Node, Node, float) closest = GetClosestPointToTarget(_target.position);

        int endNodeIndex = _railData.Nodes.FindIndex(e => e == closest.Item2);
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
    
    private (Node, Node, float) GetClosestPointToTarget(Vector3 target)
    {
        Node nodeFrom = null;
        Node nodeTo = null;
        float offsetOnNode = 0;
        float distanceResult = float.MaxValue;
        
        foreach (Node node in _railData.Nodes)
        {
            float dist = (target - node.Position).sqrMagnitude;
            if (dist < distanceResult)
            {
                distanceResult = dist;
                nodeFrom = node;
                nodeTo = node;
            }
        }
        
        // foreach (var from in _railData.Connections)
        // {
        //     foreach (var to in from.Value.Nodes)
        //     {
        //         float distance = Vector3.Dot(_target.position - _railData.Nodes[from.Key].Position, 
        //             (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized);
        //         
        //         if(distance < 0 || distance > (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).magnitude) continue;
        //         
        //         Vector3 pointOnLine = _railData.Nodes[from.Key].Position + (_railData.Nodes[to.NodeIndex].Position - _railData.Nodes[from.Key].Position).normalized * distance;
        //         float dist = (target - pointOnLine).sqrMagnitude;
        //         if (dist < distanceResult)
        //         {
        //             distanceResult = dist;
        //             offsetOnNode = distance;
        //             nodeFrom = _railData.Nodes[from.Key];
        //             nodeTo = _railData.Nodes[to.NodeIndex];
        //         }
        //     }
        // }
        
        return (nodeFrom, nodeTo, offsetOnNode);
    }

    public void UpdateScore()
    {
        _score = 0;
    }

    public float GetScore()
    {
        return _score;
    }

    public void ControlCamera(Camera camera)
    {
        _controllingCamera = true;
        // transform.position = Vector3.MoveTowards(transform.position, _railData.Nodes[path[0]].Position, Time.deltaTime * _speed);
    }

    public void SwappedCamera()
    {
        _controllingCamera = false;
    }
}
