using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Dissertation.Narrative.ActionFunctionLibrary;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dissertation.Narrative
{
	[CreateAssetMenu(fileName = "Action.asset", menuName = "Dissertation/Scriptables/Narrative/Action")]
	public class Action : ScriptableObject
	{
		[HideInInspector] public string guid;

		public List<WorldPropertyScriptable> Preconditions = new List<WorldPropertyScriptable>();
		public List<WorldPropertyScriptable> Postconditions = new List<WorldPropertyScriptable>();

		[SerializeField] private List<WorldPropertyScriptable> ExitCondition = new List<WorldPropertyScriptable>();

		private List<WorldProperty> _runtimeExitConditions = new List<WorldProperty>();
		[NonSerialized, HideInInspector] public List<WorldProperty> RuntimePreconditions = new List<WorldProperty>();

		public Actions PerformFunction = Actions.NONE;

		public static Action Deserialise (BinaryReader reader)
		{
			string uid = reader.ReadString();
			Action action = NarrativeDictionary.GetAsset().GetAction(uid);
			if(action == null)
			{
				return null;
			}

			foreach (WorldPropertyScriptable prop in action.ExitCondition)
			{
				action._runtimeExitConditions.Add(prop.GetRuntimeProperty());
			}

			foreach(WorldPropertyScriptable precondition in action.Preconditions)
			{
				action.RuntimePreconditions.Add(precondition.GetRuntimeProperty());
			}

			return action;
		}

		public void Serialise(BinaryWriter writer)
		{
#if UNITY_EDITOR
			if (string.IsNullOrEmpty(guid))
			{
				guid = GUID.Generate().ToString();
				EditorUtility.SetDirty(this);
			}
#endif //UNITY_EDITOR

			writer.Write(guid);
		}

		public void Perform()
		{
			if (!ActionFunctionLibrary.PerformAction(PerformFunction))
			{
				Debug.Assert(false, string.Format("The performFunction is incorrectly set to '{0}'", PerformFunction));
			}
		}

		public bool CanExit(WorldStateManager worldState)
		{
			return worldState.IsInState(_runtimeExitConditions);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(Action))]
	public class ActionEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			Action myTarget = (Action)target;
			if (GUILayout.Button("Generate GUID"))
			{
				myTarget.guid = GUID.Generate().ToString();
			}

			DrawDefaultInspector();
		}
	}
#endif //UNITY_EDITOR
}