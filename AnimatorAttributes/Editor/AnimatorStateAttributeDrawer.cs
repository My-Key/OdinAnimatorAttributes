using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor.Animations;
using UnityEngine;

public abstract class AnimatorStateBaseAttributeDrawer<TType> : AnimatorBaseDrawer<AnimatorStateAttribute, TType>
{
	public const string PATH_SEPARATOR = "/";
	public const string HASH_SEPARATOR = ".";
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
			var name = GetNameFromHash(nextStateMachine.stateMachine, pathPrefix, hashPrefix);
			
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
}