using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Dissertation.Narrative.ActionFunctionLibrary;

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
		public Actions PerformFunction = Actions.NONE;

		public static Action Deserialise (BinaryReader reader)
		{
			return NarrativeDictionary.GetAsset().GetAction(reader.ReadString());
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