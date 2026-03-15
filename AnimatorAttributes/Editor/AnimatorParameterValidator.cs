using Sirenix.OdinInspector.Editor.Validation;

[assembly: RegisterValidator(typeof(AnimatorParameterValidator))]
[assembly: RegisterValidator(typeof(AnimatorParameterStringValidator))]

public class AnimatorParameterValidator : AnimatorBaseValidator<AnimatorParameterAttribute, int>
{
	protected override void Validate(ValidationResult result)
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;

		foreach (var parameter in animator.parameters)
		{
			if (parameter.nameHash == Value)
				return;
		}

		result.AddError("Animator parameter is not set to valid value");
	}
}

public class AnimatorParameterStringValidator : AnimatorBaseValidator<AnimatorParameterAttribute, string>
{
	protected override void Validate(ValidationResult result)
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;

		foreach (var parameter in animator.parameters)
		{
			if (parameter.name == Value)
				return;
		}

		result.AddError("Animator parameter is not set to valid value");
	}
}