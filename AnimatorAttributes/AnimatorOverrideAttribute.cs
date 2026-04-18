
public class AnimatorOverrideAttribute : AnimatorBaseAttribute
{
	/// <param name="animatorField">String to resolve Animator or RuntimeAnimatorController.
	/// If left empty first field or property with correct type is used</param>
	public AnimatorOverrideAttribute(string animatorField = null)
	{
		AnimatorField = animatorField;
	}
}