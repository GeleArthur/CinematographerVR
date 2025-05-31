using System;
using UnityEngine;

public class SingleCamera : MonoBehaviour
{
    public float Score;
    public FSMCamera Owner { private get; set; }

    public void UpdateScore()
    {
        Score = 0;
        Vector3 directionToTarget = Owner.MainTarget.transform.position - transform.position;
        if (!Physics.Raycast(transform.position, directionToTarget))
        {
            Score += 100;
        }

        // foreach (ReplayTarget target in Owner.Targets)
        // {
        //     if(target == Owner.MainTarget) continue;
        //     if (!Physics.Raycast(transform.position, target.transform.position - transform.position))
        //     {
        //         _score += 10;
        //     }
        // }

        if (directionToTarget.magnitude > 5)
        {
            Score += MathHelper.Map(directionToTarget.magnitude, 20, 100, 100, 0);
        }


        // float center = Mathf.Cos(45 * Mathf.Deg2Rad);
        // float value = directionToTarget.normalized.y;
        // float range = Mathf.Max(Mathf.Abs(center - (-1)), Mathf.Abs(center - 1));
        // float t = (value - center) / range;
        // float score = Mathf.Clamp01(1 - t * t) * 100;
        // Score += score;
        


    }
}
