using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class InputHandler : MonoBehaviour
    {
        private float vertical;
        private float horizontal;
        private float delta;

        private StateManager _stateManager;
        private CameraManager _cameraManager;
        void Start()
        {
            _stateManager = GetComponent<StateManager>();
            _stateManager.Init();
            _cameraManager = CameraManager.Instance;
            _cameraManager.Init(transform);
        }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            _stateManager.FixedTick(delta);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            _cameraManager.Tick(delta);
        }

        private void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
        }

        private void UpdateStates()
        {
            _stateManager.Horizontal = horizontal;
            _stateManager.Vertical = vertical;

            Vector3 v = _stateManager.Vertical * _cameraManager.transform.forward;
            Vector3 h = _stateManager.Horizontal * _cameraManager.transform.right;
            _stateManager.MoveDirection = (v + h).normalized;
            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            _stateManager.MoveAmount = Mathf.Clamp01(m);
        }
    }
}