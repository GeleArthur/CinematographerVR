using System;
using UnityEngine;
using System.Linq;

public class UtitlyController : MonoBehaviour
{
    // private Camera _cameraControl;
    // [SerializeField] private ReplayTarget[] _targets;
    [SerializeField] private ReplayTarget _mainTarget;
    private CameraOnRail _cameraOnRail;
    private SingleCamera[] _singleCameras;

    // public ReplayTarget[] Targets => _targets;
    public ReplayTarget MainTarget => _mainTarget;
    private CameraScore[] _cameras;
    private CameraScore _currentCamera;
    
    
    void Start()
    {
        _cameraOnRail = FindObjectsByType<CameraOnRail>(FindObjectsSortMode.None)[0];
        _singleCameras = FindObjectsByType<SingleCamera>(FindObjectsSortMode.None);

        foreach (SingleCamera singleCamera in _singleCameras)
        {
            singleCamera.Owner = this;
        }

        _cameras = new CameraScore[_singleCameras.Length + 1];
        _cameras[0] = _cameraOnRail;
        Array.Copy(_singleCameras, 0, _cameras, 1, _singleCameras.Length);
    }

    void Update()
    {
        foreach (CameraScore singleCamera in _cameras)
        {
            singleCamera.UpdateScore();
        }

        Array.Sort(_cameras, ((score1, score2) => score1.GetScore().CompareTo(score2.GetScore())));

        CameraScore newCamera = _cameras[0];

        if (_currentCamera != newCamera)
        {
            _currentCamera = newCamera;
            foreach (CameraScore singleCamera in _cameras)
            {
                singleCamera.SwappedCamera();
            }
        }

        _currentCamera.ControlCamera(Camera.main);
    }
}
