using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

public class SingleCamera : MonoBehaviour, CameraScore
{
    public float Score;
    public UtitlyController Owner { private get; set; }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position + transform.up, Score.ToString(CultureInfo.InvariantCulture));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one*0.3f);
    }

    public void ControlCamera(Camera camera)
    {
        camera.transform.position = transform.position;
        camera.transform.LookAt(Owner.MainTarget.transform.position);
    }

    public void SwappedCamera()
    {
        Score = Mathf.Max(0.0f, Score - 0.5f);
    }
    
    public void UpdateScore()
    {
        Vector3 directionToTarget = Owner.MainTarget.transform.position - transform.position;
        if (!Physics.Raycast(transform.position, directionToTarget))
        {
            Score += 0.01f * Time.deltaTime;
        }

        if (directionToTarget.magnitude > 5)
        {
            Score += MathHelper.Map(directionToTarget.magnitude, 20, 100, 100, 0);
        }
        
    }

    public float GetScore()
    {
        return Score;
    }
}
