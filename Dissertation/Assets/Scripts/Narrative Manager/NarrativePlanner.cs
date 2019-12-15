#define DEBUG_PLANNER

using Dissertation.Narrative.Editor;
using Dissertation.NodeGraph;
using Dissertation.Util;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Narrative
{
	public class NarrativePlanner
	{
		private List<Beat> _beatSet = new List<Beat>();
		private WorldStateManager _worldState = null;

		private Plan _currentPlan = null;
		private bool IsPlanning { get { return _currentPlan == null && _nextMajorBeat != null; } }

		private MonoBehaviour _toRunCoroutineOn = null;
		private Beat _currentMajorBeat;
		private Beat _nextMajorBeat;
		private Beat _currentBeat;

		private bool Enabled = false;

		private System.Action OnFinished;

		private Beat _extraBeat = null;
		private const float _frequencyToRunExtraBeats = 60.0f;
		private float _lastRunExtraBeat = 0.0f;

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

			OnFinished += OnFinishedPlanning;
		}

		public void Enable()
		{
			Enabled = true;
			Replan();
		}

		private void Replan()
		{
			_currentMajorBeat = GetNextMajorBeat();
			_currentBeat = _currentMajorBeat;
			_nextMajorBeat = GetNextMajorBeat();

			if (_nextMajorBeat != null)
			{
				GetPlan(_nextMajorBeat);
			}
		}

		public void Update()
		{
			if(!Enabled)
			{
				return;
			}

			if(_extraBeat == null)
			{
				if(!IsPlanning && Time.time - _lastRunExtraBeat > _frequencyToRunExtraBeats)
				{
					_extraBeat = SelectExtraBeat();
					if(_extraBeat != null)
					{
						_extraBeat.Perform();
					}
				}
			}
			else if(!_extraBeat.Update(_worldState))
			{
				if(_extraBeat.ExceededMaxRepititions)
				{
					_beatSet.Remove(_extraBeat);
				}

				_extraBeat = null;
				_lastRunExtraBeat = Time.time;
			}

			if(!IsPlanning && _currentBeat != null && _currentPlan != null)
			{
				if(!_currentBeat.Update(_worldState))
				{
					if (_currentBeat.ExceededMaxRepititions)
					{
						_beatSet.Remove(_currentBeat);
					}

					_currentBeat = _currentPlan.NextBeat();
					if(_currentBeat == null || !_currentBeat.MeetsPreconditions(_worldState))
					{
						Replan();
					}
					else
					{
						_currentBeat.Perform();
					}
				}
			}
		}

		private Beat GetNextMajorBeat()
		{
			if(_currentMajorBeat != null)
			{
				int nextBeatOrder = _currentMajorBeat.Order + 1;
				return _beatSet.Find(beat => beat.Importance == 1.0f && beat.Order == nextBeatOrder && !beat.ExceededMaxRepititions);
			}
			else
			{
				return _beatSet.Find(beat => beat.Importance == 1.0f && beat.Order == 0 && !beat.ExceededMaxRepititions);
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
			return !beat.ExceededMaxRepititions && beat.MeetsPreconditions(_worldState);
		}

		private void OnFinishedPlanning()
		{
			if(_currentPlan != null)
			{
				_currentBeat = _currentPlan.NextBeat();
				_currentBeat.Perform();
			}
		}

		private Beat SelectExtraBeat()
		{
			List<Beat> potentialBeats = new List<Beat>(_beatSet.Capacity);
			foreach(Beat beat in _beatSet)
			{
				if(beat.Importance != 1.0f && IsBeatViable(beat) && (_currentPlan == null || _currentPlan.IsBeatInPlan(beat)))
				{
					potentialBeats.Add(beat);
				}
			}

			if (potentialBeats.Count > 0)
			{
				potentialBeats.Sort();
				return potentialBeats[0];
			}

			return null;
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
				if(!IsBeatViable(starterBeats[idx]) || starterBeats[idx] == _currentBeat)
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
				GoalOrientedActionPlanner[] jobs = new GoalOrientedActionPlanner[numPlans];
				for (int idx = 0; idx < numPlans; idx++)
				{
					jobs[idx] = new GoalOrientedActionPlanner()
					{
						State = _worldState.GetCurrentWorldState(),
						Beats = beatSet,
						GoalState = goalState,
						StartBeat = starterBeats[idx],
						Archetype = _worldState.Archetype,
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
				foreach (GoalOrientedActionPlanner job in jobs)
				{
					if (job.NarrativePlan != null && job.NarrativePlan.Score < bestPlan.Score)
					{
						bestPlan = job.NarrativePlan;
					}
				}

				_currentPlan = bestPlan;
				if(_currentPlan == null)
				{
					Debug.LogError("No plan found to beat " + targetState.Title);
				}
			}
			else
			{
				Debug.LogError("Couldn't generate any plans as there are no viable starter beats");
			}

			OnFinished.InvokeSafe();
		}

		private class Plan
		{
			private List<Beat> Beats;
			public float Score { get; private set; }
			private PlayerArchetype Archetype;
			private int _currentBeat = -1;
			public Beat Current
			{
				get
				{
					if (_currentBeat != -1 && _currentBeat < Beats.Count)
					{
						return Beats[_currentBeat];
					}

					return null;
				}
			}

			public Plan(PlannerNode plan, PlayerArchetype archetype)
			{
				Archetype = archetype;

				Beats = new List<Beat>();
				while (plan != null)
				{
					Beats.Add(plan.NodeBeat);
					plan = plan.Parent;
				}

				CalculateScore();
			}

			//Increments the plan and returns the new current beat
			public Beat NextBeat()
			{
				_currentBeat++;
				return Current;
			}

			private void CalculateScore()
			{
				float affinity = 0.0f;

				foreach( Beat beat in Beats )
				{
					Score += beat.Cost;
					Score += beat.RepetitionsPerformed;
					affinity += Archetype * beat.Archetype;
				}

				Score += (1.0f / affinity) * 10.0f; // * 10 as we want the affinity to have a large impact on the score
			}

			public bool IsBeatInPlan(Beat beat)
			{
				return Beats.Contains(beat);
			}
		}

		private struct GoalOrientedActionPlanner
		{
			public WorldState State; //Initial world state

			public List<Beat> Beats; //Set of all available beats
			public List<WorldProperty> GoalState;

			public Beat StartBeat; //current beat (may not have finished executing - think this might be wrong)

			private List<PlannerNode> Leaves; //Nodes containing potential plans
			public Plan NarrativePlan; //Result, this is what to look at

			public PlayerArchetype Archetype;

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

					NarrativePlan = new Plan(bestPlan, Archetype);
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
				WorldState newState = new WorldState(ref worldState);

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