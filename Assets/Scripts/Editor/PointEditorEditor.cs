using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[CustomEditor(typeof(PointEditor))]
public class PointEditorEditor : Editor
{
     private int from = 0;
     private int to = 1;
     
     void OnSceneGUI()
     {
         PointEditor pointEditor = (PointEditor)target;
         for (int i = 0; i < pointEditor._dataToEdit._nodes.Count; i++)
         {
             EditorGUI.BeginChangeCheck();
             Vector3 newPos = Handles.PositionHandle(pointEditor._dataToEdit._nodes[i]._position, Quaternion.identity);
             if (EditorGUI.EndChangeCheck())
             {
                 Undo.RecordObject(pointEditor, "Move Point");
                 pointEditor._dataToEdit._nodes[i]._position = newPos;
                 EditorUtility.SetDirty(pointEditor);
             }
         }
     }
     

     public override void OnInspectorGUI()
     {
         DrawDefaultInspector();
         PointEditor pointEditor = (PointEditor)target;

         EditorGUILayout.LabelField("Points");
         RailData data = pointEditor._dataToEdit;
         for (int i = 0; i < data._nodes.Count; i++)
         {
             Node node = data._nodes[i];
             node._position = EditorGUILayout.Vector3Field($"{i}", node._position);
         }
         
         if (GUILayout.Button("Add Point"))
         {
             Undo.RecordObject(pointEditor, "Add Point");
             data._nodes.Add(new Node(Vector3.zero));
             data._connections.Add(data._nodes.Count-1, new ListOfNode());
             EditorUtility.SetDirty(pointEditor);
         }

         if (data._nodes.Count > 0 && GUILayout.Button("Remove Last Point"))
         {
             Undo.RecordObject(pointEditor, "Remove Point");
             data._connections.Remove(data._nodes.Count-1);
             data._nodes.RemoveAt(data._nodes.Count - 1);
             
             EditorUtility.SetDirty(pointEditor);
         }
         
         
         // Add connection UI
         GUILayout.Space(10);
         GUILayout.Label("Connections", EditorStyles.boldLabel);
         if (data._nodes.Count > 2)
         {
             GUILayout.BeginHorizontal();
             GUILayout.Label("From:", GUILayout.Width(40));
             from = EditorGUILayout.IntField(from, GUILayout.Width(30));
             GUILayout.Label("To:", GUILayout.Width(25));
             to = EditorGUILayout.IntField(to, GUILayout.Width(30));
             if (GUILayout.Button("Add Connection"))
             {
                 if (from != to && from >= 0 && to >= 0 && from < data._nodes.Count && to < data._nodes.Count)
                 {
                     Undo.RecordObject(pointEditor, "Add Connection");
                     
                     if(!data._connections[from]._nodes.Contains(to))
                     {
                         data._connections[from]._nodes.Add(to);
                         data._connections[from]._nodes.Add(to);
                     }
                     EditorUtility.SetDirty(pointEditor);
                 }
             }
             
             if (GUILayout.Button("Remove Connection"))
             {
                 if (from != to && from >= 0 && to >= 0 && from < data._nodes.Count && to < data._nodes.Count)
                 {
                     Undo.RecordObject(pointEditor, "Remove Connection");
                     
                     if(data._connections[from]._nodes.Contains(to))
                     {
                         data._connections[from]._nodes.Remove(to);
                         data._connections[to]._nodes.Remove(from);
                     }
                     EditorUtility.SetDirty(pointEditor);
                 }
             }
             GUILayout.EndHorizontal();
         }
         
         foreach (var from in data._connections)
         {
             foreach (var to in from.Value._nodes)
             {
                 GUILayout.Label($"{from.Key} <-> {to}");
             }
         }

         if (GUILayout.Button("Clear All"))
         {
             data._connections.Clear();
             data._nodes.Clear();
             EditorUtility.SetDirty(pointEditor);
         }
     }
}
