using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Editor
{
	public struct ConversationFragment
	{
		public string Speaker;
		public int PreviousOption;
		public string[] ToSay;

		public ConversationFragment(string speaker, int SentenceOptions)
		{
			Speaker = speaker;
			PreviousOption = -1;

			ToSay = new string[SentenceOptions];
			for(int idx = 0; idx < SentenceOptions; idx++)
			{
				ToSay[idx] = string.Empty;
			}
		}

		public ConversationFragment(BinaryReader reader)
		{
			Speaker = reader.ReadString();

			PreviousOption = reader.ReadInt32();

			int count = reader.ReadInt32();
			ToSay = new string[count];

			for(int idx = 0; idx < count; idx++)
			{
				ToSay[idx] = reader.ReadString();
			}
		}

		public void Serialise(BinaryWriter writer)
		{
			writer.Write(Speaker);
			writer.Write(PreviousOption);
			writer.Write(ToSay.Length);

			foreach(string text in ToSay)
			{
				writer.Write(text);
			}
		}
	}

	public class ConversationNode : Node
	{
		public ConversationFragment Sentence;
		private const int _sentenceOptions = 2;
		string previousOptionField = "-1";

		public ConversationNode(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
			: base(position, size, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, guid)
		{
			Title = "Sentence";
			Sentence = new ConversationFragment(string.Empty, _sentenceOptions);
		}

		public ConversationNode(BinaryReader reader, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode)
			: base(reader, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode)
		{
			Sentence = new ConversationFragment(reader);
		}

		public override void Draw()
		{
			base.Draw();

			Rect currentContentRect = new Rect(NodeRect.x + 10, NodeRect.y + 30, NodeRect.width - 20, 20);
			EditorGUILayout.BeginVertical();

			GUI.Label(currentContentRect, new GUIContent("Speaker Tag"));
			currentContentRect.y += currentContentRect.height + 5;

			Sentence.Speaker = GUI.TextField(currentContentRect, Sentence.Speaker);
			currentContentRect.y += currentContentRect.height + 5;

			GUI.Label(currentContentRect, new GUIContent("Previous option index"));
			currentContentRect.y += currentContentRect.height + 5;

			int value = Sentence.PreviousOption;
			previousOptionField = GUI.TextField(currentContentRect, previousOptionField);
			if (int.TryParse(previousOptionField, out value))
			{
				Sentence.PreviousOption = value;
			}

			currentContentRect.y += currentContentRect.height + 5;

			for (int idx = 0; idx < _sentenceOptions; idx++)
			{
				GUI.Label(currentContentRect, new GUIContent("Sentence option " + idx));
				currentContentRect.y += currentContentRect.height + 5;

				Sentence.ToSay[idx] = GUI.TextField(currentContentRect, Sentence.ToSay[idx]);
				currentContentRect.y += currentContentRect.height + 5;
			}

			EditorGUILayout.EndVertical();
		}

		public override void Serialize(BinaryWriter writer)
		{
			if (!int.TryParse(previousOptionField, out int val))
			{
				Debug.LogErrorFormat("Couldn't parse input on previous option index for {0} - index is '{1}'", Sentence.Speaker, previousOptionField);
				_isSelected = true;
			}

			base.Serialize(writer);

			Sentence.Serialise(writer);
		}
	}
}