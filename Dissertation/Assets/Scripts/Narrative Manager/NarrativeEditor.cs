#if UNITY_EDITOR
using Dissertation.NodeGraph;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Narrative.Editor
{
	public class NarrativeEditor : NodeGraph.Editor.NodeEditor
	{
		new protected const string Title = "Narrative Editor";

		[MenuItem("Window/" + Title)]
		private static void OpenWindow()
		{
			OpenWindow<NarrativeEditor>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			DataFolder = Path.Combine(Application.dataPath, "Data", "Narrative");

			_nodeStyle.alignment = TextAnchor.UpperCenter;
			_selectedNodeStyle.alignment = TextAnchor.UpperCenter;
			_nodeStyle.contentOffset = new Vector2(0, 10);
			_selectedNodeStyle.contentOffset = new Vector2(0, 10);
		}

		protected override Node CreateNode(Vector2 mousePosition)
		{
			return new BeatNode(mousePosition, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
		}

		protected override Node CreateNode(BinaryReader reader)
		{
			return new BeatNode(reader, _nodeStyle, _selectedNodeStyle, _inStyle, _outStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
		}
	}
}
#endif //UNITY_EDITOR