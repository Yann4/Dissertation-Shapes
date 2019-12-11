﻿#define DEBUG_PLANNER

using Dissertation.Narrative.Editor;
using Dissertation.NodeGraph;
using System.Collections.Generic;
using UnityEngine;

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
				List<WorldProperty> goalState = targetState.Preconditions;
				GOAPJob[] jobs = new GOAPJob[numPlans];
				for (int idx = 0; idx < numPlans; idx++)
				{
					jobs[idx] = new GOAPJob()
					{
						State = _worldState.GetCurrentWorldState(),
						Beats = beatSet,
						GoalState = goalState,
						StartBeat = starterBeats[idx]
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

				Plan bestPlan = jobs[0].NarrativePlan;
				foreach (GOAPJob job in jobs)
				{
					if (job.NarrativePlan.Score < bestPlan.Score)
					{
						bestPlan = job.NarrativePlan;
					}
				}

				_currentPlan = bestPlan;
			}
			else
			{
				Debug.LogError("Couldn't generate any plans as there are no viable starter beats");
			}
		}

		private class Plan
		{
			public List<Beat> Beats;
			public float Score { get; private set; }

			public Plan(PlannerNode plan)
			{
				Beats = new List<Beat>();
				while (plan != null)
				{
					Beats.Add(plan.NodeBeat);
					plan = plan.Parent;
				}

				CalculateScore();
			}

			private void CalculateScore()
			{
				foreach( Beat beat in Beats )
				{
					Score += beat.Cost;
					Score += beat.RepetitionsPerformed;
				}
			}
		}

		private struct GOAPJob
		{
			public WorldState State; //Initial world state

			public List<Beat> Beats; //Set of all available beats
			public List<WorldProperty> GoalState;

			public Beat StartBeat; //current beat (may not have finished executing - think this might be wrong)

			private List<PlannerNode> Leaves; //Nodes containing potential plans
			public Plan NarrativePlan; //Result, this is what to look at

			public void Execute()
			{
				Leaves = new List<PlannerNode>();

				PlannerNode startNode = new PlannerNode(null, ApplyBeatToWorldState(State, StartBeat), StartBeat);

				if (Beats.Contains(StartBeat))
				{
					Beats = BeatSubset(Beats, StartBeat);
				}

				if (BuildGraph(startNode, Beats))
				{
					PlannerNode bestPlan = Leaves[0];
					foreach (PlannerNode leaf in Leaves)
					{
						if (leaf.Cost < bestPlan.Cost)
						{
							bestPlan = leaf;
						}
					}

					NarrativePlan = new Plan(bestPlan);
				}
			}

			private bool BuildGraph(PlannerNode parent, List<Beat> availableActions)
			{
				bool foundSolution = false;

				foreach (Beat beat in availableActions)
				{
					if (parent.State.IsInState(beat.Preconditions))
					{
						PlannerNode node = new PlannerNode(parent, ApplyBeatToWorldState(parent.State, beat), beat);

						if (node.State.IsInState(GoalState))
						{
							Leaves.Add(node);
							foundSolution = true;
						}
						else
						{
							if (BuildGraph(node, BeatSubset(availableActions, beat)))
							{
								foundSolution = true;
							}
						}
					}
				}

				return foundSolution;
			}

			private WorldState ApplyBeatToWorldState(WorldState worldState, Beat beat)
			{
				WorldState newState = new WorldState(worldState);

				foreach (WorldProperty prop in beat.Postconditions)
				{
					newState.SetState(prop);
				}

				return newState;
			}

			private List<Beat> BeatSubset(List<Beat> set, Beat toRemove)
			{
				List<Beat> subset = new List<Beat>(set.Count - 1);

				for (int idx = 0; idx < set.Count; idx++)
				{
					if (!set[idx].Equals(toRemove))
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
			public WorldState State;
			public Beat NodeBeat;

			public PlannerNode(PlannerNode parent, WorldState state, Beat beat)
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
}