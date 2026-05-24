using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public abstract class AnimatorStateBaseAttributeDrawer<TType> : AnimatorBaseDrawer<AnimatorStateAttribute, TType>, IDefinesGenericMenuItems
{
	public const string PATH_SEPARATOR = "/";
	public const string HASH_SEPARATOR = ".";

	public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
	{
		genericMenu.AddSeparator("");
		genericMenu.AddItem(new GUIContent("Select state in animator"), false, OpenAnimator, GetAnimatorController());
	}

	private void OpenAnimator(object data)
	{
		var animator = data as AnimatorController;

		if (!animator) 
			return;
		
		Selection.SetActiveObjectWithContext(animator, null);
		AssetDatabase.OpenAsset(animator);
		Selection.SetActiveObjectWithContext(animator, null);

		EditorApplication.delayCall += () => SelectState(animator);
	}

	private void SelectState(AnimatorController animator)
	{
		var state = GetState(ValueEntry.SmartValue, animator);

		var stateMachine = GetStateMachine(ValueEntry.SmartValue, animator);

		Selection.SetActiveObjectWithContext(state, null);

		AnimatorControllerToolUtil.SetActiveStateMachine(animator, stateMachine);
	}

	protected abstract AnimatorStateMachine GetStateMachine(TType value, AnimatorController animator);

	protected abstract AnimatorState GetState(TType value, AnimatorController animator);
}

public class AnimatorStateAttributeDrawer : AnimatorStateBaseAttributeDrawer< int>
{
	protected override void UpdateButton()
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;
		
		foreach (var layer in animator.layers)
		{
			var name = GetNameFromHash(layer.stateMachine, layer.name + PATH_SEPARATOR, layer.name + HASH_SEPARATOR);

			if (name != null)
			{
				m_buttonContent.text = name;
				return;
			}
		}
		
		m_buttonContent.text = "--- EMPTY ---";
	}

	private string GetNameFromHash(AnimatorStateMachine stateMachine, string pathPrefix, string hashPrefix)
	{
		foreach (var state in stateMachine.states)
		{
			var stateHash = Animator.StringToHash(hashPrefix + state.state.name);
			
			if (stateHash == ValueEntry.SmartValue)
				return pathPrefix + state.state.name;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			var name = GetNameFromHash(nextStateMachine.stateMachine, 
				pathPrefix + nextStateMachine.stateMachine.name + PATH_SEPARATOR,
				hashPrefix + nextStateMachine.stateMachine.name + HASH_SEPARATOR);
			
			if (name != null)
				return name;
		}
		
		return null;
	}

	protected override void DrawDropdown(GUIContent label)
	{
		GenericSelector<int>.DrawSelectorDropdown(label, m_buttonContent, StateSelector);
	}

	private OdinSelector<int> StateSelector(Rect rect)
	{
		var animator = GetAnimatorController();
		
		var selector = new GenericSelector<int>("States", false);
		
		foreach (var layer in animator.layers)
			AddStates(layer.stateMachine, selector, layer.name + PATH_SEPARATOR, layer.name + HASH_SEPARATOR);
		
		selector.SetSelection(ValueEntry.SmartValue);
		selector.ShowInPopup(rect);

		selector.SelectionChanged += selected =>
		{
			if (!selected.Any())
				return;
			
			ValueEntry.SmartValue = selected.First();
		};
		
		return selector;
	}
	
	private static void AddStates(AnimatorStateMachine stateMachine, 
		GenericSelector<int> selector, string pathPrefix, string hashPrefix)
	{
		var states = stateMachine.states;
		
		foreach (var state in states)
		{
			var stateHash = Animator.StringToHash(hashPrefix + state.state.name);

			selector.SelectionTree.Add(pathPrefix + state.state.name, stateHash);
		}

		var stateMachines = stateMachine.stateMachines;
		
		foreach (var childAnimatorStateMachine in stateMachines)
		{
			var currentStateMachine = childAnimatorStateMachine.stateMachine;
			
			AddStates(currentStateMachine, selector,
				pathPrefix + currentStateMachine.name + PATH_SEPARATOR,
				hashPrefix + currentStateMachine.name + HASH_SEPARATOR);
		}
	}

	protected override AnimatorStateMachine GetStateMachine(int value, AnimatorController animator)
	{
		foreach (var layer in animator.layers)
		{
			var state = GetStateMachineFromHash(layer.stateMachine, layer.name + HASH_SEPARATOR, value);

			if (state)
				return state;
		}

		return null;
	}

	private AnimatorStateMachine GetStateMachineFromHash(AnimatorStateMachine stateMachine, string hashPrefix, int hash)
	{
		foreach (var state in stateMachine.states)
		{
			var stateHash = Animator.StringToHash(hashPrefix + state.state.name);
			
			if (stateHash == hash)
				return stateMachine;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			var foundStateMachine = GetStateMachineFromHash(nextStateMachine.stateMachine, 
				hashPrefix + nextStateMachine.stateMachine.name + HASH_SEPARATOR,
				hash);
			
			if (foundStateMachine)
				return foundStateMachine;
		}

		return null;
	}

	protected override AnimatorState GetState(int value, AnimatorController animator)
	{
		foreach (var layer in animator.layers)
		{
			var state = GetStateFromHash(layer.stateMachine, layer.name + HASH_SEPARATOR, value);

			if (state)
				return state;
		}

		return null;
	}


	private AnimatorState GetStateFromHash(AnimatorStateMachine stateMachine, string hashPrefix, int hash)
	{
		foreach (var state in stateMachine.states)
		{
			var stateHash = Animator.StringToHash(hashPrefix + state.state.name);
			
			if (stateHash == hash)
				return state.state;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			var state = GetStateFromHash(nextStateMachine.stateMachine, 
				hashPrefix + nextStateMachine.stateMachine.name + HASH_SEPARATOR,
				hash);
			
			if (state)
				return state;
		}

		return null;
	}
}

