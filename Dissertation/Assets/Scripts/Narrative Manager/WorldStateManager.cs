using System.Collections.Generic;

namespace Dissertation.Narrative
{
	public class WorldStateManager
	{
		private List<WorldProperty> _worldProperties = new List<WorldProperty>();

		public bool IsInState( WorldProperty property )
		{
			List<WorldProperty> propertiesWithID = _worldProperties.FindAll(prop => prop.ObjectID == property.ObjectID);
			if(propertiesWithID.Count > 0)
			{
				foreach(WorldProperty withID in propertiesWithID)
				{
					if(withID.Property == property.Property)
					{
						return WorldProperty.Query(withID.Property, withID.Value, property.Value);
					}
				}
			}

			return false;
		}

		public void SetState(WorldProperty property)
		{
			SetState(_worldProperties, property);
		}

		public static void SetState(List<WorldProperty> state, WorldProperty property)
		{
			WorldProperty toUpdate = state.Find(prop => prop.Key == property.Key);
			if (toUpdate != null)
			{
				toUpdate.Value = property.Value;
			}
			else
			{
				state.Add(property);
			}
		}

		public static bool IsInState(List<WorldProperty> worldState, List<WorldProperty> state)
		{
			foreach(WorldProperty property in state)
			{
				if(!IsInState(worldState, property))
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsInState(List<WorldProperty> worldState, WorldProperty state)
		{
			WorldProperty worldProperty = worldState.Find(prop => prop.Key.Equals(state.Key));
			if(worldProperty != null)
			{
				return WorldProperty.Query(state.Property, worldProperty.Value, state.Value);
			}

			return false;
		}

		public List<WorldProperty> GetCurrentWorldState()
		{
			List<WorldProperty> state = new List<WorldProperty>(_worldProperties.Count);
			foreach(WorldProperty prop in _worldProperties)
			{
				state.Add(prop);
			}

			return state;
		}
	}
}