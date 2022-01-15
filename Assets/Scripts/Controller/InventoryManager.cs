using System.Collections;
using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon currentWeapon;
        
        public void Init()
        {
            currentWeapon.w_hook.CloseDamageColliders();
        }
    }

    [System.Serializable]
    public class Weapon
    {
        public List<Action> actions;
        public List<Action> twoHandedActions;
        public GameObject weaponModel;
        public WeaponHook w_hook;
    }
}

