using UnityEngine;

namespace Dissertation.Character.AI
{
	/*
	 * Process for creating a new state
	 * 1. Add enum value to States.cs
	 * 2. Implement new class, inheriting from State
	 *	a) Must implement at least a Constructor, and it's a bit stupid to not implement Update() & IsValid()
	 *	b) When implemening Update(), make sure you call base.Update()
	 * 3. (Optional) Implement new Config class inheriting from StateConfig
	 * 4. Add to StateFactory::GetState()
	 * 5. (Optional) Add to StateFactory::GetDefaultState()
	*/

	public enum StatePriority
	{
		Immediate,
		Normal,
		LongTerm
	}

	public abstract class State
	{
		public StateConfig Config { get; private set; }

		public State(StateConfig config)
		{
			Debug.Assert(config != null);

			Config = config;
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
			Config.Owner.CharacterYoke.Reset();
		}

		public virtual bool Update()
		{
			if(!IsValid())
			{
				Config.Owner.PopState(this);
				return false;
			}

			return true;
		}

		public virtual void Destroy()
		{
			OnDisable();
		}

		protected virtual bool IsValid()
		{
			return false;
		}
	}
}