using Dissertation.NodeGraph;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif //UNITY_EDITOR

namespace Dissertation.Narrative.Editor
{
	public class BeatNode : Node
	{
		private Beat BeatData;

		int _numPreconditions = 0;
		int _numRequiredActions = 0;
		int _numOptionalActions = 0;

#if UNITY_EDITOR
		public BeatNode(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
			: base(position, size, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, guid)
		{
			_unselectedSize = size;
			_selectedSize = new Vector2(300.0f, 700.0f);
			Title = "Sentence";
			BeatData = new Beat();
		}

		protected override void OnDeselect()
		{
			base.OnDeselect();
			NodeRect.size = _unselectedSize;
		}

		protected override void OnSelect()
		{
			base.OnSelect();
			NodeRect.size = _selectedSize;
		}

		protected override void DrawContent(ref Rect currentContentRect)
		{
			base.DrawContent(ref currentContentRect);
			GUILayout.BeginArea(NodeRect);

			DrawList<WorldProperty>(BeatData.Preconditions, "Preconditions", ref _numPreconditions);

			BeatData.Archetype.DrawContent();

			DrawList<Action>(BeatData.RequiredActions, "Required Actions", ref _numRequiredActions);
			DrawList<Action>(BeatData.OptionalActions, "Optional Actions", ref _numOptionalActions);

			GUILayout.EndArea();
		}

		private void DrawList<T>(List<T> container, string label, ref int count) where T : class
		{
			count = EditorGUILayout.IntField(new GUIContent(label), count);

			while (container.Count < _numPreconditions)
			{
				container.Add(default(T));
			}

			while (container.Count > count)
			{
				container.RemoveAt(container.Count - 1);
			}

			for (int idx = 0; idx < count; idx++)
			{
				container[idx] = EditorGUILayout.ObjectField(BeatData.Preconditions[idx], typeof(T), false) as T;
			}
		}
#endif //UNITY_EDITOR

			public BeatNode(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
			: base(reader, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode)
		{ }

		public BeatNode(BinaryReader reader) : base(reader)
		{ }

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			writer.Write(_numPreconditions);
			writer.Write(_numRequiredActions);
			writer.Write(_numOptionalActions);

			BeatData.Serialise(writer);
		}

		protected override void Deserialise(BinaryReader reader)
		{
			base.Deserialise(reader);

			_unselectedSize = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			_selectedSize = new Vector2(reader.ReadSingle(), reader.ReadSingle());

			_numPreconditions = reader.ReadInt32();
			_numRequiredActions = reader.ReadInt32();
			_numOptionalActions = reader.ReadInt32();

			BeatData = new Beat(reader);
		}
	}
}