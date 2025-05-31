using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class NodeRecord
{
    public int Node;
    public float CostSoFar; // accumulated g-costs of all the connections leading up to this one
    public float EstimatedTotalCost; // f-cost (= costSoFar + h-cost)
    public float Hcost;

    public NodeRecord(int node, float estimatedTotalCost, float costSoFar, float hcost)
    {
        Node = node;
        Hcost = hcost;
        EstimatedTotalCost = estimatedTotalCost;
        CostSoFar = costSoFar;
    }
}

public class CameraOnRail : MonoBehaviour
{
    [SerializeField] private RailData _railData = null;
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 0.1f;

    private int _fromIndex = 0;
    private int _toIndex = 1;
    private float _distance = 0.0f;

    private void OnEnable()
    {
        _fromIndex = 0;
        _toIndex = 0;
        _distance = 0.0f;
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
        // if (Physics.Raycast(transform.position, transform.forward, (_target.position - transform.position).magnitude))
        {
            (Node, Node, float) closest = GetClosestPointToTarget(_target.position);

            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(Vector3.Lerp(closest.Item1.Position, closest.Item2.Position, closest.Item3), 0.3f);
            
            int endNodeIndex = _railData.Nodes.FindIndex(e => e == closest.Item2);
            List<int> path = FindPathToNode(_fromIndex, endNodeIndex);

            Gizmos.color = Color.red;
            for (int i = 0; i < path.Count-1; i++)
            {
                Gizmos.DrawLine(_railData.Nodes[path[i]].Position, _railData.Nodes[path[i + 1]].Position);
            }

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
        
    
        transform.LookAt(_target);
    }
    
    
    private List<int> FindPathToNode(int startNodeIndex, int endNodeIndex)
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

            foreach (Connection neighbor in _railData.Connections[current].Nodes)
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
}
