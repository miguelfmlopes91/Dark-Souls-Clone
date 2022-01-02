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
        
        public void Init(StateManager st)//TODO: dependency injection
        {
            _stateManager = st;
            _animator = _stateManager.Anim;
        }

        void OnAnimatorMove()
        {
            if (_stateManager.CanMove)
                return;

            _stateManager.RgBody.drag = 0;//we don't want drag cus we moving with motions
            float multiplier = 1;

            Vector3 delta = _animator.deltaPosition;
            delta.y = 0;
            Vector3 v = (delta * multiplier) / _stateManager.Delta;
            _stateManager.RgBody.velocity = v;
        }
    }
}
