using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using Enemies;
using UnityEngine;

namespace Enemies
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvincible;
        public bool CanMove;
        public Animator Anim;
        private EnemyTarget _enemyTarget;
        private AnimatorHook _animatorHook;
        public Rigidbody _Rigidbody;
        public AnimationCurve roll_curve { get; set; }
        public float delta { get; set; }

        private void Start()
        {
            Anim = GetComponentInChildren<Animator>();
            _enemyTarget = GetComponent<EnemyTarget>();
            _enemyTarget.Init(Anim);

            _Rigidbody = GetComponent<Rigidbody>();
            
            _animatorHook = Anim.GetComponent<AnimatorHook>();
            if (_animatorHook == null)
                _animatorHook = Anim.gameObject.AddComponent<AnimatorHook>();
            _animatorHook.Init(null, this);
        }

        private void Update()
        {
            delta = Time.deltaTime;
            CanMove = Anim.GetBool("canMove");
            if (isInvincible)
            {
                isInvincible = !CanMove;
            }

            if (CanMove)
            {
                Anim.applyRootMotion = false;
            }
            
        }

        public void DoDamage(float v)
        {
            if (isInvincible)
                return;
            health -= v;
            isInvincible = true;
            Anim.Play("damage_1");
            Anim.applyRootMotion = true;
        }
    }
}

