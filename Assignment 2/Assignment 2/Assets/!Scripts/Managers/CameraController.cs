using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private float _yawSpeed = 130f;
    [SerializeField] private float _maxYawSpeed = 130f;

    private CinemachineOrbitalTransposer _orbital;
    private CinemachineVirtualCamera _vcam;
    private GameController _gc;

    private Vector2 _currentVelocity = Vector2.zero;
    private float _timeSinceRightClick = 0;


    private void Start()
    {
        if (TryGetComponent(out _vcam))
            _orbital = _vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private void LateUpdate()
    {
        if (_currentVelocity.sqrMagnitude > 0)
            _currentVelocity = new((Mathf.Abs(_currentVelocity.x) - 2) * Mathf.Sign(_currentVelocity.x), (Mathf.Abs(_currentVelocity.y) - 2) * Mathf.Sign(_currentVelocity.y));

        if (GameController.Instance.InDebugMode == false && Input.GetMouseButton(1))
        {
            _timeSinceRightClick = 0;

            if (Cursor.lockState != CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.Locked;

            DoYaw();
        }
        else if (Cursor.lockState == CursorLockMode.Locked)
            Cursor.lockState = CursorLockMode.None;
        else
        {
            if (_timeSinceRightClick <= 1)
                _timeSinceRightClick += Time.deltaTime;

            if (_currentVelocity.sqrMagnitude > 0)
            {
                _currentVelocity = Vector2.Lerp(_currentVelocity, Vector2.zero, Mathf.Clamp01(_timeSinceRightClick));
            }
        }
    }

    private void DoYaw()
    {
        float mag = Input.GetAxisRaw("Mouse X");
        mag *= _yawSpeed;
        _currentVelocity.x += mag;

        if (MathF.Abs(_currentVelocity.x) > _maxYawSpeed)
        {
            _currentVelocity.x = _maxYawSpeed * Mathf.Sign(_currentVelocity.x);
        }

        _orbital.m_XAxis.Value += _currentVelocity.x * Time.deltaTime;
    }

    public void SetActiveCamera(bool debugActive)
    {
        _vcam.Priority = debugActive ? -1 : 1;
    }
}
