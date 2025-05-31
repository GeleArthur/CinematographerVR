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
        foreach (Node node in _dataToEdit.Nodes)
        {
            Gizmos.DrawWireSphere(node.Position, 0.3f);
        }
        
        Gizmos.color = Color.cyan;
        foreach (var from in _dataToEdit.Connections)
        {
            foreach (var to in from.Value.Nodes)
            {
                Gizmos.DrawLine(_dataToEdit.Nodes[from.Key].Position, _dataToEdit.Nodes[to.NodeIndex].Position);
            }
        }
    }
}




