using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    public class ActionManager : MonoBehaviour
    {
        public List<Action> actionSlots = new List<Action>();

        public ActionManager()
        {
            for (int i = 0; i < 4; i++)
            {
                Action a = new Action();
                a.input = (ActionInput)i;
                actionSlots.Add(a);
            }
        }
        public void Init()
        {
            
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
    }
}

