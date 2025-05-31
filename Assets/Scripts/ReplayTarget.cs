using UnityEngine;
using System;

public class ReplayTarget : MonoBehaviour
{
    private ReplayOutSide _output;
    private int _step = 0;
    private float _timePassed = 0;
    
    void OnEnable()
    {
        _output =  JsonUtility.FromJson<ReplayOutSide>(Data.BigHyperDashString);
    }

    private void Update()
    {
        transform.position = new Vector3(_output.data[_step].x, _output.data[_step].y, _output.data[_step].z);

        _timePassed += Time.deltaTime;
        if (_timePassed > 1.0f / 165.0f)
        {
            _timePassed = 0;
            _step = (++_step) % _output.data.Length;
        }
    }
}
