using UnityEngine;

namespace Dissertation.Player
{
	[CreateAssetMenu(fileName = "PlayerConfig.asset", menuName = "Dissertation/Scriptables/Player Config")]
	public class PlayerConfig : ScriptableObject
	{
		[Header("Movement config")]
		[SerializeField] private float _gravity = -25f;
		public float Gravity { get { return _gravity; } }

		[SerializeField] private float _runSpeed = 8f;
		public float RunSpeed { get { return _runSpeed; } }

		[SerializeField, Tooltip("How fast do we change direction? higher means faster")] private float _groundDamping = 20f;
		public float GroundDamping { get { return _groundDamping; } }

		[SerializeField] private float _inAirDamping = 5f;
		public float InAirDamping { get { return _inAirDamping; } }

		[SerializeField] private float _jumpHeight = 3f;
		public float JumpHeight { get { return _jumpHeight; } }
	}
}