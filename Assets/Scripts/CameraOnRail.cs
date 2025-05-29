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

        Debug.DrawRay(transform.position, (_target.position - transform.position));
        if (Physics.Raycast(transform.position, transform.forward, (_target.position - transform.position).magnitude))
        {
            Gizmos.color = Color.green;
            var result = GetClostedPointToTarget(_target.position);
            Vector3 finalLocation = result.Item1._position + (result.Item2._position - result.Item1._position).normalized * result.Item3;
            Gizmos.DrawSphere(finalLocation, 0.6f);
            transform.position = Vector3.Lerp(transform.position, finalLocation, 0.3f);
        }
        
    
        transform.LookAt(_target);
    }
    
    private (Node, Node, float) GetClostedPointToTarget(Vector3 target)
    {
        Node nodeFrom = null;
        Node nodeTo = null;
        float offsetOnNode = 0;
        float distanceResult = float.MaxValue;
        
        foreach (Node node in _railData._nodes)
        {
            float dist = (target - node._position).sqrMagnitude;
            if (dist < distanceResult)
            {
                distanceResult = dist;
                nodeFrom = node;
                nodeTo = node;
            }
        }
        
        foreach (var from in _railData._connections)
        {
            foreach (var to in from.Value._nodes)
            {
                float distance = Vector3.Dot(_target.position - _railData._nodes[from.Key]._position, 
                    (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).normalized);
                
                if(distance < 0 || distance > (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).magnitude) continue;
                
                Vector3 pointOnLine = _railData._nodes[from.Key]._position + (_railData._nodes[to]._position - _railData._nodes[from.Key]._position).normalized * distance;
                float dist = (target - pointOnLine).sqrMagnitude;
                if (dist < distanceResult)
                {
                    distanceResult = dist;
                    offsetOnNode = distance;
                    nodeFrom = _railData._nodes[from.Key];
                    nodeTo = _railData._nodes[to];
                }
                
            }
    
            
        }
        
        return (nodeFrom, nodeTo, offsetOnNode);
    }
}
