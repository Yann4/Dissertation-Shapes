using Dissertation.Util;
using UnityEngine;

namespace Dissertation.Character
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageSource : MonoBehaviour
	{
		public BaseCharacterController Owner;

		[SerializeField] int _damage = 0;
		public int Damage { get { return _damage; } }

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

			if(!_collider.enabled)
			{
				Debug.LogWarning("Collider is disabled on object " + gameObject.name, gameObject);
			}
		}

		public void Setup(BaseCharacterController owner)
		{
			Owner = owner;
		}
	}
}