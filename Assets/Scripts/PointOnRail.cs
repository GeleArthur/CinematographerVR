using UnityEngine;

public class PointOnRail
{
    public PointOnRail(Node node, CameraOnRail owner)
    {
        Node = node;
        _owner = owner;
    }
    
    public Node Node { get; private set; }
    private CameraOnRail _owner;
    private float _score = 0;
    
    public void UpdateScore()
    {
        
    }

    public float GetScore()
    {
        return _score;
    }
    

    public void SwappedCamera()
    {
        
    }
}
