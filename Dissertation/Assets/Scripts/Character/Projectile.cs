using Dissertation.Util;
using System;
using UnityEngine;

namespace Dissertation.Character
{
	public class Projectile : DamageSource
	{
		[SerializeField] private float _spinSpeed = 10.0f;
		[SerializeField] private Transform _image;
		private float _spinMultiplier = 1.0f;

		private PrefabPool _owningPool;
		private Vector3 _velocity;
		private float _destructionTime;
		private bool _active = false;

		public Action<Projectile> OnReturnToPool;

		public void Setup(BaseCharacterController owner, int damage, Vector3 velocity, PrefabPool owningPool, float lifetime = 5.0f)
		{
			base.Setup(owner, damage);

			_image.localRotation = Quaternion.identity;

			Debug.Assert(owningPool != null);
			_owningPool = owningPool;
			_velocity = velocity;

			_destructionTime = Time.time + lifetime;
			_active = true;

			_spinMultiplier = _velocity.x > 0.0f ? -1.0f : 1.0f;
		}

		private void Update()
		{
			transform.position += _velocity * Time.deltaTime;

			if(Time.time >= _destructionTime)
			{
				ReturnToPool();
			}

			_image.Rotate(new Vector3(0.0f, 0.0f, _spinMultiplier * _spinSpeed * Time.deltaTime));
		}

		protected override void OnTriggerEnter2D(Collider2D collision)
		{
			base.OnTriggerEnter2D(collision);

			if (!CollisionIsOwner(collision) || collision.IsTouchingLayers(Layers.ObstacleMask))
			{
				ReturnToPool();
			}
		}

		private void ReturnToPool()
		{
			if(_active)
			{
				_owningPool.ReturnInstance(gameObject);
				OnReturnToPool.InvokeSafe(this);
				_active = false;
			}
		}
	}
}
