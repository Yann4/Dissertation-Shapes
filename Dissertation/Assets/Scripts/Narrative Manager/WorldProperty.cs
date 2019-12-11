using System;

namespace Dissertation.Narrative
{
	[Serializable]
	public class WorldProperty
	{
		public long ObjectID { get; private set; }
		public EProperty Property { get; private set; }

		public PropertyKey Key;
		public PropertyValue Value;

		public WorldProperty(long ID, EProperty property, int i, bool b, float f)
		{
			ObjectID = ID;
			Property = property;

			Key = new PropertyKey(this);
			Value = new PropertyValue(i, b, f);
		}

		public WorldProperty(long ID, EProperty property, int value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(this);
			Value = new PropertyValue(value);
		}

		public WorldProperty(long ID, EProperty property, float value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(this);
			Value = new PropertyValue(value);
		}

		public WorldProperty(long ID, EProperty property, bool value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(this);
			Value = new PropertyValue(value);
		}

		public ObjectClass GetObjectClass()
		{
			int cl = (int)(ObjectID >> 32);
			return (ObjectClass)cl;
		}

		public static bool Query(EProperty property, PropertyValue actualValue, PropertyValue expectedValue)
		{
			switch (property)
			{
				case EProperty.IsDead:
				case EProperty.CanMelee:
				case EProperty.CanDash:
				case EProperty.CanShoot:
					return actualValue.bVal == expectedValue.bVal;
				case EProperty.MoneyEqual:
					return actualValue.iVal == expectedValue.iVal;
				case EProperty.MoneyGreaterThan:
					return actualValue.iVal > expectedValue.iVal;
				case EProperty.MoneyLessThan:
					return actualValue.iVal < expectedValue.iVal;
				case EProperty.INVALID:
				default:
					UnityEngine.Debug.LogErrorFormat("Invalid property type to query {0}", property);
					return false;
			}
		}

		public int GetClassIndex()
		{
			long idx = ObjectID << 32;
			return (int)(idx >> 32);
		}

		public static long GetObjectID(ObjectClass type, int index)
		{
			return ((long)type << 32) | ((long)index);
		}

		public static long GetObjectID(ObjectClass type)
		{
			return GetObjectID(type, 0);
		}

		public class PropertyKey : IEquatable<PropertyKey>
		{
			public long ObjectID;
			private int _property;

			public EProperty Property { get { return (EProperty)_property; } }

			public PropertyKey(WorldProperty worldProperty)
			{
				ObjectID = worldProperty.ObjectID;
				_property = (int)worldProperty.Property;
			}

			public override bool Equals(object obj)
			{
				PropertyKey objKey = obj as PropertyKey;
				if(objKey != null)
				{
					return Equals(objKey);
				}

				return base.Equals(obj);
			}

			public bool Equals(PropertyKey other)
			{
				return ObjectID == other.ObjectID && _property == other._property;
			}

			public override int GetHashCode()
			{
				var hashCode = 667362690;
				hashCode = hashCode * -1521134295 + ObjectID.GetHashCode();
				hashCode = hashCode * -1521134295 + _property.GetHashCode();
				hashCode = hashCode * -1521134295 + Property.GetHashCode();
				return hashCode;
			}

			public static bool operator==(PropertyKey lhs, PropertyKey rhs)
			{
				return lhs.Equals(rhs);
			}

			public static bool operator !=(PropertyKey lhs, PropertyKey rhs)
			{
				return !lhs.Equals(rhs);
			}
		}

		public class PropertyValue
		{
			public int iVal { get; private set; }
			public bool bVal { get; private set; }
			public float fVal { get; private set; }

			public PropertyValue(int i, bool b, float f)
			{
				iVal = i;
				bVal = b;
				fVal = f;
			}

			public PropertyValue(int val)
			{
				iVal = val;
			}

			public PropertyValue(bool val)
			{
				bVal = val;
			}

			public PropertyValue(float val)
			{
				fVal = val;
			}

			public PropertyValue(WorldProperty worldProperty)
			{
				iVal = worldProperty.Value.iVal;
				bVal = worldProperty.Value.bVal;
				fVal = worldProperty.Value.fVal;
			}
		}
	}
}