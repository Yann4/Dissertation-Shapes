using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class MoveToState : State
	{
		public class MoveToConfig : StateConfig
		{
			public Vector2 Target { get; private set; }
			public float Tolerance { get; private set; }

			public MoveToConfig(Vector2 target, AgentController owner, float tolerance = 2.0f) 
				: base(States.MoveTo, StatePriority.Immediate, owner)
			{
				Target = target;
				Tolerance = tolerance;
			}
		}

		private MoveToConfig _config;

		public MoveToState(MoveToConfig config) : base(config)
		{
			_config = config;
		}

		public override bool Update()
		{
			if(!base.Update())
			{
				return false;
			}

			float horizontalMovement = 0.0f;
			Vector3 position = Config.Owner.transform.position;

			if (position.x < _config.Target.x)
			{
				horizontalMovement = 1.0f;
			}
			else
			{
				horizontalMovement = -1.0f;
			}

			Config.Owner.CharacterYoke.Movement = new Vector2(horizontalMovement, 0.0f);

			return true;
		}

		protected override bool IsValid()
		{
			return !Positional.IsAtPosition(Config.Owner.transform, _config.Target, _config.Tolerance);
		}
	}
}