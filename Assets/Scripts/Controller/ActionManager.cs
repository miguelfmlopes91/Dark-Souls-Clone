using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();
        public ItemAction consumableItem;
        
        private StateManager _stateManager;
        public ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }
        public void Init(StateManager stateManager)
        {
            _stateManager = stateManager;
            UpdateActionsOneHanded();
        }

        public void UpdateActionsOneHanded()
        {
            EmptyAllSlots();
            if (_stateManager.InventoryManager.hasLeftHandWeapon)
            {
                UpdateActionsLeftHanded();
                return;
            }
            Weapon weapon = _stateManager.InventoryManager.RightHandWeapon;
            for (int i = 0; i < weapon .actions.Count ; i++)
            {
                Action action = GetAction(weapon.actions[i].input);
                action.targetAnimation = weapon.actions[i].targetAnimation;
            }
        }

        public void UpdateActionsLeftHanded()
        {
            Weapon r_weapon = _stateManager.InventoryManager.RightHandWeapon;
            Weapon l_weapon = _stateManager.InventoryManager.LeftHandWeapon;

            var rb = GetAction(ActionInput.rb);
            var rt = GetAction(ActionInput.rt);
            rb.targetAnimation = r_weapon.GetAction(r_weapon.actions, ActionInput.rb).targetAnimation;
            rt.targetAnimation = r_weapon.GetAction(r_weapon.actions, ActionInput.rt).targetAnimation;
            var lb = GetAction(ActionInput.lb);
            var lt = GetAction(ActionInput.lt);
            lb.targetAnimation = l_weapon.GetAction(l_weapon.actions, ActionInput.rb).targetAnimation;
            lt.targetAnimation = l_weapon.GetAction(l_weapon.actions, ActionInput.rt).targetAnimation;
            lb.mirror = true;
            lt.mirror = true;
        }
        
        public void UpdateActionsTwoHanded()
        {
            EmptyAllSlots();
            Weapon weapon = _stateManager.InventoryManager.RightHandWeapon;
            for (int i = 0; i < weapon .twoHandedActions.Count ; i++)
            {
                Action action = GetAction(weapon.twoHandedActions[i].input);
                action.targetAnimation = weapon.twoHandedActions[i].targetAnimation;
            }
        }

        private void EmptyAllSlots()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = GetAction((ActionInput)i);
                a.targetAnimation = null;
                a.mirror = false;
            }
        }

        public Action GetActionSlot(StateManager stateManager)
        {
            ActionInput a_input = GetActionInput(stateManager);
            return GetAction(a_input);
        }

        private Action GetAction(ActionInput input)
        {
            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].input == input)
                    return actionSlots[i];
            }
            return null;
        }
        
        public ActionInput GetActionInput(StateManager stateManager)
        {
            if (stateManager.rb)
                return ActionInput.rb;
            if (stateManager.rt)
                return ActionInput.rt;
            if (stateManager.lb)
                return ActionInput.lb;
            if (stateManager.lt)
                return ActionInput.lt;

            return ActionInput.rb;
        }
    }
    
    

    public enum ActionInput
    {
        rb, lb, rt, lt
    }
    
    [System.Serializable]
    public class Action
    {
        public ActionInput input;
        public string targetAnimation;
        public bool mirror;
    }

    [System.Serializable]
    public class ItemAction
    {
        public string targetAnimation;
        public string item_id;
    }
}

