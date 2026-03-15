using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorBaseValidator<TAttribute, TType> : AttributeValidator<TAttribute, TType>
	where TAttribute : AnimatorBaseAttribute
{
	private ValueResolver<RuntimeAnimatorController> m_animatorResolver;

	protected override void Initialize()
	{
		base.Initialize();

		RuntimeAnimatorController defaultValue = null;

		for (int index = 0; index < Property.Parent.Children.Count; ++index)
		{
			object obj = Property.Parent.Children[index].ValueEntry.WeakSmartValue;

			if (obj is Animator animator)
			{
				if (!animator)
					continue;

				defaultValue = animator.runtimeAnimatorController;

				break;
			}

			if (obj is RuntimeAnimatorController runtimeAnimator)
			{
				if (!runtimeAnimator)
					continue;

				defaultValue = runtimeAnimator;

				break;
			}
		}

		m_animatorResolver = ValueResolver.Get(Property, Attribute.AnimatorField, defaultValue);
	}

	protected AnimatorController GetAnimatorController()
	{
		var runtimeAnimatorController = m_animatorResolver.GetValue();

		if (!runtimeAnimatorController)
			return null;

		if (runtimeAnimatorController is AnimatorController animatorController)
			return animatorController;

		if (runtimeAnimatorController is not AnimatorOverrideController overrideController)
			return null;

		return overrideController.runtimeAnimatorController as AnimatorController;
	}
}