public class AnimatorStateStringAttributeDrawer : AnimatorStateBaseAttributeDrawer<string>
{
	protected override void UpdateButton()
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;
		
		foreach (var layer in animator.layers)
		{
			if (StateExists(layer.stateMachine, layer.name + PATH_SEPARATOR))
			{
				m_buttonContent.text = ValueEntry.SmartValue;
				return;
			}
		}
		
		m_buttonContent.text = "--- EMPTY ---";
	}

	private bool StateExists(AnimatorStateMachine stateMachine, string pathPrefix)
	{
		foreach (var state in stateMachine.states)
		{
			if (pathPrefix + state.state.name == ValueEntry.SmartValue)
				return true;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			if (StateExists(nextStateMachine.stateMachine, pathPrefix))
				return true;
		}
		
		return false;
	}

	protected override void DrawDropdown(GUIContent label)
	{
		GenericSelector<string>.DrawSelectorDropdown(label, m_buttonContent, StateSelector);
	}

	private OdinSelector<string> StateSelector(Rect rect)
	{
		var animator = GetAnimatorController();
		
		var selector = new GenericSelector<string>("States", false);
		
		foreach (var layer in animator.layers)
			AddStates(layer.stateMachine, selector, layer.name + PATH_SEPARATOR);
		
		selector.SetSelection(ValueEntry.SmartValue);
		selector.ShowInPopup(rect);

		selector.SelectionChanged += selected =>
		{
			if (!selected.Any())
				return;
			
			ValueEntry.SmartValue = selected.First();
		};
		
		return selector;
	}
	
	private static void AddStates(AnimatorStateMachine stateMachine, 
		GenericSelector<string> selector, string pathPrefix)
	{
		var states = stateMachine.states;
		
		foreach (var state in states)
		{
			selector.SelectionTree.Add(pathPrefix + state.state.name, pathPrefix + state.state.name);
		}

		var stateMachines = stateMachine.stateMachines;
		
		foreach (var childAnimatorStateMachine in stateMachines)
		{
			var currentStateMachine = childAnimatorStateMachine.stateMachine;
			
			AddStates(currentStateMachine, selector,
				pathPrefix + currentStateMachine.name + PATH_SEPARATOR);
		}
	}
	
	protected override AnimatorStateMachine GetStateMachine(string value, AnimatorController animator)
	{
		foreach (var layer in animator.layers)
		{
			var state = GetStateMachineFromPath(layer.stateMachine, layer.name + PATH_SEPARATOR, value);

			if (state)
				return state;
		}

		return null;
	}

	private AnimatorStateMachine GetStateMachineFromPath(AnimatorStateMachine stateMachine, string pathPrefix, string name)
	{
		foreach (var state in stateMachine.states)
		{
			if (pathPrefix + state.state.name == name)
				return stateMachine;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			var foundStateMachine = GetStateMachineFromPath(nextStateMachine.stateMachine, 
				pathPrefix + nextStateMachine.stateMachine.name + PATH_SEPARATOR,
				name);
			
			if (foundStateMachine)
				return foundStateMachine;
		}

		return null;
	}

	protected override AnimatorState GetState(string value, AnimatorController animator)
	{
		foreach (var layer in animator.layers)
		{
			var state = GetStateFromPath(layer.stateMachine, layer.name + PATH_SEPARATOR, value);

			if (state)
				return state;
		}

		return null;
	}

	private AnimatorState GetStateFromPath(AnimatorStateMachine stateMachine, string pathPrefix, string path)
	{
		foreach (var state in stateMachine.states)
		{
			if (pathPrefix + state.state.name == path)
				return state.state;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			var state = GetStateFromPath(nextStateMachine.stateMachine, 
				pathPrefix + nextStateMachine.stateMachine.name + PATH_SEPARATOR,
				path);
			
			if (state)
				return state;
		}

		return null;
	}
}