using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dissertation.Narrative
{
	[CreateAssetMenu(fileName = "WorldProperty.asset", menuName = "Dissertation/Scriptables/Narrative/World Property")]
	public class WorldPropertyScriptable : ScriptableObject
	{
		[HideInInspector] public string guid;

		public long ObjectID
		{
			get
			{
				if (_scriptID != null)
				{
					return WorldProperty.GetObjectID(_scriptID.Class, _scriptID.ID);
				}
				else
				{
					return WorldProperty.GetObjectID(_objectType, _objectIndex);
				}
			}
		}

		[SerializeField, Tooltip("This will take precedence over the values on this Scriptable")] ScriptableID _scriptID;
		[SerializeField] private ObjectClass _objectType = ObjectClass.ANY;
		[SerializeField] private int _objectIndex = 0;
		public EProperty Property;

		[Tooltip("Int value")]		public int iValue;
		[Tooltip("Bool value")]		public bool bValue;
		[Tooltip("Float value")]	public float fValue;

		public static WorldPropertyScriptable Deserialise(BinaryReader reader)
		{
			return NarrativeDictionary.GetAsset().GetWorldProperty(reader.ReadString());
		}

		public void Serialize(BinaryWriter writer)
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

		public WorldProperty GetRuntimeProperty()
		{
			return new WorldProperty(ObjectID, Property, iValue, bValue, fValue );
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(WorldPropertyScriptable))]
	public class WorldPropertyEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			WorldPropertyScriptable myTarget = (WorldPropertyScriptable)target;
			if (GUILayout.Button("Generate GUID"))
			{
				myTarget.guid = GUID.Generate().ToString();
			}

			DrawDefaultInspector();
		}
	}
#endif //UNITY_EDITOR
}