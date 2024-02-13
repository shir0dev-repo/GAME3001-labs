using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _cameraArm;
    [SerializeField] private Transform _cameraPivot;
    [Space]
    [SerializeField] private float _panSpeed;

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
        float panDirection = Mathf.Clamp(Input.GetAxisRaw("Mouse X"), -1, 1);

        _cameraArm.Rotate(panDirection * _panSpeed * 50 * Time.deltaTime * Vector3.up);
    }
}