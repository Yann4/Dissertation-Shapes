using System;
using System.Collections.Generic;
using System.IO;

namespace Dissertation.Narrative
{
	[Serializable]
	public class Beat
	{
		public List<WorldProperty> Preconditions { get; private set; } = new List<WorldProperty>();
		private List<WorldProperty> _postconditions = new List<WorldProperty>();

		public PlayerArchetype Archetype { get; private set; } = new PlayerArchetype();

		public float Importance { get; private set; } = 0.0f;
		public int Order { get; private set; } = -1;

		public int MaxRepetitions { get; set; } = 1;
		public int RepetitionsPerformed { get; private set; } = 0;

		public List<Action> RequiredActions { get; private set; } = new List<Action>();
		public List<Action> OptionalActions { get; private set; } = new List<Action>();

		public Beat()
		{ }

		public Beat(BinaryReader reader)
		{
			int preconditionCount = reader.ReadInt32();
			for(int idx = 0; idx < preconditionCount; idx++)
			{
				Preconditions.Add(WorldProperty.Deserialise(reader));
			}

			Archetype = new PlayerArchetype(reader);

			Importance = reader.ReadInt32();
			Order = reader.ReadInt32();

			MaxRepetitions = reader.ReadInt32();
			RepetitionsPerformed = reader.ReadInt32();

			int requiredActionCount = reader.ReadInt32();
			for (int idx = 0; idx < requiredActionCount; idx++)
			{
				RequiredActions.Add(Action.Deserialise(reader));
			}

			int optionalActionCount = reader.ReadInt32();
			for (int idx = 0; idx < optionalActionCount; idx++)
			{
				OptionalActions.Add(Action.Deserialise(reader));
			}

			CalculatePostconditions();
		}

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(Preconditions.Count);
			foreach(WorldProperty property in Preconditions)
			{
				property.Serialize(writer);
			}

			Archetype.Serialize(writer);

			writer.Write(Importance);
			writer.Write(Order);

			writer.Write(MaxRepetitions);
			writer.Write(RepetitionsPerformed);

			writer.Write(RequiredActions.Count);
			for (int idx = 0; idx < RequiredActions.Count; idx++)
			{
				RequiredActions[idx].Serialise(writer);
			}

			writer.Write(OptionalActions.Count);
			for (int idx = 0; idx < OptionalActions.Count; idx++)
			{
				OptionalActions[idx].Serialise(writer);
			}
		}

		private void CalculatePostconditions()
		{
			_postconditions.Clear();

			foreach(Action action in RequiredActions)
			{
				_postconditions.AddRange(action.Postconditions);
			}
		}

		public void Update()
		{ }
	}
}