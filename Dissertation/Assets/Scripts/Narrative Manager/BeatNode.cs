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

		private int _numPreconditions = 0;
		private int _numRequiredActions = 0;
		private int _numOptionalActions = 0;

		private readonly float _baseSelectedHeight;
		private readonly float _elementHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
#if UNITY_EDITOR
		public BeatNode(Vector2 position, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
			: base(position, Vector2.zero, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, guid)
		{
			_baseSelectedHeight = (10 * _elementHeight) + (PlayerArchetype.NumArchetypes * _elementHeight);

			_unselectedSize = new Vector2(200.0f, _baseSelectedHeight);
			_selectedSize = new Vector2(300.0f, 700.0f);

			NodeRect.size = _unselectedSize;

			Title = "";
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
			Rect contentRect = NodeRect;
			contentRect.size = _isSelected ? _selectedSize : _unselectedSize;
			
			contentRect.x += 10;
			contentRect.y += 10;
			contentRect.width -= 20;
			contentRect.height -= 20;
			GUILayout.BeginArea(contentRect);

			DrawList<WorldPropertyScriptable>(BeatData.Preconditions, "Preconditions", ref _numPreconditions);

			BeatData.Archetype.DrawContent();

			DrawList<Action>(BeatData.RequiredActions, "Required Actions", ref _numRequiredActions);
			DrawList<Action>(BeatData.OptionalActions, "Optional Actions", ref _numOptionalActions);

			EditorGUILayout.Space();

			BeatData.MaxRepetitions = EditorGUILayout.IntField(new GUIContent("Max Repetitions"), BeatData.MaxRepetitions);
			BeatData.MaxRepetitions = BeatData.MaxRepetitions >= 1 ? BeatData.MaxRepetitions : 1;

			_selectedSize.y = _baseSelectedHeight + (_numOptionalActions + _numPreconditions + _numRequiredActions) * _elementHeight;

			NodeRect.size = contentRect.size + new Vector2(20, 20);

			GUILayout.EndArea();
		}

		private void DrawList<T>(List<T> container, string label, ref int count) where T : UnityEngine.Object
		{
			count = EditorGUILayout.IntField(new GUIContent(label), count);
			count = count >= 0 ? count : 0;

			while (container.Count < count)
			{
				container.Add(default(T));
			}

			while (container.Count > count)
			{
				container.RemoveAt(container.Count - 1);
			}

			if (_isSelected)
			{
				for (int idx = 0; idx < count; idx++)
				{
					container[idx] = EditorGUILayout.ObjectField(container[idx], typeof(T), false) as T;
				}
			}
		}
#endif //UNITY_EDITOR

		public BeatNode(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
		: base(reader, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode)
		{
			_baseSelectedHeight = (10 * _elementHeight) + (PlayerArchetype.NumArchetypes * _elementHeight);

			_unselectedSize = new Vector2(200.0f, _baseSelectedHeight);
			_selectedSize = new Vector2(300.0f, 700.0f);
		}

		public BeatNode(BinaryReader reader) : base(reader)
		{
			_baseSelectedHeight = (10 * _elementHeight) + (PlayerArchetype.NumArchetypes * _elementHeight);

			_unselectedSize = new Vector2(200.0f, _baseSelectedHeight);
			_selectedSize = new Vector2(300.0f, 700.0f);
		}

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

			_numPreconditions = reader.ReadInt32();
			_numRequiredActions = reader.ReadInt32();
			_numOptionalActions = reader.ReadInt32();

			BeatData = new Beat(reader);
		}

		public override void Connect(List<Node> connectsTo)
		{
			base.Connect(connectsTo);

			if(BeatData.Importance == 1)
			{
				int next = BeatData.Order + 1;
				List<Node> nextNodes = connectsTo.FindAll(node =>
				{
					Beat beat = (node as BeatNode).BeatData;
					return beat.Importance == 1 && beat.Order == next;
				});

				if(nextNodes.Count == 1) //Ideal
				{
					BeatData.NextMajorBeat = (nextNodes[0] as BeatNode).BeatData;
				}
				else if (nextNodes.Count > 1)
				{
					Debug.LogErrorFormat("Multiple beats with order {0} is invalid", next);
				}
			}
		}
	}
}