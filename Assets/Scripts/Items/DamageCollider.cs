using System;
using System.Collections;
using System.Collections.Generic;
using Enemies;
using UnityEngine;


namespace Items
{
    public class DamageCollider : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            EnemyStates _states = other.transform.GetComponentInParent<EnemyStates>();

            if (_states == null)
                return;
            
            _states.DoDamage(35);
        }
    }
}

