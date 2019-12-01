using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dissertation.Narrative
{
	[CreateAssetMenu(fileName = "WorldProperty.asset", menuName = "Dissertation/Scriptables/Narrative/World Property")]
	public class WorldProperty : ScriptableObject
	{
		[HideInInspector] public string guid;

		public long ObjectID
		{
			get
			{
				return ((long)ObjectType << 32) | ((long)ObjectIndex);
			}
		}

		public ObjectClass ObjectType = ObjectClass.ANY;
		public int ObjectIndex = 0;
		public EProperty Property;

		[Tooltip("Int value")]		public int iValue;
		[Tooltip("Bool value")]		public bool bValue;
		[Tooltip("Float value")]	public float fValue;
		[Tooltip("String value")]	public string sValue;

		public static WorldProperty Deserialise(BinaryReader reader)
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

		public ObjectClass FromID()
		{
			int cl = (int)(ObjectID >> 32);
			return (ObjectClass)cl;
		}

		public int GetIndexFromID()
		{
			long idx = ObjectID << 32;
			return (int)(idx >> 32);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(WorldProperty))]
	public class WorldPropertyEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			WorldProperty myTarget = (WorldProperty)target;
			if (GUILayout.Button("Generate GUID"))
			{
				myTarget.guid = GUID.Generate().ToString();
			}

			DrawDefaultInspector();
		}
	}
#endif //UNITY_EDITOR
}