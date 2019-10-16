using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public abstract class AttackState : State
	{
		public class AttackConfig : StateConfig
		{
			public BaseCharacterController Target;

			public AttackConfig(AgentController owner, BaseCharacterController target)
				:base(States.Attack, StatePriority.Normal, owner)
			{
				Target = target;
			}
		}

		protected AttackConfig _attackConfig;
		protected float _maxAttackRange = 1.0f;
		protected float _minAttackRange = 0.5f;

		private Vector3 _currentRepositionTarget;
		private const float _targetMoveThresholdSqr = 10.0f * 10.0f;

		private Facing _targetDirection;

		public AttackState(AttackConfig config, float maxRange, float preferredRange) : base(config)
		{
			_attackConfig = config;
			_maxAttackRange = maxRange;
			_minAttackRange = preferredRange;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			_currentRepositionTarget = _attackConfig.Target.transform.position;
		}

		public override bool Update()
		{
			if(!base.Update())
			{
				return false;
			}

			_targetDirection = _attackConfig.Target.transform.position.x > Config.Owner.transform.position.x ? Facing.Right : Facing.Left;

			if (ShouldReposition(out Vector3 targetPosition))
			{
				_currentRepositionTarget = _attackConfig.Target.transform.position;
				Config.Owner.PushState(new PathToState.PathToConfig(Config.Owner, targetPosition, () => HasTargetMoved()));
				return true;
			}

			//Maybe don't attack in the wrong direction, you'll look like a bit of a plonker
			if (Config.Owner.FacingDirection != _targetDirection)
			{
				Config.Owner.CharacterYoke.Movement = new Vector2((float)_targetDirection, 0.0f);
			}
			else
			{
				Config.Owner.CharacterYoke.Movement = Vector2.zero;
				Attack(CanAttack());
			}

			return true;
		}

		protected bool HasTargetMoved()
		{
			return Vector3.SqrMagnitude(_attackConfig.Target.transform.position - _currentRepositionTarget) > _targetMoveThresholdSqr;
		}

		protected virtual bool ShouldReposition(out Vector3 repositionTarget)
		{
			Vector3 targetPosition = _attackConfig.Target.transform.position;
			Vector3 ownerPosition = Config.Owner.transform.position;
			float targetDistance = (ownerPosition - targetPosition).magnitude;

			//TODO: Handle platform edges
			//If we're at an uncomfortable range, reposition
			if (targetDistance > _maxAttackRange || targetDistance < _minAttackRange)
			{
				repositionTarget = GetRepositionTarget();
				return true;
			}

			if(!App.AIBlackboard.CanSeeCharacter(Config.Owner, _attackConfig.Target))
			{
				repositionTarget = GetRepositionTarget();
				return true;
			}

			repositionTarget = Vector3.zero;
			return false;
		}

		protected abstract bool CanAttack();
		protected abstract void Attack(bool attack);

		protected virtual Vector3 GetRepositionTarget()
		{
			Vector3 targetPosition = _attackConfig.Target.transform.position;
			Vector3 ownerPosition = Config.Owner.transform.position;
			
			//TODO: Handle platform edges
			//TODO: Handle obstacles
			//TODO: Handle repositioning player

			Vector3 repositionTarget;

			//If we're on the left, stay on the left
			if (_targetDirection == Facing.Right)
			{
				repositionTarget = targetPosition;
				repositionTarget.x -= (_maxAttackRange - _minAttackRange) / 2.0f;
			}
			else
			{
				repositionTarget = targetPosition;
				repositionTarget.x += (_maxAttackRange - _minAttackRange) / 2.0f;
			}

			Transform platform = Util.Positional.GetPlatform(repositionTarget);
			if(platform == null)
			{
				Transform targetPlatform = Util.Positional.GetCurrentPlatform(_attackConfig.Target.transform, 25.0f);
				//It's possible that the target isn't over a platform right now. In that case, stay where we are til they land
				if(targetPlatform == null)
				{
					return ownerPosition;
				}

				Bounds platformBounds = targetPlatform.GetComponent<Collider2D>().bounds;
				if (_targetDirection == Facing.Right)
				{
					repositionTarget = new Vector3(platformBounds.min.x, platformBounds.max.y);
				}
				else
				{
					repositionTarget = new Vector3(platformBounds.max.x, platformBounds.max.y);
				}
			}

			return repositionTarget;
		}

		protected override bool IsValid()
		{
			return !_attackConfig.Target.Health.IsDead && !Config.Owner.Health.IsDead;
		}
	}
}