using Sirenix.OdinInspector.Editor.Validation;
using UnityEditor.Animations;
using UnityEngine;

[assembly: RegisterValidator(typeof(AnimatorStateValidator))]
[assembly: RegisterValidator(typeof(AnimatorStateStringValidator))]

public class AnimatorStateValidator : AnimatorBaseValidator<AnimatorStateAttribute, int>
{
	protected override void Validate(ValidationResult result)
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;

		foreach (var layer in animator.layers)
		{
			if (IsHashForStateValid(layer.stateMachine, layer.name + AnimatorStateAttributeDrawer.HASH_SEPARATOR))
				return;
		}

		result.AddError("Animator state is not set to valid value");
	}

	private bool IsHashForStateValid(AnimatorStateMachine stateMachine, string hashPrefix)
	{
		foreach (var state in stateMachine.states)
		{
			var stateHash = Animator.StringToHash(hashPrefix + state.state.name);
			
			if (stateHash == Value)
				return true;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			if (IsHashForStateValid(nextStateMachine.stateMachine, hashPrefix))
				return true;
		}
		
		return false;
	}
}

public class AnimatorStateStringValidator : AnimatorBaseValidator<AnimatorStateAttribute, string>
{
	protected override void Validate(ValidationResult result)
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;

		foreach (var layer in animator.layers)
		{
			if (IsHashForStateValid(layer.stateMachine, layer.name + AnimatorStateAttributeDrawer.PATH_SEPARATOR))
				return;
		}

		result.AddError("Animator state is not set to valid value");
	}

	private bool IsHashForStateValid(AnimatorStateMachine stateMachine, string namePrefix)
	{
		foreach (var state in stateMachine.states)
		{
			if (namePrefix + state.state.name == Value)
				return true;
		}

		foreach (var nextStateMachine in stateMachine.stateMachines)
		{
			if (IsHashForStateValid(nextStateMachine.stateMachine, namePrefix))
				return true;
		}
		
		return false;
	}
}