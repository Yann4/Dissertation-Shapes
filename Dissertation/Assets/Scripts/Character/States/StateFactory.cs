using UnityEngine;

namespace Dissertation.Character.AI
{
	public static class StateFactory
	{
		public static State GetState(StateConfig config)
		{
			UnityEngine.Profiling.Profiler.BeginSample("Construct state");

			State state = null;
			switch (config.StateType)
			{
				case States.MoveTo:
					Debug.Assert(config is MoveToState.MoveToConfig);
					state = new MoveToState(config as MoveToState.MoveToConfig);
					break;
				case States.Idle:
					Debug.Assert(config is IdleState.IdleConfig);
					state = new IdleState(config as IdleState.IdleConfig);
					break;
				case States.Traverse:
					Debug.Assert(config is TraverseState.TraverseStateConfig);
					state = new TraverseState(config as TraverseState.TraverseStateConfig);
					break;
				case States.PathTo:
					Debug.Assert(config is PathToState.PathToConfig);
					state = new PathToState(config as PathToState.PathToConfig);
					break;
				case States.Attack:
					{
						Debug.Assert(config is AttackState.AttackConfig);
						switch (config.Owner.Config.Faction)
						{
							case CharacterFaction.Circle:
								state = new DashAttackState(config as AttackState.AttackConfig);
								break;
							case CharacterFaction.Square:
								state = new MeleeAttackState(config as AttackState.AttackConfig);
								break;
							case CharacterFaction.Triangle:
								state = new RangedAttackState(config as AttackState.AttackConfig);
								break;
							default:
								Debug.Assert(false, "Shouldn't have got here");
								break;
						}
						break;
					}
				case States.INVALID:
				default:
					Debug.LogError("Factory not set up for state type " + config.StateType);
					break;
			}

			UnityEngine.Profiling.Profiler.EndSample();
			return state;
		}

		public static StateConfig GetDefaultState(States type, AgentController owner)
		{
			switch (type)
			{
				case States.Idle:
					return new IdleState.IdleConfig(owner, 3.0f);
				default:
					Debug.LogError("State type " + type + " is not currently a valid default state");
					return null;
			}
		}
	}
}