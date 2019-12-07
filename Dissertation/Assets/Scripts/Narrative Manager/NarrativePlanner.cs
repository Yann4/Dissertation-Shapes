using Dissertation.Narrative.Editor;
using Dissertation.NodeGraph;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace Dissertation.Narrative
{
	public class NarrativePlanner
	{
		private List<Beat> _beatSet = new List<Beat>();
		private WorldStateManager _worldState = null;

		public NarrativePlanner(WorldStateManager worldState, TextAsset beatAsset)
		{
			_worldState = worldState;

			List<Node> nodes = new List<Node>();
			BeatNode startNode = NodeUtils.LoadGraph(beatAsset.bytes, NodeUtils.CreateNode<BeatNode>, nodes) as BeatNode;

			_beatSet.Clear();
			_beatSet.Capacity = nodes.Capacity;

			foreach(Node node in nodes)
			{
				_beatSet.Add((node as BeatNode).BeatData);
			}
		}

		private bool IsBeatViable(Beat beat)
		{
			return beat.RepetitionsPerformed < beat.MaxRepetitions && beat.MeetsPreconditions(_worldState);
		}

		private IEnumerator GeneratePlans(Beat targetState)
		{
			UnityEngine.Profiling.Profiler.BeginSample("Gather viable beats");

			List<Beat> starterBeats = new List<Beat>(_beatSet);
			for(int idx = starterBeats.Count - 1; idx >= 0; idx--)
			{
				if(!IsBeatViable(starterBeats[idx]))
				{
					starterBeats.RemoveAt(idx);
				}
			}

			UnityEngine.Profiling.Profiler.EndSample(); //Gather viable beats

			yield return null;

			UnityEngine.Profiling.Profiler.BeginSample("Setup jobs");
			List<GOAPJob> jobs = new List<GOAPJob>(starterBeats.Count);
			List<JobHandle> handles = new List<JobHandle>(starterBeats.Count);

			for (int idx = 0; idx < starterBeats.Count; idx++)
			{
				jobs.Add(new GOAPJob()
				{
					WorldState = _worldState.GetCurrentWorldState()
				});

				handles.Add(jobs[idx].Schedule());
			}

			UnityEngine.Profiling.Profiler.EndSample(); //Setup jobs

		}
	}

	struct GOAPJob : IJob
	{
		[DeallocateOnJobCompletion] public NativeHashMap<WorldProperty.Key, WorldProperty.Value> WorldState;

		public void Execute()
		{
			throw new System.NotImplementedException();
		}
	}
}