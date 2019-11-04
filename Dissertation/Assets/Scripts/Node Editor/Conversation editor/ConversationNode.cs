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
		private string _enumTemp = "None";
		private string[] _intTemp = new string[_sentenceOptions];
		private string[] _floatTemp = new string[_sentenceOptions];

		private Vector2 _selectedSize;
		private Vector2 _unselectedSize;

#if UNITY_EDITOR
		public ConversationNode(Vector2 position, Vector2 size, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> onClickInPoint, Action<ConnectionPoint> onClickOutPoint, Action<Node> onRemoveNode, GUID? guid = null)
			: base(position, size, nodeStyle, selectedStyle, inPointStyle, outPointStyle, onClickInPoint, onClickOutPoint, onRemoveNode, guid)
		{
			_unselectedSize = size;
			_selectedSize = new Vector2(300.0f, 700.0f);
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
			if (_isSelected)
			{
				NodeRect.size = _unselectedSize;
			}

			base.Serialize(writer);

			if (_isSelected)
			{
				NodeRect.size = _selectedSize;
			}

			writer.Write(_unselectedSize.x);
			writer.Write(_unselectedSize.y);
			writer.Write(_selectedSize.x);
			writer.Write(_selectedSize.y);

			Sentence.Serialise(writer);
		}

		protected override void Deserialise(BinaryReader reader)
		{
			base.Deserialise(reader);

			_unselectedSize = new Vector2(reader.ReadSingle(), reader.ReadSingle());
			_selectedSize = new Vector2(reader.ReadSingle(), reader.ReadSingle());

			Sentence = new ConversationFragment(reader);

			_enumTemp = Sentence.Output.ToString();
			for(int idx = 0; idx < Sentence.OptionOutputData.Length; idx++)
			{
				_intTemp[idx] = Sentence.OptionOutputData[idx].iVal.ToString();
				_floatTemp[idx] = Sentence.OptionOutputData[idx].fVal.ToString();
			}
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

			if (_isSelected)
			{
				GUI.Label(currentContentRect, new GUIContent("ConversationOutput value (should map to enum)"));
				currentContentRect.y += currentContentRect.height + 5;
				_enumTemp = GUI.TextField(currentContentRect, _enumTemp);
				if (Enum.TryParse(_enumTemp, out ConversationOutput val) && val != Sentence.Output)
				{
					Sentence.Output = val;
				}
				currentContentRect.y += currentContentRect.height + 5;

				for (int idx = 0; idx < Sentence.OptionOutputData.Length; idx++)
				{
					OptionFieldData(ref currentContentRect, idx, Sentence.OptionOutputData[idx], ref _intTemp[idx], ref _floatTemp[idx]);
				}
			}
		}

		private void OptionFieldData(ref Rect currentContentRect, int dataIndex, Data data, ref string intTemp, ref string floatTemp)
		{
			GUI.Label(currentContentRect, new GUIContent("ConversationOutput " + dataIndex + " values"));
			currentContentRect.y += currentContentRect.height + 10;

			GUI.Label(currentContentRect, new GUIContent("ConversationOutput " + dataIndex + " string value"));
			currentContentRect.y += currentContentRect.height + 5;
			data.sVal = GUI.TextField(currentContentRect, data.sVal);
			currentContentRect.y += currentContentRect.height + 5;

			GUI.Label(currentContentRect, new GUIContent("ConversationOutput " + dataIndex + "  int value"));
			currentContentRect.y += currentContentRect.height + 5;
			intTemp = GUI.TextField(currentContentRect, intTemp);
			if (int.TryParse(intTemp, out int ival))
			{
				data.iVal = ival;
			}
			currentContentRect.y += currentContentRect.height + 5;

			data.bVal = GUI.Toggle(currentContentRect, data.bVal, new GUIContent("ConversationOutput " + dataIndex + " bool value"));
			currentContentRect.y += currentContentRect.height + 5;

			GUI.Label(currentContentRect, new GUIContent("ConversationOutput  " + dataIndex + " float value"));
			currentContentRect.y += currentContentRect.height + 5;
			floatTemp = GUI.TextField(currentContentRect, floatTemp);
			if (float.TryParse(floatTemp, out float fval))
			{
				data.fVal = fval;
			}
			currentContentRect.y += currentContentRect.height + 5;
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
#endif //UNITY_EDITOR
	}
}