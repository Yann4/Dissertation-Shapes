using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dissertation.Narrative
{
	[Serializable]
	public class Beat
	{
		public string Title = "Beat";

		public List<WorldPropertyScriptable> ScriptablePreconditions = new List<WorldPropertyScriptable>();
		private List<WorldPropertyScriptable> ScriptablePostconditions = new List<WorldPropertyScriptable>();

		public List<WorldProperty> Preconditions = new List<WorldProperty>();
		public List<WorldProperty> Postconditions = new List<WorldProperty>();

		public PlayerArchetype Archetype { get; private set; } = new PlayerArchetype();

		public float Importance { get; set; } = 0.0f;
		public int Order { get; set; } = -1;
		public Beat NextMajorBeat { get; set; }

		public int MaxRepetitions { get; set; } = 1;
		public int RepetitionsPerformed { get; private set; } = 0;

		public List<Action> RequiredActions { get; private set; } = new List<Action>();
		public List<Action> OptionalActions { get; private set; } = new List<Action>();

		public int UID { get; private set; }

		public float Cost = 1;

		public Beat()
		{ }

		public Beat(BinaryReader reader, int uid)
		{
			UID = uid;

			int preconditionCount = reader.ReadInt32();
			for(int idx = 0; idx < preconditionCount; idx++)
			{
				ScriptablePreconditions.Add(WorldPropertyScriptable.Deserialise(reader));
				Preconditions.Add(ScriptablePreconditions[idx].GetRuntimeProperty());
			}

			Archetype = new PlayerArchetype(reader);

			Importance = reader.ReadSingle();
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

			Title = reader.ReadString();

			CalculatePostconditions();
		}

		public void Serialise(BinaryWriter writer)
		{
			int nonNullCount = ScriptablePreconditions.Count(x => x != null);
			writer.Write(nonNullCount);
			foreach(WorldPropertyScriptable property in ScriptablePreconditions)
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

			writer.Write(Title);
		}

		private void CalculatePostconditions()
		{
			ScriptablePostconditions.Clear();
			Postconditions.Clear();

			foreach(Action action in RequiredActions)
			{
				ScriptablePostconditions.AddRange(action.Postconditions);
			}

			for(int idx = 0; idx < ScriptablePostconditions.Count; idx++)
			{
				Postconditions.Add(ScriptablePostconditions[idx].GetRuntimeProperty());
			}
		}

		public void Perform()
		{
			RepetitionsPerformed++;
		}

		public void Update()
		{ }

		internal bool MeetsPreconditions(WorldStateManager worldState)
		{
			foreach (WorldProperty condition in Preconditions)
			{
				if (worldState.IsInState(condition))
				{
					return true;
				}
			}

			return Preconditions.Count == 0;
		}
	}
}