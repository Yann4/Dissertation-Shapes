using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Editor
{
	public class ConversationEditor : NodeEditor
	{
		new protected const string Title = "Conversation Editor";

		[MenuItem("Window/" + Title)]
		private static void OpenWindow()
		{
			OpenWindow<ConversationEditor>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			_nodeStyle.alignment = TextAnchor.UpperCenter;
			_selectedNodeStyle.alignment = TextAnchor.UpperCenter;
			_nodeStyle.contentOffset = new Vector2(0, 10);
			_selectedNodeStyle.contentOffset = new Vector2(0, 10);

			_defaultNodeSize = new Vector2(200.0f, 250.0f);
		}

		protected override Node CreateNode(Vector2 mousePosition)
		{
			return new ConversationNode(mousePosition, _defaultNodeSize, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
		}

		protected override Node CreateNode(BinaryReader reader)
		{
			return new ConversationNode(reader, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
		}
	}
}