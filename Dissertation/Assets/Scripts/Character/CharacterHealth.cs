using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	[RequireComponent(typeof(BaseCharacterController), typeof(Collider2D))]
	public class CharacterHealth : MonoBehaviour
	{
		private BaseCharacterController _character;

		private int _currentHealth;
		public int CurrentHealth
		{
			get { return _currentHealth; }
			set
			{
				_currentHealth = value;
				OnHealthChanged.InvokeSafe(CurrentHealth);

				if(_currentHealth <= 0)
				{
					App.WorldState.SetState(new Narrative.WorldProperty(_character.ID, Narrative.EProperty.IsDead, true));
					OnDied.InvokeSafe(_character);
				}
			}
		}

		public float HealthPercentage { get { return (float)CurrentHealth / (float)_character.Config.MaxHealth; } }

		public bool IsDead { get { return CurrentHealth <= 0; } }

		public Action<int> OnHealthChanged;
		public Action<BaseCharacterController> OnDied;
		public Action OnRespawn;
		public Action<DamageSource> OnDamaged;

		private void Start()
		{
			_character = GetComponent<BaseCharacterController>();
			CurrentHealth = _character.Config.MaxHealth;
			App.WorldState.SetState(new Narrative.WorldProperty(_character.ID, Narrative.EProperty.IsDead, false));
		}

		public void Damage(DamageSource damage)
		{
			ModifyHealth(-(int)damage.Damage);
			OnDamaged.InvokeSafe(damage);
		}

		public void Heal(uint healBy)
		{
			ModifyHealth((int)healBy);
		}

		public void FullHeal()
		{
			ModifyHealth(_character.Config.MaxHealth);
		}

		private void ModifyHealth(int modifyBy)
		{
			int healTo = Mathf.Clamp(CurrentHealth + (int)modifyBy, 0, _character.Config.MaxHealth);
			CurrentHealth = healTo;
		}

		public void Respawn()
		{
			_currentHealth = _character.Config.MaxHealth;
			App.WorldState.SetState(new Narrative.WorldProperty(_character.ID, Narrative.EProperty.IsDead, false));
			OnRespawn.InvokeSafe();
		}
	}
}