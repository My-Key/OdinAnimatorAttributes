using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using UnityEngine;

public abstract class AnimatorParameterBaseAttributeDrawer<TType> : AnimatorBaseDrawer<AnimatorParameterAttribute, TType>
{
	protected ValueResolver<AnimatorControllerParameterType?> m_typeResolver;

	protected override void Initialize()
	{
		base.Initialize();

		m_typeResolver = ValueResolver.Get(Property, Attribute.TypeField, Attribute.Type);
	}
	
	protected override void DrawPropertyLayout(GUIContent label)
	{
		m_typeResolver.DrawError();
		
		base.DrawPropertyLayout(label);
	}
}

public class AnimatorParameterAttributeDrawer : AnimatorParameterBaseAttributeDrawer<int>
{
	protected override void UpdateButton()
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;
		
		foreach (var parameter in animator.parameters)
		{
			if (parameter.nameHash == ValueEntry.SmartValue)
			{
				m_buttonContent.text = parameter.name;
				return;
			}
		}
		
		m_buttonContent.text = "--- EMPTY ---";
	}

	protected override void DrawDropdown(GUIContent label) =>
		GenericSelector<int>.DrawSelectorDropdown(label, m_buttonContent, ParameterSelector);

	private OdinSelector<int> ParameterSelector(Rect rect)
	{
		var parameters = new List<GenericSelectorItem<int>>();
		
		var animator = GetAnimatorController();

		var resolvedType = m_typeResolver.GetValue();
		
		foreach (var parameter in animator.parameters)
		{
			if (!resolvedType.HasValue || parameter.type == resolvedType.Value)
				parameters.Add(new GenericSelectorItem<int>(parameter.name, parameter.nameHash));
		}

		var selector = new GenericSelector<int>("Parameters", false, parameters);
		
		selector.SetSelection(ValueEntry.SmartValue);
		selector.ShowInPopup(rect);

		selector.SelectionChanged += selected =>
		{
			if (!selected.Any())
				return;
			
			ValueEntry.SmartValue = selected.First();
		};
		
		return selector;
	}
}

public class AnimatorParameterStringAttributeDrawer : AnimatorParameterBaseAttributeDrawer<string>
{
	protected override void UpdateButton()
	{
		var animator = GetAnimatorController();
		
		if (!animator)
			return;
		
		foreach (var parameter in animator.parameters)
		{
			if (parameter.name == ValueEntry.SmartValue)
			{
				m_buttonContent.text = parameter.name;
				return;
			}
		}
		
		m_buttonContent.text = "--- EMPTY ---";
	}

	protected override void DrawDropdown(GUIContent label) =>
		GenericSelector<string>.DrawSelectorDropdown(label, m_buttonContent, ParameterSelector);

	private OdinSelector<string> ParameterSelector(Rect rect)
	{
		var parameters = new List<string>();
		
		var animator = GetAnimatorController();

		var resolvedType = m_typeResolver.GetValue();
		
		foreach (var parameter in animator.parameters)
		{
			if (!resolvedType.HasValue || parameter.type == resolvedType.Value)
				parameters.Add(parameter.name);
		}

		var selector = new GenericSelector<string>("Parameters", false, parameters);
		
		selector.SetSelection(ValueEntry.SmartValue);
		selector.ShowInPopup(rect);

		selector.SelectionChanged += selected =>
		{
			if (!selected.Any())
				return;
			
			ValueEntry.SmartValue = selected.First();
		};
		
		return selector;
	}
}