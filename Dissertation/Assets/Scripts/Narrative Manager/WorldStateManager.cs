using System.Collections.Generic;

namespace Dissertation.Narrative
{
	public class WorldStateManager
	{
		private WorldState _state = new WorldState();
		public PlayerArchetype Archetype { get; private set; } = new PlayerArchetype();
		public EntityManager Entities { get; private set; } = new EntityManager();

		public bool IsInState( WorldProperty property )
		{
			return _state.IsInState(property);
		}

		public bool IsInState(List<WorldProperty> state)
		{
			return _state.IsInState(state);
		}

		public void SetState(WorldProperty property)
		{
			_state.SetState(property);
		}

		public void ModifyPlayerArchetype(PlayerArchetype modifyBy)
		{
			Archetype += modifyBy;
		}

		//Returns copy of world state
		public WorldState GetCurrentWorldState()
		{
			return new WorldState(_state);
		}
	}
}