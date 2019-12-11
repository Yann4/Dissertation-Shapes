#define DEBUG_PLANNER

using Dissertation.Narrative.Editor;
using Dissertation.NodeGraph;
using System.Collections.Generic;
using UnityEngine;
using PropertyMap = System.Collections.Generic.List<Dissertation.Narrative.WorldProperty>;

namespace Dissertation.Narrative
{
	public class NarrativePlanner
	{
		private List<Beat> _beatSet = new List<Beat>();
		private WorldStateManager _worldState = null;

		private Plan _currentPlan = null;

		private MonoBehaviour _toRunCoroutineOn = null;

		private Beat _currentMajorBeat;
		private Beat _nextMajorBeat;

		public NarrativePlanner(WorldStateManager worldState, MonoBehaviour toRunCoroutineOn, TextAsset beatAsset)
		{
			_worldState = worldState;
			_toRunCoroutineOn = toRunCoroutineOn;

			List<Node> nodes = new List<Node>();
			BeatNode startNode = NodeUtils.LoadGraph(beatAsset.bytes, NodeUtils.CreateNode<BeatNode>, nodes) as BeatNode;

			_beatSet.Clear();
			_beatSet.Capacity = nodes.Capacity;

			foreach(Node node in nodes)
			{
				_beatSet.Add((node as BeatNode).BeatData);
			}

			_currentMajorBeat = GetNextMajorBeat();
			_currentMajorBeat.Perform();
			_nextMajorBeat = GetNextMajorBeat();

			GetPlan(_nextMajorBeat);
		}

		private Beat GetNextMajorBeat()
		{
			if(_currentMajorBeat != null)
			{
				int nextBeatOrder = _currentMajorBeat.Order + 1;
				return _beatSet.Find(beat => beat.Importance == 1.0f && beat.Order == nextBeatOrder);
			}
			else
			{
				return _beatSet.Find(beat => beat.Importance == 1.0f && beat.Order == 0);
			}
		}

		private void GetPlan(Beat targetBeat)
		{
#if DEBUG_PLANNER
			GeneratePlans(targetBeat);
#else
			_toRunCoroutineOn.StartCoroutine(GeneratePlans(targetBeat));
#endif
		}

		private bool IsBeatViable(Beat beat)
		{
			return beat.RepetitionsPerformed < beat.MaxRepetitions && beat.MeetsPreconditions(_worldState);
		}

#if DEBUG_PLANNER
		private void GeneratePlans(Beat targetState)
#else
		private IEnumerator GeneratePlans(Beat targetState)
#endif
		{
			_currentPlan = null;

			UnityEngine.Profiling.Profiler.BeginSample("Gather viable beats");

			List<Beat> starterBeats = new List<Beat>(_beatSet);
			for(int idx = starterBeats.Count - 1; idx >= 0; idx--)
			{
				if(!IsBeatViable(starterBeats[idx]))
				{
					starterBeats.RemoveAt(idx);
				}
			}

			List<Beat> beatSet = new List<Beat>();
			foreach(Beat beat in _beatSet)
			{
				if(beat.RepetitionsPerformed < beat.MaxRepetitions)
				{
					beatSet.Add(beat);
				}
			}

			UnityEngine.Profiling.Profiler.EndSample(); //Gather viable beats

#if !DEBUG_PLANNER
			yield return null;
#endif

			UnityEngine.Profiling.Profiler.BeginSample("Setup jobs");

			int numPlans = starterBeats.Count;
			if (numPlans > 0)
			{
				PropertyMap goalState = targetState.Preconditions;
				GOAPJob[] jobs = new GOAPJob[numPlans];
				for (int idx = 0; idx < numPlans; idx++)
				{
					jobs[idx] = new GOAPJob()
					{
						WorldState = _worldState.GetCurrentWorldState(),
						Beats = beatSet,
						GoalState = goalState,
						startBeat = starterBeats[idx]
					};

					jobs[idx].Execute();
#if !DEBUG_PLANNER
				yield return null;
#endif
				}

				UnityEngine.Profiling.Profiler.EndSample(); //Setup jobs

#if !DEBUG_PLANNER
			yield return null;
#endif //!DEBUG_PLANNER

				Plan[] plans = new Plan[jobs.Length];

				for (int idx = 0; idx < jobs.Length; idx++)
				{
					plans[idx] = new Plan(this, jobs[idx].Plan);
				}

				Plan bestPlan = plans[0];
				foreach (Plan plan in plans)
				{
					if (plan.Score < bestPlan.Score)
					{
						bestPlan = plan;
					}
				}

				_currentPlan = bestPlan;
			}
			else
			{
				UnityEngine.Debug.LogError("Couldn't generate any plans as there are no viable starter beats");
			}
		}

