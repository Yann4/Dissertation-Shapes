using Dissertation.Pathfinding;
using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public abstract class AttackState : State
	{
		public class HasTargetMoved : PathToState.ValidCheck
		{
			private Transform _target;
			private Vector3 _initialPosition;
			private const float _moveToleranceSqr = 5.0f * 5.0f;

			public HasTargetMoved(Transform target, Vector3 initialPos)
			{
				_target = target;
				_initialPosition = initialPos;
			}

			public override bool IsValid()
			{
				return Vector3.SqrMagnitude(_target.position - _initialPosition) <= _moveToleranceSqr;
			}
		}
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

		private Facing _targetDirection;
		private float _lostSightTime = 0.0f;
		private const float _giveUpTime = 3.0f;

		public AttackState(AttackConfig config, float maxRange, float minAttackRange) : base(config)
		{
			_attackConfig = config;
			_maxAttackRange = maxRange;
			_minAttackRange = minAttackRange;
		}

		public override bool Update()
		{
			if(!base.Update())
			{
				return false;
			}

			if( _lostSightTime == 0.0f )
			{
				if (!App.AIBlackboard.CanSeeCharacter(Config.Owner, _attackConfig.Target))
				{
					_lostSightTime = Time.time;
				}
			}
			else if( App.AIBlackboard.CanSeeCharacter(Config.Owner, _attackConfig.Target) )
			{
				_lostSightTime = 0.0f;
			}

			_targetDirection = _attackConfig.Target.transform.position.x > Config.Owner.transform.position.x ? Facing.Right : Facing.Left;

			if(ShouldFlee(out Vector3 fleeTo))
			{
				Config.Owner.PopState(this);
				Config.Owner.PushState(new PathToState.PathToConfig(Config.Owner, fleeTo,
					new HasTargetMoved(_attackConfig.Target.transform, _attackConfig.Target.transform.position)));
				return true;
			}

			if (ShouldReposition(out Vector3 targetPosition))
			{
				Config.Owner.PushState(new PathToState.PathToConfig(Config.Owner, targetPosition, 
					new HasTargetMoved(_attackConfig.Target.transform, _attackConfig.Target.transform.position)));
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

		protected virtual bool ShouldReposition(out Vector3 repositionTarget)
		{
			float targetDistance = (Config.Owner.transform.position - _attackConfig.Target.transform.position).magnitude;

			//TODO: Handle platform edges
			//If we're at an uncomfortable range, reposition
			if (targetDistance > _maxAttackRange || targetDistance < _minAttackRange 
				|| !App.AIBlackboard.CanSeeCharacter(Config.Owner, _attackConfig.Target))
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
			float idealDistance = (_maxAttackRange - _minAttackRange) / 2.0f;
			//If we're on the left, stay on the left
			if (_targetDirection == Facing.Right)
			{
				repositionTarget = targetPosition;
				repositionTarget.x -= idealDistance;
			}
			else
			{
				repositionTarget = targetPosition;
				repositionTarget.x += idealDistance;
			}

			Transform platform = Util.Positional.GetPlatform(repositionTarget, 50.0f);

			bool validPoint = platform != null;
			if (validPoint)
			{
				Collider2D collider = platform.GetComponent<Collider2D>();
				validPoint &= collider.bounds.max.y <= repositionTarget.y;
			}

			if(!validPoint)
			{
				Transform targetPlatform = Util.Positional.GetPlatform(targetPosition, 50.0f);
				//It's possible that the target isn't over a platform right now. In that case, move as close as we can
				if(targetPlatform == null)
				{
					if (_targetDirection == Facing.Right)
					{
						return new Vector3(targetPosition.x - _minAttackRange, targetPosition.y);
					}
					else
					{
						return new Vector3(targetPosition.x + _minAttackRange, targetPosition.y);
					}
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
			return !_attackConfig.Target.Health.IsDead && !Config.Owner.Health.IsDead && (_lostSightTime == 0.0f || Time.time - _lostSightTime < _giveUpTime);
		}

		protected bool ShouldFlee(out Vector3 fleeTo)
		{
			if (Config.Owner.AvailableBehaviours.HasFlag(SpecialistStates.Defend) && Config.Owner.CharacterHome != null && Config.Owner.CharacterHome.Contains(Config.Owner.transform.position))
			{
				fleeTo = Vector3.zero;
				return false;
			}

			float healthPercentage = (float)Config.Owner.Health.CurrentHealth / (float)Config.Owner.Config.MaxHealth;
			if (healthPercentage <= Config.Owner._agentConfig.FleeHealthPercentage)
			{
				if (Config.Owner.CharacterHome != null && !Config.Owner.CharacterHome.Contains(Config.Owner.transform.position))
				{
					//If we're not home, run there
					fleeTo = Config.Owner.CharacterHome.Centre;
				}
				else
				{
					//Just get away to not here
					int randomIndex = Random.Range(0, Node.AllNodes.Count);
					fleeTo = Node.AllNodes[randomIndex].Position;
				}

				return true;
			}

			fleeTo = Vector3.zero;
			return false;
		}
	}
}