#if UNITY_EDITOR
using UnityEditor;

namespace Dissertation.Util
{
	[CustomEditor(typeof(ScriptableID))]
	public class ScriptableIDEditor : Editor
	{
		SerializedProperty _objectClass;
		SerializedProperty _objectID;

		private void OnEnable()
		{
			_objectClass = serializedObject.FindProperty("Class");
			_objectID = serializedObject.FindProperty("ID");

			if (_objectID.intValue == -1)
			{
				_objectID.intValue = (int)System.DateTime.UtcNow.ToBinary();
				serializedObject.ApplyModifiedProperties();
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.LabelField("ID", _objectID.intValue.ToString());
			EditorGUILayout.PropertyField(_objectClass);
			serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif //UNITY_EDITOR