using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Dissertation.Narrative
{
	public class PlayerArchetype
	{
		public enum Type { Fighter, Storyteller, MethodActor, Tactician, PowerGamer }
		public static readonly int NumArchetypes = Enum.GetNames(typeof(Type)).Length;

		private float[] Values = new float[Enum.GetNames(typeof(Type)).Length];

		public PlayerArchetype()
		{ }

		public PlayerArchetype(BinaryReader reader)
		{
			int numValues = Enum.GetNames(typeof(Type)).Length;

			for(int idx = 0; idx < numValues; idx++)
			{
				Values[idx] = reader.ReadSingle();
			}
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
				Values[idx] = EditorGUILayout.FloatField(((Type)idx).ToString(), Values[idx]);
			}

			EditorGUILayout.Space();

			GUILayout.EndVertical();
		}
#endif //UNITY_EDITOR
	}
}