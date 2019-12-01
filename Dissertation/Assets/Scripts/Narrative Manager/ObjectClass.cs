using System;

namespace Dissertation.Narrative
{
	public enum ObjectClass : Int32
	{
		ANY = 0,
		Player = 1 << 0,
		Agent = 1 << 1,
		Interactable = 1 << 2,
	}
}
