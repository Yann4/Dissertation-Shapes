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

			RaycastHit2D hit = Physics2D.Raycast(Config.Owner.transform.position, Vector2.down, 200.0f, Layers.GroundMask);
			
			_currentPlatform = hit.transform;
			Debug.Assert(_currentPlatform != null);
			_currentTarget = Rand.GetRandomPointOnPlatform(_currentPlatform);
			Debug.DrawLine(Config.Owner.transform.position, _currentTarget, Color.red, 500.0f, false);
			_waitTimeRemaining = Rand.Next(_idleConfig.MaxWaitDuration);
		}

		public override void Update()
		{
			base.Update();

			if(_waitTimeRemaining > 0.0f)
			{
				_waitTimeRemaining -= Time.deltaTime;
				return;
			}

			Config.Owner.PushState(new MoveToState.MoveToConfig(_currentTarget, Config.Owner));
		}

		protected override bool IsValid()
		{
			//I can do this all day
			return true;
		}
	}
}