using System.IO;
using UnityEngine;

namespace Dissertation.Narrative
{
	public class WorldProperty : ScriptableObject
	{
		public long ObjectID;
		public EProperty Property;

		public int iValue;
		public bool bValue;
		public float fValue;
		public string sValue;

		public WorldProperty(long objectID, EProperty property)
		{
			ObjectID = objectID;
			Property = property;

			iValue = 0;
			bValue = false;
			fValue = 0.0f;
			sValue = string.Empty;
		}

		public WorldProperty(BinaryReader reader)
		{
			ObjectID = reader.ReadInt64();
			Property = (EProperty)reader.ReadInt32();

			iValue = reader.ReadInt32();
			bValue = reader.ReadBoolean();
			fValue = reader.ReadSingle();
			sValue = reader.ReadString();
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(ObjectID);
			writer.Write((int)Property);

			writer.Write(iValue);
			writer.Write(bValue);
			writer.Write(fValue);
			writer.Write(sValue);
		}
	}
}