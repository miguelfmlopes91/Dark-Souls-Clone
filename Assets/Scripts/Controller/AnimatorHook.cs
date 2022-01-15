using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Controller
{
    public class AnimatorHook : MonoBehaviour
    {
        private Animator _animator;
        private StateManager _stateManager;
        public float RootMotionMultiplier { get; set; }
        private bool rolling;
        private float roll_t;
        
        public void Init(StateManager st)//TODO: dependency injection
        {
            _stateManager = st;
            _animator = _stateManager.Anim;
        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0f;
        }

        public void CloseRoll()
        {
            if (!rolling)
                return;

            RootMotionMultiplier = 1;
            roll_t = 0f;
            rolling = false;
        }
        
        private void OnAnimatorMove()
        {
            if (_stateManager == null) return;
            if (_stateManager.CanMove) return;

            _stateManager.RgBody.drag = 0;//we don't want drag cus we moving with motions

            if (RootMotionMultiplier == 0) 
                RootMotionMultiplier = 1;

            if (!rolling)
            {
                Vector3 delta = _animator.deltaPosition;
                delta.y = 0;
                Vector3 v = (delta * RootMotionMultiplier) / _stateManager.Delta;
                _stateManager.RgBody.velocity = v;
            }
            else
            {
                //sample the curve
                roll_t += _stateManager.Delta / 0.5f;
                if (roll_t > 1)
                {
                    roll_t = 1;
                }
                float zValue = _stateManager.roll_curve.Evaluate(roll_t);
                
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * RootMotionMultiplier);
                _stateManager.RgBody.velocity = v2;
            }
        }

        public void OpenDamageColliders()
        {
            if (_stateManager == null) return;
            _stateManager.InventoryManager.currentWeapon.w_hook.OpenDamageColliders();
        }

        public void CloseDamageColliders()
        {
            if (_stateManager == null) return;
            _stateManager.InventoryManager.currentWeapon.w_hook.CloseDamageColliders();
        }
    }
}

