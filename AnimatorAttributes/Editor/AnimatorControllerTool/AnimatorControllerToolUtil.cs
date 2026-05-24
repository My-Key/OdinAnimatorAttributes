// Credits: akof1314
// https://gist.github.com/akof1314/65ca8ffcf64ccdc802730ddade71a8ff

using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEditor.Graphs;

public static class AnimatorControllerToolUtil
{
    private static AnimatorControllerToolReflect animatorControllerToolReflect;

    private static void InitReflect()
    {
        if (animatorControllerToolReflect == null)
        {
            animatorControllerToolReflect = new AnimatorControllerToolReflect();
        }
    }

    public static void SyncGraphToUnitySelection()
    {
        InitReflect();
        GraphGUI graphGUI = animatorControllerToolReflect.stateMachineGraphGUI;

        // 重要，没有这个设置，将会被底层返回
        GUIUtility.hotControl = 0;
        graphGUI.SyncGraphToUnitySelection();
    }

    public static void SetActiveStateMachine(AnimatorController animatorController, AnimatorStateMachine animatorStateMachine)
    {
        InitReflect();

        AnimatorStateMachine activeStateMachine = animatorControllerToolReflect.activeStateMachine;
        if (animatorStateMachine != activeStateMachine && animatorStateMachine)
        {
            List<AnimatorStateMachine> hierarchy = new List<AnimatorStateMachine>();

            var layerStateMachineIndex =
                AnimatorControllerUtil.AnimatorController_LayerStateMachine(animatorController, animatorStateMachine);
            AnimatorControllerUtil.MecanimUtilities_StateMachineRelativePath(
                animatorController.layers[layerStateMachineIndex].stateMachine, animatorStateMachine, ref hierarchy);
            
            animatorControllerToolReflect.SetCurrentLayer(layerStateMachineIndex);

            animatorControllerToolReflect.BuildBreadCrumbsFromSMHierarchy(hierarchy);
            
        }
    }

    public static void ScrollToState(AnimatorStateMachine animatorStateMachine, AnimatorState state)
    {
        InitReflect();

        Vector3 pos = AnimatorControllerUtil.AnimatorStateMachine_GetStatePosition(animatorStateMachine, state);
        Rect graphExtents = animatorControllerToolReflect.graphExtents;
        Rect graphClientArea = animatorControllerToolReflect.graphClientArea;

        Vector2 pos2 = new Vector2(pos.x - graphExtents.x - graphClientArea.width / 2 + 100f, 
            pos.y - graphExtents.y - graphClientArea.height / 2 + 25f);
        animatorControllerToolReflect.scrollPosition = pos2;
    }
}