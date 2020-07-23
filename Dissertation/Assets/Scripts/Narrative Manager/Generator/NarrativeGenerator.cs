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
		private RuleManager _ruleManager;

		private List<Node> _nodes = new List<Node>();
		private List<Connection> _connections = new List<Connection>();

		private Vector2 _generatedBeatStartLocation = new Vector2(-100, 0);

		public NarrativeGenerator(TextAsset nodeGraph, TextAsset ruleSet, BeatTemplates templates, WorldStateManager worldState)
		{
			_nodeGraph = nodeGraph;
			_nodeGraphPath = AssetDatabase.GetAssetPath(_nodeGraph);
			_worldState = worldState;
			_templates = templates;
			_ruleManager = new RuleManager(ruleSet, _templates);
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

			foreach(Rule storyline in _ruleManager.GetStorylineRules())
			{
				Beat[] storyBeats = GenerateStoryline(storyline);
				if(storyBeats != null)
				{
					generatedBeats.AddRange(storyBeats);
				}
			}

			Vector2 pos = _generatedBeatStartLocation;
			foreach(Beat beat in generatedBeats)
			{
				_nodes.Add(new BeatNode(pos, beat));
				pos.y -= (_nodes[0].NodeRect.height + 10);
			}

			NodeUtils.SaveGraph(_nodeGraphPath, _nodes, _connections);
		}

		private Beat[] GenerateStoryline(Rule storyline)
		{
			Rule toDecompose = new Rule(storyline);
			HashSet<NarrativeObject> ruleObjects = new HashSet<NarrativeObject>();

			for (int idx = 0; idx < toDecompose.Right.Count; idx++)
			{
				if (toDecompose.Right[idx].CanDecompose)
				{
					Rule replaceWith = _ruleManager.GetWeightedRandomMatchingRule(toDecompose.Right[idx], ruleObjects);
					if(replaceWith == null)
					{
						//If we can't decompose any of the tokens in the rule, we just can't fulfil that rule, so quit out
						Debug.LogWarningFormat($"Can't decompose token {toDecompose.Right[idx]}, when generating from storyline {storyline}");
						return null;
					}

					toDecompose.Right.RemoveAt(idx);
					toDecompose.Right.InsertRange(idx, replaceWith.Right);
				}
			}

			List<Beat> beats = new List<Beat>();
			foreach(Token token in toDecompose.Right)
			{
				Debug.Assert(!token.CanDecompose, "Shouldn't be able to decompose this token");
				beats.AddRange(NarrativePlanner.LoadBeats(token.Graph));
			}

			return beats.ToArray();
		}
	}
}
#endif //UNITY_EDITOR