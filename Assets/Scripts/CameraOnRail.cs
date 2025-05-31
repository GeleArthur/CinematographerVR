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

            // _fromIndex = path[0];
            // Gizmos.color = Color.green;
            //Gizmos.DrawWireCube(_railData.Nodes[_fromIndex].Position, new Vector3(1, 1, 1));
            // Gizmos.DrawWireCube(_railData.Nodes[endNodeIndex].Position, new Vector3(1, 1, 1));
            // _toIndex = path[1];
            //
            // _distance += Time.deltaTime / 10000;
            //
            // if (_distance > 1)
            // {
            //     _fromIndex = _toIndex;
            //     _toIndex = path[2];
            //     _distance = 0;
            // }

            // UpdatePosition();
            // Gizmos.color = Color.green;
            // var result = GetClosestPointToTarget(_target.position);
            // Vector3 finalLocation = result.Item1.Position + (result.Item2.Position - result.Item1.Position).normalized * result.Item3;
            // Gizmos.DrawSphere(finalLocation, 0.6f);
            // transform.position = Vector3.MoveTowards(transform.position, finalLocation, 0.3f);
        }
        
    
        transform.LookAt(_target);
    }
    
    

    private void UpdatePosition()
    {
        transform.position = Vector3.Lerp(
            _railData.Nodes[_fromIndex].Position, 
            _railData.Nodes[_toIndex].Position,
            _distance);
    }
    
    private List<int> FindPathToNode(int startNodeIndex, int endNodeIndex)
    {
        Queue<int> openList = new();
        Dictionary<int, int> cameFrom = new();
        Dictionary<int, float> costSoFar = new();
        
        // public Dictionary<Location, Location> cameFrom = new Dictionary<Location, Location>();
        // public Dictionary<Location, double> costSoFar = new Dictionary<Location, double>();

        costSoFar[startNodeIndex] = 0;
        openList.Enqueue(startNodeIndex);
        
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
                //if (cameFrom.ContainsKey(neighbor.NodeIndex)) continue;
                float newCost = costSoFar[current] + neighbor.Weight;
                if (!cameFrom.ContainsKey(neighbor.NodeIndex) || newCost < costSoFar[current])
                {
                    openList.Enqueue(neighbor.NodeIndex);
                    cameFrom[neighbor.NodeIndex] = current;
                    costSoFar[neighbor.NodeIndex] = newCost;
                }
                

            }
        }
        
        return new List<int>();
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
