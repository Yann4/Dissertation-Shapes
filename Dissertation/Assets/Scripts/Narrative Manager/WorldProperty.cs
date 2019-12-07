using System;

namespace Dissertation.Narrative
{
	[Serializable]
	public class WorldProperty
	{
		public long ObjectID { get; private set; }
		public EProperty Property { get; private set; }

		private int iValue;
		private bool bValue;
		private float fValue;
		private string sValue;

		public WorldProperty(long ID, EProperty property, int i, bool b, float f, string s)
		{
			ObjectID = ID;
			Property = property;
			iValue = i;
			bValue = b;
			fValue = f;
			sValue = s;
		}

		public WorldProperty(long ID, EProperty property, int value)
		{
			ObjectID = ID;
			Property = property;
			iValue = value;
		}

		public WorldProperty(long ID, EProperty property, float value)
		{
			ObjectID = ID;
			Property = property;
			fValue = value;
		}

		public WorldProperty(long ID, EProperty property, bool value)
		{
			ObjectID = ID;
			Property = property;
			bValue = value;
		}

		public WorldProperty(long ID, EProperty property, string value)
		{
			ObjectID = ID;
			Property = property;
			sValue = value;
		}

		public ObjectClass GetObjectClass()
		{
			int cl = (int)(ObjectID >> 32);
			return (ObjectClass)cl;
		}

		public static bool Query(EProperty property, Value value)
		{
			switch (property)
			{
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

		public struct Key : IEquatable<Key>
		{
			public long ObjectID;
			private int _property;

			public EProperty Property { get { return (EProperty)_property; } }

			public Key(WorldProperty worldProperty)
			{
				ObjectID = worldProperty.ObjectID;
				_property = (int)worldProperty.Property;
			}

			public bool Equals(Key other)
			{
				return ObjectID == other.ObjectID && _property == other._property;
			}
		}

		public struct Value
		{
			private int iVal;
			private int bVal;
			private float fVal;
			private string sVal;

			public Value(WorldProperty worldProperty)
			{
				iVal = worldProperty.iValue;
				bVal = worldProperty.bValue ? 1 : 0;
				fVal = worldProperty.fValue;
				sVal = worldProperty.sValue;
			}
		}
	}
}