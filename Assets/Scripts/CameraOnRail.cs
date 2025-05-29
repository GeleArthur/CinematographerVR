using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraOnRail : MonoBehaviour
{
    [SerializeField] private RailData _railData;
    [SerializeField] private Transform _target;

    private void OnDrawGizmos()
    {
        HashSet<int> nodeChecked = new HashSet<int>();
        
        foreach (var from in _railData._connections)
        {
            nodeChecked.Add(from.Key);
            foreach (var to in from.Value._nodes)
            {
                if(nodeChecked.Contains(to)) break;
                float distance = Vector3.Dot(_target.position - _railData._nodes[from.Key]._position, 
                    (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).normalized);
                
                if(distance < 0 || distance > (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).magnitude) continue;
                
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(_railData._nodes[from.Key]._position + (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).normalized * distance, 0.2f);
            }
            
            
            
            
        }
        
        // Gizmos.color = Color.green;
        // var result = GetClostedPointToTarget(_target.position);
        // Vector3 finalLocation = result.Item1.Node1.Position + (result.Item1.Node2.Position - result.Item1.Node1.Position).normalized * result.Item2;
        // Gizmos.DrawSphere(finalLocation, 0.6f);
    
        // transform.position = Vector3.Lerp(transform.position, finalLocation, 0.3f);
        // transform.LookAt(_target);
    }
    
    // private (Connection, float) GetClostedPointToTarget(Vector3 target)
    // {
    //     Node closestNode = null;
    //     float offsetOnNode = 0;
    //     float distanceResult = float.MaxValue;
    //     
    //     foreach (Node node in RailData._nodes)
    //     {
    //         float dist = (target - node._position).sqrMagnitude;
    //         if (dist < distanceResult)
    //         {
    //             distanceResult = dist;
    //             closestNode = node;
    //         }
    //     }
    //     
    //     foreach (Connection connection in RailData._connections)
    //     {
    //         float distance = Vector3.Dot(_target.position - connection.Node1.Position,
    //             (connection.Node2.Position - connection.Node1.Position).normalized);
    //
    //         if(distance < 0 || distance > (connection.Node2.Position - connection.Node1.Position).magnitude) continue;
    //         Vector3 pointOnLine = connection.Node1.Position + (connection.Node2.Position - connection.Node1.Position).normalized * distance;
    //         
    //         float dist = (target - pointOnLine).sqrMagnitude;
    //         if (dist < distanceResult)
    //         {
    //             distanceResult = dist;
    //             offsetOnNode = distance;
    //             closestNode = connection.Node1;
    //         }
    //     }
    //     
    //     return (_railData.ConnectionLookUp[closestNode!.Index][0], offsetOnNode);
    // }
}
