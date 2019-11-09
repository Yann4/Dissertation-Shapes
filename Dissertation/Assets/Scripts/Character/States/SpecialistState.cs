namespace Dissertation.Character.AI
{
	public abstract class SpecialistState : State
	{
		public SpecialistState(States type) : base(new StateConfig(type, StatePriority.LongTerm, null))
		{ }

		public SpecialistState(StateConfig config) : base(config)
		{ }

		public abstract bool ShouldRunState(AgentController owner, out StateConfig config);
	}
}