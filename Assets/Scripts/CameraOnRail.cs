using System;
using System.Collections.Generic;
using UnityEngine;



public class CameraOnRail : MonoBehaviour
{
    [SerializeField] private RailData _railData;
    [SerializeField] private Transform _target;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        foreach (Node node in RailData._nodes)
        {
            Gizmos.DrawSphere(node.Position, 0.3f);
        }
        
        foreach (Connection connection in RailData._connections)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(connection.Node1.Position, connection.Node2.Position);


            float distance = Vector3.Dot(_target.position - connection.Node1.Position,
                (connection.Node2.Position - connection.Node1.Position).normalized);

            if(distance < 0 || distance > (connection.Node2.Position - connection.Node1.Position).magnitude) continue;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(connection.Node1.Position + (connection.Node2.Position - connection.Node1.Position).normalized * distance, 0.2f);
        }
        
        Gizmos.color = Color.green;
        var result = GetClostedPointToTarget(_target.position);
        Vector3 finalLocation = result.Item1.Node1.Position + (result.Item1.Node2.Position - result.Item1.Node1.Position).normalized * result.Item2;
        Gizmos.DrawSphere(finalLocation, 0.6f);

        transform.position = Vector3.Lerp(transform.position, finalLocation, 0.3f);
        transform.LookAt(_target);
    }

    private (Connection, float) GetClostedPointToTarget(Vector3 target)
    {
        Node closestNode = null;
        float offsetOnNode = 0;
        float distanceResult = float.MaxValue;
        
        foreach (Node node in RailData._nodes)
        {
            float dist = (target - node.Position).sqrMagnitude;
            if (dist < distanceResult)
            {
                distanceResult = dist;
                closestNode = node;
            }
        }
        
        foreach (Connection connection in RailData._connections)
        {
            float distance = Vector3.Dot(_target.position - connection.Node1.Position,
                (connection.Node2.Position - connection.Node1.Position).normalized);

            if(distance < 0 || distance > (connection.Node2.Position - connection.Node1.Position).magnitude) continue;
            Vector3 pointOnLine = connection.Node1.Position + (connection.Node2.Position - connection.Node1.Position).normalized * distance;
            
            float dist = (target - pointOnLine).sqrMagnitude;
            if (dist < distanceResult)
            {
                distanceResult = dist;
                offsetOnNode = distance;
                closestNode = connection.Node1;
            }
        }
        
        return (_railData.ConnectionLookUp[closestNode!.Index][0], offsetOnNode);
    }
}
