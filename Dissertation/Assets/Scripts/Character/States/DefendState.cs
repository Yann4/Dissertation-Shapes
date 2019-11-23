namespace Dissertation.Character.AI
{
	public class DefendState : SpecialistState
	{
		public DefendState() : base(States.Defend)
		{ }

		public DefendState(StateConfig config) : base(config)
		{ }

		public override bool ShouldRunState(AgentController owner, out StateConfig config)
		{
			config = null;
			return false;
		}
	}
}