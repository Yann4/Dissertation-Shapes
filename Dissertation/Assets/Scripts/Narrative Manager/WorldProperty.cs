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

		public bool Query<ValueType>(ValueType value)
		{
			switch (Property)
			{
				case EProperty.INVALID:
				default:
					UnityEngine.Debug.LogErrorFormat("Invalid property type to query {0}", Property);
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
	}
}