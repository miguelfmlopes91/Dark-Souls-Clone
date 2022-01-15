using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;

namespace Enemies
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvincible;
        private Animator _animator;
        private EnemyTarget _enemyTarget;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _enemyTarget = GetComponent<EnemyTarget>();
            _enemyTarget.Init(_animator);
        }

        private void Update()
        {
            if (isInvincible)
            {
                isInvincible = !_animator.GetBool("canMove");    
            }
            
        }

        public void DoDamage(float v)
        {
            if (isInvincible)
                return;
            health -= v;
            isInvincible = true;
            _animator.Play("damage_1");
        }
    }
}