		public Beat FindBeat(int id)
		{
			return _beatSet.Find(x => x.UID == id);
		}

		private class Plan
		{
			public List<Beat> Beats;
			public float Score { get; private set; }

			public Plan(NarrativePlanner planner, Queue<Beat> plan)
			{
				Beats = new List<Beat>(plan.Count);
				while(plan.Count > 0)
				{
					Beats.Add(plan.Dequeue());
				}

				CalculateScore();
			}

			private void CalculateScore()
			{
				foreach(Beat beat in Beats)
				{
					Score += beat.Cost;
				}
			}
		}
	}

	struct GOAPJob
	{
		public PropertyMap WorldState;

		public List<Beat> Beats;
		public List<WorldProperty> GoalState;

		public Queue<Beat> Plan;

		public Beat startBeat;

		private List<PlannerNode> Leaves;

		public void Execute()
		{
			Leaves = new List<PlannerNode>();
			Plan = new Queue<Beat>();

			PlannerNode startNode = new PlannerNode(null, ApplyBeatToWorldState(ref WorldState, startBeat), startBeat);

			if(Beats.Contains(startBeat))
			{
				Beats = BeatSubset(Beats, startBeat);
			}

			if(BuildGraph(startNode, Beats))
			{
				PlannerNode bestPlan = Leaves[0];
				foreach(PlannerNode leaf in Leaves)
				{
					if(leaf.Cost < bestPlan.Cost)
					{
						bestPlan = leaf;
					}
				}

				PlannerNode node = bestPlan;
				while(node != null)
				{
					Plan.Enqueue(node.NodeBeat);
					node = node.Parent;
				}
			}
		}

		private bool BuildGraph(PlannerNode parent, List<Beat> availableActions)
		{
			bool foundSolution = false;

			foreach(Beat beat in availableActions)
			{
				if(WorldStateManager.IsInState(parent.State, beat.Preconditions))
				{
					PropertyMap newState = ApplyBeatToWorldState(ref parent.State, beat);
					PlannerNode node = new PlannerNode(parent, newState, beat );

					if(WorldStateManager.IsInState(newState, GoalState))
					{
						Leaves.Add(node);
						foundSolution = true;
					}
					else
					{
						if(BuildGraph( node, BeatSubset(availableActions, beat) ))
						{
							foundSolution = true;
						}
					}
				}
			}

			return foundSolution;
		}

		private PropertyMap ApplyBeatToWorldState(ref PropertyMap worldState, Beat beat)
		{
			PropertyMap newState = new PropertyMap(worldState.Count + beat.Postconditions.Count);
			for(int idx = 0; idx < worldState.Count; idx++)
			{
				newState.Add(worldState[idx]);
			}

			foreach(WorldProperty prop in beat.Postconditions)
			{
				WorldStateManager.SetState(newState, prop);
			}

			return newState;
		}

		private List<Beat> BeatSubset( List<Beat> set, Beat toRemove )
		{
			List<Beat> subset = new List<Beat>(set.Count - 1);

			for (int idx = 0; idx < set.Count; idx++)
			{
				if(!set[idx].Equals(toRemove))
				{
					subset.Add(set[idx]);
				}
			}

			return subset;
		}
	}

	public class PlannerNode
	{
		public PlannerNode Parent;
		public float Cost { get; private set; }
		public PropertyMap State;
		public Beat NodeBeat;

		public PlannerNode(PlannerNode parent, PropertyMap state, Beat beat)
		{
			Parent = parent;

			if (parent != null)
			{
				Cost = parent.Cost + beat.Cost;
			}
			else
			{
				Cost = beat.Cost;
			}

			State = state;
			NodeBeat = beat;
		}
	}
}