namespace Dissertation.Character.AI
{
	[System.Flags]
	public enum States
	{
		INVALID,
		MoveTo,
		Idle,
		Traverse,
		PathTo,
		Attack,
	}
}