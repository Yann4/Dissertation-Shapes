using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dissertation.Narrative
{
	[Serializable]
	public class Beat
	{
		public List<WorldPropertyScriptable> Preconditions { get; private set; } = new List<WorldPropertyScriptable>();
		private List<WorldPropertyScriptable> _postconditions = new List<WorldPropertyScriptable>();

		public PlayerArchetype Archetype { get; private set; } = new PlayerArchetype();

		public float Importance { get; private set; } = 0.0f;
		public int Order { get; private set; } = -1;
		public Beat NextMajorBeat { get; set; }

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
				Preconditions.Add(WorldPropertyScriptable.Deserialise(reader));
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
			int nonNullCount = Preconditions.Count(x => x != null);
			writer.Write(nonNullCount);
			foreach(WorldPropertyScriptable property in Preconditions)
			{
				if (property != null)
				{
					property.Serialize(writer);
				}
			}

			Archetype.Serialize(writer);

			writer.Write(Importance);
			writer.Write(Order);

			writer.Write(MaxRepetitions);
			writer.Write(RepetitionsPerformed);

			nonNullCount = RequiredActions.Count(x => x != null);
			writer.Write(nonNullCount);
			for (int idx = 0; idx < RequiredActions.Count; idx++)
			{
				if (RequiredActions[idx] != null)
				{
					RequiredActions[idx].Serialise(writer);
				}
			}

			nonNullCount = OptionalActions.Count(x => x != null);
			writer.Write(nonNullCount);
			for (int idx = 0; idx < OptionalActions.Count; idx++)
			{
				if (OptionalActions[idx] != null)
				{
					OptionalActions[idx].Serialise(writer);
				}
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