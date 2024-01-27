using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraArm;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _cameraTarget;
    [Space]
    [SerializeField] private float _panSpeed;

    private void Awake()
    {
        _cameraPivot.LookAt(_cameraTarget);
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;

            PanCamera();
        }
        else if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
    }

    private void PanCamera()
    {
        float panDirection = Input.GetAxisRaw("Mouse X");

        _cameraArm.Rotate(panDirection * _panSpeed * 100f * Time.deltaTime * Vector3.up);
    }
}