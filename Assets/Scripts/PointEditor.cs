using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class PointEditor : MonoBehaviour
{
    public RailData _dataToEdit;

    private void OnDrawGizmos()
    {
        if(_dataToEdit == null) return;
        Gizmos.color = Color.yellow;
        foreach (Node node in _dataToEdit._nodes)
        {
            Gizmos.DrawWireSphere(node._position, 0.3f);
        }
        
        Gizmos.color = Color.cyan;
        foreach (var from in _dataToEdit._connections)
        {
            foreach (int to in from.Value._nodes)
            {
                Gizmos.DrawLine(_dataToEdit._nodes[from.Key]._position, _dataToEdit._nodes[to]._position);
            }
        }
    }
}


