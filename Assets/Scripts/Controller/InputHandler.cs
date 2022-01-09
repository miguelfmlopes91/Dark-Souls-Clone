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
        private bool b_Input;
        private bool a_Input;
        private bool x_Input;
        private bool y_Input;
        
        private bool rb_Input;
        private float rt_Axis;
        private bool rt_Input;
        private bool lb_Input;
        private float lt_Axis;
        private bool lt_Input;

        private bool leftAxis_down;
        private bool rightAxis_down;

        private float b_timer;
        private float rt_timer;

        private StateManager _stateManager;
        private CameraManager _cameraManager;
        void Start()
        {
            _stateManager = GetComponent<StateManager>();
            _stateManager.Init();
            _cameraManager = CameraManager.Instance;
            _cameraManager.Init(_stateManager);
        }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            _stateManager.FixedTick(delta);
            _cameraManager.FixedTick(delta);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            _stateManager.Tick(delta);
            ResetInputAndStates();
        }

        private void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            b_Input = Input.GetButton("B");
            a_Input = Input.GetButton("A");
            x_Input = Input.GetButton("X");
            y_Input = Input.GetButtonUp("Y");
            rt_Input = Input.GetButton("RT");
            rt_Axis = Input.GetAxis("RT");
            if (rt_Axis != 0)
                rt_Input = true;
            
            lt_Input = Input.GetButton("LT");
            lt_Axis = Input.GetAxis("LT");
            if (lt_Axis != 0)
                lt_Input = true;
            
            rb_Input = Input.GetButton ("RB");
            lb_Input = Input.GetButton ("LB");
            rightAxis_down = Input.GetButtonUp ("L");

            if (b_Input)
                b_timer += delta;
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

            if (x_Input)
                b_Input = false;
            
            if (b_Input && b_timer > 0.5f)
            {
                _stateManager.Running = (_stateManager.MoveAmount > 0);
            }

            if (!b_Input && b_timer > 0 && b_timer < 0.5f)
                _stateManager.RollInput = true;

            _stateManager.ItemInput = x_Input;
            _stateManager.rt = rt_Input;
            _stateManager.lt = lt_Input;
            _stateManager.rb = rb_Input;
            _stateManager.lb = lb_Input;

            if (y_Input)
            {
                _stateManager.IsTwoHanded = !_stateManager.IsTwoHanded;
                _stateManager.HandleTwoHanded();
            }

            if (rightAxis_down)
            {
                _stateManager.LockOn = !_stateManager.LockOn;
                if (_stateManager.LockOnTarget == null)
                {
                    _stateManager.LockOn = false;
                }

                _cameraManager.lockOnTarget = _stateManager.LockOnTarget;
                _stateManager.LockOnTransform = _cameraManager.lockOnTargetTransform;
                _cameraManager.LockOn = _stateManager.LockOn;
            }
        }

        private void ResetInputAndStates()
        {
            if (!b_Input)
                b_timer = 0;
            if (_stateManager.RollInput)
                _stateManager.RollInput = false;
            if (_stateManager.Running)
                _stateManager.Running = false; 
        }
    }
}