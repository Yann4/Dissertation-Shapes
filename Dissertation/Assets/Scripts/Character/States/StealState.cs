using Dissertation.Util;
using System.Linq;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class StealState : SpecialistState
	{
		public class StealConfig : StateConfig
		{
			public Inventory ToStealFrom;
			public StealConfig(Inventory stealFrom, AgentController owner) : base(States.Steal, StatePriority.LongTerm, owner)
			{
				ToStealFrom = stealFrom;
			}
		}

		private StealConfig _stealConfig;

		public StealState() : base(States.Steal)
		{ }

		public StealState(StealConfig config) : base(config)
		{
			_stealConfig = config;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			if(!Positional.IsAtPosition(Config.Owner.transform, _stealConfig.ToStealFrom.transform.position, 3.0f))
			{
				Config.Owner.PushState(new MoveToState.MoveToConfig(_stealConfig.ToStealFrom.transform.position, Config.Owner, 3.0f));
			}
		}

		public override bool Update()
		{
			if (!base.Update())
			{
				return false;
			}

			if(Positional.IsAtPosition(Config.Owner.transform, _stealConfig.ToStealFrom.transform.position, 3.0f))
			{
				Config.Owner.CharacterYoke.Interact = true;
			}

			return true;
		}

		protected override bool IsValid()
		{
			return !_stealConfig.ToStealFrom.Contents.IsEmpty() && IsInventoryUnattended(_stealConfig.ToStealFrom, Config.Owner);
		}

		private bool IsInventoryUnattended(Inventory inventory, AgentController theif)
		{
			foreach(BaseCharacterController opponent in App.AIBlackboard.GetVisibleCharacters(theif))
			{
				if(App.AIBlackboard.LineOfSightCheck(inventory.transform, opponent.transform, theif._agentConfig.VisionRange))
				{
					return false;
				}
			}

			return true;
		}

		public override bool ShouldRunState(AgentController owner, out StateConfig config)
		{
			if(!owner.IsInState<StealState>(false))
			{
				Inventory[] inventories = GameObject.FindObjectsOfType<Inventory>();
				Inventory toSteal = inventories.FirstOrDefault(inventory => inventory.OnGround && inventory.Owner != owner && IsInventoryUnattended(inventory, owner));
				if (toSteal != null)
				{
					config = new StealConfig(toSteal, owner);
					return true;
				}
			}

			config = null;
			return false;
		}
	}
}