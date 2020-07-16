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
		private BeatTemplates _templates;

		private List<Node> _nodes = new List<Node>();
		private List<Connection> _connections = new List<Connection>();

		private Vector2 _generatedBeatStartLocation = new Vector2(-100, 0);

		public NarrativeGenerator(TextAsset nodeGraph, TextAsset ruleSet, BeatTemplates templates, WorldStateManager worldState)
		{
			_nodeGraph = nodeGraph;
			_nodeGraphPath = AssetDatabase.GetAssetPath(_nodeGraph);
			_worldState = worldState;
			_templates = templates;
			RuleManager rules = new RuleManager(ruleSet);
		}

		public void RunGeneration()
		{
			_nodes.Clear();
			_connections.Clear();

			NodeUtils.LoadGraph(_nodeGraph.bytes, (reader) => new BeatNode(reader), _nodes, _connections);

			GeneratorUtils.DeleteAllGeneratedAssets();

			for(int idx = _nodes.Count - 1; idx >= 0; idx--)
			{
				if((_nodes[idx] as BeatNode).BeatData.Generated)
				{
					_nodes.RemoveAt(idx);
				}
			}

			List<Beat> generatedBeats = new List<Beat>();

// 			foreach(Beat template in _templates.Templates)
// 			{
// 				CheckTemplateBeat(template, generatedBeats);
// 			}

			Vector2 pos = _generatedBeatStartLocation;
			foreach(Beat beat in generatedBeats)
			{
				_nodes.Add(new BeatNode(pos, beat));
				pos.y -= (_nodes[0].NodeRect.height + 10);
			}

			NodeUtils.SaveGraph(_nodeGraphPath, _nodes, _connections);
		}

		private void CheckTemplateBeat(Beat template, List<Beat> outGeneratedBeats)
		{
			switch(template.Title)
			{
				default:
					Debug.LogErrorFormat("Not sure how to generate beats for the template '{0}'", template.Title);
					return;
			}
		}
	}
}
#endif //UNITY_EDITOR