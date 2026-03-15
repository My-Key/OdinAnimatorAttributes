using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class AnimatorConverter : ConvertUtility.ICustomConverter
{
	static AnimatorConverter() => ConvertUtility.AddCustomConverter(new AnimatorConverter());

	public bool CanConvert(Type from, Type to) => from == typeof(Animator) && to == typeof(RuntimeAnimatorController);

	public bool TryConvert(object obj, Type to, out object result)
	{
		if (obj is not Animator animator)
		{
			result = null;

			return false;
		}

		if (!animator)
		{
			result = null;
			return true;
		}
		
		result = animator.runtimeAnimatorController;
		return true;
	}
}