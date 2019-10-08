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
					OnDied.InvokeSafe();
				}
			}
		}

		public bool IsDead { get { return CurrentHealth <= 0; } }

		public Action<int> OnHealthChanged;
		public Action OnDied;

		private void Start()
		{
			_character = GetComponent<BaseCharacterController>();
			CurrentHealth = _character.Config.MaxHealth;
		}

		public void ModifyHealth(int modifyBy)
		{
			int healTo = Mathf.Clamp(CurrentHealth + (int)modifyBy, 0, _character.Config.MaxHealth);
			CurrentHealth = healTo;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if(collision.gameObject.layer != Layers.DamageSource)
			{
				return;
			}

			DamageSource source = collision.gameObject.GetComponent<DamageSource>();
			Debug.Assert(source != null);

			//Can't damage yourself. Eventually this will want to be a more fully fledged check
			if(source.Owner == _character)
			{
				return;
			}

			ModifyHealth(-source.Damage);
		}
	}
}