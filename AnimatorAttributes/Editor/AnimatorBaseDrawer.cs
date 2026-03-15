using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public abstract class AnimatorBaseDrawer<TAttribute, TType> : OdinAttributeDrawer<TAttribute, TType>
	where TAttribute : AnimatorBaseAttribute
{
	protected ValueResolver<RuntimeAnimatorController> m_animatorResolver;
	protected GUIContent m_buttonContent = new();
	
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
		
		UpdateButton();
		
		ValueEntry.OnValueChanged+= ValueChanged;
		ValueEntry.OnChildValueChanged += ValueChanged;
	}

	private void ValueChanged(int obj) => UpdateButton();

	protected abstract void UpdateButton();

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
	
	protected override void DrawPropertyLayout(GUIContent label)
	{
		m_animatorResolver.DrawError();
		
		var animator = GetAnimatorController();

		if (!animator)
		{
			SirenixEditorGUI.ErrorMessageBox("Selected animator is null");
			
			if (label != null)
				EditorGUILayout.LabelField(label);
			
			return;
		}

		DrawDropdown(label);
	}

	protected abstract void DrawDropdown(GUIContent label);
}