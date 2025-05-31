using System;
using UnityEngine;
using System.Linq;

public class FSMCamera : MonoBehaviour
{
    // private Camera _cameraControl;
    [SerializeField] private ReplayTarget[] _targets;
    [SerializeField] private ReplayTarget _mainTarget;
    private CameraOnRail _cameraOnRail;
    private SingleCamera[] _singleCameras;

    public ReplayTarget[] Targets => _targets;
    public ReplayTarget MainTarget => _mainTarget;
    
    private float _scoreRedTeam = 0;
    private float _scoreBlueTeam = 0;
    
    
    void Start()
    {
        _cameraOnRail = FindObjectsByType<CameraOnRail>(FindObjectsSortMode.None)[0];
        _singleCameras = FindObjectsByType<SingleCamera>(FindObjectsSortMode.None);

        foreach (SingleCamera singleCamera in _singleCameras)
        {
            singleCamera.Owner = this;
        }
    }

    void Update()
    {
        foreach (SingleCamera singleCamera in _singleCameras)
        {
            singleCamera.UpdateScore();
        }

        SingleCamera bestCamera = _singleCameras[0];
        foreach (SingleCamera singleCamera in _singleCameras)
        {
            if (singleCamera.Score > bestCamera.Score)
            {
                bestCamera = singleCamera;
            }
        }

        Camera.main.transform.position = bestCamera.transform.position;
        Camera.main.transform.LookAt(MainTarget.transform.position);

        // float bestCamera = _singleCameras.(camera => camera.Score);

    }
}
