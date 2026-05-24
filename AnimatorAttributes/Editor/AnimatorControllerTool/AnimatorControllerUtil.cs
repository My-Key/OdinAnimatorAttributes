// Credits: akof1314
// https://gist.github.com/akof1314/65ca8ffcf64ccdc802730ddade71a8ff

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Animations;

public static class AnimatorControllerUtil
{
    #region AnimatorController
    /// <summary>
    /// 获取动画控制器 Base Layer 层的状态机
    /// </summary>
    /// <param name="animatorController"></param>
    /// <returns></returns>
    public static AnimatorStateMachine AnimatorController_BaseLayerStateMachine(AnimatorController animatorController)
    {
        if (animatorController.layers.Length > 0)
        {
            return animatorController.layers[0].stateMachine;
        }
        return null;
    }
    
    public static int AnimatorController_LayerStateMachine(AnimatorController animatorController, AnimatorStateMachine stateMachine)
    {
        if (animatorController.layers.Length <= 0)
            return 0;

        for (var index = 0; index < animatorController.layers.Length; index++)
        {
            var layer = animatorController.layers[index];

            if (layer.stateMachine == stateMachine ||
                AnimatorController_HasStateMachine(layer.stateMachine, stateMachine))
            {
                return index;
            }
        }
        
        return 0;
    }

    public static bool AnimatorController_HasStateMachine(AnimatorStateMachine parent,
        AnimatorStateMachine stateMachine)
    {
        if (parent == stateMachine)
            return true;

        foreach (var childAnimatorStateMachine in parent.stateMachines)
        {
            if (childAnimatorStateMachine.stateMachine == stateMachine)
                return true;
            
            if (AnimatorController_HasStateMachine(childAnimatorStateMachine.stateMachine, stateMachine))
                return true;
        }

        return false;
    }

    #endregion

    #region AnimatorStateMachine
    /// <summary>
    /// 动画状态机是否包含指定的动画状态（不递归）
    /// </summary>
    /// <param name="stateMachines"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    public static bool AnimatorStateMachine_HasState(AnimatorStateMachine stateMachine, AnimatorState state, bool recursive = false)
    {
        for (int i = 0; i < stateMachine.states.Length; i++)
        {
            if (stateMachine.states[i].state == state)
            {
                return true;
            }
        }

        if (recursive)
        {
            return AnimatorStateMachine_StatesRecursive(stateMachine).Any(s => s.state == state);
        }

        return false;
    }

    /// <summary>
    /// 获取所有子状态机（递归）
    /// </summary>
    /// <param name="stateMachines"></param>
    /// <returns></returns>
    public static List<ChildAnimatorStateMachine> AnimatorStateMachine_StateMachinesRecursive(AnimatorStateMachine stateMachine)
    {
        List<ChildAnimatorStateMachine> list = new List<ChildAnimatorStateMachine>();
        list.AddRange(stateMachine.stateMachines);
        for (int i = 0; i < stateMachine.stateMachines.Length; i++)
        {
            list.AddRange(AnimatorStateMachine_StateMachinesRecursive(stateMachine.stateMachines[i].stateMachine));
        }
        return list;
    }

    /// <summary>
    /// 获取所有的动画状态（递归）
    /// </summary>
    /// <param name="stateMachines"></param>
    /// <returns></returns>
    public static List<ChildAnimatorState> AnimatorStateMachine_StatesRecursive(AnimatorStateMachine stateMachine)
    {
        List<ChildAnimatorState> list = new List<ChildAnimatorState>();
        list.AddRange(stateMachine.states);
        for (int i = 0; i < stateMachine.stateMachines.Length; i++)
        {
            list.AddRange(AnimatorStateMachine_StatesRecursive(stateMachine.stateMachines[i].stateMachine));
        }
        return list;
    }

    public static Vector3 AnimatorStateMachine_GetStatePosition(AnimatorStateMachine stateMachine, AnimatorState state)
    {
        ChildAnimatorState[] states = stateMachine.states;
        for (int i = 0; i < states.Length; i++)
        {
            if (state == states[i].state)
            {
                return states[i].position;
            }
        }
        return Vector3.zero;
    }
    #endregion

    #region AnimatorState
    /// <summary>
    /// 查找动画状态的所在状态机
    /// </summary>
    /// <param name="state"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    public static AnimatorStateMachine AnimatorState_FindParent(AnimatorState state, AnimatorStateMachine root)
    {
        if (AnimatorStateMachine_HasState(root, state))
        {
            return root;
        }

        return AnimatorStateMachine_StateMachinesRecursive(root).Find(machine =>
            AnimatorStateMachine_HasState(machine.stateMachine, state)
        ).stateMachine;
    }
    #endregion

    #region MecanimUtilities
    public static bool MecanimUtilities_StateMachineRelativePath(AnimatorStateMachine parent, AnimatorStateMachine toFind, ref List<AnimatorStateMachine> hierarchy)
    {
        hierarchy.Add(parent);
        if (parent == toFind)
        {
            return true;
        }
        for (int i = 0; i < parent.stateMachines.Length; i++)
        {
            if (MecanimUtilities_StateMachineRelativePath(parent.stateMachines[i].stateMachine, toFind, ref hierarchy))
            {
                return true;
            }
        }
        hierarchy.Remove(parent);
        return false;
    }
    #endregion
}