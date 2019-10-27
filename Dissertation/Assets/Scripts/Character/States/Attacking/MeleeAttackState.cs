namespace Dissertation.Character.AI
{
	public class MeleeAttackState : AttackState
	{
		public MeleeAttackState(AttackConfig config) : base(config, 10.0f, 1.0f) //bad, fix
		{ }

		protected override void Attack(bool attack)
		{
			Config.Owner.CharacterYoke.MeleeAttack = attack;
		}

		protected override bool CanAttack()
		{
			return Config.Owner.CanMeleeAttack();
		}
	}
}