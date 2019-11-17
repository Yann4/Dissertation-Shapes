namespace Dissertation.Character.AI
{
	public class ServeJusticeState : SpecialistState
	{
		public class JusticeConfig : StateConfig
		{
			public BaseCharacterController Criminal;

			public JusticeConfig(AgentController owner, BaseCharacterController criminal) : base(States.Justice, StatePriority.LongTerm, owner)
			{
				Criminal = criminal;
			}
		}

		private JusticeConfig _config;
		private const float _safetyDesireThreshold = 2.0f;

		public ServeJusticeState() : base(States.Steal)
		{ }

		public ServeJusticeState(JusticeConfig config) : base(config)
		{
			_config = config;
			_config.Criminal.Health.OnDied += OnServeJustice;
		}

		private void OnServeJustice(BaseCharacterController criminal)
		{
			Config.Owner.ResetDesire(DesireType.Safety);
			Config.Owner.ResetDesire(DesireType.Power);
		}

		public override void OnEnable()
		{
			base.OnEnable();

			if (!_config.Criminal.Health.IsDead)
			{
				Config.Owner.PushState(new AttackState.AttackConfig(Config.Owner, _config.Criminal));
			}
		}

		protected override bool IsValid()
		{
			return App.AIBlackboard.Criminals.Contains(_config.Criminal);
		}

		public override bool ShouldRunState(AgentController owner, out StateConfig config)
		{
			if( owner.GetAbsoluteDesireValue(DesireType.Safety) > _safetyDesireThreshold && App.AIBlackboard.Criminals.Count > 0 && !owner.IsInState<ServeJusticeState>(false) )
			{
				config = new JusticeConfig(owner, App.AIBlackboard.Criminals[0]);
				return true;
			}

			config = null;
			return false;
		}
	}
}