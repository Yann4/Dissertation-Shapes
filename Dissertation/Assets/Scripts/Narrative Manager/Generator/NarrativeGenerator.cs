#if UNITY_EDITOR
using Dissertation.Narrative.Editor;
using Dissertation.NodeGraph;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissertation.Narrative.Generator
{
	public class NarrativeGenerator
	{
		private TextAsset _nodeGraph;
		private string _nodeGraphPath;
		private WorldStateManager _worldState;

		private List<Node> _nodes = new List<Node>();
		private List<Connection> _connections = new List<Connection>();

		private Vector2 _generatedBeatStartLocation = new Vector2(-100, 0);

		public NarrativeGenerator(TextAsset nodeGraph, WorldStateManager worldState)
		{
			_nodeGraph = nodeGraph;
			_nodeGraphPath = AssetDatabase.GetAssetPath(_nodeGraph);
			_worldState = worldState;
		}

		public void RunGeneration()
		{
			_nodes.Clear();
			_connections.Clear();

			NodeUtils.LoadGraph(_nodeGraph.bytes, CreateNode, _nodes, _connections, null);

			GeneratorUtils.DeleteAllGeneratedAssets();

			for(int idx = _nodes.Count - 1; idx >= 0; idx--)
			{
				if((_nodes[idx] as BeatNode).BeatData.Generated)
				{
					_nodes.RemoveAt(idx);
				}
			}

			Vector2 pos = _generatedBeatStartLocation;
			for(int idx = 0; idx < 10; idx++)
			{
				_nodes.Add(new BeatNode(pos, new Beat(true)));
				pos.y -= (_nodes[0].NodeRect.height + 10);
			}

			NodeUtils.SaveGraph(_nodeGraphPath, _nodes, _connections);
		}

		private Node CreateNode(BinaryReader reader)
		{
			return new BeatNode(reader);
		}
	}
}
#endif //UNITY_EDITOR