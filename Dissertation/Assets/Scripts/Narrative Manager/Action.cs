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

		public List<WorldProperty> Preconditions = new List<WorldProperty>();
		public List<WorldProperty> Postconditions = new List<WorldProperty>();
		public Actions PerformFunction = Actions.NONE;

		public static Action Deserialise (BinaryReader reader)
		{
			return NarrativeDictionary.GetAsset().GetAction(reader.ReadString());
		}

		public void Serialise(BinaryWriter writer)
		{
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