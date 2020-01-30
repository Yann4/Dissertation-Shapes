using System;

namespace Dissertation.Narrative
{
	[Serializable]
	public struct WorldProperty
	{
		public long ObjectID { get; private set; }
		public EProperty Property { get; private set; }

		public PropertyKey Key;
		public PropertyValue Value;

		public WorldProperty(ObjectClass type, int index, EProperty property, int i, bool b, float f)
		{
			ObjectID = GetObjectID(type, index);
			Property = property;

			Key = new PropertyKey(ObjectID, Property);
			Value = new PropertyValue(i, b, f);
		}

		public WorldProperty(long ID, EProperty property, int i, bool b, float f)
		{
			ObjectID = ID;
			Property = property;

			Key = new PropertyKey(ID, property);
			Value = new PropertyValue(i, b, f);
		}

		public WorldProperty(long ID, EProperty property, int value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(ID, property);
			Value = new PropertyValue(value);
		}

		public WorldProperty(long ID, EProperty property, float value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(ID, property);
			Value = new PropertyValue(value);
		}

		public WorldProperty(long ID, EProperty property, bool value)
		{
			ObjectID = ID;
			Property = property;
			Key = new PropertyKey(ID, property);
			Value = new PropertyValue(value);
		}

		public WorldProperty(WorldProperty property, PropertyValue overwriteValue)
		{
			ObjectID = property.ObjectID;
			Property = property.Property;
			Key = property.Key;
			Value = overwriteValue;
		}

		public ObjectClass GetObjectClass()
		{
			int cl = (int)(ObjectID >> 32);
			return (ObjectClass)cl;
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

		public override string ToString()
		{
			return string.Format("ID {0}, Property {1} - {2}, {3}, {4}", Key.ObjectID, Key.Property, Value.bVal, Value.iVal, Value.fVal);
		}

		public struct PropertyKey : IEquatable<PropertyKey>
		{
			public long ObjectID;
			private int _property;

			public EProperty Property { get { return (EProperty)_property; } }

			public PropertyKey(WorldProperty worldProperty)
			{
				ObjectID = worldProperty.ObjectID;
				_property = (int)worldProperty.Property;
			}

			public PropertyKey(long id, EProperty property)
			{
				ObjectID = id;
				_property = (int)property;
			}

			public override bool Equals(object obj)
			{
				try
				{
					PropertyKey objKey = (PropertyKey)obj;
					return Equals(objKey);
				}
				catch(InvalidCastException)
				{
					return false;
				}
			}

			public bool Equals(PropertyKey other)
			{
				return ObjectID == other.ObjectID && _property == other._property;
			}

			public override int GetHashCode()
			{
				var hashCode = 667362690;
				hashCode = (hashCode * -1521134295) + ObjectID.GetHashCode();
				hashCode = (hashCode * -1521134295) + _property.GetHashCode();
				hashCode = (hashCode * -1521134295) + Property.GetHashCode();
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

		public struct PropertyValue
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
				bVal = false;
				fVal = 0.0f;
			}

			public PropertyValue(bool val)
			{
				bVal = val;
				iVal = 0;
				fVal = 0.0f;
			}

			public PropertyValue(float val)
			{
				fVal = val;
				iVal = 0;
				bVal = false;
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