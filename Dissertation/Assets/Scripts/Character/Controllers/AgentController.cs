using Dissertation.Character.Player;
using Dissertation.UI;
using Dissertation.Util.Localisation;
using System;
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

		public AgentConfig _agentConfig { get; private set; }

		private AgentDebugUI _debugUI;
		private Conversation _conversation;

		private Desire[] _desires = new Desire[(int)DesireType.COUNT];
		public SpecialistStates AvailableBehaviours { get; private set; } = SpecialistStates.INVALID;

		protected override void Start()
		{
			base.Start();
			Debug.Assert(_config is AgentConfig);

			_agentConfig = _config as AgentConfig;

			for(int traitType = 0; traitType < (int)DesireType.COUNT; traitType++)
			{
				_desires[traitType] = new Desire((DesireType)traitType);
			}

			foreach(Trait trait in _agentConfig.Traits)
			{
				foreach(Desire.Modifier modifier in trait.DesireModifiers)
				{
					_desires[(int)modifier.ToModify].ApplyModifier(modifier);
				}

				AvailableBehaviours |= trait.SpecialBehaviours;
			}

			PushState( StateFactory.GetDefaultState(_agentConfig.DefaultState, this) );

			_debugUI = HUD.Instance.CreateMenu<AgentDebugUI>();
			_debugUI.Setup(this);

			_conversation = GetComponent<Conversation>();

			Health.OnDamaged += OnTakeDamage;
			Inventory.OnGetCurrency += OnAcquireCurrency;
			Inventory.OnLoseCurrency += OnLoseCurrency;
		}

		protected override void Update()
		{
			UpdateDesires();
			CheckSpecialistStates();

			UnityEngine.Profiling.Profiler.BeginSample("Update agent state");
			UnityEngine.Profiling.Profiler.BeginSample("Update agent state " + Current.Config.StateType);
			Current.Update();
			UnityEngine.Profiling.Profiler.EndSample();
			UnityEngine.Profiling.Profiler.EndSample();

			base.Update();
		}

		private void CheckSpecialistStates()
		{
			for(int state = 0; state < StateFactory.NumSpecialistStates; state++)
			{
				SpecialistStates specialistState = (SpecialistStates)state;
				if (specialistState != SpecialistStates.INVALID && AvailableBehaviours.HasFlag(specialistState) && StateFactory.ShouldEnterState(this, specialistState, out StateConfig config))
				{
					PushState( config );
				}
			}
		}

		private void UpdateDesires()
		{
			foreach(Desire desire in _desires)
			{
				desire.Update();
			}
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

		private void OnTakeDamage(DamageSource source)
		{
			if(source.Owner == null)
			{
				//means it's debug stuff and can be ignored
				return;
			}

			if (source.Owner.Config.Faction == CharacterFaction.Player)
			{
				App.AIBlackboard.MarkAsHostileToPlayer(this);
				HUD.Instance.CreateMenu<SpeechBubble>().Show(transform, LocManager.GetTranslation("/Dialogue/Agent/Hostile/Attack"), () => App.AIBlackboard.Player.Health.IsDead);
			}

			if(!IsInState<AttackState>(false)) //We're not already attacking something
			{
				PushState(new AttackState.AttackConfig(this, source.Owner));
			}
		}

		private void OnAcquireCurrency(int amount)
		{
			_desires[(int)DesireType.Money].Reset();
		}

		private void OnLoseCurrency(int amount)
		{
			_desires[(int)DesireType.Money].ApplyModifier(new Desire.Modifier() { ToModify = DesireType.Money, FillRate = 0.1f });
		}

		public bool IsInState<T>(bool activeOnly) where T : State
		{
			Type type = typeof(T);

			if (activeOnly)
			{
				return Current.GetType() == type;
			}
			else
			{
				Func<State, bool> stateIsType = state => state != null && type.IsAssignableFrom(state.GetType());

				if( _immediate.Count != 0 && stateIsType(_immediate.Peek()))
				{
					return true;
				}

				if (_normal.Count != 0 && stateIsType(_normal.Peek()))
				{
					return true;
				}
				
				if(_longTerm.Count != 0 && stateIsType(_longTerm.Peek()))
				{
					return true;
				}

				return false;
			}
		}

		public float GetAbsoluteDesireValue(DesireType desire)
		{
			return _desires[(int)desire].Value;
		}

		public State[] GetImmediateStack_Debug()
		{
			return _immediate.ToArray();
		}

		public State[] GetNormalStack_Debug()
		{
			return _normal.ToArray();
		}

		public State[] GetLongTermStack_Debug()
		{
			return _longTerm.ToArray();
		}

		public Desire GetDesire_Debug(DesireType desire)
		{
			Debug.Assert(desire != DesireType.COUNT);
			return _desires[(int)desire];
		}
	}
}