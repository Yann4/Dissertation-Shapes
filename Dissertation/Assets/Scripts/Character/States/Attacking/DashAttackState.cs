namespace Dissertation.Character.AI
{
	public class DashAttackState : AttackState
	{
		public DashAttackState(AttackConfig config) 
			: base(config, config.Owner.Config.DashAttackDistance * 0.9f, 0.5f)
		{ }

		protected override void Attack(bool attack)
		{
			Config.Owner.CharacterYoke.DashAttack = attack;
		}

		protected override bool CanAttack()
		{
			return Config.Owner.CanDashAttack();
		}
	}
}