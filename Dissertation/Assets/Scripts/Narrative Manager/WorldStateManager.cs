using System.Collections.Generic;
using Unity.Collections;

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
						return WorldProperty.Query(withID.Property, new WorldProperty.Value(property));
					}
				}
			}

			return false;
		}

		public NativeHashMap<WorldProperty.Key, WorldProperty.Value> GetCurrentWorldState()
		{
			NativeHashMap<WorldProperty.Key, WorldProperty.Value> state = new NativeHashMap<WorldProperty.Key, WorldProperty.Value>(_worldProperties.Count, Allocator.TempJob);

			foreach (WorldProperty worldProperty in _worldProperties)
			{
				state.TryAdd(new WorldProperty.Key(worldProperty), new WorldProperty.Value(worldProperty));
			}

			return state;
		}
	}
}