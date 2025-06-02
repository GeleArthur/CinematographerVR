using System;
using UnityEngine;
using System.Linq;

public class UtitlyController : MonoBehaviour
{
    [SerializeField] private ReplayTarget _mainTarget = null;
    private CameraOnRail _cameraOnRail;
    
    public ReplayTarget MainTarget => _mainTarget;
    
    void Start()
    {
        _cameraOnRail = FindObjectsByType<CameraOnRail>(FindObjectsSortMode.None)[0];
    }
    

    void Update()
    {
        // foreach (CameraScore singleCamera in _cameras)
        // {
        //     singleCamera.UpdateScore();
        // }
        //
        // Array.Sort(_cameras, ((score1, score2) => score1.GetScore().CompareTo(score2.GetScore())));
        //
        // CameraScore newCamera = _cameras[0];
        //
        // if (_currentCamera != newCamera)
        // {
        //     _currentCamera = newCamera;
        //     foreach (CameraScore singleCamera in _cameras)
        //     {
        //         singleCamera.SwappedCamera();
        //     }
        // }
        //
        // _currentCamera.ControlCamera(Camera.main);
    }
}
