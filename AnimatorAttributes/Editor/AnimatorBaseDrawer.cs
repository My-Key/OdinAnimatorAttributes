using System.Linq;
using System.Reflection;
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
		
		InitResolver(ref m_animatorResolver, Attribute, Property);

		UpdateButton();
		
		ValueEntry.OnValueChanged+= ValueChanged;
		ValueEntry.OnChildValueChanged += ValueChanged;
	}

	public static void InitResolver(ref ValueResolver<RuntimeAnimatorController> resolver, AnimatorBaseAttribute animatorAttribute, InspectorProperty property)
	{
		var stringToResolve = animatorAttribute.AnimatorField;
		
		var currentProperty = property;

		if (currentProperty.ParentType.IsArray)
			currentProperty = currentProperty.ParentValueProperty;

		AnimatorBaseAttribute currentAttribute;

		// Try to get override
		do
		{
			currentAttribute = currentProperty.ParentValueProperty
				?.GetAttribute<AnimatorOverrideAttribute>();

			if (currentAttribute != null)
			{
				currentProperty = currentProperty.ParentValueProperty;
				stringToResolve = currentAttribute.AnimatorField;
			}
		} while (currentAttribute != null);
		
		if (string.IsNullOrEmpty(stringToResolve))
		{
			var fields =
				currentProperty.ParentType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			foreach (var field in fields)
			{
				if (field.FieldType == typeof(Animator) || field.FieldType == typeof(RuntimeAnimatorController))
				{
					stringToResolve = $"${field.Name}";
					break;
				}
			}
			
			if (string.IsNullOrEmpty(stringToResolve))
			{
				var properties =
					currentProperty.ParentType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic |
					                                         BindingFlags.Public);

				foreach (var parentProperty in properties)
				{
					if (parentProperty.PropertyType == typeof(Animator) ||
					    parentProperty.PropertyType == typeof(RuntimeAnimatorController))
					{
						stringToResolve = $"${parentProperty.Name}";

						break;
					}
				}
			}
		}

		resolver = ValueResolver.Get<RuntimeAnimatorController>(currentProperty, stringToResolve);
	}

	private void ValueChanged(int obj) => UpdateButton();

	protected abstract void UpdateButton();

	public static AnimatorController GetAnimatorController(ValueResolver<RuntimeAnimatorController> animatorResolver)
	{
		var runtimeAnimatorController = animatorResolver.GetValue();
		
		if (!runtimeAnimatorController)
			return null;

		if (runtimeAnimatorController is AnimatorController animatorController)
			return animatorController;

		if (runtimeAnimatorController is not AnimatorOverrideController overrideController)
			return null;

		return overrideController.runtimeAnimatorController as AnimatorController;
	}

	public AnimatorController GetAnimatorController() => GetAnimatorController(m_animatorResolver);

	protected override void DrawPropertyLayout(GUIContent label)
	{
		m_animatorResolver.DrawError();
		
		var animator = GetAnimatorController(m_animatorResolver);

		if (!animator)
		{
			SirenixEditorGUI.ErrorMessageBox($"Property '{m_animatorResolver.Context.ResolvedString}' is not set");
			
			if (label != null)
				EditorGUILayout.LabelField(label, m_buttonContent);
			
			return;
		}

		DrawDropdown(label);
	}

	protected abstract void DrawDropdown(GUIContent label);
}