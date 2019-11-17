using Dissertation.Pathfinding;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class FleeState : SpecialistState
	{
		public class FleeConfig : StateConfig
		{
			public BaseCharacterController Attacker { get; private set; }

			public FleeConfig( AgentController owner, BaseCharacterController attacker ) : base(States.Flee, StatePriority.Immediate, owner)
			{
				Attacker = attacker;
			}
		}

		private FleeConfig _config;
		private const float _runSpeedModifier = 1.5f;

		public FleeState() : base(States.Flee)
		{ }

		public FleeState(FleeConfig config) : base(config)
		{
			_config = config;
			_config.Owner.RunSpeedModifier = _runSpeedModifier;
		}

		public override void Destroy()
		{
			base.Destroy();
			_config.Owner.RunSpeedModifier = 1.0f;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			if(App.AIBlackboard.CanSeeCharacter(Config.Owner, _config.Attacker))
			{
				Vector3 fleeTo;
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

				Config.Owner.PushState(new MoveToState.MoveToConfig(fleeTo, Config.Owner));
			}
		}

		protected override bool IsValid()
		{
			return App.AIBlackboard.CanSeeCharacter(Config.Owner, _config.Attacker);
		}

		public override bool ShouldRunState(AgentController owner, out StateConfig config)
		{
			config = null;
			return false;
		}
	}
}