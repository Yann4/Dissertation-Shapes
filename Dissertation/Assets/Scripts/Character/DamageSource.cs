﻿using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageSource : MonoBehaviour
	{
		public BaseCharacterController Owner;

		[SerializeField] int _damage = 0;
		public int Damage { get { return _damage; } }

		public Action<BaseCharacterController /*hit*/> OnHit;

		private Collider2D _collider;

		private void Awake()
		{
			if (gameObject.layer != Layers.DamageSource)
			{
				Debug.LogWarning(string.Format("Changed layer of {0} to Damage Source as it was set to {1}", gameObject.name, gameObject.layer.ToString()), gameObject);
				gameObject.layer = Layers.DamageSource;
			}

			_collider = GetComponent<Collider2D>();
			_collider.isTrigger = true;
		}

		public virtual void Setup(BaseCharacterController owner, int damage)
		{
			Owner = owner;
			_damage = damage;
		}

		protected virtual void OnTriggerEnter2D(Collider2D collision)
		{
			if(CollisionIsOwner(collision))
			{
				return;
			}

			BaseCharacterController characterHit = collision.gameObject.GetComponent<BaseCharacterController>();
			if (characterHit != null)
			{
				OnHit.InvokeSafe(characterHit);
				characterHit.Health.Damage((uint)Damage);
			}
		}

		protected bool CollisionIsOwner(Collider2D collision)
		{
			return Owner != null && GameObject.ReferenceEquals(collision.gameObject, Owner.gameObject);
		}
	}
}
