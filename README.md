# OdinAnimatorAttributes

## Installation

Put `AnimatorAttributes` folder in project

# Right click menu

Property has context menu option `Select state in animator` that opens animator window and selects chosen state

# Example
![](Images/Example.PNG)

```cs
[SerializeField]
private Animator m_animator;
		
[SerializeField]
private RuntimeAnimatorController m_animatorController;

[SerializeField]
[AnimatorParameter(nameof(m_animator))]
private int m_parameter;

[SerializeField]
[AnimatorParameter(AnimatorControllerParameterType.Float, nameof(m_animatorController))]
private string m_parameterString;

[SerializeField]
[AnimatorState(nameof(m_animatorController))]
private int m_state;
	
[SerializeField]
[AnimatorState]
private string m_stateString;
```

# Special thanks

akof1314 - helper functions to open Animator window - https://gist.github.com/akof1314/65ca8ffcf64ccdc802730ddade71a8ff
