using Dissertation.Environment;
using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageSource : MonoBehaviour
	{
		public BaseCharacterController Owner { get; private set; }

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
				characterHit.Health.Damage(this);
			}

			Wall wall = collision.gameObject.GetComponent<Wall>();
			if(wall != null)
			{
				wall.Damage(this);
			}
		}

		protected bool CollisionIsOwner(Collider2D collision)
		{
			return Owner != null && GameObject.ReferenceEquals(collision.gameObject, Owner.gameObject);
		}
	}
}
