namespace Dissertation.Narrative
{
	public class WorldStateManager
	{
		private WorldState _state = new WorldState();

		public bool IsInState( WorldProperty property )
		{
			return _state.IsInState(property);
		}

		public void SetState(WorldProperty property)
		{
			_state.SetState(property);
		}

		//Returns copy of world state
		public WorldState GetCurrentWorldState()
		{
			return new WorldState(_state);
		}
	}
}