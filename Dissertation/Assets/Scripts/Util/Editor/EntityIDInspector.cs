#if UNITY_EDITOR
using UnityEditor;

namespace Dissertation.Util
{
	[CustomEditor(typeof(EntityID))]
	public class EntityIDEditor : Editor
	{
		SerializedProperty _scriptable;

		private void OnEnable()
		{
			_scriptable = serializedObject.FindProperty("_scriptID");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.ObjectField(_scriptable);

			if (_scriptable.objectReferenceValue == null)
			{
				EditorGUILayout.LabelField("ID", "-1");
				EditorGUILayout.LabelField("Class", "ANY");
			}
			else
			{
				ScriptableID scriptable = _scriptable.objectReferenceValue as ScriptableID;
				EditorGUILayout.LabelField("ID", scriptable.ID.ToString());
				EditorGUILayout.LabelField("Class", scriptable.Class.ToString());
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif //UNITY_EDITOR