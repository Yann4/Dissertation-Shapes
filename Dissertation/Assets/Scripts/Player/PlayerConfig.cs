using UnityEngine;

namespace Dissertation.Player
{
	[CreateAssetMenu(fileName = "PlayerConfig.asset", menuName = "Dissertation/Scriptables/Player Config")]
	public class PlayerConfig : ScriptableObject
	{
		[SerializeField] private float _jumpForce = 1000.0f;
		public float JumpForce { get { return _jumpForce; } }

		[SerializeField] private float _moveForce = 365.0f;
		public float MoveForce { get { return _moveForce; } }

		[SerializeField] private float _maxSpeed = 5.0f;
		public float MaxSpeed { get { return _maxSpeed; } }
	}
}