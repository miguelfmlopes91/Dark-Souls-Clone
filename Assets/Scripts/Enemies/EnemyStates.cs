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
        public bool isDead;
        
        public Animator Anim;
        private EnemyTarget _enemyTarget;
        private AnimatorHook _animatorHook;
        public Rigidbody _Rigidbody;
        public AnimationCurve roll_curve { get; set; }
        public float delta { get; set; }

        private List<Rigidbody> _ragDollRigidbodies = new List<Rigidbody>();
        private List<Collider> _ragDollColliders = new List<Collider>();

        private void Start()
        {
            health = 100;
            Anim = GetComponentInChildren<Animator>();
            _enemyTarget = GetComponent<EnemyTarget>();
            _enemyTarget.Init(this);

            _Rigidbody = GetComponent<Rigidbody>();
            
            _animatorHook = Anim.GetComponent<AnimatorHook>();
            if (_animatorHook == null)
                _animatorHook = Anim.gameObject.AddComponent<AnimatorHook>();
            _animatorHook.Init(null, this);
            
            InitRagdoll();
        }

        private void InitRagdoll()
        {
            var rigs = GetComponentsInChildren<Rigidbody>();
            foreach (var rg in rigs)
            {
                if(rg == _Rigidbody) continue;
                rg.isKinematic = true;
                var col = rg.GetComponent<Collider>();
                col.isTrigger = true;
                _ragDollColliders.Add(col);
                _ragDollRigidbodies.Add(rg);
            }
        }

        public void EnableRagDoll()
        {
            for (int i = 0; i < _ragDollRigidbodies.Count; i++)
            {
                _ragDollRigidbodies[i].isKinematic = false;
                _ragDollColliders[i].isTrigger = false;
            }

            var controllerCollider = _Rigidbody.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            _Rigidbody.isKinematic = true;
            
            StartCoroutine(CloseAnimator());
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForEndOfFrame();
            Anim.enabled = false;
            this.enabled = false;
        }

        private void Update()
        {
            delta = Time.deltaTime;
            CanMove = Anim.GetBool("canMove");

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    EnableRagDoll();
                }
            }
            
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

