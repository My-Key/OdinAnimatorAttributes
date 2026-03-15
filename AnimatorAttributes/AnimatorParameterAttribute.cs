using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

[assembly: OdinVisualDesignerAttributeItem("Animator", typeof(AnimatorParameterAttribute))]
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Conditional("UNITY_EDITOR")]
public class AnimatorParameterAttribute : AnimatorBaseAttribute
{
	[OdinDesignerBinding(nameof(Type))]
	[ShowInInspector]
	public AnimatorControllerParameterType? Type { get; }
	
	[OdinDesignerBinding(nameof(TypeField))]
	[ShowInInspector]
	public string TypeField { get; }

	/// <param name="animatorField">String to resolve Animator or RuntimeAnimatorController.
	/// If left empty first field or property with correct type is used</param>
	public AnimatorParameterAttribute(string animatorField = null)
	{
		AnimatorField = animatorField;
	}

	/// <param name="type">Limit dropdown to desired parameter type</param>
	/// <param name="animatorField">String to resolve Animator or RuntimeAnimatorController.
	/// If left empty first field or property with correct type is used</param>
	public AnimatorParameterAttribute(AnimatorControllerParameterType type, string animatorField = null)
	{
		AnimatorField = animatorField;
		Type = type;
	}

	/// <param name="type">String to resolve desired parameter type</param>
	/// <param name="animatorField">String to resolve Animator or RuntimeAnimatorController.
	/// If left empty first field or property with correct type is used</param>
	public AnimatorParameterAttribute(string type, string animatorField = null)
	{
		AnimatorField = animatorField;
		TypeField = type;
	}
}