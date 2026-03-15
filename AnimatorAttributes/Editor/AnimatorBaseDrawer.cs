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
		
		if (string.IsNullOrEmpty(stringToResolve))
		{
			for (int index = 0; index < property.Parent.Children.Count; ++index)
			{
				var inspectorProperty = property.Parent.Children[index];
				var type = inspectorProperty.ValueEntry.BaseValueType;

				if (type == typeof(Animator) || type == typeof(RuntimeAnimatorController))
				{
					stringToResolve = $"${inspectorProperty.Name}";

					break;
				}
			}
		}

		resolver = ValueResolver.Get<RuntimeAnimatorController>(property, stringToResolve);
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