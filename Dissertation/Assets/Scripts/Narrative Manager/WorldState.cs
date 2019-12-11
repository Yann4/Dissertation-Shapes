using System.Collections.Generic;

namespace Dissertation.Narrative
{
	public class WorldState
	{
		private List<WorldProperty> _worldProperties = new List<WorldProperty>();
		private Dictionary<long, int> _money = new Dictionary<long, int>();

		public WorldState() { }

		public WorldState(WorldState other)
		{
			_worldProperties.AddRange(other._worldProperties);
			foreach (KeyValuePair<long, int> wallet in other._money)
			{
				_money.Add(wallet.Key, wallet.Value);
			}
		}

		private bool Query(WorldProperty.PropertyKey key, WorldProperty.PropertyValue actualValue, WorldProperty.PropertyValue expectedValue)
		{
			switch (key.Property)
			{
				case EProperty.IsDead:
				case EProperty.CanMelee:
				case EProperty.CanDash:
				case EProperty.CanShoot:
					return actualValue.bVal == expectedValue.bVal;
				case EProperty.MoneyEqual:
					{
						if (_money.TryGetValue(key.ObjectID, out int value))
						{
							return value == expectedValue.iVal;
						}

						return false;
					}
				case EProperty.MoneyGreaterThan:
					{
						if (_money.TryGetValue(key.ObjectID, out int value))
						{
							return value > expectedValue.iVal;
						}

						return false;
					}
				case EProperty.MoneyLessThan:
					{
						if (_money.TryGetValue(key.ObjectID, out int value))
						{
							return value < expectedValue.iVal;
						}

						return false;
					}
				case EProperty.INVALID:
				default:
					UnityEngine.Debug.LogErrorFormat("Invalid property type to query {0}", key.Property);
					return false;
			}
		}

		public bool IsInState(List<WorldProperty> properties)
		{
			foreach (WorldProperty property in properties)
			{
				if (!IsInState(property))
				{
					return false;
				}
			}

			return true;
		}

		public bool IsInState(WorldProperty property)
		{
			List<WorldProperty> propertiesWithID = _worldProperties.FindAll(prop => prop.ObjectID == property.ObjectID);
			if (propertiesWithID.Count > 0)
			{
				foreach (WorldProperty withID in propertiesWithID)
				{
					if (withID.Property == property.Property)
					{
						return Query(withID.Key, withID.Value, property.Value);
					}
				}
			}

			return false;
		}

		public void SetState(WorldProperty property)
		{
			switch (property.Property)
			{
				case EProperty.MoneyEqual:
					_money[property.ObjectID] = property.Value.iVal;
					return;
				case EProperty.MoneyGreaterThan:
					_money[property.ObjectID] = property.Value.iVal + 1;
					return;
				case EProperty.MoneyLessThan:
					_money[property.ObjectID] = property.Value.iVal - 1;
					return;
			}

			WorldProperty toUpdate = _worldProperties.Find(prop => prop.Key == property.Key);
			if (toUpdate != null)
			{
				toUpdate.Value = property.Value;
			}
			else
			{
				_worldProperties.Add(property);
			}
		}
	}
}