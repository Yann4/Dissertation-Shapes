using System;

namespace Dissertation.Character.AI
{
	public enum States
	{
		INVALID,
		MoveTo,
		Idle,
		Traverse,
		PathTo,
		Attack,
	}

	[Flags]
	public enum SpecialistStates
	{
		COUNT
	}
}