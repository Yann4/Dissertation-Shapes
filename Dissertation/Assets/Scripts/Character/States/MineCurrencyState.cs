using Dissertation.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dissertation.Character.AI
{
	public class MineCurrencyState : SpecialistState
	{
		public class MineCurrencyConfig : StateConfig
		{
			public Inventory ToMine { get; private set; }

			public MineCurrencyConfig(AgentController Owner, Inventory toMine) : base(States.Mine, StatePriority.LongTerm, Owner)
			{
				ToMine = toMine;
			}
		}

		private const float _moneyDesireThreshold = 15.0f;
		private const float _minePointDistanceThresholdBase = 30.0f;

		private Inventory ToMine;

		public MineCurrencyState() : base(States.Mine)
		{ }

		public MineCurrencyState(MineCurrencyConfig config) : base(config)
		{
			ToMine = config.ToMine;
		}

		public override void OnEnable()
		{
			base.OnEnable();

			if(!Positional.IsAtPosition(Config.Owner.transform, ToMine.transform.position))
			{
				Config.Owner.PushState( new PathToState.PathToConfig( Config.Owner, ToMine.transform.position ) );
				return;
			}
		}

		public override bool Update()
		{
			if (!base.Update())
			{
				return false;
			}

			Config.Owner.CharacterYoke.Interact = true;

			return true;
		}

		protected override bool IsValid()
		{
			return !ToMine.Contents.IsEmpty();
		}

		public override bool ShouldRunState(AgentController owner, out StateConfig config)
		{
			if ( !owner.IsInState<MineCurrencyState>(false) )
			{
				var thing = App.AIBlackboard.GetAvailableMinePoints();
				Inventory minePoint = thing.Find(inventory => Vector3.Distance(inventory.transform.position, owner.transform.position) < DistanceThreshold(owner));
				if(minePoint != null)
				{
					config = new MineCurrencyConfig(owner, minePoint);
					return true;
				}
			}

			config = null;
			return false;
		}

		private float DistanceThreshold(AgentController owner)
		{
			return _minePointDistanceThresholdBase + (owner.GetAbsoluteDesireValue(DesireType.Money) - _moneyDesireThreshold);
		}
	}
}