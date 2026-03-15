using Sirenix.OdinInspector;
using UnityEngine;

public class AnimatorBaseAttribute : PropertyAttribute
{
	[OdinDesignerBinding(nameof(AnimatorField))]
	[ShowInInspector]
	public string AnimatorField { get; protected set; }
}