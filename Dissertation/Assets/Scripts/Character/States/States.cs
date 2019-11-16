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
		Steal
	}

	[Flags]
	public enum SpecialistStates
	{
		INVALID = 0,
		Steal = 1 << 0,
// 		Justice = 1 << 1,
// 		Flee = 1 << 2,
// 		Defend = 1 << 3,
	}
}