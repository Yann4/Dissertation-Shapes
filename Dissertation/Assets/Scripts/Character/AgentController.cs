using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class AgentController : BaseCharacterController
	{
		private Stack<State> _immediate = new Stack<State>();
		private Stack<State> _normal = new Stack<State>();
		private Stack<State> _longTerm = new Stack<State>();

		private State Current
		{
			get
			{
				if(_immediate.Count != 0)
				{
					return _immediate.Peek();
				}

				if(_normal.Count != 0)
				{
					return _normal.Peek();
				}

				if(_longTerm.Count != 0)
				{
					return _longTerm.Peek();
				}

				return null;
			}
		}

		private AgentConfig _agentConfig;

		protected override void Awake()
		{
			base.Awake();
			Debug.Assert(_config is AgentConfig);

			_agentConfig = _config as AgentConfig;

			PushState( StateFactory.GetDefaultState(_agentConfig.DefaultState, this) );
			PushState(new PathToState.PathToConfig(this, new Vector3(-38, -5))); //Testing stuff
			PushState(new PathToState.PathToConfig(this, new Vector3(46, 2))); //Testing stuff
		}

		protected override void Update()
		{

			UnityEngine.Profiling.Profiler.BeginSample("Update agent state");
			UnityEngine.Profiling.Profiler.BeginSample("Update agent state " + Current.Config.StateType);
			Current.Update();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.EndSample();

			base.Update();
		}

		public void PushState(StateConfig config)
		{
			Debug.Assert(config != null);

			State previousActiveState = Current;

			State state = StateFactory.GetState(config);
			switch (config.Priority)
			{
				case StatePriority.Immediate:
					_immediate.Push(state);
					break;
				case StatePriority.Normal:
					_normal.Push(state);
					break;
				case StatePriority.LongTerm:
					_longTerm.Push(state);
					break;
				default:
					Debug.LogError("Invalid priority on state " + config.StateType);
					break;
			}

			//If we've swapped what state is active, notify the states
			if(Current == state)
			{
				previousActiveState?.OnDisable();
				state.OnEnable();
			}
		}

		public void PopState(State state)
		{
			Debug.Assert(state != null);

			bool popped = false;
			switch (state.Config.Priority)
			{
				case StatePriority.Immediate:
					if(_immediate.Peek() == state)
					{
						_immediate.Pop();
						popped = true;
					}
					else
					{
						Debug.LogError("Couldn't pop state as it wasn't on top of the stack");
					}
					break;
				case StatePriority.Normal:
					if (_normal.Peek() == state)
					{
						_normal.Pop();
						popped = true;
					}
					else
					{
						Debug.LogError("Couldn't pop state as it wasn't on top of the stack");
					}
					break;
				case StatePriority.LongTerm:
					if (_longTerm.Peek() == state)
					{
						_longTerm.Pop();
						popped = true;
					}
					else
					{
						Debug.LogError("Couldn't pop state as it wasn't on top of the stack");
					}
					break;
				default:
					Debug.LogError("Invalid priority on state " + state.Config.StateType);
					break;
			}

			if(popped)
			{
				state.Destroy();
				Current.OnEnable();
			}
		}
	}
}