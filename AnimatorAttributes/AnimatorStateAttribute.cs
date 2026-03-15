using System;
using System.Diagnostics;
	
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;

[assembly: OdinVisualDesignerAttributeItem("Animator", typeof(AnimatorStateAttribute))]
#endif

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
[Conditional("UNITY_EDITOR")]
public class AnimatorStateAttribute : AnimatorBaseAttribute
{
	/// <param name="animatorField">String to resolve Animator or RuntimeAnimatorController.
	/// If left empty first field or property with correct type is used</param>
	public AnimatorStateAttribute(string animatorField = null)
	{
		AnimatorField = animatorField;
	}
}