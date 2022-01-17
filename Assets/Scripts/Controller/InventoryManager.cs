using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;

namespace Controller
{
    public class InventoryManager : MonoBehaviour
    {
        public Weapon RightHandWeapon;
        public Weapon LeftHandWeapon;
        public bool hasLeftHandWeapon = true;
        public void Init()
        {
            CloseAllDamageColliders();
        }
        
        public void CloseAllDamageColliders(){
            if(RightHandWeapon.w_hook != null)
                RightHandWeapon.w_hook.CloseDamageColliders();
            if(LeftHandWeapon.w_hook != null)
                LeftHandWeapon.w_hook.CloseDamageColliders();
        }
    }
    
    [System.Serializable]
    public class Weapon
    {
        public List<Action> actions;
        public List<Action> twoHandedActions;
        public bool LeftHandMirror;
        public GameObject weaponModel;
        public WeaponHook w_hook;

        public Action GetAction(List<Action> l, ActionInput input)
        {
            return l.FirstOrDefault(action => action.input == input);
        }
    }
}

