using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class WeaponHook : MonoBehaviour
    {
        public GameObject[] damageCollider;

        public void OpenDamageColliders()
        {
            foreach (var dmgCollider in damageCollider)
            {
                dmgCollider.SetActive(true);
            }
        }

        public void CloseDamageColliders()
        {
            foreach (var dmgCollider in damageCollider)
            {
                dmgCollider.SetActive(false);

            }
        }
    }

}
