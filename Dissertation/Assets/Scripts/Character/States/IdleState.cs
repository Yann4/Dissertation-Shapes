using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character.AI
{
	//Wanders up and down the current platform
	public class IdleState : State
	{
		public class IdleConfig : StateConfig
		{
			public float MaxWaitDuration { get; private set; }

			public IdleConfig(AgentController owner, float maxWaitDuration, StatePriority prio = StatePriority.LongTerm) :
				base(States.Idle, prio, owner)
			{
				MaxWaitDuration = maxWaitDuration;
			}
		}

		private Transform _currentPlatform;
		private Vector2 _currentTarget;
		private float _waitTimeRemaining;
		protected IdleConfig _idleConfig;

		public IdleState(IdleConfig config) : base(config)
		{
			_idleConfig = config;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			_currentPlatform = Positional.GetCurrentPlatform(Config.Owner.transform);
			Debug.Assert(_currentPlatform != null);
			_currentTarget = Rand.GetRandomPointOnPlatform(_currentPlatform);
			Debug.DrawLine(Config.Owner.transform.position, _currentTarget, Color.red, 500.0f, false);
			_waitTimeRemaining = Rand.Next(_idleConfig.MaxWaitDuration);
		}

		public override bool Update()
		{
			if(!base.Update())
			{
				return false;
			}

			if(_waitTimeRemaining > 0.0f)
			{
				_waitTimeRemaining -= Time.deltaTime;
				return true;
			}

			Config.Owner.PushState(new MoveToState.MoveToConfig(_currentTarget, Config.Owner));
			return true;
		}

		protected override bool IsValid()
		{
			//I can do this all day
			return true;
		}
	}
}