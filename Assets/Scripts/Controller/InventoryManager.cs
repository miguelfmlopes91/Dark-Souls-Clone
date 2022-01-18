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

        private StateManager _stateManager;
        
        public void Init(StateManager st)
        {
            _stateManager = st;
            EquipWeapon(RightHandWeapon);
            EquipWeapon(RightHandWeapon, true);
            CloseAllDamageColliders();
        }

        public void EquipWeapon(Weapon w, bool isLeft = false)
        {
            string targetIdle = w.oh_idle;
            targetIdle += (isLeft) ? "_l" : "_r";
            _stateManager.Anim.SetBool("mirror", isLeft);
            _stateManager.Anim.Play("changeWeapon");
            _stateManager.Anim.Play(targetIdle);

        }
        
        public void CloseAllDamageColliders(){
            if(RightHandWeapon.w_hook != null)
                RightHandWeapon.w_hook.CloseDamageColliders();
            if(LeftHandWeapon.w_hook != null)
                LeftHandWeapon.w_hook.CloseDamageColliders();
        }
        
        public void OpenAllDamageColliders(){
            if(RightHandWeapon.w_hook != null)
                RightHandWeapon.w_hook.OpenDamageColliders();
            if(LeftHandWeapon.w_hook != null)
                LeftHandWeapon.w_hook.OpenDamageColliders();
        }
    }
    
    [System.Serializable]
    public class Weapon
    {
        public string oh_idle;
        public string th_idle;
        
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

