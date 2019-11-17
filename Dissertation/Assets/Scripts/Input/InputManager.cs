using System;

namespace Dissertation.Input
{
	//Must be the same name as in the unity input system
	public enum InputAction
	{
		MoveHorizontal,
		MoveVertical,
		Jump,
		Drop,
		TogglePause,
		ShowInventory,
		MeleeAttack,
		RangedAttack,
		DashAttack,
		Interact,
		Transform,
	}

	/*
	 * Steps to add a new InputAction:
	 * 1. Add it to the InputAction Enum
	 * 2. Add it to the Unity Input system
	 * 3. Add it to Yoke (as a member, to Reset() and in all of the accessor function switch statements)
	 * 4. Add it to PlayerController Update
	*/ 

	public static class InputManager
	{
		private static readonly string[] _inputActions = Enum.GetNames(typeof(InputAction));

		private static string GetAction(InputAction action)
		{
			return _inputActions[(int)action];
		}

		public static bool GetButton(InputAction action)
		{
			return UnityEngine.Input.GetButton(GetAction(action));
		}

		public static bool GetButtonUp(InputAction action)
		{
			return UnityEngine.Input.GetButtonUp(GetAction(action));
		}

		public static bool GetButtonDown(InputAction action)
		{
			return UnityEngine.Input.GetButtonDown(GetAction(action));
		}

		public static float GetAxis(InputAction action)
		{
			return UnityEngine.Input.GetAxis(GetAction(action));
		}
	}
}