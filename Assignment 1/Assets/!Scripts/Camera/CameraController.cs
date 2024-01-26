using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _focusTransform;
    [SerializeField] private Transform _cameraPivot;
    [SerializeField] private Transform _cameraArm;

    [SerializeField] private float _rotationSpeed = 30f;
    [SerializeField] private float _zoomSpeed = 10f;
    [SerializeField] private float _maxDistance = 30f;
    [SerializeField] private float _minDistance = 10f;
    [SerializeField] private float _cameraHeight = 15f;

    private float _currentDistance;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;

        _cameraPivot.LookAt(_focusTransform);
    }

    private void LateUpdate()
    {
        // pan
        if (Input.GetMouseButton(1))
        {
            PanCamera();
        }

        // zoom
        if (Input.mouseScrollDelta.sqrMagnitude > 0.1f)
        {
            ZoomCamera();
        }
    }

    private void PanCamera()
    {
        float rotDirectionX = Input.GetAxisRaw("Mouse X");
        _cameraArm.Rotate(rotDirectionX * _rotationSpeed * Time.deltaTime * Vector3.up);
    }

    private void ZoomCamera()
    {
        float zoomDirection = Input.GetAxisRaw("Mouse ScrollWheel");
        Vector3 newPosition = _zoomSpeed * zoomDirection * _cameraPivot.forward;
        _cameraPivot.position += newPosition;

        _currentDistance = Vector3.Distance(_cameraPivot.position, _focusTransform.position);
        if (_currentDistance > _maxDistance)
        {
            _cameraPivot.position = (_cameraPivot.position - _focusTransform.position).normalized * _maxDistance;
        }
        else if (_currentDistance < _minDistance)
        {
            _cameraPivot.position = (_cameraPivot.position - _focusTransform.position).normalized * _minDistance;
        }
    }
}
