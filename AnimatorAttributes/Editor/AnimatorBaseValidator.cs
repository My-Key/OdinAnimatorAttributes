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
		
		AnimatorBaseDrawer<TAttribute, TType>.InitResolver(ref m_animatorResolver, Attribute, Property);
	}

	protected AnimatorController GetAnimatorController() =>
		AnimatorBaseDrawer<TAttribute, TType>.GetAnimatorController(m_animatorResolver);
}