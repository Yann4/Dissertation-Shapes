using UnityEngine;

namespace Dissertation.Character
{
	[RequireComponent(typeof(Collider2D))]
	public class DamageSource : MonoBehaviour
	{
		public BaseCharacterController Owner;

		[SerializeField] int _damage = 0;
		public int Damage { get { return _damage; } }

		public void Setup(BaseCharacterController owner)
		{
			Owner = owner;
		}
	}
}