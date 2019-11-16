namespace Dissertation.Character.AI
{
	public class StateConfig
	{
		public States StateType { get; private set; }
		public StatePriority Priority { get; private set; }
		public AgentController Owner { get; private set; }

		public StateConfig(States type, StatePriority prio, AgentController owner)
		{
			StateType = type;
			Priority = prio;
			Owner = owner;
		}
	}
}