using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class TraverseState : State
	{
		public class TraverseStateConfig : StateConfig
		{
			public Transform Target { get; private set; }
			public Bounds TargetBounds { get; private set; }
			public float JumpThreshold { get; private set; }
			public TraverseStateConfig(AgentController owner, Transform targetPlatform, float jumpThreshold = 1.0f) :
				base(States.Traverse, StatePriority.Immediate, owner)
			{
				Target = targetPlatform;
				JumpThreshold = jumpThreshold;
				TargetBounds = Target.GetComponent<BoxCollider2D>().bounds;
			}
		}

		private TraverseStateConfig _config;

		private Transform _currentPlatform = null;
		private bool _jump = false;
		private Vector3 _launchPoint;
		private bool _moveLeft = false;

		public TraverseState(TraverseStateConfig config) : base(config)
		{
			_config = config;
			_currentPlatform = Positional.GetCurrentPlatform(Config.Owner.transform);
			Debug.Assert(_currentPlatform != null);
			BoxCollider2D currentPlatformCollider = _currentPlatform.GetComponent<BoxCollider2D>();
			Bounds currentPlatformBounds = currentPlatformCollider.bounds;

			//Jump if the target is higher, or there's a gap between the platforms
			_jump = currentPlatformBounds.max.y <= _config.TargetBounds.max.y ||
				currentPlatformBounds.max.x + 1.0f <= _config.TargetBounds.min.x ||
				currentPlatformBounds.min.x - 1.0f >= _config.TargetBounds.max.x;

			//If the target is on the left
			if (_config.TargetBounds.max.x <= currentPlatformBounds.center.x)
			{
				_moveLeft = true;

				//If the targets rhs overlaps
				if(_config.TargetBounds.max.x >= currentPlatformBounds.min.x && _config.TargetBounds.max.y > currentPlatformBounds.max.y)
				{
					_launchPoint = _config.TargetBounds.max;
				}
				else
				{
					_launchPoint = currentPlatformBounds.min;
				}
			}
			else if (_config.TargetBounds.min.x >= currentPlatformBounds.center.x)
			{
				_moveLeft = false;

				//if the targets lhs overlaps
				if(_config.TargetBounds.min.x <= currentPlatformBounds.max.x && _config.TargetBounds.max.y > currentPlatformBounds.max.y)
				{
					_launchPoint = _config.TargetBounds.min;
				}
				else
				{
					_launchPoint = currentPlatformBounds.max;
				}
			}
			else
			{
				Debug.LogError("Shouldn't ever get here");
			}
		}

		public override bool Update()
		{
			if (!base.Update())
			{
				return false;
			}

			Config.Owner.CharacterYoke.Movement = _moveLeft ? Vector2.left : Vector2.right;
			Config.Owner.CharacterYoke.Jump = _jump 
				&& (_moveLeft ? Config.Owner.transform.position.x - _config.JumpThreshold <= _launchPoint.x : Config.Owner.transform.position.x + _config.JumpThreshold >= _launchPoint.x) 
				&& Positional.GetCurrentPlatform(Config.Owner.transform) != _config.Target;

			return true;
		}

		protected override bool IsValid()
		{
			return Positional.GetCurrentPlatform(Config.Owner.transform) != _config.Target; //Distance being small means that we're also implicitly checking that we're grounded
		}
	}
}