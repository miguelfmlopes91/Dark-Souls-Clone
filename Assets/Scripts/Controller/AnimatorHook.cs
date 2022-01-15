using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;


namespace Controller
{
    public class AnimatorHook : MonoBehaviour
    {
        private Animator _animator;
        private StateManager _stateManager;
        private Rigidbody _rgBody;
        private EnemyStates _enemyStates;
        private AnimationCurve _animationCurve;
        public float RootMotionMultiplier { get; set; }
        private bool rolling;
        private float roll_t;
        private float delta;
        
        public void Init(StateManager st, EnemyStates enemyStates)//TODO: dependency injection
        {
            _stateManager = st;
            _enemyStates = enemyStates;
            if (_stateManager != null)
            {
                _animator = _stateManager.Anim;
                _rgBody = _stateManager.RgBody;
                _animationCurve = _stateManager.roll_curve;
                delta = _stateManager.Delta;
            }

            if (_enemyStates != null)
            {
                _animator = _enemyStates.Anim;
                _rgBody = _enemyStates._Rigidbody;
                _animationCurve = _enemyStates.roll_curve;
                delta = _enemyStates.delta;
            }
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
            if (_stateManager == null && _enemyStates == null) return;
            if (_rgBody == null) return;
            if (_stateManager != null)
            {
                if( !_stateManager.CanMove) return;
                delta = _stateManager.Delta;
            }

            if (_enemyStates != null)
            {
                if(!_enemyStates.CanMove) return;
                delta = _enemyStates.delta;
            } 

            _rgBody.drag = 0;//we don't want drag cus we moving with motions

            if (RootMotionMultiplier == 0) 
                RootMotionMultiplier = 1;

            if (!rolling)
            {
                Vector3 delta2 = _animator.deltaPosition;
                delta2.y = 0;
                Vector3 v = (delta2 * RootMotionMultiplier) / delta;
                _rgBody.velocity = v;
            }
            else
            {
                //sample the curve
                roll_t += delta / 0.5f;
                if (roll_t > 1)
                {
                    roll_t = 1;
                }
                
                if (_stateManager == null) return;
                float zValue = _animationCurve.Evaluate(roll_t);
                
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * RootMotionMultiplier);
                _rgBody.velocity = v2;
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

