using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Narrative
{
	[Serializable]
	public class Beat
	{
		public string Title = "Beat";

		[SerializeField] public List<WorldPropertyScriptable> ScriptablePreconditions = new List<WorldPropertyScriptable>();
		private List<WorldPropertyScriptable> ScriptablePostconditions = new List<WorldPropertyScriptable>();

		[HideInInspector] public List<WorldProperty> Preconditions = new List<WorldProperty>();
		[HideInInspector] public List<WorldProperty> Postconditions = new List<WorldProperty>();

		public PlayerArchetype Archetype = new PlayerArchetype();

		public float Importance { get; set; } = 0.0f;
		public int Order { get; set; } = -1;
		public Beat NextMajorBeat { get; set; }

		public int MaxRepetitions = 1;
		public int RepetitionsPerformed { get; private set; } = 0;

		public List<Action> RequiredActions = new List<Action>();
		public List<Action> OptionalActions = new List<Action>();

		private List<Action> _runningActions = new List<Action>();
		private HashSet<Action> _completedActions = new HashSet<Action>();

		public int UID { get; private set; }

		public float Cost = 1;

		[HideInInspector] public bool Generated = false;

		private const int Version = 1;

		public Beat(bool generated)
		{
			Generated = generated;
		}

		public Beat(BinaryReader reader, int uid)
		{
			UID = uid;

			int version = reader.ReadInt32();

			if (version == 1)
			{
				DeserialiseVersion1(reader);
			}
			else
			{
				UnityEngine.Debug.LogErrorFormat("Can't deserialise beat of version {0}. Write deserialisation function", version);
			}

			CalculatePostconditions();
		}

#if UNITY_EDITOR
		public Beat(List<WorldPropertyScriptable> preconditions, PlayerArchetype archetype, float importance, int maxRepititons, 
			List<Action> requiredActions, List<Action> optionalActions, string title)
		{
			UID = UnityEditor.GUID.Generate().GetHashCode();

			ScriptablePreconditions = preconditions;
			Archetype = archetype;

			Importance = importance;
			MaxRepetitions = maxRepititons;

			RequiredActions = requiredActions;
			OptionalActions = optionalActions;

			Title = title;

			CalculatePostconditions();
		}
#endif //UNITY_EDITOR

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(Version);

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

			writer.Write(Generated);
		}

		private void DeserialiseVersion1(BinaryReader reader)
		{
			int preconditionCount = reader.ReadInt32();
			for (int idx = 0; idx < preconditionCount; idx++)
			{
				ScriptablePreconditions.Add(WorldPropertyScriptable.Deserialise(reader));
				if (ScriptablePreconditions[idx] != null)
				{
					Preconditions.Add(ScriptablePreconditions[idx].GetRuntimeProperty());
				}
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

			Generated = reader.ReadBoolean();
		}

		private void CalculatePostconditions()
		{
			ScriptablePostconditions.Clear();
			Postconditions.Clear();

			foreach(Action action in RequiredActions)
			{
				if (action != null)
				{
					ScriptablePostconditions.AddRange(action.Postconditions);
				}
			}

			for(int idx = 0; idx < ScriptablePostconditions.Count; idx++)
			{
				if (ScriptablePostconditions[idx] != null)
				{
					Postconditions.Add(ScriptablePostconditions[idx].GetRuntimeProperty());
				}
			}
		}

		public void Perform()
		{
			RepetitionsPerformed++;

			_runningActions.AddRange(RequiredActions);
		}

		public bool Update(WorldStateManager worldState)
		{
			foreach(Action optionalAction in OptionalActions)
			{
				if(!_completedActions.Contains(optionalAction) && !_runningActions.Contains(optionalAction) && worldState.IsInState(optionalAction.RuntimePreconditions))
				{
					_runningActions.Add(optionalAction);
				}
			}

			for (int idx = _runningActions.Count - 1; idx >= 0; idx--)
			{
				if(!_runningActions[idx].CanExit(worldState))
				{
					_runningActions[idx].Perform();
				}
				else
				{
					_completedActions.Add(_runningActions[idx]);
					_runningActions.RemoveAt(idx);
				}
			}

			if(_runningActions.Count == 0)
			{
				Cleanup();
				return false;
			}

			return true;
		}

		private void Cleanup()
		{
			_runningActions.Clear();
			_completedActions.Clear();
		}

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