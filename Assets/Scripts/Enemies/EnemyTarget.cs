using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Enemies
{
public class EnemyTarget : MonoBehaviour
{
    public int index;
    public List<Transform> targets = new List<Transform>();
    public List<HumanBodyBones> HumanBodyBonesList = new List<HumanBodyBones>();
    private Animator _animator;
    public EnemyStates _EnemyStates;
    
    public void Init(EnemyStates states)
    {
        _EnemyStates = states;
        _animator = states.Anim;
        if (!_animator.isHuman)
            return;
        foreach (HumanBodyBones bones in HumanBodyBonesList)
        {
            targets.Add(_animator.GetBoneTransform(bones));
        }
    }

    public Transform GetTarget(bool negative = false)
    {
        if (targets.Count == 0) return transform;
        
        if (!negative)
        {
            if (index < targets.Count - 1)
                index++;
            else
                index = 0;
        }
        else
        {
            if (index < 0)
                index = targets.Count - 1;
            else
                index--;
        }

        index = Mathf.Clamp(index, 0, targets.Count);
        return targets[index];
    }
}
}

