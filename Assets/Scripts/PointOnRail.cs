using UnityEngine;

public class PointOnRail
{
    public PointOnRail(Node node, CameraOnRail owner)
    {
        Node = node;
        _owner = owner;
    }
    
    public Node Node { get; private set; }
    private readonly CameraOnRail _owner;
    private float _score = 0;
    private float _canSeeTargetTime = 0;
    
    public void UpdateScore()
    {
        Vector3 toTarget = _owner.Target.transform.position - Node.Position;
        if (!Physics.Raycast(Node.Position, toTarget.normalized, out RaycastHit hit, toTarget.magnitude))
        {
            _score = 0;
            _canSeeTargetTime = Mathf.Clamp01(_canSeeTargetTime + Time.deltaTime / 10.0f);

            float dist = (toTarget).magnitude;
            float distanceScore = MathHelper.Map(dist, 0, 100, 1, 0);
            
            float diffPitchAngle = Mathf.Abs(
                Mathf.DeltaAngle(Mathf.Asin(toTarget.normalized.y) * Mathf.Rad2Deg, 
                    -15));
            float scorePitch = Mathf.Clamp01(1f - diffPitchAngle / 180f);

            float angle = Mathf.Atan2(toTarget.normalized.z, toTarget.normalized.x) * Mathf.Rad2Deg;
            float angleCamera = Mathf.Atan2(_owner.transform.forward.z, _owner.transform.forward.x) * Mathf.Rad2Deg;
            float diffYaw = Mathf.Abs(Mathf.DeltaAngle(angle, angleCamera));
            float scoreYaw = Mathf.Clamp01(1f - diffYaw / 180f);
            
            _score += scorePitch;
            _score += scoreYaw;
            // _score += _canSeeTargetTime;
            _score += distanceScore;
        }
        else
        {
            _canSeeTargetTime = 0;
            _score = 0;
        }
    }

    public float GetScore()
    {
        return _score;
    }
}
