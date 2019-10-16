using UnityEngine;

namespace Dissertation.Character.AI
{
	public class RangedAttackState : AttackState
	{
		private const int _burstLength = 3;
		private int _currentBurstLength = 0;
		private const float _burstDelay = 1.0f;
		private float _lastAttackTime;

		public RangedAttackState(AttackConfig config) : base(config, 50.0f, 2.0f)
		{ }

		protected override void Attack(bool attack)
		{
			Config.Owner.CharacterYoke.RangedAttack = attack;

			if (attack)
			{
				_currentBurstLength++;
				_lastAttackTime = Time.time;
			}
		}

		protected override bool CanAttack()
		{
			//Fire x shots then wait a while before firing a few more
			if(_currentBurstLength >= _burstLength)
			{
				if (Time.time - _lastAttackTime > _burstDelay)
				{
					_currentBurstLength = 0;
				}
				else
				{
					return false;
				}
			}

			return Config.Owner.CanRangedAttack();
		}
	}
}