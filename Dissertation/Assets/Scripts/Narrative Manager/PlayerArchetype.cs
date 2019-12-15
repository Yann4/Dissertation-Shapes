using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Dissertation.Narrative
{
	[Serializable]
	public class PlayerArchetype : PropertyAttribute
	{
		public enum Type { Fighter, Storyteller, MethodActor, Tactician, PowerGamer }
		public static readonly int NumArchetypes = Enum.GetNames(typeof(Type)).Length;

		public float[] Values = new float[Enum.GetNames(typeof(Type)).Length];

		public PlayerArchetype()
		{ }

		public PlayerArchetype(params float[] values)
		{
			Debug.Assert(values.Length == NumArchetypes, string.Format("Must have exactly {0} values passed through", NumArchetypes));
			Values = values;
		}

		public PlayerArchetype(BinaryReader reader)
		{
			int numValues = Enum.GetNames(typeof(Type)).Length;

			for(int idx = 0; idx < numValues; idx++)
			{
				Values[idx] = reader.ReadSingle();
			}
		}

		public static float operator*(PlayerArchetype lhs, PlayerArchetype rhs)
		{
			float ret = 0.0f;

			for(int idx = 0; idx < NumArchetypes; idx++)
			{
				ret += lhs.Values[idx] * rhs.Values[idx];
			}

			return ret;
		}

		public static PlayerArchetype operator+(PlayerArchetype lhs, PlayerArchetype rhs)
		{
			PlayerArchetype ret = new PlayerArchetype();

			for (int idx = 0; idx < NumArchetypes; idx++)
			{
				ret.Values[idx] = lhs.Values[idx] + rhs.Values[idx];
			}

			return ret;
		}

		public static PlayerArchetype operator-(PlayerArchetype lhs, PlayerArchetype rhs)
		{
			PlayerArchetype ret = new PlayerArchetype();

			for (int idx = 0; idx < NumArchetypes; idx++)
			{
				ret.Values[idx] = lhs.Values[idx] - rhs.Values[idx];
			}

			return ret;
		}

		public float this[Type type]
		{
			get { return Values[(int)type]; }
			set { Values[(int)type] = value; }
		}

		public void Serialize(BinaryWriter writer)
		{
			foreach(float val in Values)
			{
				writer.Write(val);
			}
		}

#if UNITY_EDITOR
		public void DrawContent()
		{
			GUILayout.BeginVertical();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Player archetype values");

			for (int idx = 0; idx < NumArchetypes; idx++)
			{
				Values[idx] = EditorGUILayout.Slider(((Type)idx).ToString(), Values[idx], 0.0f, 1.0f);
			}

			EditorGUILayout.Space();

			GUILayout.EndVertical();
		}
#endif //UNITY_EDITOR
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(PlayerArchetype))]
	public class PlayerArchetypeDrawer : PropertyDrawer
	{
		bool _foldout = false;
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUILayout.BeginVertical();

			_foldout = EditorGUILayout.Foldout(_foldout, label.text);
			if (_foldout)
			{
				SerializedProperty arrayProp = property.FindPropertyRelative("Values");
				for (int i = 0; i < PlayerArchetype.NumArchetypes; i++)
				{
					// This will display an Inspector Field for each array item (layout this as desired)
					SerializedProperty value = arrayProp.GetArrayElementAtIndex(i);
					EditorGUILayout.Slider(value, 0.0f, 1.0f, new GUIContent(Enum.GetName(typeof(PlayerArchetype.Type), i)));
				}
			}

			EditorGUILayout.EndVertical();
		}
	}
#endif //UNITY_EDITOR
}