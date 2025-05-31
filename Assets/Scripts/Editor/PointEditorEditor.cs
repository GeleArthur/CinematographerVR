using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[CustomEditor(typeof(PointEditor))]
public class PointEditorEditor : Editor
{
     private int fromInsector = 0;
     private int toinsepctor = 1;
     
     void OnSceneGUI()
     {
         PointEditor pointEditor = (PointEditor)target;
         RailData pointEditorDataToEdit = pointEditor._dataToEdit;
         for (int i = 0; i < pointEditorDataToEdit.Nodes.Count; i++)
         {
             EditorGUI.BeginChangeCheck();
             Vector3 newPos = Handles.PositionHandle(pointEditorDataToEdit.Nodes[i].Position, Quaternion.identity);
             if (EditorGUI.EndChangeCheck())
             {
                 Undo.RecordObject(pointEditor, "Move Point");
                 pointEditorDataToEdit.Nodes[i].Position = newPos;
                 EditorUtility.SetDirty(pointEditorDataToEdit);
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
         for (int i = 0; i < data.Nodes.Count; i++)
         {
             Node node = data.Nodes[i];
             node.Position = EditorGUILayout.Vector3Field($"{i}", node.Position);
         }
         
         if (GUILayout.Button("Add Point"))
         {
             Undo.RecordObject(pointEditor, "Add Point");
             data.Nodes.Add(new Node(Vector3.zero));
             data.Connections.Add(data.Nodes.Count-1, new ListOfNode());
             data.Nodes[^1].Position = SceneView.lastActiveSceneView.camera.transform.position;
             EditorUtility.SetDirty(data);
             EditorUtility.SetDirty(pointEditor);
         }

         if (data.Nodes.Count > 0 && GUILayout.Button("Remove Last Point"))
         {
             Undo.RecordObject(pointEditor, "Remove Point");
             data.Connections.Remove(data.Nodes.Count-1);
             data.Nodes.RemoveAt(data.Nodes.Count - 1);
             
             EditorUtility.SetDirty(data);
             EditorUtility.SetDirty(pointEditor);
         }
         
         
         // Add connection UI
         GUILayout.Space(10);
         GUILayout.Label("Connections", EditorStyles.boldLabel);
         if (data.Nodes.Count > 2)
         {
             if (GUILayout.Button("Generate Connections"))
             {
                 HashSet<int> looked_over = new();
                 foreach (KeyValuePair<int,ListOfNode> connection in data.Connections)
                 {
                     connection.Value.Nodes.Clear();
                 }

                 for (int i = 0; i < data.Nodes.Count; i++)
                 {
                     looked_over.Add(i);
                     for (int j = 0; j < data.Nodes.Count; j++)
                     {
                         Vector3 from = data.Nodes[i].Position;
                         Vector3 to = data.Nodes[j].Position;
                         
                         if (looked_over.Contains(j)) continue;
                         float distance = (to - from).magnitude;
                         if (Physics.Raycast(from, (to - from), distance) == false)
                         {
                             data.Connections[i].Nodes.Add(new Connection(j, distance));
                             data.Connections[j].Nodes.Add(new Connection(i, distance));
                         }
                     }
                 }
                 
                 EditorUtility.SetDirty(data);
                 EditorUtility.SetDirty(pointEditor);
             }
             
             
             GUILayout.BeginHorizontal();
             GUILayout.Label("From:", GUILayout.Width(40));
             fromInsector = EditorGUILayout.IntField(fromInsector, GUILayout.Width(30));
             GUILayout.Label("To:", GUILayout.Width(25));
             toinsepctor = EditorGUILayout.IntField(toinsepctor, GUILayout.Width(30));
             if (GUILayout.Button("Add Connection"))
             {
                 if (fromInsector != toinsepctor && fromInsector >= 0 && toinsepctor >= 0 && fromInsector < data.Nodes.Count && toinsepctor < data.Nodes.Count)
                 {
                     Undo.RecordObject(pointEditor, "Add Connection");
                     
                     if(!data.Connections[fromInsector].Nodes.Exists(e => toinsepctor == e.NodeIndex))
                     {
                         float distance = (data.Nodes[fromInsector].Position - data.Nodes[toinsepctor].Position).magnitude;

                         
                         data.Connections[fromInsector].Nodes.Add(new Connection(toinsepctor, distance));
                         data.Connections[toinsepctor].Nodes.Add(new Connection(fromInsector, distance));
                     }
                     EditorUtility.SetDirty(data);
                     EditorUtility.SetDirty(pointEditor);
                     
                 }
             }
             
             if (GUILayout.Button("Remove Connection"))
             {
                 if (fromInsector != toinsepctor && fromInsector >= 0 && toinsepctor >= 0 && fromInsector < data.Nodes.Count && toinsepctor < data.Nodes.Count)
                 {
                     Undo.RecordObject(pointEditor, "Remove Connection");
                     
                     if(data.Connections[fromInsector].Nodes.Exists(e => e.NodeIndex == toinsepctor))
                     {
                         data.Connections[fromInsector].Nodes.RemoveAll(e => e.NodeIndex == toinsepctor);
                         data.Connections[toinsepctor].Nodes.RemoveAll(e => e.NodeIndex == fromInsector);
                     }
                     EditorUtility.SetDirty(pointEditor);
                     EditorUtility.SetDirty(data);
                 }
             }
             GUILayout.EndHorizontal();
         }
         
         foreach (var from in data.Connections)
         {
             foreach (var to in from.Value.Nodes)
             {
                 GUILayout.Label($"{from.Key} <-> {to.NodeIndex} : {to.Weight}");
             }
         }

         if (GUILayout.Button("Clear All"))
         {
             data.Connections.Clear();
             data.Nodes.Clear();
             EditorUtility.SetDirty(pointEditor);
             EditorUtility.SetDirty(data);
         }
     }
}
