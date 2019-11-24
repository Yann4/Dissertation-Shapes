using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Dissertation.Narrative.ActionFunctionLibrary;

namespace Dissertation.Narrative
{
	[Serializable]
	public class Action : ScriptableObject
	{
		public List<WorldProperty> Preconditions = new List<WorldProperty>();
		public List<WorldProperty> Postconditions = new List<WorldProperty>();
		public Actions PerformFunction = Actions.NONE;

		public Action(BinaryReader reader)
		{
			int preconditionCount = reader.ReadInt32();
			for(int idx = 0; idx < preconditionCount; idx++)
			{
				Preconditions.Add(new WorldProperty(reader));
			}

			int postconditionCount = reader.ReadInt32();
			for(int idx = 0; idx < postconditionCount; idx++)
			{
				Postconditions.Add(new WorldProperty(reader));
			}

			PerformFunction = (Actions)reader.ReadInt32();
		}

		public void Serialize(BinaryWriter writer)
		{
			writer.Write(Preconditions.Count);
			foreach(WorldProperty property in Preconditions)
			{
				property.Serialize(writer);
			}

			writer.Write(Postconditions.Count);
			foreach(WorldProperty property in Postconditions)
			{
				property.Serialize(writer);
			}

			writer.Write((int)PerformFunction);
		}

		public void Perform()
		{
			if (!ActionFunctionLibrary.PerformAction(PerformFunction))
			{
				Debug.Assert(false, string.Format("The performFunction is incorrectly set to '{0}'", PerformFunction));
			}
		}
	}
}