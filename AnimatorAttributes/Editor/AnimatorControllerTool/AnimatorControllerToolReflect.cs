// Credits: akof1314
// https://gist.github.com/akof1314/65ca8ffcf64ccdc802730ddade71a8ff

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;
using UnityEngine;

public class AnimatorControllerToolReflect
{
    private Assembly m_Assembly;
    private Type m_TypeAnimatorWindow;
    private Type m_TypeGraph;
    private Type m_TypeGraphGUI;
    private EditorWindow m_AnimatorWindow;
    private Graph m_StateMachineGraph;
    private GraphGUI m_StateMachineGraphGUI;

    private PropertyInfo m_ActiveStateMachineInfo;
    private MethodInfo m_AddBreadCrumbInfo;
    private MethodInfo m_BuildBreadCrumbsFromSMHierarchyInfo;
    private MethodInfo m_SetCurrentLayer;
    private FieldInfo m_ScrollPositionInfo;
    private FieldInfo m_GraphExtentsInfo;
    private FieldInfo m_GraphClientAreaInfo;

    private Assembly assembly
    {
        get
        {
            if (m_Assembly == null)
            {
                m_Assembly = Assembly.GetAssembly(typeof(Graph));
            }
            return m_Assembly;
        }
    }

    private Type animatorWindowType
    {
        get
        {
            if (m_TypeAnimatorWindow == null)
            {
                m_TypeAnimatorWindow = assembly.GetType("UnityEditor.Graphs.AnimatorControllerTool");
            }
            return m_TypeAnimatorWindow;
        }
    }

    private Type graphType
    {
        get
        {
            if (m_TypeGraph == null)
            {
                m_TypeGraph = assembly.GetType("UnityEditor.Graphs.AnimationStateMachine.Graph");
            }
            return m_TypeGraph;
        }
    }

    private Type graphGUIType
    {
        get
        {
            if (m_TypeGraphGUI == null)
            {
                m_TypeGraphGUI = assembly.GetType("UnityEditor.Graphs.AnimationStateMachine.GraphGUI");
            }
            return m_TypeGraphGUI;
        }
    }

    public EditorWindow animatorWindow
    {
        get
        {
            if (m_AnimatorWindow == null)
            {
                FieldInfo toolInfo = animatorWindowType.GetField("tool", BindingFlags.Public | BindingFlags.Static);
                m_AnimatorWindow = toolInfo.GetValue(null) as EditorWindow;
            }
            return m_AnimatorWindow;
        }
    }

    public Graph stateMachineGraph
    {
        get
        {
            if (m_StateMachineGraph == null)
            {
                FieldInfo stateMachineGraphInfo = animatorWindowType.GetField("stateMachineGraph", BindingFlags.Public | BindingFlags.Instance);
                m_StateMachineGraph = stateMachineGraphInfo.GetValue(animatorWindow) as Graph;
            }
            return m_StateMachineGraph;
        }
    }

    public GraphGUI stateMachineGraphGUI
    {
        get
        {
            if (m_StateMachineGraphGUI == null)
            {
                FieldInfo stateMachineGraphGUIInfo = animatorWindowType.GetField("stateMachineGraphGUI", BindingFlags.Public | BindingFlags.Instance);
                m_StateMachineGraphGUI = stateMachineGraphGUIInfo.GetValue(animatorWindow) as GraphGUI;
            }
            return m_StateMachineGraphGUI;
        }
    }

    private PropertyInfo activeStateMachineInfoInfo
    {
        get
        {
            if (m_ActiveStateMachineInfo == null)
            {
                m_ActiveStateMachineInfo = graphType.GetProperty("activeStateMachine", BindingFlags.Instance | BindingFlags.Public);
            }
            return m_ActiveStateMachineInfo;
        }
    }

    public AnimatorStateMachine activeStateMachine
    {
        get
        {
            return activeStateMachineInfoInfo.GetValue(stateMachineGraph, null) as AnimatorStateMachine;
        }
    }

    private MethodInfo addBreadCrumbInfo
    {
        get
        {
            if (m_AddBreadCrumbInfo == null)
            {
                m_AddBreadCrumbInfo = animatorWindowType.GetMethod("AddBreadCrumb", BindingFlags.Instance | BindingFlags.Public);
            }
            return m_AddBreadCrumbInfo;
        }
    }

    public void AddBreadCrumb(UnityEngine.Object target)
    {
        addBreadCrumbInfo.Invoke(animatorWindow, new object[] {target});
    }

    private MethodInfo buildBreadCrumbsFromSMHierarchyInfo
    {
        get
        {
            if (m_BuildBreadCrumbsFromSMHierarchyInfo == null)
            {
                m_BuildBreadCrumbsFromSMHierarchyInfo = animatorWindowType.GetMethod("BuildBreadCrumbsFromSMHierarchy", BindingFlags.Instance | BindingFlags.Public);
            }
            return m_BuildBreadCrumbsFromSMHierarchyInfo;
        }
    }

    public void BuildBreadCrumbsFromSMHierarchy(IEnumerable<AnimatorStateMachine> hierarchy)
    {
        buildBreadCrumbsFromSMHierarchyInfo.Invoke(animatorWindow, new object[] { hierarchy });
    }

    private MethodInfo setCurrentLayer
    {
        get
        {
            if (m_SetCurrentLayer == null)
            {
                m_SetCurrentLayer = animatorWindowType.GetMethod("SetCurrentLayer", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return m_SetCurrentLayer;
        }
    }

    public void SetCurrentLayer(int layerIndex)
    {
        setCurrentLayer.Invoke(animatorWindow, new object[] {layerIndex});
    }

    private FieldInfo scrollPositionInfo
    {
        get
        {
            if (m_ScrollPositionInfo == null)
            {
                m_ScrollPositionInfo = graphGUIType.GetField("m_ScrollPosition", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return m_ScrollPositionInfo;
        }
    }

    public Vector2 scrollPosition
    {
        get
        {
            return (Vector2)scrollPositionInfo.GetValue(stateMachineGraphGUI);
        }
        set
        {
            scrollPositionInfo.SetValue(stateMachineGraphGUI, value);
        }
    }

    private FieldInfo graphExtentsInfo
    {
        get
        {
            if (m_GraphExtentsInfo == null)
            {
                m_GraphExtentsInfo = graphType.GetField("graphExtents", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return m_GraphExtentsInfo;
        }
    }

    public Rect graphExtents
    {
        get
        {
            return (Rect)graphExtentsInfo.GetValue(stateMachineGraph);
        }
    }

    private FieldInfo graphClientAreaInfo
    {
        get
        {
            if (m_GraphClientAreaInfo == null)
            {
                m_GraphClientAreaInfo = graphGUIType.GetField("m_GraphClientArea", BindingFlags.Instance | BindingFlags.NonPublic);
            }
            return m_GraphClientAreaInfo;
        }
    }

    public Rect graphClientArea
    {
        get
        {
            return (Rect)graphClientAreaInfo.GetValue(stateMachineGraphGUI);
        }
    }
}