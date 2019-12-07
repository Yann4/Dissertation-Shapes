using System.Collections.Generic;

namespace Dissertation.Narrative
{
	public class WorldStateManager
	{
		private List<WorldProperty> _worldProperties = new List<WorldProperty>();

		public bool IsInState<PropertyValue>(ObjectClass objectType, EProperty toQuery, PropertyValue value)
		{
			return IsInState(objectType, 0, toQuery, value);
		}

		public bool IsInState<PropertyValue>(ObjectClass objectType, int objectIndex, EProperty toQuery, PropertyValue value)
		{
			return IsInState(WorldProperty.GetObjectID(objectType, objectIndex), toQuery, value);
		}

		private bool IsInState<PropertyValue>(long id, EProperty toQuery, PropertyValue value)
		{
			List<WorldProperty> propertiesWithID = _worldProperties.FindAll(property => property.ObjectID == id);
			if(propertiesWithID.Count > 0)
			{
				foreach(WorldProperty property in propertiesWithID)
				{
					if(property.Property == toQuery)
					{
						return property.Query(value);
					}
				}
			}

			return false;
		}
	}
}