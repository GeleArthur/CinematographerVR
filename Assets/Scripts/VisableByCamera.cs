using System.Collections.Generic;
using UnityEngine;

public class VisableByCamera : MonoBehaviour
{
    public List<Transform> _listToCheck = new List<Transform>();

    private Camera _camera;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void OnDrawGizmos()
    {
        _camera = GetComponent<Camera>();
        foreach (Transform thing in _listToCheck)
        {
            Vector3 ndcSpace = _camera.WorldToViewportPoint(thing.position);
            if (ndcSpace.x > 0 && ndcSpace.x < 1 && ndcSpace.y > 0 && ndcSpace.y < 1)
            {
                DebugExtension.DebugCircle(thing.position, Color.black);
            }
        }
    }
}

// Camera thats still in a room.
// Camera that follows 1 player around on a bar

// Then the cinematrographer is going to deside which camera we use.
