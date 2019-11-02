using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Dissertation.Character;
using System.Collections.Generic;

namespace Dissertation.NodeGraph
{
	public class ConversationNode : Node
	{
		public ConversationFragment Sentence;
		private const int _sentenceOptions = 2;

#if UNITY_EDITOR
		public ConversationNode(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
			: base(position, size, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, guid)
		{
			Title = "Sentence";
			Sentence = new ConversationFragment(false, _sentenceOptions);
		}
#endif //UNITY_EDITOR

		public ConversationNode(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
			: base(reader, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode)
		{ }

		public ConversationNode(BinaryReader reader) : base(reader)
		{ }

		public override void Connect(List<Node> connectsTo)
		{
			base.Connect(connectsTo);

			Sentence.NextFragments = new ConversationFragment[ConnectedTo.Length];
			for(int idx = 0; idx < ConnectedTo.Length; idx++)
			{
				ConversationNode next = ConnectedTo[idx] as ConversationNode;
				Debug.Assert(next != null);

				Sentence.NextFragments[idx] = next.Sentence;
			}
		}

		public override void Serialize(BinaryWriter writer)
		{
			base.Serialize(writer);

			Sentence.Serialise(writer);
		}

		protected override void Deserialise(BinaryReader reader)
		{
			base.Deserialise(reader);

			Sentence = new ConversationFragment(reader);
		}

#if UNITY_EDITOR
		protected override void DrawContent(ref Rect currentContentRect)
		{
			base.DrawContent(ref currentContentRect);

			Sentence.IsPlayer = GUI.Toggle(currentContentRect, Sentence.IsPlayer, new GUIContent("Is player?"));
			currentContentRect.y += currentContentRect.height + 5;

			for (int idx = 0; idx < _sentenceOptions; idx++)
			{
				GUI.Label(currentContentRect, new GUIContent("Sentence option " + idx));
				currentContentRect.y += currentContentRect.height + 5;

				Sentence.ToSay[idx] = GUI.TextField(currentContentRect, Sentence.ToSay[idx]);
				currentContentRect.y += currentContentRect.height + 5;
			}
		}
#endif //UNITY_EDITOR
	}
}